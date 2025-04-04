$directoryPath = "dist"  # Change this to your directory
$approvedDomains = @("kaylumah.nl", "localhost:4280")  # List of approved domains
$results = @()
$assetList = @()

function Normalize-Url {
    param ($url)

    # If it's already relative, return as is
    if ($url -match "^(\/|\.\/|\.\.\/|[^:]+$)") {
        return $url
    }

    # Extract domain from absolute URL
    if ($url -match "^https?:\/\/([^\/]+)(\/?.*)$") {
        $domain = $matches[1]
        $path = $matches[2]

        if ($approvedDomains -contains $domain) {
            return $path  # Remove domain, keep the path
        }
    }

    return $url  # Keep as is if unapproved or unknown
}

# Regex to capture asset URLs (including JS, CSS, images, fonts, media)
$regexPattern = '(?<=["''])(?:https?:\/\/[^\/]+)?(\/[^"'']+\.(?:png|jpg|jpeg|gif|webp|svg|ico|webmanifest))(?=["''])'
$ContentFiles = @()
$ContentFiles += Get-ChildItem -Path $directoryPath -Recurse -Filter "*.html"
$ContentFiles += Get-ChildItem -Path $directoryPath -Recurse -Filter "*.webmanifest"

$ContentFiles| ForEach-Object {
    $filePath = $_.FullName
    $fileContent = Get-Content -Path $filePath -Raw

    $fileContent = [regex]::Replace($fileContent, '<script type=["'']application/ld\+json["''].*?>.*?</script>', '', 'Singleline')

    # Extract URLs using regex
    $matches = [regex]::Matches($fileContent, $regexPattern) | ForEach-Object { $_.Value }

    foreach ($url in $matches) {
        if ($url) {
            $normalizedUrl = Normalize-Url $url
            $results += $normalizedUrl
        }
    }
}

# used in feed.xml
$results += "/assets/logo_alt.svg"

# Get asset files
$assetFiles = Get-ChildItem -Path "$directoryPath/assets" -Recurse | Where-Object { $_.Mode -notmatch "d" }
$assetFiles | ForEach-Object {
    $relativePath = $_.FullName -replace "^$([regex]::Escape((Get-Item $directoryPath).FullName))", ""
    $relativePath = $relativePath -replace "\\", "/"  # Normalize path separators
    $assetList += $relativePath
}

# Find items in results that are NOT in the assets list
$missingFromAssets = $results | Where-Object { $_ -notin $assetList }

# Find items in assets that are NOT in the results list
$missingFromResults = $assetList | Where-Object { $_ -notin $results }

# Output missing items
Write-Output "Items in results but not in assets:"
$missingFromAssets | Sort-Object -Unique

Write-Output "Items in assets but not in results:"
$missingFromResults | Sort-Object -Unique