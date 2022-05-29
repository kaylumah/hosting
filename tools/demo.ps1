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
  if($code -eq 302) {
    $Location = $resp.Headers.Location
    Write-Output $Location
  }
}


GetResolvedUrl -RequestUrl "$Url/feed"


