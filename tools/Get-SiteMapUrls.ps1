#requires -Version 7.2

param (
    [Parameter(Mandatory=$true, HelpMessage = "The URL for a XML SiteMap file")]
    [string] $SiteMapUrl
)

$ErrorActionPreference  = "Stop"

Write-Verbose "Attempt to download '$SiteMapUrl'"
[System.Xml.XmlDocument] $XmlResponse = Invoke-WebRequest -Uri $SiteMapUrl -TimeoutSec 180;

$NamespaceManager = New-Object System.Xml.XmlNamespaceManager($XmlResponse.NameTable)
$NamespaceManager.AddNamespace("sitemap", $XmlResponse.DocumentElement.NamespaceURI)

[System.Xml.XmlElement] $RootNode = $XmlResponse.SelectSingleNode("//sitemap:urlset", $NamespaceManager)

if (!$RootNode)
{
    Write-Error "Response not recognized as a valid SiteMap"
}

$Urls = $RootNode.SelectNodes("//sitemap:url//sitemap:loc/text()", $NamespaceManager)
Write-Verbose "SiteMap has '$($Urls.Count)' urls"
$Urls | Foreach-Object { $_.Value | Out-String } 