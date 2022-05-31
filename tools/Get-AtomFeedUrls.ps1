#requires -Version 7.2

param (
    [Parameter(Mandatory=$true, HelpMessage = "The URL for a XML AtomFeed file")]
    [string] $AtomFeedUrl
)

$ErrorActionPreference  = "Stop"

Write-Verbose "Attempt to download '$AtomFeedUrl'"
[System.Xml.XmlDocument] $XmlResponse = Invoke-WebRequest -Uri $AtomFeedUrl -TimeoutSec 180;

$NamespaceManager = New-Object System.Xml.XmlNamespaceManager($XmlResponse.NameTable)
$NamespaceManager.AddNamespace("atom", $XmlResponse.DocumentElement.NamespaceURI)

[System.Xml.XmlElement] $RootNode = $XmlResponse.SelectSingleNode("//atom:feed", $NamespaceManager)

if (!$RootNode)
{
    Write-Error "Response not recognized as a valid Atom RSS Feed"
}

$Urls = $RootNode.SelectNodes("//atom:entry//atom:link/@href", $NamespaceManager)
Write-Verbose "Feed has '$($Urls.Count)' urls"
$Urls | Foreach-Object { $_.Value | Out-String } 