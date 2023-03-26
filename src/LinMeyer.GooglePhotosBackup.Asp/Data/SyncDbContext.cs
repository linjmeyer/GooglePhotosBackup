using LinMeyer.GooglePhotosBackup.Config;
using Microsoft.EntityFrameworkCore;

namespace LinMeyer.GooglePhotosBackup.Asp.Data;

public class SyncDbContext : DbContext
{
    public DbSet<FileStatusEntity> FileStatuses { get; set; }
    public string DbPath { get; }

    public SyncDbContext()
    {
        DbPath = PathsConfig.GetDatabaseFile();
        var dbPathDir = Path.GetDirectoryName(DbPath);
        if (!Directory.Exists(dbPathDir))
        {
            Directory.CreateDirectory(dbPathDir);
        }
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={DbPath}");
}