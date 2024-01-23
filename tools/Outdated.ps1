#requires -Version 7.2

param (
    [Parameter(Mandatory=$true, HelpMessage = "The path to the project file")]
    [string] $ProjectPath
)

$OutdatedOutput = dotnet list $ProjectPath package --outdated --format json
$OutdatedOutputAsJson = $OutdatedOutput | ConvertFrom-json
$Projects = $OutdatedOutputAsJson.Projects

$Result = @()
foreach ($Project in $Projects)
{
    $Frameworks = $Project.Frameworks
    if ($Frameworks -ne $null)
    {
        $Framework = $Frameworks[0]
        $TopLevelPackages = $Framework.TopLevelPackages
        foreach ($TopLevelPackage in $TopLevelPackages)
        {
            $Result += $TopLevelPackage.Id
        }
    }
}

$Result = $Result | Select -Unique | Sort-Object
if ($Result.Count -gt 0)
{
    $sb = [System.Text.StringBuilder]::new()
    [void]$sb.AppendLine("The following dependencies are outdated!")
    foreach($Item in $Result)
    {
        [void]$sb.AppendLine(" - $Item")
    }
    $sb | Write-warning
}