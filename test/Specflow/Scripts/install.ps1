$RepoRoot = Split-Path $PSScriptRoot -Parent
$PlaywrightScript = "$RepoRoot/bin/Debug/net8.0/playwright.ps1"
& $PlaywrightScript install