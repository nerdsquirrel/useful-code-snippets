using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Utility
{
    public interface IDirectoryUtility
    {
        Task<DirectoryInfo> CreateFolderIfNotExistAsync(string folderNameWithPath);

        Task<Result> CreateFoldersIfNotExistAsync(params string[] folderNameWithPaths);

        Task<DirectoryInfo> RecreateFolderIntoPath(string folderNameToRecreate, string containerPath);

        Task CopyFilesToFolder(List<FileInfo> filesToCopy, DirectoryInfo destinationFolder);

        Task DeleteExtractedUpgradePackageFoldersIfExits();

        void CopyDirectoryStructure(DirectoryInfo source, DirectoryInfo target);

        Result Move(string sourcePath, string targetPath);

        List<FileInfo> SortNumericFileName(List<FileInfo> files, string fileExtension, SortOrder sortOrder);

        Task DeleteDirectoryAsync(string path, bool isDeleteSubfolders);
       
        Result DeleteDirectoryRecursively(string path);

        Task DeleteFileAsync(string path);

        Result DeleteFile(string path);

        Task<Result> SafelyDeleteFileAsync(string path);

        void DeleteFilesByPattern(string directoryPath, string searchPattern);

        void DeleteEmptyFoldersFromDirectory(string directoryPath);
    }
}
