param (
  [string]$CoverageRoot
)

Write-Host "Coverage Search '$CoverageRoot'"

$latest = Get-ChildItem -Recurse -Path $CoverageRoot -Filter coverage.cobertura.xml |
            Sort-Object LastWriteTime -Descending |
            Select-Object -First 1

if ($latest) {
    $xmlDir = $latest.Directory.FullName
    Write-Host "✅ Found coverage file at: $($latest.FullName)"
    Write-Host "📂 Outputting HTML report to: $xmlDir/HtmlCoverage"

    dotnet reportgenerator `
        -reports:$latest.FullName `
        -targetdir:"$xmlDir/HtmlCoverage" `
        -reporttypes:Html
} else {
  Write-Host "⚠ No coverage file found in $CoverageRoot"
}