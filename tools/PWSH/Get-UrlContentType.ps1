#requires -Version 7.2

param (
    [Parameter(Mandatory=$true, HelpMessage = "The URL to check")]
    [string] $Url
)

$ErrorActionPreference  = "Stop"

$Response = Invoke-WebRequest $Url -SkipHttpErrorCheck
$Headers = $Response.Headers
$ContentType = $Headers["Content-Type"]
$ContentType | Out-String