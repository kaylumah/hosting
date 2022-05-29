#Requires -Version 7.2

param (
    [string]$Url = "http://localhost:4280"
)

# https://stackoverflow.com/questions/45574479/powershell-determine-new-url-of-a-permanently-moved-redirected-resource
Function Get-UrlRedirection {
  [CmdletBinding()]
  Param (
    [Parameter(Mandatory, ValueFromPipeline)] [Uri] $Url,
    [int] $MaxRedirections = 50 # Use same default as [System.Net.HttpWebRequest]
  )

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
}

Get-UrlRedirection $Url -Verbose