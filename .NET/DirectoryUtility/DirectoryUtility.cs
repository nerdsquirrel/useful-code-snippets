using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Utility
{
    public class DirectoryUtility : IDirectoryUtility
    {
        private const string SystemVolumeInformation = "System Volume Information";

        private readonly ILogger _logger;

        public DirectoryUtility(ILogger logger)
        {
            _logger = logger;
        }

        public Task<DirectoryInfo> CreateFolderIfNotExistAsync(string folderNameWithPath)
        {
            if (string.IsNullOrWhiteSpace(folderNameWithPath))
                throw new ArgumentException("Path is null or empty.", nameof(folderNameWithPath));

            DirectoryInfo createdDirectory;

            try
            {
                createdDirectory = Directory.CreateDirectory(folderNameWithPath);
            }
            catch (Exception ex)
            {
                createdDirectory = null;
                _logger.Error(ex, $"Failed to create directory from method:{nameof(CreateFolderIfNotExistAsync)}");
            }

            return Task.FromResult(createdDirectory);
        }

        public async Task<Result> CreateFoldersIfNotExistAsync(params string[] folderNameWithPaths)
        {
            List<Result> results = new List<Result>();

            foreach (var item in folderNameWithPaths)
            {
                DirectoryInfo createdDirectory = await CreateFolderIfNotExistAsync(item);

                results.Add(createdDirectory == null ?
                    Result.Failure($"Directory creation failed for the given path: {item}") :
                    Result.Success($"{createdDirectory.CreationTime}"));
            }

            return Result.Combine(results.ToArray());
        }

        public Task<DirectoryInfo> RecreateFolderIntoPath(string folderNameToRecreate, string containerPath)
        {
            if (string.IsNullOrWhiteSpace(folderNameToRecreate))
                throw new ArgumentException("Path is null or empty.", nameof(folderNameToRecreate));

            if (string.IsNullOrWhiteSpace(containerPath))
                throw new ArgumentException("Path is null or empty.", nameof(containerPath));

            var directoryInfoOfFolderToCreate = new DirectoryInfo(Path.Combine(containerPath, folderNameToRecreate));

            try
            {
                if (!directoryInfoOfFolderToCreate.Exists)
                    directoryInfoOfFolderToCreate.Create();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(RecreateFolderIntoPath)}(\"{folderNameToRecreate}\", \"{containerPath}\") failed.");
            }

            var rv = Task.FromResult(directoryInfoOfFolderToCreate);
            return rv;
        }

        public async Task CopyFilesToFolder(List<FileInfo> filesToCopy, DirectoryInfo destinationFolder)
        {
            if (filesToCopy == null)
                throw new ArgumentNullException(nameof(filesToCopy));
            if (destinationFolder == null)
                throw new ArgumentNullException(nameof(destinationFolder));

            await Task.Run(() =>
            {
                foreach (var file in filesToCopy)
                    FileHelper.TryMoveFile(file.FullName, Path.Combine(destinationFolder.FullName, file.Name), true);
            });
        }

        public async Task DeleteExtractedUpgradePackageFoldersIfExits()
        {
            var naviTabUpdateFolder = new DirectoryInfo(Constants.Upgrader.PackagePath);

            await Task.Run(() =>
            {
                if (!naviTabUpdateFolder.Exists)
                    return;

                foreach (var file in naviTabUpdateFolder.GetFiles())
                    file.Delete();

                foreach (var folder in naviTabUpdateFolder.GetDirectories())
                    folder.Delete(true);
            });
        }

        public Result Move(string sourcePath, string targetPath)
        {
            try
            {

                Directory.Move(sourcePath, targetPath);
                return Result.Ok();
            }
            catch (Exception e)
            {
                return Result.Failure(e.Message);
            }
        }

        public void CopyDirectoryStructure(DirectoryInfo source, DirectoryInfo target)
        {
            if (!source.Exists)
                return;

            Directory.CreateDirectory(target.FullName);

            foreach (var fi in source.GetFiles())
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);

            foreach (var diSourceSubDir in source.GetDirectories())
                CopyDirectoryStructure(diSourceSubDir, target.CreateSubdirectory(diSourceSubDir.Name));
        }

        public List<FileInfo> SortNumericFileName(List<FileInfo> files, string fileExtension, SortOrder sortOrder)
        {
            var results = new List<FileInfo>();

            if (sortOrder == SortOrder.Ascending)
            {
                results = (from file in files
                           where file.Extension == fileExtension
                           orderby GetFileNumberFromName(file)
                           select file).ToList();
            }
            else if (sortOrder == SortOrder.Descending)
            {
                results = (from file in files
                           where file.Extension == fileExtension
                           orderby GetFileNumberFromName(file) descending
                           select file).ToList();
            }

            return results;
        }

        private int GetFileNumberFromName(FileSystemInfo file) => int.Parse(file.Name.Substring(0, file.Name.LastIndexOf(".", StringComparison.Ordinal)));

        public Result DeleteDirectoryRecursively(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Result.Failure($"{nameof(path)} is null/empty");

            if (Path.GetInvalidPathChars().Any(path.Contains))
                return Result.Failure($"[{path}] contains invalid characters. Validity checked against [{string.Join(",", Path.GetInvalidPathChars())}]");

            if (!Directory.Exists(path))
                return Result.Ok($"[{path}] is already deleted");

            try
            {
                Directory.Delete(path, true);
                return Result.Ok();
            }
            catch (Exception e)
            {
                return Result.Failure(e.Message);
            }
        }

        public async Task DeleteDirectoryAsync(string path, bool isDeleteSubfolders)
        {
            await Task.Run(() =>
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, isDeleteSubfolders);
            });
        }

        public async Task DeleteFileAsync(string path)
        {
            await Task.Run(() =>
            {
                if (File.Exists(path))
                    File.Delete(path);
            });
        }

        public Task<Result> SafelyDeleteFileAsync(string path)
        {
            return Task.Run(() => Task.FromResult(DeleteFile(path)));
        }

        public Result DeleteFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Result.Failure($"{nameof(path)} is null/empty");

            if (!File.Exists(path))
                return Result.Ok($"[{path}] is already deleted");

            try
            {
                File.Delete(path);
                return Result.Ok();
            }
            catch (Exception e)
            {
                return Result.Failure(e.Message);
            }
        }

        public void DeleteFilesByPattern(string directoryPath, string searchPattern)
        {
            foreach (var filePath in Directory.EnumerateFiles(directoryPath, searchPattern))
            {
                File.Delete(filePath);
            }
        }

        public void DeleteEmptyFoldersFromDirectory(string directoryPath)
        {
            if (directoryPath.Contains(SystemVolumeInformation))
                return;

            var directories = Directory.GetDirectories(directoryPath);

            if (!directories.Any())
                return;

            foreach (var dir in directories)
            {
                DeleteEmptyFoldersFromDirectory(dir);

                if (!dir.Contains(SystemVolumeInformation) && Directory.GetFiles(dir).Length == 0 && Directory.GetDirectories(dir).Length == 0)
                    Directory.Delete(dir, false);
            }
        }
    }
}
