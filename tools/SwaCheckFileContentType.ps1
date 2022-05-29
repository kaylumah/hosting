#Requires -Version 7.2

$BaseUrl = "http://localhost:4280"

function CheckUrl()
{
    param (
        [string]$Url
    )

    Write-Host "Checking $Url"
    $Response = Invoke-WebRequest $Url
    $StatusCode = $Response.StatusCode
    $Headers = $Response.Headers
    $ContentType = $Headers["Content-Type"]
    Write-Host "ContentType = $ContentType"
    Write-Host "Done Checking '$Url' ($StatusCode)"
}

$StaticFiles = @("robots.txt", "feed.xml", "sitemap.xml")
foreach($StaticFile in $StaticFiles)
{
    CheckUrl -Url "$BaseUrl/$StaticFile"
}


Write-Host "Done checking"