#Requires -Version 7.4

# See: https://git-scm.com/docs/git-diff
$diff = git diff --name-only HEAD^ HEAD
$diff

Write-Host "------------------- histogram:"
git diff --histogram HEAD^ HEAD
Write-Host "------------------- stat:"
git diff --stat HEAD^ HEAD
Write-Host "------------------- compact:"
git diff --compact-summary HEAD^ HEAD
Write-Host "------------------- summary:"
git diff --summary HEAD^ HEAD
Write-Host "------------------- name + s:"
git diff --name-status HEAD^ HEAD
