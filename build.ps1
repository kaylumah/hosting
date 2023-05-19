param(
    [Parameter()]
    [string] $BuildId = 1,
    [Parameter()]
    [string] $BuildNumber = (Get-Date).ToString("yyyyMMdd.hhmmss"),
    [Parameter()]
    [switch] $CleanDevDependencies
)

$ErrorActionPreference = "Stop"
Write-Host "[args] BuildId '$BuildId' BuildNumber '$BuildNumber'"

[string] $RepoRoot = $PSScriptRoot
# $RepoRoot = Split-Path $PSScriptRoot -Parent
Write-Host "RepoRoot is '$RepoRoot'"

$ReportScript = "$RepoRoot/tools/test-reports.ps1"

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
    dotnet "src/Component/Client/SiteGenerator/bin/$BuildConfiguration/net7.0/Kaylumah.Ssg.Client.SiteGenerator.dll" SiteConfiguration:AssetDirectory=assets
}
else
{
    Write-Host "PullRequest Build ($PrBuildId)"
    [string] $BaseUrl="https://green-field-0353fee03-$PrBuildId.westeurope.1.azurestaticapps.net"
    dotnet "src/Component/Client/SiteGenerator/bin/$BuildConfiguration/net7.0/Kaylumah.Ssg.Client.SiteGenerator.dll" Site:Url=$BaseUrl
}

& $ReportScript

# https://docs.microsoft.com/en-us/powershell/scripting/samples/managing-current-location?view=powershell-7.2

try
{
    Set-Location $DistFolder
    npm i
    npm run build:prod
    if ($CleanDevDependencies)
    {
        Write-Host "Cleaning Up DevDependencies"
        Remove-Item styles.css
        Remove-Item package.json
        Remove-Item package-lock.json
        Remove-Item tailwind.config.js
        Remove-Item node_modules -Recurse -Force
    }
}
finally
{
    Set-Location $RepoRoot
}