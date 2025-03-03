function Get-FolderSize($path) {
    (Get-ChildItem -Path $path -Recurse | Measure-Object -Property Length -Sum).Sum / 1KB
}

# Measure Size Before
$sizeBefore = Get-FolderSize "dist"
Write-Output "🔍 Size Before: $sizeBefore KB"

# Run Optimization Commands from package.json
# npm run optimize:js

# Minify all JavaScript files in dist/ using Terser
Get-ChildItem -Path "dist" -Recurse -Filter "*.js" | ForEach-Object {
    $file = $_.FullName
    Write-Output "Minifying $file"
    npx terser $file --compress --mangle --output $file
}

# npm run optimize:html
# Optimize HTML Files (One by One)
Get-ChildItem -Path "dist" -Recurse -Filter "*.html" | ForEach-Object {
    $file = $_.FullName
    Write-Output "⚡ Minifying HTML: $file"
    npx html-minifier-terser --collapse-whitespace --remove-comments --minify-css true --minify-js true --input-dir (Split-Path -Path $file) --output-dir (Split-Path -Path $file) --file-ext html
}

# Measure Size After
$sizeAfter = Get-FolderSize "dist"
Write-Output "✅ Size After: $sizeAfter KB"

# Calculate Reduction Percentage
if ($sizeBefore -gt 0) {
    $reduction = (($sizeBefore - $sizeAfter) / $sizeBefore) * 100
    Write-Output "📉 Reduction: $reduction% saved"
} else {
    Write-Output "⚠️ Unable to calculate reduction (Size Before was 0 KB)"
}