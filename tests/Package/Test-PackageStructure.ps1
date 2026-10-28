<#
.SYNOPSIS
    Checks the layout, dependencies and versions of an iNKORE.UI.WPF.Modern package.
.DESCRIPTION
    Lists every problem it finds and exits with 1 if there is any.
.EXAMPLE
    tests/Package/Test-PackageStructure.ps1 -Package .nuget/iNKORE.UI.WPF.Modern.0.10.3-test1.nupkg -Version 0.10.3-test1
#>
param(
    [Parameter(Mandatory = $true)] [string] $Package,
    # The version the package and its assemblies should carry.
    [Parameter(Mandatory = $true)] [string] $Version
)

$ErrorActionPreference = 'Stop'

$Package = (Resolve-Path $Package).Path
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path

$netFolders = 'net6.0-windows10.0.18362', 'net6.0-windows7.0'
$libFolders = @('net452') + $netFolders
$assemblies = 'iNKORE.UI.WPF.Modern', 'iNKORE.UI.WPF.Modern.Controls'
# As packed today: the Controls project doesn't generate an XML documentation file.
$libFiles = 'iNKORE.UI.WPF.Modern.dll', 'iNKORE.UI.WPF.Modern.pdb', 'iNKORE.UI.WPF.Modern.xml',
            'iNKORE.UI.WPF.Modern.Controls.dll', 'iNKORE.UI.WPF.Modern.Controls.pdb'

$problems = New-Object System.Collections.Generic.List[string]

function Join-Sorted($items) { (@($items) | Sort-Object -CaseSensitive) -join ', ' }

# Why a file must stay out of the package, or $null if it may be packed.
function Get-ForbiddenReason($item) {
    if ($item.PSIsContainer) { return $null }
    elseif ($item.Name -eq 'iNKORE.UI.WPF.dll') { return 'it comes in through the iNKORE.UI.WPF dependency' }
    elseif ($item.Name -eq 'System.ValueTuple.dll') { return 'it comes in through the System.ValueTuple dependency' }
    elseif ($item.Name -like '*.deps.json') { return 'it is build output, not part of the library' }
    return $null
}

function Get-FolderHashes($dir) {
    Get-ChildItem $dir -Recurse -File |
        ForEach-Object { $_.FullName.Substring($dir.Length + 1) + ' ' + (Get-FileHash $_.FullName -Algorithm SHA256).Hash } |
        Sort-Object
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
$root = Join-Path ([IO.Path]::GetTempPath()) ([IO.Path]::GetRandomFileName())
[IO.Compression.ZipFile]::ExtractToDirectory($Package, $root)
try {
    $lib = Join-Path $root 'lib'

    # Layout
    $found = @(Get-ChildItem $lib -Directory -ErrorAction SilentlyContinue | ForEach-Object Name)
    if ((Join-Sorted $found) -cne (Join-Sorted $libFolders)) {
        $problems.Add("The lib folders are [$(Join-Sorted $found)], expected [$(Join-Sorted $libFolders)].")
    }
    foreach ($item in Get-ChildItem $root -Recurse) {
        $reason = Get-ForbiddenReason $item
        if ($reason) { $problems.Add("$($item.FullName.Substring($root.Length + 1)) is packed, but $reason.") }
    }
    foreach ($tfm in $libFolders) {
        $dir = Join-Path $lib $tfm
        if (-not (Test-Path $dir)) { continue }
        $files = @(Get-ChildItem $dir -File)
        foreach ($name in $libFiles) {
            if (@($files.Name) -cnotcontains $name) { $problems.Add("lib\$tfm is missing $name.") }
        }
        foreach ($file in $files) {
            if ($libFiles -cnotcontains $file.Name -and -not (Get-ForbiddenReason $file)) { $problems.Add("lib\$tfm has an unexpected file, $($file.Name).") }
        }
    }

    # The satellite assemblies (translations) are embedded in the DLLs on every target, so no culture folders.
    foreach ($tfm in $libFolders) {
        $subfolders = @(Get-ChildItem (Join-Path $lib $tfm) -Directory -ErrorAction SilentlyContinue | ForEach-Object Name)
        if ($subfolders.Count -gt 0) {
            $problems.Add("lib\$tfm has subfolders [$(Join-Sorted $subfolders)], but the translations should be embedded in the DLLs.")
        }
    }
    $netDirs = @($netFolders | ForEach-Object { Join-Path $lib $_ } | Where-Object { Test-Path $_ })
    if ($netDirs.Count -eq 2 -and ((Get-FolderHashes $netDirs[0]) -join "`n") -cne ((Get-FolderHashes $netDirs[1]) -join "`n")) {
        $problems.Add("lib\$($netFolders[0]) and lib\$($netFolders[1]) should hold the same files.")
    }

    # Dependencies
    $targets = [xml](Get-Content (Join-Path $repoRoot 'Directory.Build.targets') -Raw)
    $uiwpfVersion = $targets.SelectSingleNode("//PackageReference[@Update='iNKORE.UI.WPF']/Version").InnerText
    $nuspec = [xml](Get-Content (Get-ChildItem $root -Filter *.nuspec | Select-Object -First 1).FullName -Raw -Encoding UTF8)
    $metadata = $nuspec.package.metadata
    $groups = @($metadata.dependencies.group | Where-Object { $_ })
    if ($groups.Count -ne $libFolders.Count) { $problems.Add("The nuspec has $($groups.Count) dependency groups, expected $($libFolders.Count).") }
    foreach ($group in $groups) {
        $dependencies = @($group.dependency | Where-Object { $_ })
        foreach ($dependency in $dependencies) {
            if (-not $dependency.version) { $problems.Add("The $($group.targetFramework) dependency on $($dependency.id) has no version.") }
        }
        $uiwpf = $dependencies | Where-Object { $_.id -eq 'iNKORE.UI.WPF' }
        if (-not $uiwpf) {
            $problems.Add("The $($group.targetFramework) dependencies don't include iNKORE.UI.WPF.")
        }
        elseif ($uiwpf.version -cne $uiwpfVersion) {
            $problems.Add("The $($group.targetFramework) dependency on iNKORE.UI.WPF is version '$($uiwpf.version)', but Directory.Build.targets sets $uiwpfVersion.")
        }
    }

    # Versions. AssemblyVersion and FileVersion are the numeric part of the version, padded to four parts.
    if ($metadata.version -cne $Version) { $problems.Add("The package version is $($metadata.version), expected $Version.") }
    $numeric = [version](($Version -split '[-+]')[0])
    $assemblyVersion = New-Object Version $numeric.Major, $numeric.Minor, ([Math]::Max($numeric.Build, 0)), ([Math]::Max($numeric.Revision, 0))
    foreach ($tfm in $libFolders) {
        foreach ($name in $assemblies) {
            $dll = Join-Path $lib "$tfm\$name.dll"
            if (-not (Test-Path $dll)) { continue }
            $actual = [Reflection.AssemblyName]::GetAssemblyName($dll).Version
            $info = [Diagnostics.FileVersionInfo]::GetVersionInfo($dll)
            $informational = ($info.ProductVersion -split '\+')[0]
            if ($actual -ne $assemblyVersion) { $problems.Add("lib\$tfm\$name.dll has AssemblyVersion $actual, expected $assemblyVersion.") }
            if ($info.FileVersion -ne "$assemblyVersion") { $problems.Add("lib\$tfm\$name.dll has FileVersion $($info.FileVersion), expected $assemblyVersion.") }
            if ($informational -cne $Version) { $problems.Add("lib\$tfm\$name.dll has InformationalVersion $($info.ProductVersion), expected $Version.") }
        }
    }
}
finally {
    Remove-Item $root -Recurse -Force
}

$name = Split-Path $Package -Leaf
if ($problems.Count -gt 0) {
    foreach ($problem in $problems) { Write-Host "FAIL $problem" }
    Write-Host "$name has $($problems.Count) problem(s)."
    exit 1
}
Write-Host "OK   $name`: lib folders, files, no culture folders, dependencies and versions."
