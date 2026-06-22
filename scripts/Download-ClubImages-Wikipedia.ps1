<#
.SYNOPSIS
    Downloads the remaining ~40 club logos that TheSportsDB couldn't supply,
    using Wikipedia's REST summary API (PNG thumbnails, no API key needed).
.NOTES
    Run from repo root:  .\scripts\Download-ClubImages-Wikipedia.ps1
    Uses a 5-second delay between requests to stay within Wikipedia's rate limit.
    Re-running is safe — existing files are skipped unless -Force is specified.
    If you still hit rate limits, wait 10+ minutes and re-run.
#>

param(
    [int]$DelaySecs = 5,
    [switch]$Force
)

$badgesDir = "BigClubDebate.Web\wwwroot\images\badges"
$bgDir     = "BigClubDebate.Web\wwwroot\backgrounds"

# Wikipedia REST slug -> [logo filename, background filename (null = skip)]
# Slugs use underscores; verified against live Wikipedia pages.
$clubs = @(
    @("Millwall_F.C.",             "millwall_logo.png",        "millwall_background.jpg"),
    @("Norwich_City_F.C.",         "norwich_logo.png",         "norwich_background.jpg"),
    @("Portsmouth_F.C.",           "portsmouth_logo.png",      "portsmouth_background.jpg"),
    @("Preston_North_End_F.C.",    "preston_logo.png",         "preston_background.jpg"),
    @("Queens_Park_Rangers_F.C.",  "qpr_logo.png",             "qpr_background.jpg"),
    @("Stoke_City_F.C.",           "stoke_logo.png",           "stoke_background.jpg"),
    @("Sunderland_A.F.C.",         "sunderland_logo.png",      "sunderland_background.jpg"),
    @("Swansea_City_A.F.C.",       "swansea_logo.png",         "swansea_background.jpg"),
    @("Watford_F.C.",              "watford_logo.png",         "watford_background.jpg"),
    @("West_Bromwich_Albion_F.C.", "westbrom_logo.png",        "westbrom_background.jpg"),
    @("Birmingham_City_F.C.",      "Birmingham_logo.png",      $null),
    @("Bolton_Wanderers_F.C.",     "bolton_logo.png",          "bolton_background.jpg"),
    @("Bristol_Rovers_F.C.",       "bristolrovers_logo.png",   "bristolrovers_background.jpg"),
    @("Charlton_Athletic_F.C.",    "charlton_logo.png",        "charlton_background.jpg"),
    @("Crawley_Town_F.C.",         "crawley_logo.png",         "crawley_background.jpg"),
    @("Exeter_City_F.C.",          "exeter_logo.png",          "exeter_background.jpg"),
    @("Lincoln_City_F.C.",         "lincoln_logo.png",         "lincoln_background.jpg"),
    @("MK_Dons",                   "mkdons_logo.png",          "mkdons_background.jpg"),
    @("Northampton_Town_F.C.",     "northampton_logo.png",     "northampton_background.jpg"),
    @("Rotherham_United_F.C.",     "rotherham_logo.png",       "rotherham_background.jpg"),
    @("Shrewsbury_Town_F.C.",      "shrewsbury_logo.png",      "shrewsbury_background.jpg"),
    @("Wrexham_A.F.C.",            "wrexham_logo.png",         "wrexham_background.jpg"),
    @("Accrington_Stanley_F.C.",   "accrington_logo.png",      "accrington_background.jpg"),
    @("Barrow_A.F.C.",             "barrow_logo.png",          "barrow_background.jpg"),
    @("Carlisle_United_F.C.",      "carlisle_logo.png",        "carlisle_background.jpg"),
    @("Cheltenham_Town_F.C.",      "cheltenham_logo.png",      "cheltenham_background.jpg"),
    @("Colchester_United_F.C.",    "colchester_logo.png",      "colchester_background.jpg"),
    @("Crewe_Alexandra_F.C.",      "crewe_logo.png",           "crewe_background.jpg"),
    @("Fleetwood_Town_F.C.",       "fleetwood_logo.png",       "fleetwood_background.jpg"),
    @("Gillingham_F.C.",           "gillingham_logo.png",      "gillingham_background.jpg"),
    @("Grimsby_Town_F.C.",         "grimsby_logo.png",         "grimsby_background.jpg"),
    @("Harrogate_Town_A.F.C.",     "harrogate_logo.png",       "harrogate_background.jpg"),
    @("Morecambe_F.C.",            "morecambe_logo.png",       "morecambe_background.jpg"),
    @("Newport_County_A.F.C.",     "newport_logo.png",         "newport_background.jpg"),
    @("Port_Vale_F.C.",            "portvale_logo.png",        "portvale_background.jpg"),
    @("Salford_City_F.C.",         "salford_logo.png",         "salford_background.jpg"),
    @("Swindon_Town_F.C.",         "swindon_logo.png",         "swindon_background.jpg"),
    @("Tranmere_Rovers_F.C.",      "tranmere_logo.png",        "tranmere_background.jpg"),
    @("Walsall_F.C.",              "walsall_logo.png",         "walsall_background.jpg"),
    @("Chesterfield_F.C.",         "chesterfield_logo.png",    "chesterfield_background.jpg")
)

$dl = 0; $skipped = 0; $failed = 0

foreach ($entry in $clubs) {
    $slug, $logoFile, $bgFile = $entry
    $logoPath = Join-Path $badgesDir $logoFile

    if ((Test-Path $logoPath) -and -not $Force) {
        Write-Host "- skip $logoFile (exists)" -ForegroundColor DarkGray
        $skipped++
        continue
    }

    Start-Sleep -Seconds $DelaySecs

    try {
        $resp = Invoke-RestMethod "https://en.wikipedia.org/api/rest_v1/page/summary/$slug" -TimeoutSec 15
        $thumb = $resp.thumbnail.source
        if ($thumb) {
            # Upscale thumbnail to 300px width for decent quality
            $thumb = $thumb -replace '/\d+px-', '/300px-'
            Invoke-WebRequest $thumb -OutFile $logoPath -TimeoutSec 20
            Write-Host "✓ $logoFile  [$($resp.title)]" -ForegroundColor Green
            $dl++
        } else {
            Write-Host "? No thumbnail for $slug (page: $($resp.title))" -ForegroundColor Yellow
            $failed++
        }
    } catch {
        Write-Host "✗ $slug : $($_.Exception.Message)" -ForegroundColor Red
        $failed++
    }
}

Write-Host ""
Write-Host "=== Done ===" -ForegroundColor White
Write-Host "Downloaded : $dl"
Write-Host "Skipped    : $skipped  (already exist)"
Write-Host "Failed     : $failed"
if ($failed -gt 0) {
    Write-Host ""
    Write-Host "Tip: If you hit rate limits, wait 10+ minutes then re-run." -ForegroundColor Yellow
}
