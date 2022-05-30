
[string] $Url = "https://kaylumah.nl/feed.xml"

[System.Xml.XmlDocument] $AtomFeedResponse = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 180;
[System.Xml.XmlElement] $Feed = $AtomFeedResponse.Feed
if (!$Feed)
{
    Write-Error "Feed Element does not exist"
    exit 1
}

$Entries = $Feed.entry
ForEach($Entry in $Entries)
{
    $Link = $Entry.Link.href
    $Link
}

# $nsmgr = New-Object System.Xml.XmlNamespaceManager $xml.NameTable
# $nsmgr.AddNamespace('dns','http://embassy/schemas/dudezilla/')
# $ns = New-Object System.Xml.XmlNamespaceManager($AtomFeedResponse.NameTable)
# $ns.AddNamespace("", $AtomFeedResponse.DocumentElement.NamespaceURI)
# [System.Xml.XmlNodeList] $FeedEntries = $Feed.SelectNodes("/Entry", $ns)
# ForEach($Node in $FeedEntries)
# {
#     Write-Host "1"
#     Write-Host $Node
# }