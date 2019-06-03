public static class FileOpener
{
    private static string GetFilterAttribute(List<FileFilterInfo> fileFilters)
    {
        if (fileFilters == null || !fileFilters.Any())
            return string.Empty;

        return string.Join("|", fileFilters.Select(x => x.FileDialogFilter));
    }

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