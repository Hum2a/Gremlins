#requires -Version 5.1
<#
.SYNOPSIS
  Build an MSIX for Gremlins using only the .NET SDK + Windows SDK (MakeAppx). No Visual Studio IDE.

.DESCRIPTION
  1) dotnet publish → flat folder with Gremlins.exe + runtime.
  2) Writes AppxManifest.xml from template (must match Partner Center identity).
  3) Generates placeholder PNG tiles under Assets\ (replace with real art before Store submission).
  4) MakeAppx.exe pack → .msix

.PREREQUISITES
  - .NET SDK 8+
  - Windows SDK (install "Windows SDK" from https://developer.microsoft.com/windows/downloads/windows-sdk/ )
    This provides MakeAppx.exe under Program Files (x86)\Windows Kits\10\bin\...\x64\

.EXAMPLE
  cd packaging
  .\Build-StoreMsix.ps1 -IdentityName "Contoso.Gremlins" -Publisher 'CN=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx'

  Identity / Publisher must match what Partner Center shows under Product identity (reserved app).

.NOTES
  - Store uploads are re-signed by Microsoft; local signing is optional (sideload testing).
  - Use /p:PublishSingleFile=false below for fewer packaging edge cases (adjust if you prefer single-file).
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string] $RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path,

    [Parameter(Mandatory = $false)]
    [string] $IdentityName = "YourPublisher.Gremlins",

    # Exact string from Partner Center → Product identity (Publisher), e.g. CN=...
    [Parameter(Mandatory = $false)]
    [string] $Publisher = "CN=Development Gremlins",

    [Parameter(Mandatory = $false)]
    [string] $PublisherDisplayName = "Your Name",

    [Parameter(Mandatory = $false)]
    [string] $Description = "A desktop app that subtly gaslights you while you work.",

    [Parameter(Mandatory = $false)]
    [string] $Version = "1.0.0.0",

    [Parameter(Mandatory = $false)]
    [string] $OutputMsixName = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Find-MakeAppx {
    param([string[]] $SearchRoots)
    foreach ($root in $SearchRoots) {
        if (-not (Test-Path $root)) { continue }
        $found = Get-ChildItem -Path $root -Recurse -Filter "MakeAppx.exe" -ErrorAction SilentlyContinue |
            Where-Object { $_.FullName -match '\\x64\\' } |
            Sort-Object { $_.FullName } -Descending
        if ($found) { return $found[0].FullName }
    }
    return $null
}

function Ensure-PlaceholderAssets {
    param([string] $AssetsDir)
    New-Item -ItemType Directory -Force -Path $AssetsDir | Out-Null

    try {
        Add-Type -AssemblyName System.Drawing
    }
    catch {
        Write-Warning "Could not load System.Drawing; copy PNGs manually into $AssetsDir (see AppxManifest)."
        return
    }

    $specs = @{
        "Square44x44Logo.png"   = @(44, 44)
        "Square150x150Logo.png" = @(150, 150)
        "Wide310x150Logo.png"   = @(310, 150)
        "StoreLogo.png"         = @(50, 50)
    }

    $fill = [System.Drawing.Color]::FromArgb(46, 168, 62)
    foreach ($entry in $specs.GetEnumerator()) {
        $path = Join-Path $AssetsDir $entry.Key
        if (Test-Path $path) { continue }

        $w = $entry.Value[0]
        $h = $entry.Value[1]
        $bmp = New-Object System.Drawing.Bitmap($w, $h)
        $g = [System.Drawing.Graphics]::FromImage($bmp)
        $g.Clear($fill)
        $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
        $g.Dispose()
        $bmp.Dispose()
        Write-Host "Generated placeholder $($entry.Key)"
    }
}

$makeAppx = Find-MakeAppx @(
    "${env:ProgramFiles(x86)}\Windows Kits\10\bin"
    "${env:ProgramFiles}\Windows Kits\10\bin"
)

if (-not $makeAppx) {
    Write-Error @"
MakeAppx.exe not found. Install the Windows SDK (not Visual Studio):
https://developer.microsoft.com/windows/downloads/windows-sdk/

After install, MakeAppx.exe is typically under:
  Program Files (x86)\Windows Kits\10\bin\<build>\x64\MakeAppx.exe
"@
}

Write-Host "Using MakeAppx: $makeAppx"

$layout = Join-Path $PSScriptRoot "layout"
$outDir = Join-Path $PSScriptRoot "out"
if (Test-Path $layout) { Remove-Item -Recurse -Force $layout }
New-Item -ItemType Directory -Force -Path $layout | Out-Null
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

$publishDir = Join-Path $layout "_publish"
dotnet publish (Join-Path $RepoRoot "Gremlins.csproj") `
    -c Release `
    -r win-x64 `
    --self-contained true `
    /p:PublishSingleFile=false `
    /p:PublishReadyToRun=true `
    /p:IncludeNativeLibrariesForSelfExtract=true `
    -o $publishDir

if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# Flatten publish output into package root (exe + DLLs next to manifest)
Copy-Item -Path (Join-Path $publishDir "*") -Destination $layout -Recurse -Force
Remove-Item -Recurse -Force $publishDir

$assetsDir = Join-Path $layout "Assets"
Ensure-PlaceholderAssets $assetsDir

$templatePath = Join-Path $PSScriptRoot "AppxManifest.template.xml"
$manifestOut = Join-Path $layout "AppxManifest.xml"
$xml = Get-Content -Raw -LiteralPath $templatePath
$xml = $xml.Replace("__IDENTITY_NAME__", [System.Security.SecurityElement]::Escape($IdentityName))
$xml = $xml.Replace("__PUBLISHER__", [System.Security.SecurityElement]::Escape($Publisher))
$xml = $xml.Replace("__VERSION__", $Version)
$xml = $xml.Replace("__PUBLISHER_DISPLAY__", [System.Security.SecurityElement]::Escape($PublisherDisplayName))
$xml = $xml.Replace("__DESCRIPTION__", [System.Security.SecurityElement]::Escape($Description))
Set-Content -LiteralPath $manifestOut -Value $xml -Encoding UTF8

if (-not $OutputMsixName) {
    $verFlat = $Version.Replace(".", "_")
    $OutputMsixName = "Gremlins_${verFlat}_x64.msix"
}

$msixPath = Join-Path $outDir $OutputMsixName
if (Test-Path $msixPath) { Remove-Item -Force $msixPath }

& $makeAppx pack /v /o /h sha256 /d $layout /p $msixPath
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host ""
Write-Host "MSIX created: $msixPath"
Write-Host "Upload this file in Partner Center → Packages."
Write-Host "Ensure Identity Name / Publisher match Product identity. Replace placeholder Assets PNGs with final Store art before submission."
