#Requires -Version 7.4

param (
    [string] $TargetFolder = "."
)

$RepoRoot = Split-Path $PSScriptRoot -Parent
$Pattern = "*.received.*"

$ReceivedFiles = Get-ChildItem -Path "$TargetFolder/$Pattern" -Recurse | Select-Object -Expand FullName
$ExpectedVerifiedFiles = $ReceivedFiles | foreach {$_ -replace "received", "verified"}
$VerifiedFiles = @()

foreach ($File in $ExpectedVerifiedFiles) {
    if (Test-Path -Path $File) {
        $VerifiedFiles += $File
    }
    else {
        Write-Warning "Expected '$File' to exist"
    }
}

$Combined = $($ReceivedFiles; $VerifiedFiles)
$Result = $Combined | foreach { $_.Replace($RepoRoot, "") }
$Result