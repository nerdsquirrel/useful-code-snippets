### Open dialog for windows application to open or import any file with approprite filtering 

```csharp
var filters = new List<FileFilterInfo>
{
    new FileFilterInfo
    {
        FileType = "text files",
        Pattern = "*.txt"
    },
    new FileFilterInfo
    {
        FileType = "All files",
        Pattern = "*.*"
    }
};

var rtzFilePath = FileOpener.OpenDialogAndGetFileInfo(filters)?.FileNameWithFullPath;
```
### Open dialog for windows application to save file with approprite filtering 

```csharp
var rtzFilePath = FileSaver.OpenDialogAndGetFileInfo(new FileFilterInfo
            {
                FileType = "text file",
                Pattern = "*.txt"
            })?.FileNameWithFullPath;
```