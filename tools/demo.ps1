#Requires -Version 7.2

param (
    [string]$Url = "http://localhost:4280"
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

  Write-Host "$StartUrl"
  foreach($item in $ResolvedUrls)
  {
    Write-Host "$item"
  }
}

# "http://microsoft.com/about"
$TestUrls = @("$Url/feed")
foreach($TestUrl in $TestUrls)
{
  Write-Host "Begin Testing $TestUrl"
  TestRedirects -StartUrl $TestUrl
}


