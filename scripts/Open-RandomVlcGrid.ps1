[CmdletBinding()]
param(
    [Parameter(Mandatory, Position = 0)]
    [string]$FolderName,

    [string]$VlcPath = '',
    [int]$GridSize = 4,
    [int]$WindowCount = 16
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if ($GridSize -lt 1 -or $WindowCount -lt 1) {
    throw 'GridSize and WindowCount must both be positive integers.'
}

$folder = Resolve-Path -Path $FolderName -ErrorAction Stop
$videoExtensions = @('.mp4', '.mkv', '.avi', '.mov', '.wmv', '.webm', '.m4v', '.mpg', '.mpeg', '.ts', '.m2ts', '.flv')
$videos = @(
    Get-ChildItem -LiteralPath $folder -File -Recurse |
        Where-Object { $videoExtensions -contains $_.Extension.ToLowerInvariant() }
)

if ($videos.Count -eq 0) {
    throw "No supported video files were found under '$folder'."
}

if ([string]::IsNullOrWhiteSpace($VlcPath)) {
    $vlcCommand = Get-Command vlc.exe -ErrorAction SilentlyContinue
    if ($vlcCommand) {
        $VlcPath = $vlcCommand.Source
    } else {
        $knownPaths = @(
            "$env:ProgramFiles\VideoLAN\VLC\vlc.exe",
            "${env:ProgramFiles(x86)}\VideoLAN\VLC\vlc.exe",
            "$env:LOCALAPPDATA\Programs\VideoLAN\VLC\vlc.exe"
        )
        $VlcPath = $knownPaths | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
    }
}

if ([string]::IsNullOrWhiteSpace($VlcPath) -or -not (Test-Path -LiteralPath $VlcPath)) {
    throw 'VLC was not found. Install VLC or pass its path with -VlcPath.'
}

if (-not ('VlcGridNativeMethods' -as [type])) {
    Add-Type -TypeDefinition @'
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class VlcGridNativeMethods
{
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    public delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdc, ref RECT rect, IntPtr data);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MONITORINFO
    {
        public int CbSize;
        public RECT RcMonitor;
        public RECT RcWork;
        public uint DwFlags;
    }

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc callback, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr clip, MonitorEnumProc callback, IntPtr data);

    [DllImport("user32.dll")]
    public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO info);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr insertAfter, int x, int y, int width, int height, uint flags);
}
'@
}

$vlcArguments = @(
    '--no-one-instance',
    '--qt-minimal-view',
    '--no-video-title-show',
    '--no-qt-video-autoresize',
    '--loop'
)

# Shuffle once per invocation, then cycle if the folder has fewer than 16 videos.
$shuffledVideos = @($videos | Get-Random -Count $videos.Count)
$selectedVideos = for ($index = 0; $index -lt $WindowCount; $index++) {
    $shuffledVideos[$index % $shuffledVideos.Count]
}

$startedProcesses = [System.Collections.Generic.List[object]]::new()
foreach ($video in $selectedVideos) {
    $arguments = @($vlcArguments + ('"' + $video.FullName.Replace('"', '\"') + '"'))
    $startedProcesses.Add((Start-Process -FilePath $VlcPath -ArgumentList $arguments -PassThru))
}

$processIds = [System.Collections.Generic.HashSet[uint]]::new()
foreach ($process in $startedProcesses) {
    [void]$processIds.Add([uint32]$process.Id)
}

$windows = [System.Collections.Generic.List[object]]::new()
$deadline = [DateTime]::UtcNow.AddSeconds(20)
while ($windows.Count -lt $WindowCount -and [DateTime]::UtcNow -lt $deadline) {
    $windows.Clear()
    $callback = [VlcGridNativeMethods+EnumWindowsProc]{
        param([IntPtr]$hWnd, [IntPtr]$lParam)

        [uint32]$windowProcessId = 0
        [void][VlcGridNativeMethods]::GetWindowThreadProcessId($hWnd, [ref]$windowProcessId)
        if ($processIds.Contains($windowProcessId) -and [VlcGridNativeMethods]::IsWindowVisible($hWnd)) {
            $windows.Add([pscustomobject]@{ Handle = $hWnd; ProcessId = $windowProcessId })
        }
        return $true
    }
    [void][VlcGridNativeMethods]::EnumWindows($callback, [IntPtr]::Zero)
    if ($windows.Count -lt $WindowCount) { Start-Sleep -Milliseconds 250 }
}

if ($windows.Count -eq 0) {
    throw 'VLC processes started, but no VLC windows became available.'
}

$monitors = [System.Collections.Generic.List[object]]::new()
$monitorCallback = [VlcGridNativeMethods+MonitorEnumProc]{
    param([IntPtr]$hMonitor, [IntPtr]$hdc, [ref]$rect, [IntPtr]$lParam)

    $info = [VlcGridNativeMethods+MONITORINFO]::new()
    $info.CbSize = [Runtime.InteropServices.Marshal]::SizeOf($info)
    if ([VlcGridNativeMethods]::GetMonitorInfo($hMonitor, [ref]$info)) {
        $monitors.Add([pscustomobject]@{
            Left = $info.RcWork.Left
            Top = $info.RcWork.Top
            Right = $info.RcWork.Right
            Bottom = $info.RcWork.Bottom
            Primary = (($info.DwFlags -band 1) -ne 0)
        })
    }
    return $true
}
[void][VlcGridNativeMethods]::EnumDisplayMonitors([IntPtr]::Zero, [IntPtr]::Zero, $monitorCallback, [IntPtr]::Zero)

$primaryMonitor = $monitors | Where-Object Primary | Select-Object -First 1
if (-not $primaryMonitor) {
    throw 'The primary monitor could not be detected.'
}
$monitors = @($primaryMonitor)

$windowsPerMonitor = [int][Math]::Ceiling($windows.Count / [double]$monitors.Count)
for ($index = 0; $index -lt $windows.Count; $index++) {
    $monitor = $monitors[[Math]::Min([int][Math]::Floor($index / [double]$windowsPerMonitor), $monitors.Count - 1)]
    $slot = $index % $windowsPerMonitor
    $columns = [Math]::Min($GridSize, $windowsPerMonitor)
    $rows = [int][Math]::Ceiling($windowsPerMonitor / [double]$columns)
    $column = $slot % $columns
    $row = [int][Math]::Floor($slot / [double]$columns)
    $monitorWidth = $monitor.Right - $monitor.Left
    $monitorHeight = $monitor.Bottom - $monitor.Top
    $x = $monitor.Left + [int][Math]::Floor($column * $monitorWidth / [double]$columns)
    $y = $monitor.Top + [int][Math]::Floor($row * $monitorHeight / [double]$rows)
    $nextX = $monitor.Left + [int][Math]::Floor(($column + 1) * $monitorWidth / [double]$columns)
    $nextY = $monitor.Top + [int][Math]::Floor(($row + 1) * $monitorHeight / [double]$rows)

    [void][VlcGridNativeMethods]::SetWindowPos(
        $windows[$index].Handle,
        [IntPtr]::Zero,
        $x,
        $y,
        ($nextX - $x),
        ($nextY - $y),
        0x0004 -bor 0x0010)
}

Write-Host "Opened and arranged $($windows.Count) VLC windows from '$folder'." -ForegroundColor Green
