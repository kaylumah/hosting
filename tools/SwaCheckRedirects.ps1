#Requires -Version 7.2

param (
    [string]$Url = "http://localhost:4280"
    # [string]$Url = "http://green-field-0353fee03-101.westeurope.1.azurestaticapps.net"
)

# https://stackoverflow.com/questions/45574479/powershell-determine-new-url-of-a-permanently-moved-redirected-resource

Function GetResolvedUrl()
{
  param (
    [string]$RequestUrl
  )
  $resp = Invoke-WebRequest -Method HEAD $RequestUrl -MaximumRedirection 0 -ErrorAction Ignore -SkipHttpErrorCheck
  $code = $resp.StatusCode
  if($code -eq 301 -or $code -eq 302) {
    $Location = $resp.Headers.Location
    if ($Location.StartsWith("/"))
    {
      Write-Host "Location is relative for '$RequestUrl'"
      $TempUri = [System.Uri]$RequestUrl
      $Location = $TempUri.AbsoluteUri -Replace $TempUri.AbsolutePath, $Location
    }
    Write-Output $Location
  }
}

Function TestRedirects()
{
  param (
    [string]$StartUrl
  )

  $CurrentUrl = $StartUrl
  $ContinueLoop = $true
  [System.Collections.ArrayList] $ResolvedUrls = @()
  while($ContinueLoop)
  {
    $Result = GetResolvedUrl -RequestUrl $CurrentUrl
    if ([string]::IsNullOrEmpty($Result))
    {
      $ContinueLoop = $false
    }
    else
    {
      $ResolvedUrls.Add($Result) > $null
      $CurrentUrl = $Result
    }
  }

  if ($ResolvedUrls.Count -gt 0)
  {
    Write-Host "'$StartUrl' was redirected:"
    foreach($item in $ResolvedUrls)
    {
      Write-Host "'$item'"
    } 
  }
  else
  {
    Write-Host "No redirects for '$StartUrl'"
  }
}

# "http://microsoft.com/about"
$TestUrls = @(
  "$Url/feed",
  "$Url/sitemap",
  "$Url"
)
Write-Host ""
foreach($TestUrl in $TestUrls)
{
  Write-Host "Begin Testing '$TestUrl'"
  TestRedirects -StartUrl $TestUrl
  Write-Host ""
}


