using LinMeyer.GooglePhotosBackup.Asp.Data;

namespace LinMeyer.GooglePhotosBackup.Config;

public static class PathsConfig
{
    public const string LinMeyerSync = nameof(LinMeyerSync);
    
    private static string _localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    
    public static string GetPath(params string[] paths)
    {
        var pathList = new List<string>()
        {
            Path.Join(_localData, LinMeyerSync)
        };
        pathList.AddRange(paths);
        return Path.Join(pathList.ToArray());
    }

    public static string GetTempDirectory() => GetPath("tmp");
    public static string GetFilesDirectory(RemoteFileKind kind) => GetPath("sync", kind.ToString().ToLower());
    public static string GetDatabaseFile() => GetPath("db", "sync.db");
}