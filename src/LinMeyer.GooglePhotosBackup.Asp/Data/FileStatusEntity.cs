namespace LinMeyer.GooglePhotosBackup.Asp.Data;

public enum RemoteFileKind
{
    GooglePhotos = 0
}

public class FileStatusEntity 
{
    public int Id { get; set; }
    public RemoteFileKind RemoteKind { get; set; }
    public string RemoteId { get; set; }
    public string LocalPath { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public DateTime Checked { get; set; }
}