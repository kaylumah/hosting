#requires -Version 7.2

param (
    [uri] $BaseUrl
)

$ErrorActionPreference  = "Stop"

$ScriptRoot = $PSScriptRoot
$GetUrlRedirectsScript = "$ScriptRoot/Get-UrlRedirects.ps1"
$GetUrlContentTypeScript = "$ScriptRoot/Get-UrlContentType.ps1"

$HttpBaseUrl = "http://$($BaseUrl.Authority)"
$TestUrl = "$HttpBaseUrl/blog"

$Urls = & $GetUrlRedirectsScript -Url $TestUrl
$FinalRedirectUrl = $($Urls | Select-Object -Last 1).Trim()
$ContentType = $(& $GetUrlContentTypeScript -Url $FinalRedirectUrl).Trim()

Write-Host "For '$TestUrl' the resolved url is '$FinalRedirectUrl' with content-type '$ContentType'"