param(
    [Parameter()]
    [string] $BuildId = 1,
    [Parameter()]
    [string] $BuildNumber = (Get-Date).ToString("yyyyMMdd.hhmmss"),
    [Parameter()]
    [switch] $CleanDevDependencies
)

$ErrorActionPreference = "Stop"
[string] $RepoRoot = $PSScriptRoot
[string] $BuildConfiguration = "Release"
[string] $TargetFramework = "net8.0"
[string] $PrBuildId = $env:PR_BUILD_ID
[string] $BaseUrl = ![string]::IsNullOrEmpty($PrBuildId) ? "https://green-field-0353fee03-$PrBuildId.westeurope.1.azurestaticapps.net" : "https://kaylumah.nl"

Write-Host "Using build-id '$BuildId'"
Write-Host "Using build-number '$BuildNumber'"
Write-Host "Using configuration '$BuildConfiguration'"
Write-Host "Using framework '$TargetFramework'"
Write-Host "Using base url '$BaseUrl'"

$ReportScript = "$RepoRoot/tools/test-reports.ps1"

[string] $DistFolder = "$RepoRoot/dist"
if (Test-Path $DistFolder)
{ 
    Write-Host "dist folder from previous run exists, removing now."
    Remove-Item $DistFolder -Recurse -Force
}

dotnet restore
dotnet build --configuration $BuildConfiguration --no-restore /p:BuildId=$BuildId /p:BuildNumber=$BuildNumber
# https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test
dotnet test ./test/Specflow/Test.Unit.csproj --configuration $BuildConfiguration
# dotnet test --configuration $BuildConfiguration --no-build --verbosity normal /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=TestResults/lcov.info
& $ReportScript -BuildConfiguration $BuildConfiguration

dotnet "src/Component/Client/SiteGenerator/bin/$BuildConfiguration/$TargetFramework/Kaylumah.Ssg.Client.SiteGenerator.dll" Site:Url=$BaseUrl

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