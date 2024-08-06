#Requires -Version 7.4

# See: https://git-scm.com/docs/git-diff
$diff = git diff --name-only HEAD^ HEAD
$diff

git diff --stat HEAD^ HEAD
git diff --name-status HEAD^ HEAD

# Example check with regex
# $SiteFolderDiff = $diff | Where-Object { $_ -match '^_site/' }
# $HasSite = $SiteFolderDiff.Length -gt 0
# $HasSite