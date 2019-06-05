public static class FileOpener
{
    private static string GetFilterAttribute(List<FileFilterInfo> fileFilters)
    {
        if (fileFilters == null || !fileFilters.Any())
            return string.Empty;

        return string.Join("|", fileFilters.Select(x => x.FileDialogFilter));
    }

    /// <summary>
    /// open file dialog for desktop application to import any file.
    /// </summary>
    /// <param name="fileFilterInfos"></param>
    /// <returns>returns file name with full name.</returns>
    public static FileInfo OpenDialogAndGetFileInfo(List<FileFilterInfo> fileFilterInfos)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = GetFilterAttribute(fileFilterInfos)
        };

        openFileDialog.ShowDialog();

        return new FileInfo
        {
            FileNameWithFullPath = openFileDialog.FileName
        };
    }
}


public static class FileSaver
{
    /// <summary>
    /// open file dialog for desktop application to save any file.
    /// </summary>
    /// <param name="fileFilterInfo"></param>
    /// <returns>returns file name with full name.</returns>
    public static FileInfo OpenDialogAndGetFileInfo(FileFilterInfo fileFilterInfo)
    {
        var openFileDialog = new SaveFileDialog
        {
            Filter = fileFilterInfo != null ? fileFilterInfo.FileDialogFilter : string.Empty
        };

        openFileDialog.ShowDialog();

        return new FileInfo
        {
            FileNameWithFullPath = openFileDialog.FileName
        };
    }
}