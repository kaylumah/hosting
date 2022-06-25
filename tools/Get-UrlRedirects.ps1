#requires -Version 7.2

param (
    [Parameter(Mandatory=$true, HelpMessage = "The URL to check")]
    [string] $Url
)

$ErrorActionPreference  = "Stop"

[System.Collections.ArrayList] $Urls = @()
$ContinueLoop = $false
[uri] $RequestUrl = $Url
Do
{
    # Write-Host "Requesting '$RequestUrl'"
    $Response = Invoke-WebRequest -Method HEAD $RequestUrl -MaximumRedirection 0 -ErrorAction Ignore -SkipHttpErrorCheck
    $ResponseStatusCode = $Response.StatusCode
    # Write-Host "Status for '$RequestUrl' is '$ResponseStatusCode'"
    if ($ResponseStatusCode -eq 301 -or $ResponseStatusCode -eq 302)
    {
        [string] $ResolvedLocation = $Response.Headers.Location
        if ($ResolvedLocation.StartsWith("/"))
        {
            # Write-Host "Location '$ResolvedLocation' is relative"
            $ResolvedLocation = $RequestUrl.AbsoluteUri -Replace $RequestUrl.AbsolutePath, $ResolvedLocation
        }
        # Write-Host "Redirect Location is '$ResolvedLocation'"
        $Urls.Add($ResolvedLocation) > $null
        $RequestUrl = $ResolvedLocation
        $ContinueLoop = $true
    }
    else
    {
        $ContinueLoop = $false
    }
}
While ($ContinueLoop)

$Urls | Foreach-Object { $_ | Out-String } 