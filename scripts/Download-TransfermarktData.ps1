$ErrorActionPreference = "Stop"

$gameDataDir = Join-Path $PSScriptRoot "..\Content\GameData\transfermarkt"
$csvUrl = "https://pub-e682421888d945d684bcae8890b0ec20.r2.dev/data/games.csv.gz"
$gzFile = Join-Path $gameDataDir "games.csv.gz"
$csvFile = Join-Path $gameDataDir "games.csv"

Write-Host "Creating directory: $gameDataDir" -ForegroundColor Cyan
New-Item -ItemType Directory -Force -Path $gameDataDir | Out-Null

Write-Host "Downloading games.csv.gz..." -ForegroundColor Cyan
Invoke-WebRequest -Uri $csvUrl -OutFile $gzFile

Write-Host "Decompressing..." -ForegroundColor Cyan
$stream = [System.IO.File]::OpenRead($gzFile)
$gzip = New-Object System.IO.Compression.GZipStream($stream, [System.IO.Compression.CompressionMode]::Decompress)
$output = [System.IO.File]::Create($csvFile)
$gzip.CopyTo($output)
$gzip.Close(); $stream.Close(); $output.Close()

Remove-Item $gzFile

$lineCount = (Get-Content $csvFile | Measure-Object -Line).Lines
$sizeMB = [math]::Round((Get-Item $csvFile).Length / 1MB, 1)

Write-Host "Done! $lineCount rows, ${sizeMB}MB" -ForegroundColor Green
Write-Host $csvFile
