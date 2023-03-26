
namespace LinMeyer.GooglePhotosBackup.Asp.Background;

using LinMeyer.GooglePhotosBackup.Asp.Data;
using LinMeyer.GooglePhotosBackup.Syncs;

public class GooglePhotosBackgroundService : IHostedService, IDisposable
{
    private static bool _enabled = true;
    private readonly ILogger<GooglePhotosBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHostEnvironment _env;
    private readonly PhotosClient _photos;
    private Timer? _timer = null;
    private int _lock = 0;

    public GooglePhotosBackgroundService(ILogger<GooglePhotosBackgroundService> logger, 
        IHostEnvironment env,
        PhotosClient photos,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _env = env;
        _photos = photos;
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
        DoWork(null);
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        if (!_enabled) return;
        // Exit if the lock is not open
        if (Interlocked.CompareExchange(ref _lock, 1, 0) != 0) return;

        try 
        {
            // Grab the lock
            _logger.LogDebug("Obtaining lock");
            Monitor.Enter(_lock); 
            // Get scoped Db for tracking files/statuses
            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<SyncDbContext>();
            // Create timeout cancellation token
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));
            cts.Token.ThrowIfCancellationRequested();
            await _photos.LoginAsync();
            await _photos.SyncAsync(db, (e, localFile, mediaItem) => {
                _logger.LogError(e, $"Error syncing file {localFile} for Google Photos Item {mediaItem.baseUrl}");
            });
        }
        catch (OperationCanceledException e)
        {
            SetEnabled(false);
            _logger.LogWarning(e, "Logout canceled, disabling sync service to avoid redundant popups");
        }
        catch (Exception e) when (!_env.IsDevelopment())
        {
            _logger.LogError(e, "Exception during background sync");
        }
        finally
        {
            _logger.LogDebug("Releasing lock");
            Interlocked.Decrement(ref _lock);
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(GooglePhotosBackgroundService)} stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose() => _timer?.Dispose();

    public static void SetEnabled(bool enabled)
    {
        _enabled = enabled;
    }
}