#requires -Version 7.2

param (
    [Parameter(Mandatory=$true, HelpMessage = "The base URL for website")]
    [string] $BaseUrl
)

$ErrorActionPreference  = "Stop"
$ScriptRoot = $PSScriptRoot

$SiteMapUrl = "$BaseUrl/sitemap.xml"
$SiteMapUrlsScript = "$ScriptRoot/Get-SiteMapUrls.ps1"
$SiteMapUrls = & $SiteMapUrlsScript -SiteMapUrl $SiteMapUrl

# https://www.bing.com/indexnow
$Body = @{
    "host" = $BaseUrl;
    "urlList" = $SiteMapUrls;
    "key" = "34aee63bdbd3438894a5e479e0fd7b25";
}
$Headers = @{
    'Content-Type'='application/json; charset=utf-8';
}
Invoke-WebRequest -Method 'Post' -Uri "https://www.bing.com/indexnow" -Body ($Body | ConvertTo-Json) -Headers $Headers