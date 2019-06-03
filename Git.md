## useful git command
1. git initials:
  ```bash
  git init # initialize an empty git repository
  git remote # to check what remote/source you have
  git remote add origin "repository link" # add remote
  git remote rm <existing-remote-name> # delete existing remote
  git remote set-url origin https://x.y.x # overwrite existing remote origin
  git config --list # check current git config
  git config user.name "username" # set username
  git fetch && git checkout branch-name # fetch the reposity and go to working branch
  # press q to exit from command
  ```
2. git save changes:
```bash
git add -A .  # new added, modified, deleted
git add -u  # modified and deleted
git reset # remove all add
git reset <file-name> 
```
3. git stashing: 
```bash
  git stash   # stash all current changes and push to stash stack
  git stash save "stash_name"   # saving with a name
  git stash list   # show stash list
  git stash apply  # stash is a stack. apply the top one
  git stash apply stash@{n}   # To apply a stash and keep it in the stash stack
  git stash drop   # stash is a stack. drop the top one
  git stash drop stash@{n} # To drop a specific stash from list
  git stash pop stash@{n}   # To apply a stash and remove it from the stash stack
  git stash clear  # clear all stash
  ``` 
4. git branching: 
```bash
  git checkout -b branch-name   # create new branch
  git checkout branch-name  # move to an existing brance
  git branch  # to see all branch
  git branch -d branch-name  # to delete a branch
  git checkout --track origin/newsletter # checkout to remote branch
  git branch --set-upstream-to <remote-branch> # set remote branch
  ```  
  5. git reset:
  ```bash
  git reset --hard HEAD
  git reset -- main/*
  git reset -- main/dontcheckmein.txt
  ```
  6. git untrack a file in local repo only and keep it in the remote repo
  ```bash
  git update-index --assume-unchanged nbproject/project.properties   # untrack the project.properties
  git update-index --no-assume-unchanged  # revert the st
  git ls-files -v | grep '^h '  # see list of untracked file  
  ```

  7. git commit:
  ```bash
  git log # see commit logs
  git reset --soft HEAD~1 # undo last local commit
  git reset --hard HEAD~ # undo all local commits
  git reset --hard @{u} # undo all local changes and align with remote
  ```

  8. git merge:
  ```bash
  git merge <:branch_you_want_to_merge:> # first move to branch you working
  ```

