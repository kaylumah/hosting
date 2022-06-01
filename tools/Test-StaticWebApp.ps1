#requires -Version 7.2

param (
)

$ErrorActionPreference  = "Stop"

$ScriptRoot = $PSScriptRoot
$GetUrlRedirectsScript = "$ScriptRoot/Get-UrlRedirects.ps1"
$GetUrlContentTypeScript = "$ScriptRoot/Get-UrlContentType.ps1"


$TestUrl = "http://kaylumah.nl/blog"
$Urls = & $GetUrlRedirectsScript -Url $TestUrl

$FinalRedirectUrl = $($Urls[$Urls.Count -1]).Trim()
$ContentType = $(& $GetUrlContentTypeScript -Url $FinalRedirectUrl).Trim()

Write-Host "For '$TestUrl' the resolved url is '$FinalRedirectUrl' with content-type '$ContentType'"