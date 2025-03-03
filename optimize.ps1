# Function to get size of each file in KB
function Get-FolderSize($path) {
    Get-ChildItem -Path $path -Recurse -File | ForEach-Object {
        $fileSize = $_.Length / 1KB  # Convert bytes to KB
        [PSCustomObject]@{
            File = $_.FullName
            "Size (KB)" = [math]::Round($fileSize, 2)  # Round to 2 decimal places
        }
    }
}

# Measure Size Before
$sizeBeforeList = Get-FolderSize "dist"
$sizeBeforeTotal = ($sizeBeforeList | Measure-Object -Property "Size (KB)" -Sum).Sum
Write-Output "🔍 Size Before Optimization (Total: $sizeBeforeTotal KB):"
# $sizeBeforeList | Format-Table -AutoSize

# Optimize JavaScript Files (One by One)
Get-ChildItem -Path "dist" -Recurse -Filter "*.js" -File | ForEach-Object {
    $file = $_.FullName
    Write-Output "⚡ Minifying JS: $file"
    npx terser $file --compress --mangle --output $file
}

# Optimize HTML Files (One by One)
Get-ChildItem -Path "dist" -Recurse -Filter "*.html" -File | ForEach-Object {
    $file = $_.FullName
    Write-Output "⚡ Minifying HTML: $file"
    npx html-minifier-terser --collapse-whitespace --remove-comments --minify-css true --minify-js true --input-dir (Split-Path -Path $file) --output-dir (Split-Path -Path $file) --file-ext html
}

# Measure Size After
$sizeAfterList = Get-FolderSize "dist"
$sizeAfterTotal = ($sizeAfterList | Measure-Object -Property "Size (KB)" -Sum).Sum
Write-Output "✅ Size After Optimization (Total: $sizeAfterTotal KB):"
# $sizeAfterList | Format-Table -AutoSize

# Calculate Reduction Per File
$optimizationResults = $sizeBeforeList | ForEach-Object {
    $file = $_.File
    $beforeSize = $_."Size (KB)"
    $afterFile = $sizeAfterList | Where-Object { $_.File -eq $file }
    if ($afterFile) {
        $afterSize = $afterFile."Size (KB)"
        $reduction = $beforeSize - $afterSize
        $reductionPercent = ($reduction / $beforeSize) * 100
        [PSCustomObject]@{
            File = $file
            "Size Before (KB)" = $beforeSize
            "Size After (KB)" = $afterSize
            "Reduction (KB)" = [math]::Round($reduction, 2)
            "Reduction (%)" = [math]::Round($reductionPercent, 2)
        }
    }
}

# Export to CSV
$csvFile = "dist_optimization_report.csv"
$optimizationResults | Export-Csv -Path $csvFile -NoTypeInformation
Write-Output "📂 CSV Report Saved: $csvFile"

# Display results in PowerShell
Write-Output "📉 Size Reduction Per File:"
$optimizationResults | Format-Table -AutoSize

# Total Reduction Percentage
$percentageSaved = (($sizeBeforeTotal - $sizeAfterTotal) / $sizeBeforeTotal) * 100
Write-Output "📉 Total Reduction: $([math]::Round($sizeBeforeTotal - $sizeAfterTotal, 2)) KB saved ($([math]::Round($percentageSaved, 2))%)"