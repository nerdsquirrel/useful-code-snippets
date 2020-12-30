## WSL
Need new windows 10 
Check windows version - cmd -> winver 
Need to turn on wsl from windows turn on features
wsl linix distro install should be through microsoft store

```bash
wsl --list # see list of wsl installed
wslconfig /list /all # see list of wsl in older version of os
wsl -d distro-name # login to distro
explorer.exe . # to open in windows explorer
exit # to logout from the distro
wsl --unregister distro-name # uninstall a distro
```
To uninstall the wsl turn off the feature.