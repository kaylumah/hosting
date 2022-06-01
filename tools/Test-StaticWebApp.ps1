#requires -Version 7.2

param (
)

$ErrorActionPreference  = "Stop"

$ScriptRoot = $PSScriptRoot
$GetUrlRedirectsScript = "$ScriptRoot/Get-UrlRedirects.ps1"
$Urls = & $GetUrlRedirectsScript -Url "http://kaylumah.nl/blog"

foreach($Url in $Urls)
{
    Write-Host "$Url"
}