<#
.SYNOPSIS
    Downloads club badge (logo) and fanart (background) images from TheSportsDB
    for all 92 English professional football clubs.
.NOTES
    Uses TheSportsDB free API (v1, key=3).
    Run from repo root:  .\scripts\Download-ClubImages.ps1
#>

$badgesDir     = "BigClubDebate.Web\wwwroot\images\badges"
$backgroundDir = "BigClubDebate.Web\wwwroot\backgrounds"

# Mapping: [ TheSportsDB search name, logo filename, background filename (null = skip) ]
$clubs = @(
    # Premier League
    @("Arsenal",                     "Arsenal_Logo.png",          $null),
    @("Aston Villa",                 "villa_logo.png",            $null),
    @("Bournemouth",                 "bournemouth_logo.png",      "bournemouth_background.jpg"),
    @("Brentford",                   "brentford_logo.png",        "brentford_background.jpg"),
    @("Brighton and Hove Albion",    "brighton_logo.png",         "brighton_background.jpg"),
    @("Chelsea",                     "Chelsea_Logo.png",          $null),
    @("Crystal Palace",              "crystalpalace_logo.png",    "crystalpalace_background.jpg"),
    @("Everton",                     "Everton_logo.png",          $null),
    @("Fulham",                      "fulham_logo.png",           "fulham_background.jpg"),
    @("Ipswich Town",                "ipswich_logo.png",          "ipswich_background.jpg"),
    @("Leicester City",              "leicester_logo.png",        "Leicester_background.jpg"),
    @("Liverpool",                   "Liverpool_Logo.png",        $null),
    @("Manchester City",             "manc_logo.png",             "manc_background.jpg"),
    @("Manchester United",           "manu_logo.png",             "manu_background.jpg"),
    @("Newcastle United",            "newcastle_logo.png",        "newcastle_background.jpg"),
    @("Nottingham Forest",           "nottmforest_logo.png",      "nottmforest_background.jpg"),
    @("Southampton",                 "southampton_logo.png",      "southampton_background.jpg"),
    @("Tottenham Hotspur",           "tottenham_logo.png",        $null),
    @("West Ham United",             "WestHam_Logo.png",          $null),
    @("Wolverhampton Wanderers",     "wolves_logo.png",           "wolves_background.jpg"),
    # Championship
    @("Blackburn Rovers",            "Blackburn_logo.png",        $null),
    @("Bristol City",                "bristolcity_logo.png",      "bristolcity_background.jpg"),
    @("Burnley",                     "burnley_logo.png",          "burnley_background.jpg"),
    @("Cardiff City",                "cardiff_logo.png",          "cardiff_background.jpg"),
    @("Coventry City",               "coventry_logo.png",         "coventry_background.jpg"),
    @("Derby County",                "derby_logo.png",            "derby_background.jpg"),
    @("Hull City",                   "hull_logo.png",             "hull_background.jpg"),
    @("Leeds United",                "leeds_logo.png",            "leeds_background.jpg"),
    @("Luton Town",                  "luton_logo.png",            "luton_background.jpg"),
    @("Middlesbrough",               "middlesbrough_logo.png",    "middlesbrough_background.jpg"),
    @("Millwall",                    "millwall_logo.png",         "millwall_background.jpg"),
    @("Norwich City",                "norwich_logo.png",          "norwich_background.jpg"),
    @("Oxford United",               "oxford_logo.png",           "oxford_background.jpg"),
    @("Plymouth Argyle",             "plymouth_logo.png",         "plymouth_background.jpg"),
    @("Portsmouth",                  "portsmouth_logo.png",       "portsmouth_background.jpg"),
    @("Preston North End",           "preston_logo.png",          "preston_background.jpg"),
    @("Queens Park Rangers",         "qpr_logo.png",              "qpr_background.jpg"),
    @("Sheffield United",            "sheffutd_logo.png",         "sheffutd_background.jpg"),
    @("Sheffield Wednesday",         "sheffwed_logo.png",         "sheffwed_background.jpg"),
    @("Stoke City",                  "stoke_logo.png",            "stoke_background.jpg"),
    @("Sunderland",                  "sunderland_logo.png",       "sunderland_background.jpg"),
    @("Swansea City",                "swansea_logo.png",          "swansea_background.jpg"),
    @("Watford",                     "watford_logo.png",          "watford_background.jpg"),
    @("West Bromwich Albion",        "westbrom_logo.png",         "westbrom_background.jpg"),
    # League One
    @("Barnsley",                    "barnsley_logo.png",         "barnsley_background.jpg"),
    @("Birmingham City",             "Birmingham_logo.png",       $null),
    @("Blackpool",                   "blackpool_logo.png",        "blackpool_background.jpg"),
    @("Bolton Wanderers",            "bolton_logo.png",           "bolton_background.jpg"),
    @("Bristol Rovers",              "bristolrovers_logo.png",    "bristolrovers_background.jpg"),
    @("Burton Albion",               "burton_logo.png",           "burton_background.jpg"),
    @("Cambridge United",            "cambridge_logo.png",        "cambridge_background.jpg"),
    @("Charlton Athletic",           "charlton_logo.png",         "charlton_background.jpg"),
    @("Crawley Town",                "crawley_logo.png",          "crawley_background.jpg"),
    @("Exeter City",                 "exeter_logo.png",           "exeter_background.jpg"),
    @("Huddersfield Town",           "huddersfield_logo.png",     "huddersfield_background.jpg"),
    @("Leyton Orient",               "leytonorient_logo.png",     "leytonorient_background.jpg"),
    @("Lincoln City",                "lincoln_logo.png",          "lincoln_background.jpg"),
    @("MK Dons",                     "mkdons_logo.png",           "mkdons_background.jpg"),
    @("Northampton Town",            "northampton_logo.png",      "northampton_background.jpg"),
    @("Peterborough United",         "peterborough_logo.png",     "peterborough_background.jpg"),
    @("Reading",                     "reading_logo.png",          "reading_background.jpg"),
    @("Rotherham United",            "rotherham_logo.png",        "rotherham_background.jpg"),
    @("Shrewsbury Town",             "shrewsbury_logo.png",       "shrewsbury_background.jpg"),
    @("Stevenage",                   "stevenage_logo.png",        "stevenage_background.jpg"),
    @("Stockport County",            "stockport_logo.png",        "stockport_background.jpg"),
    @("Wigan Athletic",              "wigan_logo.png",            "wigan_background.jpg"),
    @("Wrexham",                     "wrexham_logo.png",          "wrexham_background.jpg"),
    @("Wycombe Wanderers",           "wycombe_logo.png",          "wycombe_background.jpg"),
    # League Two
    @("AFC Wimbledon",               "wimbledon_logo.png",        "wimbledon_background.jpg"),
    @("Accrington Stanley",          "accrington_logo.png",       "accrington_background.jpg"),
    @("Barrow",                      "barrow_logo.png",           "barrow_background.jpg"),
    @("Bradford City",               "bradford_logo.png",         "bradford_background.jpg"),
    @("Bromley",                     "bromley_logo.png",          "bromley_background.jpg"),
    @("Carlisle United",             "carlisle_logo.png",         "carlisle_background.jpg"),
    @("Cheltenham Town",             "cheltenham_logo.png",       "cheltenham_background.jpg"),
    @("Colchester United",           "colchester_logo.png",       "colchester_background.jpg"),
    @("Crewe Alexandra",             "crewe_logo.png",            "crewe_background.jpg"),
    @("Doncaster Rovers",            "doncaster_logo.png",        "doncaster_background.jpg"),
    @("Fleetwood Town",              "fleetwood_logo.png",        "fleetwood_background.jpg"),
    @("Gillingham",                  "gillingham_logo.png",       "gillingham_background.jpg"),
    @("Grimsby Town",                "grimsby_logo.png",          "grimsby_background.jpg"),
    @("Harrogate Town",              "harrogate_logo.png",        "harrogate_background.jpg"),
    @("Mansfield Town",              "mansfield_logo.png",        "mansfield_background.jpg"),
    @("Morecambe",                   "morecambe_logo.png",        "morecambe_background.jpg"),
    @("Newport County",              "newport_logo.png",          "newport_background.jpg"),
    @("Notts County",                "nottscounty_logo.png",      "nottscounty_background.jpg"),
    @("Port Vale",                   "portvale_logo.png",         "portvale_background.jpg"),
    @("Salford City",                "salford_logo.png",          "salford_background.jpg"),
    @("Swindon Town",                "swindon_logo.png",          "swindon_background.jpg"),
    @("Tranmere Rovers",             "tranmere_logo.png",         "tranmere_background.jpg"),
    @("Walsall",                     "walsall_logo.png",          "walsall_background.jpg"),
    @("Chesterfield",                "chesterfield_logo.png",     "chesterfield_background.jpg")
)

$results = @()

foreach ($entry in $clubs) {
    $searchName, $logoFile, $bgFile = $entry
    $encodedName = [Uri]::EscapeDataString($searchName)
    $url = "https://www.thesportsdb.com/api/v1/json/3/searchteams.php?t=$encodedName"

    try {
        $resp = Invoke-RestMethod $url -TimeoutSec 10
        $team = $resp.teams | Where-Object { $_.strCountry -eq "England" -or $_.strCountry -eq "Wales" } | Select-Object -First 1
        if (-not $team) { $team = $resp.teams | Select-Object -First 1 }

        $badgeUrl  = $team.strBadge
        $fanartUrl = $team.strFanart1

        # Download logo (skip if file already exists)
        $logoPath = Join-Path $badgesDir $logoFile
        if ($badgeUrl -and -not (Test-Path $logoPath)) {
            Invoke-WebRequest $badgeUrl -OutFile $logoPath -TimeoutSec 15
            Write-Host "✓ Logo:  $logoFile" -ForegroundColor Green
        } elseif (Test-Path $logoPath) {
            Write-Host "- Skip:  $logoFile (exists)" -ForegroundColor DarkGray
        } else {
            Write-Host "✗ No badge URL for $searchName" -ForegroundColor Yellow
        }

        # Download background (skip if null or file already exists)
        if ($bgFile) {
            $bgPath = Join-Path $backgroundDir $bgFile
            if ($fanartUrl -and -not (Test-Path $bgPath)) {
                Invoke-WebRequest $fanartUrl -OutFile $bgPath -TimeoutSec 15
                Write-Host "✓ BG:    $bgFile" -ForegroundColor Cyan
            } elseif (Test-Path $bgPath) {
                Write-Host "- Skip:  $bgFile (exists)" -ForegroundColor DarkGray
            } else {
                Write-Host "✗ No fanart URL for $searchName" -ForegroundColor Yellow
            }
        }

        $results += [PSCustomObject]@{ Club=$searchName; BadgeUrl=$badgeUrl; FanartUrl=$fanartUrl; Status="OK" }
    } catch {
        Write-Host "✗ ERROR for $searchName : $_" -ForegroundColor Red
        $results += [PSCustomObject]@{ Club=$searchName; BadgeUrl=""; FanartUrl=""; Status="ERROR: $_" }
    }

    Start-Sleep -Milliseconds 300  # be kind to the free API
}

Write-Host "`n=== Summary ===" -ForegroundColor White
$results | Group-Object Status | ForEach-Object { Write-Host "$($_.Count) x $($_.Name)" }

$missing = $results | Where-Object { -not $_.BadgeUrl }
if ($missing) {
    Write-Host "`nClubs with no badge URL:" -ForegroundColor Yellow
    $missing | ForEach-Object { Write-Host "  $($_.Club)" }
}
