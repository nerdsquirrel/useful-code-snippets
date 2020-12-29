public class FileProperty
{
    public string FileName { get; set; }
    public string DownloadFileNameWithPath { get; set; }
    public string UploadFileNameWithPath { get; set; }
    public string LocalStorageFileNameWithPath { get; set; }
    public long FileSize { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}