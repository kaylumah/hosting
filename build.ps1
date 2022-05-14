param(
    [Parameter()]
    [string] $BuildId = 1,
    [Parameter()]
    [string] $BuildNumber = (Get-Date).ToString("yyyyMMdd.hhmmss")
)

Write-Host "[args] BuildId '$BuildId' BuildNumber '$BuildNumber'"

[string] $RepoRoot = $PSScriptRoot
# $RepoRoot = Split-Path $PSScriptRoot -Parent
Write-Host "RepoRoot is '$RepoRoot'"

[string] $DistFolder = "$RepoRoot/dist"
if (Test-Path $DistFolder)
{ 
    Write-Host "dist folder from previous run exists, removing now."
    Remove-Item $DistFolder -Recurse -Force
}


[string] $BuildConfiguration = "Release"
dotnet restore
dotnet build --configuration $BuildConfiguration --no-restore /p:BuildId=$BuildId /p:BuildNumber=$BuildNumber
dotnet test --configuration $BuildConfiguration --no-build --verbosity normal /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=TestResults/lcov.info

[string] $PrBuildId = $env:PR_BUILD_ID
if ([string]::IsNullOrEmpty($PrBuildId))
{
    Write-Host "Production Build"
    dotnet "artifacts/bin/Kaylumah.Ssg.Client.SiteGenerator/$BuildConfiguration/net6.0/Kaylumah.Ssg.Client.SiteGenerator.dll" SiteConfiguration:AssetDirectory=assets
}
else
{
    Write-Host "PullRequest Build ($PrBuildId)"
    [string] $BaseUrl="https://green-field-0353fee03-$PrBuildId.westeurope.1.azurestaticapps.net"
    dotnet "artifacts/bin/Kaylumah.Ssg.Client.SiteGenerator/$BuildConfiguration/net6.0/Kaylumah.Ssg.Client.SiteGenerator.dll" Site:Url=$BaseUrl
}