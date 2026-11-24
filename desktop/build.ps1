# Builds desktop\StarForge.exe from site\game\star-forge.html and copies it
# into site\downloads. Run from anywhere: powershell -File desktop\build.ps1
$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot
$root = Split-Path $here -Parent
$game = Join-Path $root 'site\game\star-forge.html'
$ico  = Join-Path $here 'star.ico'
$exe  = Join-Path $here 'StarForge.exe'
$csc  = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'

# Draw the app icon (a glowing star orb) if it doesn't exist yet.
if (-not (Test-Path $ico)) {
    Add-Type -AssemblyName System.Drawing
    $bmp = New-Object System.Drawing.Bitmap 64, 64
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = 'AntiAlias'
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $path.AddEllipse(2, 2, 60, 60)
    $brush = New-Object System.Drawing.Drawing2D.PathGradientBrush $path
    $brush.CenterPoint = New-Object System.Drawing.PointF 26, 22
    $brush.CenterColor = [System.Drawing.Color]::FromArgb(255, 255, 246, 201)
    $brush.SurroundColors = @([System.Drawing.Color]::FromArgb(255, 200, 80, 30))
    $g.FillEllipse($brush, 2, 2, 60, 60)
    $g.Dispose()
    $icon = [System.Drawing.Icon]::FromHandle($bmp.GetHicon())
    $fs = [System.IO.File]::Create($ico); $icon.Save($fs); $fs.Close()
}

& $csc /nologo /target:winexe /optimize+ "/out:$exe" "/win32icon:$ico" `
    "/resource:$game,star-forge.html" /reference:System.Windows.Forms.dll `
    (Join-Path $here 'Launcher.cs')
if ($LASTEXITCODE -ne 0) { throw "Compile failed" }

$downloads = Join-Path $root 'site\downloads'
New-Item -ItemType Directory -Force $downloads | Out-Null
Copy-Item $exe $downloads -Force
Compress-Archive -Path $game -DestinationPath (Join-Path $downloads 'StarForge.zip') -Force
Get-Item $exe | Select-Object Name, Length
