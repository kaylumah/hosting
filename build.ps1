param(
    [Parameter()]
    [string] $BuildId = 1,
    [Parameter()]
    [string] $BuildNumber = (Get-Date).ToString("yyyyMMdd.hhmmss")
)

$RepoRoot = $PSScriptRoot
# $RepoRoot = Split-Path $PSScriptRoot -Parent

Write-Host "BuildId '$BuildId' BuildNumber '$BuildNumber'"

$PrBuildId = $env:PR_BUILD_ID

Write-Host "PRBuild is '$PrBuildId'"
Write-Host "RepoRoot is '$RepoRoot'"
$DistFolder = "$RepoRoot/dist"

if (Test-Path $DistFolder)
{ 
    Write-Host "dist folder from previous run exists, removing now."
    Remove-Item $DistFolder -Recurse -Force
}

dotnet restore