#Requires -Version 7.4

# See: https://git-scm.com/docs/git-diff
$diff = git diff --name-only HEAD^ HEAD
$diff

Write-Host "-------------------"
git diff --histogram HEAD^ HEAD