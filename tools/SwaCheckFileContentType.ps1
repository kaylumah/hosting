#Requires -Version 7.2

# https://green-field-0353fee03-101.westeurope.1.azurestaticapps.net/feed.xml
$BaseUrl = "https://green-field-0353fee03-101.westeurope.1.azurestaticapps.net"

function CheckUrl()
{
    param (
        [string]$Url
    )

    #Write-Host "Checking $Url"
    #$Request = Invoke-WebRequest -Method Head -Uri $Url -SkipHttpErrorCheck
    #$Test = $Request.BaseResponse.ResponseUri.AbsoluteUri
    #$Request
    $Response = Invoke-WebRequest $Url -SkipHttpErrorCheck
    # $StatusCode = $Response.StatusCode
    # $Response.Headers
    $Headers = $Response.Headers
    $ContentType = $Headers["Content-Type"]
    Write-Host "For '$Url' ContentType = '$ContentType'"
    # Write-Host "Done Checking '$Url' ($StatusCode)"
}

# $StaticFiles = @("robots.txt", "feed.xml", "sitemap.xml")
$StaticFiles = @( "feed", "feed.xml" )
foreach($StaticFile in $StaticFiles)
{
    CheckUrl -Url "$BaseUrl/$StaticFile"

}

Write-Host "Done checking"