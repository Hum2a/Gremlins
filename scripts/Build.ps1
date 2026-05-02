#Requires -Version 5.0
<#
.SYNOPSIS
  Restores and builds Gremlins (app + tests compile).
#>
param(
    [ValidateSet("Debug", "Release")]
    [string] $Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

dotnet build (Join-Path $root "Gremlins.sln") -c $Configuration --verbosity minimal
exit $LASTEXITCODE
