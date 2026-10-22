# Helpers shared by the package tests.

# Visual Studio's MSBuild.exe, which runs on .NET Framework (dotnet build runs on .NET).
function Find-MSBuild {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path $vswhere) {
        $path = & $vswhere -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' |
            Select-Object -First 1
        if ($path) { return $path }
    }
    throw "Visual Studio's MSBuild.exe wasn't found; pass its path with -MSBuild."
}
