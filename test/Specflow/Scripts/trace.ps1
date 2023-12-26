$RepoRoot = Split-Path $PSScriptRoot -Parent
$PlaywrightScript = "$RepoRoot/bin/Debug/net8.0/playwright.ps1"

# try {
#     Push-Location "../bin/Debug/net8.0"
#     # ./playwright.ps1 show-trace trace.zip
#     $TracePath = "$PWD/trace.zip"
#     ./playwright.ps1 show-trace $TracePath
#     # Test-Path -Path trace.zip
# }
# catch {}
# finally {
#     Pop-Location
# }