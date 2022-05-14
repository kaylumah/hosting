param(
    [Parameter()]
    [string] $BuildId = 1,
    [Parameter()]
    [string] $BuildNumber = (Get-Date).ToString("yyyyMMdd.hhmmss")
)

Write-Host "BuildId '$BuildId' BuildNumber '$BuildNumber'"

$PrBuildId = $env:PR_BUILD_ID

Write-Host "PRBuild is '$PrBuildId'"