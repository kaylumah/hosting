#Requires -Version 7.2

param(
    [string]$Url = "https://green-field-0353fee03-101.westeurope.1.azurestaticapps.net/sitemap.xml"
)

# https://gist.github.com/eNeRGy164/a644417b737eb5d3af3c80d4ceb527e1
# https://swimburger.net/blog/powershell/powershell-snippet-crawling-a-sitemap

[xml] $SiteMap = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 180;
$UrlSet = $SiteMap.urlset
$Nodes = $UrlSet.ChildNodes
ForEach($Node in $Nodes)
{
    $Location = $Node.loc
    Write-Host "$Location"
}