$directoryPath = "dist"  # Change this to your directory
$approvedDomains = @("kaylumah.nl")  # List of approved domains

$results = @()

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
$regexPattern = '(?<=["''])(?:https?:\/\/[^\/]+)?(\/[^"'']+\.(?:png|jpg|jpeg|gif|webp|svg))(?=["''])'

$HtmlFiles = Get-ChildItem -Path $directoryPath -Recurse -Filter "*.html"
$HtmlFiles | ForEach-Object {
    $htmlPath = $_.FullName
    $htmlContent = Get-Content -Path $htmlPath -Raw

    $htmlContent = [regex]::Replace($htmlContent, '<script type=["'']application/ld\+json["''].*?>.*?</script>', '', 'Singleline')

    # Extract URLs using regex
    $matches = [regex]::Matches($htmlContent, $regexPattern) | ForEach-Object { $_.Value }

    foreach ($url in $matches) {
        if ($url) {
            $normalizedUrl = Normalize-Url $url
            $results += $normalizedUrl
        }
    }
}

$results | Sort-Object -Unique