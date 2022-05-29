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

      if ($Enumerate) { # Enumerate the whole redirection chain, from input URL to ultimate target,
                        # assuming the max. count of redirects is not exceeded.
        # We must walk the chain of redirections one by one.
        # If we disallow redirections, .GetResponse() fails and we must examine
        # the exception's .Response object to get the redirect target.
        $nextUrl = $Url
        $urls = @( $nextUrl.AbsoluteUri ) # Start with the input Uri
        $ultimateFound = $false
        # Note: We add an extra loop iteration so we can determine whether
        #       the ultimate target URL was reached or not.
        foreach($i in 1..$($MaxRedirections+1)) {
          Write-Verbose "Examining: $nextUrl"
          $request = [System.Net.HttpWebRequest]::Create($nextUrl)
          $request.AllowAutoRedirect = $False
          try {
            $response = $request.GetResponse()
            # Note: In .NET *Core* the .GetResponse() for a redirected resource
            #       with .AllowAutoRedirect -eq $False throws an *exception*.
            #       We only get here on *Windows*, with the full .NET Framework.
            #       We either have the ultimate target URL, or a redirection
            #       whose target URL is reflected in .Headers['Location']
            #       !! Syntax `.Headers.Location` does NOT work.
            $nextUrlStr = $response.Headers['Location']
            $response.Close()
            # If the ultimate target URL was reached (it was already
            # recorded in the previous iteration), and if so, simply exit the loop.
            if (-not $nextUrlStr) {
              $ultimateFound = $true
              break
            }
          } catch [System.Net.WebException] {
            # The presence of a 'Location' header implies that the
            # exception must have been triggered by a HTTP redirection 
            # status code (3xx). 
            # $_.Exception.Response.StatusCode contains the specific code
            # (as an enumeration value that can be case to [int]), if needed.
            # !! Syntax `.Headers.Location` does NOT work.
            $nextUrlStr = try { $_.Exception.Response.Headers['Location'] } catch {}
            # Not being able to get a target URL implies that an unexpected
            # error ocurred: re-throw it.
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
        # Output the array of URLs (chain of redirections) as a *single* object.
        Write-Output -NoEnumerate $urls
        if (-not $ultimateFound) { Write-Warning "Enumeration of $Url redirections ended before reaching the ultimate target." }

      } else { # Resolve just to the ultimate target,
                # assuming the max. count of redirects is not exceeded.

                # Note that .AllowAutoRedirect defaults to $True.
        # This will fail, if there are more redirections than the specified 
        # or default maximum.
        $request = [System.Net.HttpWebRequest]::Create($Url)
        if ($PSBoundParameters.ContainsKey('MaxRedirections')) {
          $request.MaximumAutomaticRedirections = $MaxRedirections
        }
        $response = $request.GetResponse()
        # Output the ultimate target URL.
        # If no redirection was involved, this is the same as the input URL.
        $response.ResponseUri.AbsoluteUri
        $response.Close()

       }

      } catch {
        Write-Error $_ # Report the exception as a non-terminating error.
    }
  } # process

}

Get-UrlRedirection -Enumerate $Url -Verbose