using CasCap.Models;
using CasCap.Services;
using LinMeyer.GooglePhotosBackup.Asp.Data;
using LinMeyer.GooglePhotosBackup.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinMeyer.GooglePhotosBackup.Syncs;
public class PhotosClient
{
    ILogger<PhotosClient> _logger;
    IHostEnvironment _env;
    GooglePhotosService _photos;
    GoogleConfig _config;

    public PhotosClient(ILogger<PhotosClient> logger, IHostEnvironment env, GooglePhotosService photos, GoogleConfig config)
    {
        _env = env;
        _logger = logger;
        _photos = photos;
        _config = config;
    }

    public PhotosClient(ILogger<PhotosClient> logger, IHostEnvironment env, GooglePhotosService photos, IOptions<GoogleConfig> config)
    {
        _env = env;
        _logger = logger;
        _photos = photos;
        _config = config.Value;
    }

    private string CreateTargetDirectory()
    {
        var filesDirectory = PathsConfig.GetFilesDirectory(RemoteFileKind.GooglePhotos);
        if (!Directory.Exists(filesDirectory))
        {
            Directory.CreateDirectory(filesDirectory);
        }
        return filesDirectory;
    }

    public async Task LoginAsync()
    {
        var scopes = new [] { GooglePhotosScope.ReadOnly };
        var tmpDir = PathsConfig.GetTempDirectory();
        await  _photos.LoginAsync("user", _config.ClientId, _config.ClientSecret, scopes, tmpDir);
    }

    public async Task SyncAsync(SyncDbContext db, Action<Exception, string, MediaItem> exceptionHandler)
    {
        var targetDir = CreateTargetDirectory();
        
        await foreach(var item in  _photos.GetMediaItemsAsync())
        {
            
            _logger.LogDebug("Found media item in response from API: {}", item.id);
            var localFile = Path.Join(targetDir, item.filename);
            if (File.Exists(localFile))
            {
                _logger.LogDebug("Skipping file as it already exists locally {}", localFile);
                continue;
            }   
            try
            {
                _logger.LogDebug("Will download new file {}", localFile);
                var itemBytes = await _photos.DownloadBytes(item, includeExifMetadata: true, downloadVideoBytes: item.isVideo);
                await File.WriteAllBytesAsync(localFile, itemBytes);
                _logger.LogInformation("Synced new file {}", localFile);
                await UpdateDbForFile(db, localFile, item);
            } 
            catch (Exception e)
            {
                exceptionHandler(e, localFile, item);
            }
        }
    }

    private async Task UpdateDbForFile(SyncDbContext db, string localPath, MediaItem mediaItem, bool updated=false)
    {
        var uniqueId = mediaItem.id;
        var dateTime = DateTime.UtcNow;
        var fileStatus = await db.FileStatuses.Where(f => f.RemoteId == uniqueId).FirstOrDefaultAsync();
        var existed = true;
        if(fileStatus == null)
        {
            existed = false;
            fileStatus = new FileStatusEntity();
            fileStatus.Created = dateTime;
            fileStatus.Updated = dateTime;
        }
        fileStatus.LocalPath = localPath;
        fileStatus.RemoteId = uniqueId;
        fileStatus.RemoteKind = RemoteFileKind.GooglePhotos;
        fileStatus.Checked = DateTime.Now;
        if (updated) fileStatus.Updated = dateTime;
        if (!existed) await db.AddAsync(fileStatus);
        await db.SaveChangesAsync();
        _logger.LogInformation("Updated database for file {}", localPath);
    }
}
