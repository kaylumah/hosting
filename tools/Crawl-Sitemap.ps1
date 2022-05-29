#Requires -Version 7.2

param(
    [parameter(Mandatory = $true, HelpMessage = "The URL for a XML Sitemap file")]
    [string]$Url
)

[array] $Urls = ./Download-SiteMap.ps1 -Url $Url
ForEach($Url in $Urls)
{
    Write-Host "Url = $Url"
}