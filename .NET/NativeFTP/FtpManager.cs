public class FtpManager : IFtpManager
{
    private readonly string _host;
    private readonly string _pass;
    private readonly string _user;
    private FtpWebRequest _ftpRequest;
    private FtpWebResponse _ftpResponse;
    private Stream _ftpStream;
    private StreamReader _streamReader;

    public byte[] FileBytes { get; set; }
    
    public FtpManager(string hostAddress, string userName, string password)
    {
        _host = hostAddress;
        _user = userName;
        _pass = password;
    }

    public async Task DownloadFileAsync(string ftpFilePath)
    {
        try
        {
            _ftpRequest = (FtpWebRequest)WebRequest.Create(_host + ftpFilePath);
            _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
            _ftpRequest.UseBinary = true;
            _ftpRequest.UsePassive = true;
            _ftpRequest.KeepAlive = true;
            _ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            using (_ftpResponse = (FtpWebResponse)await _ftpRequest.GetResponseAsync())
            {
                using (_ftpStream = _ftpResponse.GetResponseStream())
                {
                    FileBytes = await ReadBytesAsync(_ftpResponse.GetResponseStream());
                }
            }
        }
        finally
        {
            _ftpStream?.Close();
            _ftpResponse?.Close();
            _ftpRequest = null;
        }
    }

    public async Task UploadFileAsync(string remoteFile, Stream fs)
    {
        try
        {
            var remoteFilePathResult = PrepareRemoteFilePath(Os.Linux, _host, remoteFile);

            if (remoteFilePathResult.IsFailure)
                return;

            _ftpRequest = (FtpWebRequest)WebRequest.Create(remoteFilePathResult.Value);
            _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
            _ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            _ftpRequest.UseBinary = true;
            _ftpRequest.KeepAlive = true;

            _ftpRequest.ContentLength = fs.Length;
            var buffer = new byte[4097];
            var totalBytes = (int)fs.Length;
            var rs = _ftpRequest.GetRequestStream();
            while (totalBytes > 0)
            {
                var bytes = fs.Read(buffer, 0, buffer.Length);
                rs.Write(buffer, 0, bytes);
                totalBytes = totalBytes - bytes;
            }

            //fs.Flush();
            fs.Close();
            rs.Close();
            using (_ftpResponse = (FtpWebResponse)await _ftpRequest.GetResponseAsync())
            {
                var value = _ftpResponse.StatusDescription;
            }
        }
        catch (Exception e)
        {
            // log error
            throw;
        }
        finally
        {
            _ftpResponse?.Close();
            _ftpRequest = null;
        }
    }

    private Result<string> PrepareRemoteFilePath(Os operatingSystem = Os.Windows, params string[] paths)
    {
        if (paths == null || !paths.Any())
            return Result.Fail<string>($"{nameof(paths)} is not defined");

        try
        {
            switch (operatingSystem)
            {
                case Os.Linux: return Result.Ok(string.Join("/", paths.Select(p => p.TrimStart('/').TrimEnd('/'))));
                case Os.Windows:
                    return Result.Ok
                    (
                        System.IO.Path.Combine(paths.Select(p => p.TrimStart('\\').TrimEnd('\\')).ToArray())
                    );
                default: return Result.Fail<string>($"Operation is not defined for [{operatingSystem}]");
            }
        }
        catch (Exception e)
        {
            StartupBase.Current.DependencyResolver.Resolve<ILogger>()?.Log(e);
            return Result.Fail<string>($"Path generation failed for paths [{string.Join(@",", paths)}]. Error: {e.Message}");
        }
    }
    
    public async Task DeleteFile(string deleteFile)
    {
        try
        {
            _ftpRequest = (FtpWebRequest)WebRequest.Create(_host + deleteFile);
            _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
            _ftpRequest.UseBinary = true;
            _ftpRequest.UsePassive = true;
            _ftpRequest.KeepAlive = true;
            _ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
            _ftpResponse = (FtpWebResponse)await _ftpRequest.GetResponseAsync();
        }
        finally
        {
            _ftpResponse?.Close();
            _ftpRequest = null;
        }
    }
   
    public async Task RenameFile(string currentFileNameAndPath, string newFileName)
    {
        try
        {
            _ftpRequest = (FtpWebRequest)WebRequest.Create(_host + currentFileNameAndPath);
            _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
            _ftpRequest.UseBinary = true;
            _ftpRequest.UsePassive = true;
            _ftpRequest.KeepAlive = true;
            _ftpRequest.Method = WebRequestMethods.Ftp.Rename;
            _ftpRequest.RenameTo = newFileName;
            _ftpResponse = (FtpWebResponse)await _ftpRequest.GetResponseAsync();
        }
        finally
        {
            _ftpResponse?.Close();
            _ftpRequest = null;
        }
    }
   
    public async Task CreateDirectoryAsync(string newDirectory)
    {
        try
        {
            var remoteFilePathResult = PrepareRemoteFilePath(Os.Linux, _host, newDirectory);

            if (remoteFilePathResult.IsFailure)
                return;

            _ftpRequest = (FtpWebRequest)WebRequest.Create(remoteFilePathResult.Value);
            _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
            _ftpRequest.UseBinary = true;
            _ftpRequest.UsePassive = true;
            _ftpRequest.KeepAlive = true;
            _ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            _ftpResponse = (FtpWebResponse)await _ftpRequest.GetResponseAsync();
        }
        finally
        {
            _ftpResponse?.Close();
            _ftpRequest = null;
        }
    }

    /* Get the Date/Time a File was Created */
    public async Task<DateTime> GetLastModifiedDateOfFile(string fileName)
    {
        try
        {
            _ftpRequest = (FtpWebRequest)WebRequest.Create(_host + fileName);
            _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
            _ftpRequest.UseBinary = true;
            _ftpRequest.UsePassive = true;
            _ftpRequest.KeepAlive = true;
            _ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
            _ftpResponse = (FtpWebResponse)await _ftpRequest.GetResponseAsync();
        }
        finally
        {
            _ftpStream?.Close();
            _ftpResponse?.Close();
            _ftpRequest = null;
        }

        if (_ftpResponse == null) return DateTime.MinValue;

        return _ftpResponse.LastModified;
    }

    /* Get the Size of a File */
    public async Task<string> GetFileSize(string fileName)
    {
        try
        {
            _ftpRequest = (FtpWebRequest)WebRequest.Create(_host + fileName);
            _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
            _ftpRequest.UseBinary = true;
            _ftpRequest.UsePassive = true;
            _ftpRequest.KeepAlive = true;
            _ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
            using (_ftpResponse = (FtpWebResponse)await _ftpRequest.GetResponseAsync())
            {
                using (_ftpStream = _ftpResponse.GetResponseStream())
                {
                    if (_ftpStream != null)
                    {
                        var ftpReader = new StreamReader(_ftpStream);
                        string fileInfo = null;
                        while (ftpReader.Peek() != -1) fileInfo = ftpReader.ReadToEnd();
                        ftpReader.Close();
                        _ftpStream?.Close();
                        _ftpResponse?.Close();
                        _ftpRequest = null;
                        return fileInfo;
                    }
                }
            }
        }
        finally
        {
            _ftpResponse?.Close();
            _ftpRequest = null;
        }

        return "";
    }

    /* List Directory Contents File/Folder Name Only */
    public async Task<string[]> DirectoryList(string directory)
    {
        try
        {
            _ftpRequest = (FtpWebRequest)WebRequest.Create(_host + directory);
            _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
            _ftpRequest.UseBinary = true;
            _ftpRequest.UsePassive = true;
            _ftpRequest.KeepAlive = true;
            _ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            using (_ftpResponse = (FtpWebResponse)await _ftpRequest.GetResponseAsync())
            {
                using (_ftpStream = _ftpResponse.GetResponseStream())
                {
                    if (_ftpStream != null)
                    {
                        var ftpReader = new StreamReader(_ftpStream);
                        string directoryRaw = null;
                        try
                        {
                            while (ftpReader.Peek() != -1) directoryRaw += ftpReader.ReadLine() + "|";
                        }
                        finally
                        {
                            ftpReader?.Close();
                            _ftpStream?.Close();
                            _ftpResponse?.Close();
                            _ftpRequest = null;
                        }

                        if (directoryRaw != null)
                        {
                            var directoryList = directoryRaw.Split("|".ToCharArray());
                            return directoryList;
                        }
                    }
                }
            }
        }
        finally
        {
            _ftpResponse?.Close();
            _ftpRequest = null;
        }

        /* Return an Empty string Array if an Exception exception Occurs */
        return new[] { "" };
    }

    /* List Directory Contents in Detail (Name, Size, Created, etc.) */
    public async Task<List<FileProperty>> DirectoryListDetails(string directory)
    {
        var filePropertyList = new List<FileProperty>();
        try
        {
            _ftpRequest = (FtpWebRequest)WebRequest.Create(_host + directory);
            _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
            _ftpRequest.UseBinary = true;
            _ftpRequest.UsePassive = true;
            _ftpRequest.KeepAlive = true;
            _ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            using (_ftpResponse = (FtpWebResponse)await _ftpRequest.GetResponseAsync())
            {
                using (_ftpStream = _ftpResponse.GetResponseStream())
                {
                    if (_ftpStream != null) _streamReader = new StreamReader(_ftpStream);
                    string line;
                    while ((line = _streamReader.ReadLine()) != null)
                    {
                        var fileListArr = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                            if (fileListArr.Count() >= 4)
                            {
                                FileProperty fileProperty = new FileProperty()
                                {
                                    ModifiedDate = fileListArr[0] != null ? DateTime.ParseExact(Convert.ToString(fileListArr[0]), "MM-dd-yy", null) : DateTime.MinValue,
                                    FileName = fileListArr[3] != null ? Convert.ToString(fileListArr[3]) : string.Empty,
                                    FileSize = fileListArr[2] != null && fileListArr[2] != "<DIR>" ? long.Parse(fileListArr[2]) : 0
                                };

                                filePropertyList.Add(fileProperty);
                            }                        
                    }
                }
            }
        }
        finally
        {
            _streamReader?.Close();
            _ftpStream?.Close();
            _ftpResponse?.Close();
            _ftpRequest = null;
        }

        if (filePropertyList.Any())
            filePropertyList = filePropertyList.OrderByDescending(x => x.ModifiedDate).ToList();

        return filePropertyList;
    }
    
    public async Task<bool> CheckIfDirectoryExistsAsync(string directory)
    {
        _ftpRequest = (FtpWebRequest)WebRequest.Create(_host + directory);
        _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
        _ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
        try
        {
            using (var response = (FtpWebResponse)await _ftpRequest.GetResponseAsync())
            {
                return true;
            }
        }

        finally
        {
            _ftpRequest = null;
        }
    }

    /* Read file bytes asynchronously from input stream */
    public async Task<byte[]> ReadBytesAsync(Stream input)
    {
        using (var ms = new MemoryStream())
        {
            await Task.Factory.StartNew(() =>
            {
                var buffer = new byte[16 * 1024];

                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0) ms.Write(buffer, 0, read);
            });

            return ms.ToArray();
        }
    }

    public async Task<bool> CheckConnectionAsync()
    {
        try
        {
            _ftpRequest = (FtpWebRequest)WebRequest.Create(_host);
            _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
            _ftpRequest.UseBinary = true;
            _ftpRequest.UsePassive = true;
            _ftpRequest.KeepAlive = false;
            _ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            _ftpResponse = (FtpWebResponse)await _ftpRequest.GetResponseAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            _ftpResponse?.Close();
            _ftpRequest = null;
        }
    }
}