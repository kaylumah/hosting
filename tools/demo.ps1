#Requires -Version 7.2

param (
    [string]$Url
)

# $RepoRoot = Split-Path $PSScriptRoot -Parent
# $DistDirectory = "$RepoRoot/dist"

# [System.Net.HttpWebRequest]::Create('http://localhost:4280/feed').GetResponse().ResponseUri.AbsoluteUri

# $url="https://jigsaw.w3.org/HTTP/300/301.html"
# $resp = Invoke-WebRequest -Method HEAD $url -MaximumRedirection 0 -ErrorAction Ignore
# $code = $resp.StatusCode
# Write-Output "URL: $url"
# Write-Output "ErrorCode: $code"
# if($code -eq 301) {
#     $loc = $resp.Headers.Location
#     Write-Output "New URL: $loc"
# }

# https://stackoverflow.com/questions/45574479/powershell-determine-new-url-of-a-permanently-moved-redirected-resource
Function Get-UrlRedirection {
  [CmdletBinding()]
  Param (
    [Parameter(Mandatory, ValueFromPipeline)] [Uri] $Url,
    [switch] $Enumerate,
    [int] $MaxRedirections = 50 # Use same default as [System.Net.HttpWebRequest]
  )

  process {
    try {

      if ($Enumerate) {
        $nextUrl = $Url
        $urls = @( $nextUrl.AbsoluteUri ) # Start with the input Uri
        $ultimateFound = $false
        foreach($i in 1..$($MaxRedirections+1)) {
          Write-Verbose "Examining: $nextUrl"
          $request = [System.Net.HttpWebRequest]::Create($nextUrl)
          $request.AllowAutoRedirect = $False
          try {
            $response = $request.GetResponse()
            $nextUrlStr = $response.Headers['Location']
            $response.Close()
            if (-not $nextUrlStr) {
              $ultimateFound = $true
              break
            }
          } catch [System.Net.WebException] {
            $nextUrlStr = try { $_.Exception.Response.Headers['Location'] } catch {}
            if (-not $nextUrlStr) { Throw }
          }
          Write-Verbose "Raw target: $nextUrlStr"
          if ($nextUrlStr -match '^https?:') { # absolute URL
            $nextUrl = $prevUrl = [Uri] $nextUrlStr
          } else { # URL without scheme and server component
            $nextUrl = $prevUrl = [Uri] ($prevUrl.Scheme + '://' + $prevUrl.Authority + $nextUrlStr)
          }
          if ($i -le $MaxRedirections) { $urls += $nextUrl.AbsoluteUri }          
        }
        Write-Output -NoEnumerate $urls
        if (-not $ultimateFound) { Write-Warning "Enumeration of $Url redirections ended before reaching the ultimate target." }

      } else {
        $request = [System.Net.HttpWebRequest]::Create($Url)
        if ($PSBoundParameters.ContainsKey('MaxRedirections')) {
          $request.MaximumAutomaticRedirections = $MaxRedirections
        }
        $response = $request.GetResponse()
        $response.ResponseUri.AbsoluteUri
        $response.Close()

       }

      } catch {
        Write-Error $_ # Report the exception as a non-terminating error.
    }
  } # process

}

Get-UrlRedirection -Enumerate $Url -Verbose