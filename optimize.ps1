$BlockList = @()

$distFolder = "dist"
$distRoot = (Resolve-Path $distFolder).Path
$allFiles = Get-ChildItem -Path $distFolder -Recurse -File | Where-Object {
    $relativePath = $_.FullName -replace [regex]::Escape($distRoot), "/$distFolder"
    if ($BlockList -notcontains $relativePath) {
        $_ | Add-Member -MemberType NoteProperty -Name "RelativePath" -Value $relativePath -Force
        $_  # ✅ Only return the item if it's NOT in the blocklist
    }
}

function Get-FolderSize($path) {
    Get-ChildItem -Path $path -Recurse -File | ForEach-Object {
        # Convert bytes to KB
        $fileSize = $_.Length / 1KB  
        [PSCustomObject]@{
            File = $_.FullName
            "Size (KB)" = [math]::Round($fileSize, 2)  # Round to 2 decimal places
        }
    }
}

function Clean-JsFiles()
{
    $files = $allFiles | Where-Object { $_.Extension -eq ".js" }
    foreach ($file in $files) {
        Write-Output "⚡ Minifying JS: $( $file.RelativePath )"
        npx terser $file.FullName --compress --mangle --output $file.FullName
    }
}

function Clean-CssFiles()
{
    $files = $allFiles | Where-Object { $_.Extension -eq ".css" }
    foreach ($file in $files) {
        Write-Output "⚡ Minifying CSS: $( $file.RelativePath )"
        npx csso-cli $file.FullName --output $file.FullName
    }
}

function Clean-HtmlFiles()
{
    $files = $allFiles | Where-Object { $_.Extension -eq ".html" }
    foreach ($file in $files) {
        Write-Output "⚡ Minifying HTML: $( $file.RelativePath )"
        npx html-minifier-terser --collapse-whitespace --remove-comments --minify-css true --minify-js true --input-dir (Split-Path -Path $file.FullName) --output-dir (Split-Path -Path $file.FullName) --file-ext html
    }
}

function Clean-XmlFiles()
{
    $files = $allFiles | Where-Object { $_.Extension -eq ".xml" }
    foreach ($file in $files) {
        Write-Output "⚡ Minifying XML: $( $file.RelativePath )"
        npx html-minifier-terser --collapse-whitespace --remove-comments --input-dir (Split-Path -Path $file.FullName) --output-dir (Split-Path -Path $file.FullName) --file-ext xml
    }
}

function Clean-PngFiles()
{
    $files = $allFiles | Where-Object { $_.Extension -eq ".png" }
    foreach ($file in $files) {
        Write-Output "🎨 Compressing PNG: $( $file.RelativePath )"
        # If webp exists, delete?
        npx pngquant --quality=65-80 --speed 1 --force --ext .png -- $file.FullName
        # npx svgo --multipass --pretty --disable=removeViewBox --enable=removeMetadata --enable=removeComments --enable=collapseGroups "dist/**/*.svg"
        # npx mozjpeg -quality 75 -outfile "$file" "$file"
    }
}

$sizeBeforeList = Get-FolderSize $distFolder
$sizeBeforeTotal = ($sizeBeforeList | Measure-Object -Property "Size (KB)" -Sum).Sum

$startTime = Get-Date
# Clean-JsFiles
Clean-CssFiles
# Clean-HtmlFiles
# Clean-XmlFiles
# Clean-PngFiles
$endTime = Get-Date

$sizeAfterList = Get-FolderSize $distFolder
$sizeAfterTotal = ($sizeAfterList | Measure-Object -Property "Size (KB)" -Sum).Sum

$optimizationResults = $sizeBeforeList | ForEach-Object {
    $file = $_.File
    $fileName = $file -replace [regex]::Escape($distRoot), "/$distFolder"
    $beforeSize = $_."Size (KB)"
    $afterFile = $sizeAfterList | Where-Object { $_.File -eq $file }

    if ($afterFile) {
        $afterSize = $afterFile."Size (KB)"
        $reduction = $beforeSize - $afterSize
        $reductionPercent = ($reduction / $beforeSize) * 100
    } 
    else {
        # File was deleted (assume full reduction)
        $afterSize = 0
        $reduction = $beforeSize
        $reductionPercent = 100
    }

    [PSCustomObject]@{
        File = $fileName
        "Size Before (KB)" = $beforeSize
        "Size After (KB)" = $afterSize
        "Reduction (KB)" = [math]::Round($reduction, 2)
        "Reduction (%)" = [math]::Round($reductionPercent, 2)
    }
}

Write-Output "Size Reduction Per File:"
# consider | Where-Object { $_."Reduction (KB)" -gt 0 }
$optimizationResults | Sort-Object "Reduction (KB)" | Format-Table -AutoSize

$percentageSaved = (($sizeBeforeTotal - $sizeAfterTotal) / $sizeBeforeTotal) * 100
Write-Output "🔍 Size Before Optimization (Total: $sizeBeforeTotal KB)"
Write-Output "✅ Size After Optimization (Total: $sizeAfterTotal KB)"
Write-Output "📉 Total Reduction: $([math]::Round($sizeBeforeTotal - $sizeAfterTotal, 2)) KB saved ($([math]::Round($percentageSaved, 2))%)"
Write-Output "⏱ Optimization completed in $((New-TimeSpan -Start $startTime -End $endTime).TotalSeconds) seconds."
# $optimizationResults | Export-Csv -Path "dist_optimization_report.csv" -NoTypeInformation