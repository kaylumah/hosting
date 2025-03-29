#requires -Version 7.2

# [CmdletBinding()]
param (
    [Parameter(Mandatory=$true, HelpMessage = "The path to the project file")]
    [string] $ProjectPath
)

$OutdatedOutput = dotnet list $ProjectPath package --outdated --format json
$OutdatedOutputAsJson = $OutdatedOutput | ConvertFrom-json
$Projects = $OutdatedOutputAsJson.Projects

# You’re creating a strongly-typed System.Version object, which gives you semantic version comparison, rather than plain string comparison.
# https://learn.microsoft.com/en-us/nuget/concepts/package-versioning?tabs=semver20sort#version-ranges
# 1.0.0 -> unconstrained, 1.0.0 and later
# [1.0.0] -> Only version 1.0.0 (exact match)
# [1.0.0,2.0.0) -> 1.0.0 inclusive, up to but not including 2.0.0
# (1.0.0,2.0.0] -> Greater than 1.0.0 (exclusive), up to and including 2.0.0
# (,1.0.0] -> Anything up to and including 1.0.0
# (1.0.0,) -> Anything after 1.0.0
# [1.0.0,) -> 1.0.0 and later

$Result = @{}
foreach ($Project in $Projects)
{
    $Frameworks = $Project.Frameworks
    if ($Frameworks -ne $null)
    {
        $Framework = $Frameworks[0]
        $TopLevelPackages = $Framework.TopLevelPackages
        foreach ($Package in $TopLevelPackages)
        {
            $PackageId = $Package.Id

            if ($Result.ContainsKey($PackageId))
            {
                Write-Verbose "Skipping '$PackageId' already processed"
                continue
            }

            $ResolvedVersion = $Package.ResolvedVersion
            $LatestVersion = [version]$Package.LatestVersion
            $RequestedVersion = $Package.RequestedVersion
            $NewVersion = $null
            $Description = "N/A"

            Write-Verbose "Checking '$PackageId' for '$RequestedVersion' with current: '$ResolvedVersion' and latest: '$LatestVersion'"
            $SpecialVersionRegexMatch = $RequestedVersion -match "^(?:(?<Open>[\[\(])(?<Min>[^,\)\]]*)?,?(?<Max>[^,\)\]]*)(?<Close>[\]\)])?)$"

            if (-not $SpecialVersionRegexMatch)
            {
                Write-Verbose "Bump to latest version"
                $NewVersion = $LatestVersion
                $Description = "Regular"
            }
            else
            {
                $min = $null
                $max = $null
                $minInclusive = $Matches.Open -eq "["
                $maxInclusive = $Matches.Close -eq "]"

                $minText = $Matches.Min
                $maxText = $Matches.Max

                if ($minText -match "-" -or $maxText -match "-")
                {
                    $NewVersion = $LatestVersion
                    $Description = "Preview version check manually"
                } 
                else 
                {
                    if ($minText) { 
                        $min = [version]$minText
                    } 
                    else {
                        # Fallback to ResolvedVersion 
                        $min = $ResolvedVersion 
                    }

                    if ($maxText) { 
                        $max = [version]$maxText 
                    }
                    elseif (-not $maxInclusive)
                    {
                        # No upper version, resolve to latest
                        $max = $LatestVersion
                    }
                    elseif ($min -ne $null -and $minInclusive -and $maxInclusive)
                    {
                        # Fixed version [1.0.0]
                        $max = $min
                    }
                    else
                    {
                        throw "Unreachable code: unexpected version constraint state for '$PackageId'"
                    }

                    Write-Verbose ("Checking version constraints: ($min {0} $ResolvedVersion {1} $max)" -f ($minInclusive ? '>=' : '>'), ($maxInclusive ? '<=' : '<'))

                    if ($min -eq $max) {
                        Write-Verbose "No updated needed"
                        $NewVersion = $min
                        $Description = "Pinned"
                    }
                    elseif ($LatestVersion -le $max)
                    {
                        Write-Verbose "Bump to latest"
                        $NewVersion = $LatestVersion
                        $Description = "Below upper-constraint"
                    }
                    elseif ($LatestVersion -gt $max -and $ResolvedVersion -lt $max)
                    {
                        Write-Verbose "Since $ResolvedVersion <= $LatestVersion >= $max check NuGet"
                        $url = "https://api.nuget.org/v3-flatcontainer/$packageId/index.json"
                        $response = Invoke-RestMethod -Uri $url -ErrorAction Stop
                        $allVersions = $response.versions | Where-Object { $_ -notmatch "-" } | ForEach-Object { [version]$_ }

                        if ($minInclusive) {
                            $allVersions = $allVersions | Where-Object { $_ -ge $min }
                        } else {
                            $allVersions = $allVersions | Where-Object { $_ -gt $min }
                        }

                        if ($maxInclusive) {
                            $allVersions = $allVersions | Where-Object { $_ -le $max }
                        } else {
                            $allVersions = $allVersions | Where-Object { $_ -lt $max }
                        }
                        
                        $NewVersion = $allVersions | Sort-Object -Descending | Select-Object -First 1
                        if ($ResolvedVersion -eq $NewVersion) {
                            $Description = "No new allowed version on NuGet"
                        } else {
                            $Description = "Found version in range on NuGet"
                        }
                    }
                }
            }

            $Result[$PackageId] = [pscustomobject]@{
                Id               = $PackageId
                From             = $ResolvedVersion
                To               = $NewVersion
                Description      = $Description
            }
        }
    }
}

$Outdated = $Result.Values # | Where-Object { $_.To -ne $_.From }
if ($Outdated.Count -gt 0) {
    $sb = [System.Text.StringBuilder]::new()
    [void]$sb.AppendLine("The following dependencies have newer versions available:")
    foreach ($entry in $Outdated) {
        [void]$sb.AppendLine(" - $($entry.Id): $($entry.From) → $($entry.To) ($($entry.Description))")
    }
    $sb | Write-Warning
}