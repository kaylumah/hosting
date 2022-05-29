#Requires -Version 7.2

param (
    [string]$Url
)

# $RepoRoot = Split-Path $PSScriptRoot -Parent
# $DistDirectory = "$RepoRoot/dist"

# [System.Net.HttpWebRequest]::Create('http://localhost:4280/feed').GetResponse().ResponseUri.AbsoluteUri

# $url="https://jigsaw.w3.org/HTTP/300/301.html"
$resp = Invoke-WebRequest -Method HEAD $url -MaximumRedirection 0 -ErrorAction Ignore
$code = $resp.StatusCode
Write-Output "URL: $url"
Write-Output "ErrorCode: $code"
if($code -eq 301) {
    $loc = $resp.Headers.Location
    Write-Output "New URL: $loc"
}