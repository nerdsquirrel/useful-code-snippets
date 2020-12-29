public interface IFtpManager
{
    byte[] FileBytes { get; set; }

    Task DownloadFileAsync(string ftpFilePath);

    Task UploadFileAsync(string remoteFile, Stream fs);

    Task DeleteFile(string deleteFile);

    Task RenameFile(string currentFileNameAndPath, string newFileName);

    Task CreateDirectoryAsync(string newDirectory);

    Task<DateTime> GetLastModifiedDateOfFile(string fileName);

    Task<string> GetFileSize(string fileName);

    Task<string[]> DirectoryList(string directory);

    Task<List<FileProperty>> DirectoryListDetails(string directory);

    Task<bool> CheckIfDirectoryExistsAsync(string directory);

    Task<bool> CheckConnectionAsync();
}