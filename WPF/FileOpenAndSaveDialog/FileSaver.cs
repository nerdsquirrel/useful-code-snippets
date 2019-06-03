public static class FileSaver
{
    /// <summary>
    /// open file dialog for windows application to save any file.
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