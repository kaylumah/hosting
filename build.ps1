param(
    [Parameter()]
    [string] $BuildId = (Get-Date).ToString("yyyyMMddhhmmss"),
    [Parameter()]
    [string] $BuildNumber = 2810
)

$ErrorActionPreference = "Stop"
[string] $RepoRoot = $PSScriptRoot
[string] $BuildConfiguration = "Release"
[string] $TargetFramework = "net9.0"
[string] $BaseUrl = ./tools/Get-BaseUrl.ps1

Write-Host "Using build-id '$BuildId'"
Write-Host "Using build-number '$BuildNumber'"
Write-Host "Using configuration '$BuildConfiguration'"
Write-Host "Using framework '$TargetFramework'"
Write-Host "Using base url '$BaseUrl'"

[string] $DistFolder = "$RepoRoot/dist"
if (Test-Path $DistFolder)
{ 
    Write-Host "dist folder from previous run exists, removing now."
    Remove-Item $DistFolder -Recurse -Force
}

dotnet restore
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Restore Failure"
}

dotnet build --verbosity detailed --no-restore --configuration $BuildConfiguration /p:BuildId=$BuildId /p:BuildNumber=$BuildNumber /p:Version=1.0.0.$BuildNumber
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Build Failure"
}


dotnet format --verify-no-changes
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Formatting Failure"
}

# https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test
dotnet test --no-restore --no-build --configuration $BuildConfiguration ./test/Unit/Test.Unit.csproj
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Test Failure"
}

dotnet "src/Component/Client/SiteGenerator/bin/$BuildConfiguration/$TargetFramework/Kaylumah.Ssg.Client.SiteGenerator.dll" Site:Url=$BaseUrl
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Run Failure"
}

# https://docs.microsoft.com/en-us/powershell/scripting/samples/managing-current-location?view=powershell-7.2
try
{
    # npm run build:tailwind
    npm run build:prod
    & "./optimize.ps1"
}
finally
{
    Set-Location $RepoRoot
}