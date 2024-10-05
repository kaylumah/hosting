#requires -Version 7.2

param (
    [Parameter(Mandatory=$true, HelpMessage = "The URL for a XML SiteMapIndex file")]
    [string] $SiteMapIndexUrl
)

$ErrorActionPreference  = "Stop"

Write-Verbose "Attempt to download '$SiteMapIndexUrl'"
[System.Xml.XmlDocument] $XmlResponse = Invoke-WebRequest -Uri $SiteMapIndexUrl -TimeoutSec 180;

$NamespaceManager = New-Object System.Xml.XmlNamespaceManager($XmlResponse.NameTable)
$NamespaceManager.AddNamespace("sitemapindex", $XmlResponse.DocumentElement.NamespaceURI)

[System.Xml.XmlElement] $RootNode = $XmlResponse.SelectSingleNode("//sitemapindex:sitemap", $NamespaceManager)

if (!$RootNode)
{
    Write-Error "Response not recognized as a valid SiteMapIndex"
}

$Urls = $RootNode.SelectNodes("//sitemapindex:sitemap//sitemapindex:loc/text()", $NamespaceManager)
Write-Verbose "SiteMap has '$($Urls.Count)' urls"
$Urls | Foreach-Object { $_.Value | Out-String } 