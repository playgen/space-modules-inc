# Must be run from ./Tools directory

# Copy Gamework.Core into GameWork.unity
Push-Location "../GameWork.Unity/Tools"
& "./Copy_GameWorkCore.ps1"
Pop-Location

# Copy GameWork.Unity

function CopyGameworkComponent([string] $sourceDir, [string] $destDir)
{
    If(Test-Path $destDir)
    {
        Write-Output "Removing: $destDir"
        Remove-Item $destDir -Recurse
    }

    New-Item -ItemType directory $destDir | Out-Null

    Get-ChildItem -Path $sourceDir | Copy-Item -Destination $destDir -Recurse -Container
}

CopyGameworkComponent "..\GameWork.Unity\UnityProject\Assets\GameWork\Core" "..\Assets\GameWork\Core"
CopyGameworkComponent "..\GameWork.Unity\UnityProject\Assets\GameWork\Unity" "..\Assets\GameWork\Unity"