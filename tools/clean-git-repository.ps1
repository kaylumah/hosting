#Requires -Version 7.2

$RepoRoot = Split-Path $PSScriptRoot -Parent

$ConfirmScript = "$PSScriptRoot/confirm.ps1"
$CleanConfirmed = & $ConfirmScript -Message "Do you wish to clean your git repo?" -Caption "Selected '$RepoRoot'"
if($CleanConfirmed)
{
    Set-Location $RepoRoot
    git clean -ffdx
    git reset --hard HEAD
    Set-Location $PSScriptRoot
}