<#
.SYNOPSIS
    Builds the app in Consumers/ against an iNKORE.UI.WPF.Modern package and runs it on each target framework.
.DESCRIPTION
    The app is built twice, with dotnet build and with Visual Studio's MSBuild, since the two spell some culture
    names differently (NETSDK1187). A failed build or any NU*, NETSDK* or MSB3277 warning fails the test, and so does
    a culture folder from the package in the output: its translations are embedded in its DLLs. Each build is then
    run on every target framework; the app checks itself and exits non-zero on any failure.
.EXAMPLE
    tests/Package/Test-Consumers.ps1 -Package .nuget/iNKORE.UI.WPF.Modern.0.10.3-test1.nupkg -Version 0.10.3-test1
#>
param(
    [Parameter(Mandatory = $true)] [string] $Package,
    [Parameter(Mandatory = $true)] [string] $Version,
    # Visual Studio's MSBuild.exe; found with vswhere when left out.
    [string] $MSBuild,
    # How long each run may take before it's killed. The app gives up by itself after 60 seconds.
    [int] $TimeoutSeconds = 120
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'Common.ps1')
if (-not $MSBuild) { $MSBuild = Find-MSBuild }

$consumers = Join-Path $PSScriptRoot 'Consumers'
$project = Join-Path $consumers 'PackageConsumer'
$frameworks = 'net8.0-windows', 'net8.0-windows10.0.19041.0', 'net48'

# Consumers/nuget.config reads the package source from this variable.
$env:INKORE_PACKAGE_FEED = Split-Path (Resolve-Path $Package).Path
# The cache is keyed by version, so a package rebuilt with the same version would be served stale.
$cached = Join-Path $consumers "packages\inkore.ui.wpf.modern\$($Version.ToLowerInvariant())"
if (Test-Path $cached) { Remove-Item $cached -Recurse -Force }

function Invoke-Consumer([string] $exe) {
    $info = New-Object System.Diagnostics.ProcessStartInfo $exe
    $info.UseShellExecute = $false
    $info.RedirectStandardOutput = $true
    $info.RedirectStandardError = $true
    $info.StandardOutputEncoding = New-Object System.Text.UTF8Encoding $false
    $process = [Diagnostics.Process]::Start($info)
    $stdout = $process.StandardOutput.ReadToEndAsync()
    $stderr = $process.StandardError.ReadToEndAsync()
    $exited = $process.WaitForExit($TimeoutSeconds * 1000)
    if (-not $exited) { $process.Kill() }
    $process.WaitForExit()
    foreach ($line in ($stdout.Result + $stderr.Result) -split "`r?`n") { if ($line) { Write-Host "    $line" } }
    if (-not $exited) { return "was killed after $TimeoutSeconds seconds" }
    if ($process.ExitCode -ne 0) { return "exited with $($process.ExitCode)" }
    return $null
}

$failures = New-Object System.Collections.Generic.List[string]
$builds = @(
    @{ Name = 'dotnet build'; Command = 'dotnet'; Arguments = @('build', '-tl:off') },
    @{ Name = 'MSBuild'; Command = $MSBuild; Arguments = @('-restore') }
)
foreach ($build in $builds) {
    Write-Host "== $($build.Name)"
    # A fresh obj makes the SDK resolve the package assets again, which is when NETSDK1187 shows up.
    foreach ($dir in 'bin', 'obj') {
        $path = Join-Path $project $dir
        if (Test-Path $path) { Remove-Item $path -Recurse -Force }
    }
    $warningsLog = Join-Path ([IO.Path]::GetTempPath()) "PackageConsumer.$([IO.Path]::GetRandomFileName()).log"
    & $build.Command @($build.Arguments) (Join-Path $project 'PackageConsumer.csproj') -nologo -v:m -nr:false `
        -p:Configuration=Release "-p:ModernPackageVersion=$Version" "-flp:LogFile=$warningsLog;WarningsOnly"
    $built = $LASTEXITCODE -eq 0
    # The node prefix (1:20>) that parallel builds put on each line is dropped.
    $warnings = @(Get-Content $warningsLog | Where-Object { $_ -match '\bwarning (NU\d+|NETSDK\d+|MSB3277)\b' } |
        ForEach-Object { ($_ -replace '^\s*\d+(:\d+)?>', '').Trim() } | Sort-Object -Unique)
    Remove-Item $warningsLog
    foreach ($warning in $warnings) { $failures.Add("$($build.Name): $warning") }
    if (-not $built) {
        $failures.Add("$($build.Name): the build failed.")
        continue
    }

    foreach ($tfm in $frameworks) {
        Write-Host "-- $($build.Name), $tfm"
        $output = Join-Path $project "bin\Release\$tfm"
        $cultures = @(Get-ChildItem $output -Directory |
            Where-Object { Get-ChildItem $_.FullName -Filter 'iNKORE.UI.WPF.Modern*.resources.dll' } | ForEach-Object Name)
        if ($cultures.Count -gt 0) {
            $failures.Add("$($build.Name), $($tfm): the output has $($cultures.Count) culture folders from the package ($(($cultures | Select-Object -First 5) -join ', '), ...).")
        }
        $problem = Invoke-Consumer (Join-Path $output 'PackageConsumer.exe')
        if ($problem) { $failures.Add("$($build.Name), $($tfm): the app $problem.") }
    }
}

if ($failures.Count -gt 0) {
    foreach ($failure in $failures) { Write-Host "FAIL $failure" }
    exit 1
}
Write-Host "OK   Built with dotnet build and MSBuild without culture folders, ran on $($frameworks -join ', ')."
