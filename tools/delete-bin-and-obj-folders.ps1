#Requires -Version 7.2

$RepoRoot = Split-Path $PSScriptRoot -Parent

$ConfirmScript = "$PSScriptRoot/confirm.ps1"
$CleanConfirmed = & $ConfirmScript -Message "Do you wish to delete bin/obj folders?" -Caption "Selected '$RepoRoot'"

if($CleanConfirmed)
{
    Write-Host "Going to clean '$RepoRoot'"
    Get-ChildItem $RepoRoot -include bin,obj -Recurse | ForEach-Object ($_) { Remove-Item $_.FullName -Force -Recurse }
}