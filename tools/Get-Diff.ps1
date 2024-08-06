#Requires -Version 7.4

# See: https://git-scm.com/docs/git-diff
$DiffOutput = git diff --name-only HEAD^ HEAD

$AssetFolderDiff = $diff | Where-Object { $_ -match '^assets/' }
$AssestFolderChanged = $AssetFolderDiff.Length -gt 0

# git diff --stat HEAD^ HEAD
# git diff --name-status HEAD^ HEAD

# Example check with regex
# $SiteFolderDiff = $diff | Where-Object { $_ -match '^_site/' }
# $HasSite = $SiteFolderDiff.Length -gt 0
# $HasSite

[PSCustomObject]@{
    ChangedFiles = $DiffOutput
    Assets = $AssestFolderChanged
}