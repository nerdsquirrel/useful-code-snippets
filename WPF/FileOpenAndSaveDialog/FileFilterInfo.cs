public class FileFilterInfo
{
    /// <summary>
    /// Regular expression pattern that can be used by OpenFileDialog to filter files to be
    /// viewed to the user (example: *.xml)
    /// </summary>
    public string Pattern { get; set; }

    /// <summary>
    /// User friendly name that will be shown in the dialog box as filtering criteria (example: XML)
    /// </summary>
    public string FileType { get; set; }

    /// <summary>
    /// Text that will be used to filter files by OpenFileDialog and show filtering
    /// criteria to user (example: All Files (*.*), XML (*.xml))
    /// </summary>
    public string FileDialogFilter
        => string.IsNullOrWhiteSpace(Pattern) ? string.Empty : $@"{FileType} ({Pattern})|{Pattern}";
}

public class FileInfo
{
    public string FileNameWithFullPath { get; set; }
}
