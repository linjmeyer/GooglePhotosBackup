using LinMeyer.GooglePhotosBackup.Asp.Background;
using LinMeyer.GooglePhotosBackup.Asp.Data;
using LinMeyer.GooglePhotosBackup.Syncs;
using LinMeyer.GooglePhotosBackup.Config;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SyncDbContext>();
builder.Services.AddHostedService<GooglePhotosBackgroundService>();
builder.Services.AddOptions<GoogleConfig>().Bind(builder.Configuration.GetSection("Google"));
builder.Services.AddGooglePhotos();
builder.Services.AddSingleton<PhotosClient>();

var app = builder.Build();
using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
using(var migrationDbContext = serviceScope.ServiceProvider.GetRequiredService<SyncDbContext>())
{
    migrationDbContext.Database.Migrate();
}

app.MapGet("/", () => "LinMeyer.GooglePhotosBackup is healthy");

app.Run();
