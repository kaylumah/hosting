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
    Get-ChildItem -Path "dist" -Recurse -Filter "*.js" -File | ForEach-Object {
        $file = $_.FullName
        Write-Output "⚡ Minifying JS: $file"
        npx terser $file --compress --mangle --output $file
    }
}

function Clean-CssFiles()
{
    Get-ChildItem -Path "dist" -Recurse -Filter "*.css" | ForEach-Object {
        $file = $_.FullName
        Write-Output "🔹 Minifying CSS: $file"
        # npx csso-cli "$file" --output "$file"
    }
}

function Clean-HtmlFiles()
{
    Get-ChildItem -Path "dist" -Recurse -Filter "*.html" -File | ForEach-Object {
        $file = $_.FullName
        Write-Output "⚡ Minifying HTML: $file"
        # --minify-inline-svg true
        npx html-minifier-terser --collapse-whitespace --remove-comments --minify-css true --minify-js true --input-dir (Split-Path -Path $file) --output-dir (Split-Path -Path $file) --file-ext html
    }
}

function Clean-XmlFiles()
{
    Get-ChildItem -Path "dist" -Recurse -Filter "*.xml" -File | ForEach-Object {
        $file = $_.FullName
        Write-Output "⚡ Minifying XML: $file"
        npx html-minifier-terser --collapse-whitespace --remove-comments --input-dir (Split-Path -Path $file) --output-dir (Split-Path -Path $file) --file-ext xml
    }
}

function Clean-PngFiles()
{
    Get-ChildItem -Path "dist" -Recurse -Filter "*.png" | ForEach-Object {
        $pngFile = $_.FullName
        Write-Output "🎨 Compressing PNG: $pngFile"
        npx pngquant --quality=65-80 --speed 1 --force --ext .png -- $pngFile

        # npx svgo --multipass --pretty --disable=removeViewBox --enable=removeMetadata --enable=removeComments --enable=collapseGroups "dist/**/*.svg"
        # npx mozjpeg -quality 75 -outfile "$file" "$file"
        # $webpFile = "$pngFile.webp"  # WebP file follows .png.webp naming convention
        # if (Test-Path $webpFile) {
        #    Write-Output "🗑️ Deleting PNG: $pngFile (WebP exists: $webpFile)"
        #    Remove-Item -Path $pngFile -Force
        # }
    }
}

$sizeBeforeList = Get-FolderSize "dist"
$sizeBeforeTotal = ($sizeBeforeList | Measure-Object -Property "Size (KB)" -Sum).Sum

$startTime = Get-Date
Clean-JsFiles
Clean-CssFiles
Clean-HtmlFiles
Clean-XmlFiles
Clean-PngFiles
$endTime = Get-Date

$sizeAfterList = Get-FolderSize "dist"
$sizeAfterTotal = ($sizeAfterList | Measure-Object -Property "Size (KB)" -Sum).Sum

$distPath = (Resolve-Path "dist").Path
$optimizationResults = $sizeBeforeList | ForEach-Object {
    $file = $_.File
    $fileName = $file -replace [regex]::Escape($distPath), "/dist"
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
$optimizationResults | Sort-Object "Reduction (KB)" | Format-Table -AutoSize

$percentageSaved = (($sizeBeforeTotal - $sizeAfterTotal) / $sizeBeforeTotal) * 100
Write-Output "🔍 Size Before Optimization (Total: $sizeBeforeTotal KB)"
Write-Output "✅ Size After Optimization (Total: $sizeAfterTotal KB)"
Write-Output "📉 Total Reduction: $([math]::Round($sizeBeforeTotal - $sizeAfterTotal, 2)) KB saved ($([math]::Round($percentageSaved, 2))%)"
Write-Output "⏱ Optimization completed in $((New-TimeSpan -Start $startTime -End $endTime).TotalSeconds) seconds."
# $optimizationResults | Export-Csv -Path "dist_optimization_report.csv" -NoTypeInformation