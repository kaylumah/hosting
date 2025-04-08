---
title: "Tracking NuGet Updates with PowerShell: Handling Pinned Versions & Constraints"
description: "Extend the .NET SDK to check outdated NuGet packages using PowerShell, with special handling for version ranges and pinned dependencies."
tags:
  - powershell
  - nuget
publishedtime: '17:30'
---
Most of the time, managing NuGet dependencies in .NET projects is straightforward.
Whether you believe in "don't fix what's not broken" or "always update", there is always value in knowing about outdated packages.
You need to be able to make an informed decision either way.
While tools like Dependabot can automate this process, I sometimes prefer more control.
In this post I will share a script I wrote that extends the dotnet SDK to provide this information.

## Create a helper script

The dotnet SDK comes with a built-in command to [list the packages for a project/solution](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-package-list).
Even if you execute the command for a `.sln` file, you get the outdated packages per project.
The package version shown will always be the latest available.
However, ever since central package management was introduced, most projects in a SLN would have the same version of a package.
For this purpose we can create a very simple helper script using PowerShell.

1. List packages for solution in JSON format
2. Process every project with a valid TargetFramework (assumes single)
3. Capture outdated packages (unique by PackageId)
4. Print result

```powershell
param (
    [Parameter(Mandatory=$true, HelpMessage = "The path to the project file")]
    [string] $ProjectPath
)

$OutdatedOutput = dotnet list $ProjectPath package --outdated --format json
$OutdatedOutputAsJson = $OutdatedOutput | ConvertFrom-json
$Projects = $OutdatedOutputAsJson.Projects

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

            $Result[$PackageId] = [pscustomobject]@{
                Id               = $PackageId
                From             = $Package.ResolvedVersion
                To               = $LatestVersion
            }
        }
    }
}

$Outdated = $Result.Values
if ($Outdated.Count -gt 0) {
    $sb = [System.Text.StringBuilder]::new()
    [void]$sb.AppendLine("The following dependencies have newer versions available:")
    foreach ($entry in $Outdated) {
        [void]$sb.AppendLine(" - $($entry.Id): $($entry.From) → $($entry.To)")
    }
    $sb | Write-Warning
}
```

Where example output looks like this

```shell
WARNING: The following dependencies have newer versions available:
 - FluentAssertions: 7.2.0 → 8.2.0
```

## Lock versions

The script shared above has one big shortcoming, it does not handle pinned versions.
Sometimes, for whatever reason, you want to prevent a package from being bumped.
The example I prefer to give is locking a `Microsoft.Extensions.*` package to its corresponding `.NET` framework version.
More recently, in the .NET open source community, there have been other cases:
My first thought was `Moq` with SponsorLink (2023), then `FluentAssertions` with the new paid license model (January 2025).
This week, AutoMapper, Mediator, and MassTransit joined the club. Nudging me to finally finish this article.
Even though the announcement coincided with April Fool’s Day, it didn’t appear to be a joke.
While I fully support and understand the need for these maintainers to earn money for their hard work, depending on how the update
is handled it opens you up for liabilities.

Luckily we can pin a package version using [version ranges](https://learn.microsoft.com/en-us/nuget/concepts/package-versioning?tabs=semver20sort#version-ranges).
We can set an inclusive boundary by using `[` or `]` and an exclusive boundary by using `(` or `)`.
Following this logic
- `Moq` can be pinned with `[4.18.2]` or the equivalent `[4.18.2, 4.18.2]`
- `FluentAssertions` can receive update until the next major version with `[7.0.0, 8.0.0)`

We now need to update the script to parse these version ranges. If the latest version is higher than the resolved one but still within range, we should update.
Also keep in mind that NuGet will always resolve the earliest possible resolution.
In this case that means we get version `7.0.0`. 

The challenge is, that at the time of writing FluentAssertions has the following versions available `(7.0.0, ..., 7.2.0, 8.0.0, ..., 8.2.0)`.
This means NuGet resolves to 7.0.0, while dotnet list package reports 8.2.0 which violates the version range, and we don't know about `7.2.0` package that would be a valid upgrade.

We make the following changes to the script.
Check via regex if we detect a version range (min, max package versions), and handle the following cases
1. `Min == Max` => no update, version pinned.
2. `Latest < Max` => update, latest version within range.
3. `Latest > Max` => check NuGet, there might be a version. 

```powershell
param (
    [Parameter(Mandatory=$true, HelpMessage = "The path to the project file")]
    [string] $ProjectPath
)

$OutdatedOutput = dotnet list $ProjectPath package --outdated --format json
$OutdatedOutputAsJson = $OutdatedOutput | ConvertFrom-json
$Projects = $OutdatedOutputAsJson.Projects

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
                continue
            }

            $ResolvedVersion = $Package.ResolvedVersion
            $LatestVersion = [version]$Package.LatestVersion
            $RequestedVersion = $Package.RequestedVersion
            $NewVersion = $null
            $Description = "N/A"

            $SpecialVersionRegexMatch = $RequestedVersion -match "^(?:(?<Open>[\[\(])(?<Min>[^,\)\]]*)?,?(?<Max>[^,\)\]]*)(?<Close>[\]\)])?)$"

            if (-not $SpecialVersionRegexMatch)
            {
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
                    
                    if ($min -eq $max) {
                        $NewVersion = $min
                        $Description = "Pinned"
                    }
                    elseif ($LatestVersion -le $max)
                    {
                        $NewVersion = $LatestVersion
                        $Description = "Below upper-constraint"
                    }
                    elseif ($LatestVersion -gt $max -and $ResolvedVersion -lt $max)
                    {
                        $url = "https://api.nuget.org/v3-flatcontainer/$packageId/index.json".ToLower()
                        
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

$Outdated = $Result.Values
if ($Outdated.Count -gt 0) {
    $sb = [System.Text.StringBuilder]::new()
    [void]$sb.AppendLine("The following dependencies have newer versions available:")
    foreach ($entry in $Outdated) {
        [void]$sb.AppendLine(" - $($entry.Id): $($entry.From) → $($entry.To) ($($entry.Description))")
    }
    $sb | Write-Warning
}
```

If we have the version set to `[7.0.0, 8.0.0)` we get the following output:

```shell
WARNING: The following dependencies have newer versions available:
- FluentAssertions: 7.0.0 → 7.2.0 (Found version in range on NuGet)
```

After upgrading the range to `[7.2.0, 8.0.0)` we get the following output:

```shell
WARNING: The following dependencies have newer versions available:
 - FluentAssertions: 7.2.0 → 7.2.0 (No new allowed version on NuGet)
```

## Next steps: ensure the script always runs

For my blog's repo I took it one step further.
I included a `Directory.Solution.props`, which triggers a custom target post build.
In my case I installed PowerShell as a dotnet tool.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Target Name="CheckDependencies" AfterTargets="Build">
    <PropertyGroup>
      <PowerShellCommand>dotnet pwsh</PowerShellCommand>
      <PowerShellExecutionPolicy>Bypass</PowerShellExecutionPolicy>
      <OutdatedScript>$(MSBuildProjectDirectory)/tools/Outdated.ps1</OutdatedScript>
      <TargetProject>$(MSBuildProjectDirectory)/SSG.sln</TargetProject>
    </PropertyGroup>
    <Exec Command="$(PowerShellCommand) -ExecutionPolicy $(PowerShellExecutionPolicy) -NoProfile -File $(OutdatedScript) -ProjectPath $(TargetProject)" />
  </Target>
</Project>
```

If you are on `net9.0` you will probably not see any output.
This is due to changes in MSBuild output, where the output from the script gets suppressed.
Running `dotnet build --verbosity detailed` will provide the output.