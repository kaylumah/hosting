#Requires -Version 7.2

param(
    [parameter(Mandatory = $true, HelpMessage = "The URL for a XML Sitemap file")]
    [string]$Url
)

[xml] $SiteMap = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 180;
$UrlSet = $SiteMap.urlset
$Nodes = $UrlSet.ChildNodes
ForEach($Node in $Nodes)
{
    $Location = $Node.loc
    Write-Output "$Location"
}