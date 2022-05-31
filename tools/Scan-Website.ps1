#requires -Version 7.2

param (
    # [Parameter(Mandatory=$true, HelpMessage = "The base URL for website")]
    # [string] $BaseUrl
)
$BaseUrl = "https://kaylumah.nl"
$ErrorActionPreference  = "Stop"
$ScriptRoot = $PSScriptRoot

$AtomUrl = "$BaseUrl/feed.xml"
Write-Host "========================================"
Write-Host "Scan: $AtomUrl"
Write-Host "========================================"
$AtomFeedUrlsScript = "$ScriptRoot/Get-AtomFeedUrls.ps1"
$AtomFeedUrls = & $AtomFeedUrlsScript -AtomFeedUrl $AtomUrl
foreach ($Url in $AtomFeedUrls)
{
   Write-Host "$Url"
}

Write-Host ""

$SiteMapUrl = "$BaseUrl/sitemap.xml"
Write-Host "========================================"
Write-Host "Scan: $SiteMapUrl"
Write-Host "========================================"
$SiteMapUrlsScript = "$ScriptRoot/Get-SiteMapUrls.ps1"
$SiteMapUrls = & $SiteMapUrlsScript -SiteMapUrl $SiteMapUrl
foreach ($Url in $SiteMapUrls)
{
   Write-Host "$Url"
}