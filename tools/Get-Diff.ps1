#Requires -Version 7.4

# See: https://git-scm.com/docs/git-diff
$DiffOutput = git diff --name-only HEAD^ HEAD
# git diff --stat HEAD^ HEAD

$NameAndStatus = git diff --name-status HEAD^ HEAD
# foreach ($Item in $NameAndStatus)
# {
#     Write-Host $Item
# }

$AssetFolderDiff = $DiffOutput | Where-Object { $_ -match '^_site/assets/' }
$AssestFolderChanged = $AssetFolderDiff.Length -gt 0

[PSCustomObject]@{
    Raw = $DiffOutput
    ChangedFiles = $NameAndStatus
    Assets = $AssestFolderChanged
}