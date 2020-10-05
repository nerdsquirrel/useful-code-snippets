$folders = Get-ChildItem .\ -include bin,obj -Recurse # | Where-Object -FilterScript {$_.FullName -match 'project-name' -or $folder.FullName -match 'project-name'}
ForEach($folder in $folders)
{
    Remove-Item $folder.FullName -Force -Recurse
}