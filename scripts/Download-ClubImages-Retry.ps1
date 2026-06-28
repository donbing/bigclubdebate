<#
.SYNOPSIS
    Downloads missing club badge (logo) and fanart (background) images.
    Uses TheSportsDB bulk endpoints first, then falls back to Wikipedia REST API.
.NOTES
    Run from repo root:  .\scripts\Download-ClubImages-Retry.ps1
    Use -InitialWaitSecs 120 if you recently hit rate limits.
    Use -Force to re-download files that already exist.

    TheSportsDB league IDs (v1 free tier):
      4328-4331 = English league tiers
    Wikipedia REST API: https://en.wikipedia.org/api/rest_v1/page/summary/{slug}
#>

param(
    [int]$InitialWaitSecs = 60,
    [switch]$Force   # re-download even if file exists
)

$badgesDir     = "BigClubDebate.Web\wwwroot\images\badges"
$backgroundDir = "BigClubDebate.Web\wwwroot\backgrounds"

# File name mapping: TheSportsDB team name -> [logo filename, background filename]
$fileMap = @{
    "Arsenal"                    = @("Arsenal_Logo.png",          $null)
    "Aston Villa"                = @("villa_logo.png",            $null)
    "Bournemouth"                = @("bournemouth_logo.png",      "bournemouth_background.jpg")
    "Brentford"                  = @("brentford_logo.png",        "brentford_background.jpg")
    "Brighton and Hove Albion"   = @("brighton_logo.png",         "brighton_background.jpg")
    "Chelsea"                    = @("Chelsea_Logo.png",          $null)
    "Crystal Palace"             = @("crystalpalace_logo.png",    "crystalpalace_background.jpg")
    "Everton"                    = @("Everton_logo.png",          $null)
    "Fulham"                     = @("fulham_logo.png",           "fulham_background.jpg")
    "Ipswich Town"               = @("ipswich_logo.png",          "ipswich_background.jpg")
    "Leicester City"             = @("leicester_logo.png",        "Leicester_background.jpg")
    "Liverpool"                  = @("Liverpool_Logo.png",        $null)
    "Manchester City"            = @("manc_logo.png",             "manc_background.jpg")
    "Manchester United"          = @("manu_logo.png",             "manu_background.jpg")
    "Newcastle United"           = @("newcastle_logo.png",        "newcastle_background.jpg")
    "Nottingham Forest"          = @("nottmforest_logo.png",      "nottmforest_background.jpg")
    "Southampton"                = @("southampton_logo.png",      "southampton_background.jpg")
    "Tottenham Hotspur"          = @("tottenham_logo.png",        $null)
    "West Ham United"            = @("WestHam_Logo.png",          $null)
    "Wolverhampton Wanderers"    = @("wolves_logo.png",           "wolves_background.jpg")
    "Blackburn Rovers"           = @("Blackburn_logo.png",        $null)
    "Bristol City"               = @("bristolcity_logo.png",      "bristolcity_background.jpg")
    "Burnley"                    = @("burnley_logo.png",          "burnley_background.jpg")
    "Cardiff City"               = @("cardiff_logo.png",          "cardiff_background.jpg")
    "Coventry City"              = @("coventry_logo.png",         "coventry_background.jpg")
    "Derby County"               = @("derby_logo.png",            "derby_background.jpg")
    "Hull City"                  = @("hull_logo.png",             "hull_background.jpg")
    "Leeds United"               = @("leeds_logo.png",            "leeds_background.jpg")
    "Luton Town"                 = @("luton_logo.png",            "luton_background.jpg")
    "Middlesbrough"              = @("middlesbrough_logo.png",    "middlesbrough_background.jpg")
    "Millwall"                   = @("millwall_logo.png",         "millwall_background.jpg")
    "Norwich City"               = @("norwich_logo.png",          "norwich_background.jpg")
    "Oxford United"              = @("oxford_logo.png",           "oxford_background.jpg")
    "Plymouth Argyle"            = @("plymouth_logo.png",         "plymouth_background.jpg")
    "Portsmouth"                 = @("portsmouth_logo.png",       "portsmouth_background.jpg")
    "Preston North End"          = @("preston_logo.png",          "preston_background.jpg")
    "Queens Park Rangers"        = @("qpr_logo.png",              "qpr_background.jpg")
    "Sheffield United"           = @("sheffutd_logo.png",         "sheffutd_background.jpg")
    "Sheffield Wednesday"        = @("sheffwed_logo.png",         "sheffwed_background.jpg")
    "Stoke City"                 = @("stoke_logo.png",            "stoke_background.jpg")
    "Sunderland"                 = @("sunderland_logo.png",       "sunderland_background.jpg")
    "Swansea City"               = @("swansea_logo.png",          "swansea_background.jpg")
    "Watford"                    = @("watford_logo.png",          "watford_background.jpg")
    "West Bromwich Albion"       = @("westbrom_logo.png",         "westbrom_background.jpg")
    "Barnsley"                   = @("barnsley_logo.png",         "barnsley_background.jpg")
    "Birmingham City"            = @("Birmingham_logo.png",       $null)
    "Blackpool"                  = @("blackpool_logo.png",        "blackpool_background.jpg")
    "Bolton Wanderers"           = @("bolton_logo.png",           "bolton_background.jpg")
    "Bristol Rovers"             = @("bristolrovers_logo.png",    "bristolrovers_background.jpg")
    "Burton Albion"              = @("burton_logo.png",           "burton_background.jpg")
    "Cambridge United"           = @("cambridge_logo.png",        "cambridge_background.jpg")
    "Charlton Athletic"          = @("charlton_logo.png",         "charlton_background.jpg")
    "Crawley Town"               = @("crawley_logo.png",          "crawley_background.jpg")
    "Exeter City"                = @("exeter_logo.png",           "exeter_background.jpg")
    "Huddersfield Town"          = @("huddersfield_logo.png",     "huddersfield_background.jpg")
    "Leyton Orient"              = @("leytonorient_logo.png",     "leytonorient_background.jpg")
    "Lincoln City"               = @("lincoln_logo.png",          "lincoln_background.jpg")
    "MK Dons"                    = @("mkdons_logo.png",           "mkdons_background.jpg")
    "Northampton Town"           = @("northampton_logo.png",      "northampton_background.jpg")
    "Peterborough United"        = @("peterborough_logo.png",     "peterborough_background.jpg")
    "Reading"                    = @("reading_logo.png",          "reading_background.jpg")
    "Rotherham United"           = @("rotherham_logo.png",        "rotherham_background.jpg")
    "Shrewsbury Town"            = @("shrewsbury_logo.png",       "shrewsbury_background.jpg")
    "Stevenage"                  = @("stevenage_logo.png",        "stevenage_background.jpg")
    "Stockport County"           = @("stockport_logo.png",        "stockport_background.jpg")
    "Wigan Athletic"             = @("wigan_logo.png",            "wigan_background.jpg")
    "Wrexham"                    = @("wrexham_logo.png",          "wrexham_background.jpg")
    "Wycombe Wanderers"          = @("wycombe_logo.png",          "wycombe_background.jpg")
    "AFC Wimbledon"              = @("wimbledon_logo.png",        "wimbledon_background.jpg")
    "Accrington Stanley"         = @("accrington_logo.png",       "accrington_background.jpg")
    "Barrow"                     = @("barrow_logo.png",           "barrow_background.jpg")
    "Bradford City"              = @("bradford_logo.png",         "bradford_background.jpg")
    "Bromley"                    = @("bromley_logo.png",          "bromley_background.jpg")
    "Carlisle United"            = @("carlisle_logo.png",         "carlisle_background.jpg")
    "Cheltenham Town"            = @("cheltenham_logo.png",       "cheltenham_background.jpg")
    "Colchester United"          = @("colchester_logo.png",       "colchester_background.jpg")
    "Crewe Alexandra"            = @("crewe_logo.png",            "crewe_background.jpg")
    "Doncaster Rovers"           = @("doncaster_logo.png",        "doncaster_background.jpg")
    "Fleetwood Town"             = @("fleetwood_logo.png",        "fleetwood_background.jpg")
    "Gillingham"                 = @("gillingham_logo.png",       "gillingham_background.jpg")
    "Grimsby Town"               = @("grimsby_logo.png",          "grimsby_background.jpg")
    "Harrogate Town"             = @("harrogate_logo.png",        "harrogate_background.jpg")
    "Mansfield Town"             = @("mansfield_logo.png",        "mansfield_background.jpg")
    "Morecambe"                  = @("morecambe_logo.png",        "morecambe_background.jpg")
    "Newport County"             = @("newport_logo.png",          "newport_background.jpg")
    "Notts County"               = @("nottscounty_logo.png",      "nottscounty_background.jpg")
    "Port Vale"                  = @("portvale_logo.png",         "portvale_background.jpg")
    "Salford City"               = @("salford_logo.png",          "salford_background.jpg")
    "Swindon Town"               = @("swindon_logo.png",          "swindon_background.jpg")
    "Tranmere Rovers"            = @("tranmere_logo.png",         "tranmere_background.jpg")
    "Walsall"                    = @("walsall_logo.png",          "walsall_background.jpg")
    "Chesterfield"               = @("chesterfield_logo.png",     "chesterfield_background.jpg")
}

$leagueIds = @(4328, 4329, 4330, 4331)   # PL, Championship, L1, L2
$allTeams  = @()

function Invoke-WithBackoff {
    param([string]$Url, [int]$MaxRetries = 5)
    $delay = $InitialWaitSecs
    for ($i = 0; $i -lt $MaxRetries; $i++) {
        try {
            return Invoke-RestMethod $Url -TimeoutSec 20
        } catch {
            $msg = $_.ToString()
            if ($msg -match "1015|429|rate") {
                Write-Host "  Rate-limited. Waiting ${delay}s..." -ForegroundColor Yellow
                Start-Sleep -Seconds $delay
                $delay *= 2
            } else {
                throw
            }
        }
    }
    throw "Max retries exceeded for $Url"
}

# Fetch all teams from each league in bulk (4 API calls total)
Write-Host "Fetching team data from TheSportsDB..." -ForegroundColor White
foreach ($lid in $leagueIds) {
    Write-Host "  League $lid..." -NoNewline
    try {
        $resp = Invoke-WithBackoff "https://www.thesportsdb.com/api/v1/json/3/lookup_all_teams.php?id=$lid"
        if ($resp.teams) {
            $allTeams += $resp.teams
            Write-Host " $($resp.teams.Count) teams" -ForegroundColor Green
        } else {
            Write-Host " no data" -ForegroundColor Yellow
        }
    } catch {
        Write-Host " ERROR: $_" -ForegroundColor Red
    }
    Start-Sleep -Seconds 5   # polite pause between league requests
}

Write-Host "Total teams fetched: $($allTeams.Count)" -ForegroundColor Cyan

# Build lookup: name -> team object
$teamLookup = @{}
foreach ($t in $allTeams) {
    $teamLookup[$t.strTeam] = $t
}

# Download images
$downloaded = 0; $skipped = 0; $failed = 0

foreach ($name in $fileMap.Keys | Sort-Object) {
    $logoFile, $bgFile = $fileMap[$name]
    $team = $teamLookup[$name]

    if (-not $team) {
        Write-Host "? Not found in API: $name" -ForegroundColor Yellow
        $failed++
        continue
    }

    $badgeUrl  = $team.strBadge
    $fanartUrl = $team.strFanart1

    $logoPath = Join-Path $badgesDir $logoFile
    if ($badgeUrl -and (-not (Test-Path $logoPath) -or $Force)) {
        try {
            Invoke-WebRequest $badgeUrl -OutFile $logoPath -TimeoutSec 15
            Write-Host "✓ $logoFile" -ForegroundColor Green
            $downloaded++
        } catch { Write-Host "✗ $logoFile : $_" -ForegroundColor Red; $failed++ }
    } elseif (Test-Path $logoPath) {
        $skipped++
    } else {
        Write-Host "- No badge URL: $name" -ForegroundColor DarkGray
    }

    if ($bgFile) {
        $bgPath = Join-Path $backgroundDir $bgFile
        if ($fanartUrl -and (-not (Test-Path $bgPath) -or $Force)) {
            try {
                Invoke-WebRequest $fanartUrl -OutFile $bgPath -TimeoutSec 15
                Write-Host "✓ $bgFile" -ForegroundColor Cyan
                $downloaded++
            } catch { Write-Host "✗ $bgFile : $_" -ForegroundColor Red; $failed++ }
        } elseif (Test-Path $bgPath) {
            $skipped++
        }
    }
}

Write-Host "`n=== Done ===" -ForegroundColor White
Write-Host "Downloaded: $downloaded  |  Skipped (exist): $skipped  |  Failed: $failed"
