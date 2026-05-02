#Requires -Version 5.0
<#
.SYNOPSIS
  Builds and runs the Gremlins xUnit test suite.
.DESCRIPTION
  Run from the repository root (folder that contains Gremlins.sln), e.g.:
    .\scripts\Run-Tests.ps1
    .\scripts\Run-Tests.ps1 -Configuration Debug -Verbosity normal
#>
[CmdletBinding()]
param(
    [ValidateSet("Debug", "Release")]
    [string] $Configuration = "Release",

    [string] $Verbosity = "minimal"
)

$ErrorActionPreference = "Stop"
# Script lives in repo/scripts → repo root is one level up
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

Write-Host "==> dotnet test ( $Configuration )" -ForegroundColor Cyan
dotnet test (Join-Path $root "Gremlins.sln") -c $Configuration --verbosity $Verbosity
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "All tests passed." -ForegroundColor Green
exit 0
