#Requires -Version 7.2

[CmdletBinding()]
param(
    [string] $BuildConfiguration = "Debug",
    [string] $TargetFramework = "net7.0",
    [string] $ReportFile = "LivingDoc.html"
)

$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path $PSScriptRoot -Parent

try
{
    Push-Location $RepoRoot

    $TestProject = "./Test/Specflow/bin/$BuildConfiguration/$TargetFramework/Test.Specflow.dll"
    $TestExecutionResult = "./Test/Specflow/bin/$BuildConfiguration/$TargetFramework/TestExecution.json"

    if (!(Test-Path -Path $TestProject))
    {
        Write-Error "Build project in '$BuildConfiguration' mode first"
    }

    if (!(Test-Path -Path $TestExecutionResult))
    {
        Write-Error "Execute 'dotnet test' before running this script"
    }

    dotnet livingdoc test-assembly $TestProject --test-execution-json $TestExecutionResult --output "$ReportFile"
}
finally
{
    Pop-Location
}