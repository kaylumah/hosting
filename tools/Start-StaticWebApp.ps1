#requires -Version 7.2

param (
)

$ErrorActionPreference  = "Stop"

$ScriptRoot = $PSScriptRoot
$RepoRoot = Split-Path $ScriptRoot -Parent
$DistFolder = "$RepoRoot/dist"

If(!(Test-Path $DistFolder))
{
    Write-Error "The folder '$DistFolder' does not exist"
}

npm --prefix $DistFolder run swa