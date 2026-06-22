using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model;
using BigClubDebate.Data.Model.DataTypes;
using BigClubDebate.Data.Model.Reader;

namespace BigClubDebate.Data
{
    public class Teams : List<TeamName>
    {
        // ── Premier League ──────────────────────────────────────────────────────
        public TeamName Arsenal        = new(new[] { "Arsenal FC",                   "The Gunners",        "Arsenal"                                        }, "Arsenal_Logo.png");
        public TeamName AstonVilla     = new(new[] { "Aston Villa FC",               "Villa",              "Aston Villa"                                    }, "villa_logo.png");
        public TeamName Bournemouth    = new(new[] { "AFC Bournemouth",              "The Cherries",       "Bournemouth"                                    }, "bournemouth_logo.png",   "bournemouth_background.jpg");
        public TeamName Brentford      = new(new[] { "Brentford FC",                 "The Bees",           "Brentford"                                      }, "brentford_logo.png",     "brentford_background.jpg");
        public TeamName Brighton       = new(new[] { "Brighton & Hove Albion FC",    "The Seagulls",       "Brighton", "Brighton & Hove Albion"              }, "brighton_logo.png",      "brighton_background.jpg");
        public TeamName Chelsea        = new(new[] { "Chelsea FC",                   "The Blues",          "Chelsea"                                        }, "Chelsea_Logo.png");
        public TeamName CrystalPalace  = new(new[] { "Crystal Palace FC",            "The Eagles",         "Crystal Palace"                                 }, "crystalpalace_logo.png", "crystalpalace_background.jpg");
        public TeamName Everton        = new(new[] { "Everton FC",                   "The Toffees",        "Everton"                                        }, "Everton_logo.png");
        public TeamName Fulham         = new(new[] { "Fulham FC",                    "The Cottagers",      "Fulham"                                         }, "fulham_logo.png",        "fulham_background.jpg");
        public TeamName IpswichTown    = new(new[] { "Ipswich Town FC",              "The Tractor Boys",   "Ipswich Town", "Ipswich"                        }, "ipswich_logo.png",       "ipswich_background.jpg");
        public TeamName LeicesterCity  = new(new[] { "Leicester City FC",            "The Foxes",          "Leicester City", "Leicester"                    }, "leicester_logo.png",     "Leicester_background.jpg");
        public TeamName Liverpool      = new(new[] { "Liverpool FC",                 "The Reds",           "Liverpool"                                      }, "Liverpool_Logo.png");
        public TeamName ManCity        = new(new[] { "Manchester City FC",           "Man City",           "Manchester City"                                }, "manc_logo.png",          "manc_background.jpg");
        public TeamName ManUtd         = new(new[] { "Manchester United FC",         "Red Devils",         "Manchester United", "Manchester Utd", "Man Utd" }, "manu_logo.png",          "manu_background.jpg");
        public TeamName Newcastle      = new(new[] { "Newcastle United FC",          "The Magpies",        "Newcastle United", "Newcastle"                  }, "newcastle_logo.png",     "newcastle_background.jpg");
        public TeamName NottmForest    = new(new[] { "Nottingham Forest FC",         "Forest",             "Nottm Forest", "Nottingham Forest"              }, "nottmforest_logo.png",   "nottmforest_background.jpg");
        public TeamName Southampton    = new(new[] { "Southampton FC",               "The Saints",         "Southampton"                                    }, "southampton_logo.png",   "southampton_background.jpg");
        public TeamName Tottenham      = new(new[] { "Tottenham Hotspur FC",         "Spurs",              "Tottenham Hotspur", "Tottenham"                 }, "tottenham_logo.png");
        public TeamName WestHam        = new(new[] { "West Ham United FC",           "The Hammers",        "West Ham United", "West Ham"                    }, "WestHam_Logo.png");
        public TeamName Wolves         = new(new[] { "Wolverhampton Wanderers FC",   "Wolves",             "Wolverhampton Wanderers", "Wolverhampton"       }, "wolves_logo.png",        "wolves_background.jpg");

        // ── Championship ────────────────────────────────────────────────────────
        public TeamName BlackburnRovers  = new(new[] { "Blackburn Rovers FC",       "Rovers",             "Blackburn Rovers", "Blackburn"                  }, "Blackburn_logo.png");
        public TeamName BristolCity      = new(new[] { "Bristol City FC",           "The Robins",         "Bristol City"                                   }, "bristolcity_logo.png",   "bristolcity_background.jpg");
        public TeamName Burnley          = new(new[] { "Burnley FC",                "The Clarets",        "Burnley"                                        }, "burnley_logo.png",       "burnley_background.jpg");
        public TeamName CardiffCity      = new(new[] { "Cardiff City FC",           "The Bluebirds",      "Cardiff City", "Cardiff"                        }, "cardiff_logo.png",       "cardiff_background.jpg");
        public TeamName CoventryCity     = new(new[] { "Coventry City FC",          "The Sky Blues",      "Coventry City", "Coventry"                      }, "coventry_logo.png",      "coventry_background.jpg");
        public TeamName DerbyCounty      = new(new[] { "Derby County FC",           "The Rams",           "Derby County", "Derby"                          }, "derby_logo.png",         "derby_background.jpg");
        public TeamName HullCity         = new(new[] { "Hull City AFC",             "The Tigers",         "Hull City", "Hull"                              }, "hull_logo.png",          "hull_background.jpg");
        public TeamName Leeds            = new(new[] { "Leeds United FC",           "Leeds",              "Leeds United", "Leeds Utd"                      }, "leeds_logo.png",         "leeds_background.jpg");
        public TeamName LutonTown        = new(new[] { "Luton Town FC",             "The Hatters",        "Luton Town", "Luton"                            }, "luton_logo.png",         "luton_background.jpg");
        public TeamName Middlesbrough    = new(new[] { "Middlesbrough FC",          "Boro",               "Middlesbrough"                                  }, "middlesbrough_logo.png", "middlesbrough_background.jpg");
        public TeamName Millwall         = new(new[] { "Millwall FC",               "The Lions",          "Millwall"                                       }, "millwall_logo.png",      "millwall_background.jpg");
        public TeamName NorwichCity      = new(new[] { "Norwich City FC",           "The Canaries",       "Norwich City", "Norwich"                        }, "norwich_logo.png",       "norwich_background.jpg");
        public TeamName OxfordUnited     = new(new[] { "Oxford United FC",          "The U's",            "Oxford United", "Oxford"                        }, "oxford_logo.png",        "oxford_background.jpg");
        public TeamName PlymouthArgyle   = new(new[] { "Plymouth Argyle FC",        "The Pilgrims",       "Plymouth Argyle", "Plymouth"                    }, "plymouth_logo.png",      "plymouth_background.jpg");
        public TeamName Portsmouth       = new(new[] { "Portsmouth FC",             "Pompey",             "Portsmouth"                                     }, "portsmouth_logo.png",    "portsmouth_background.jpg");
        public TeamName PrestonNE        = new(new[] { "Preston North End FC",      "The Lilywhites",     "Preston North End", "Preston"                   }, "preston_logo.png",       "preston_background.jpg");
        public TeamName QPR              = new(new[] { "Queens Park Rangers FC",    "The R's",            "Queens Park Rangers", "QPR"                     }, "qpr_logo.png",           "qpr_background.jpg");
        public TeamName SheffUtd         = new(new[] { "Sheffield United FC",       "Blades",             "Sheffield Utd", "Sheffield United"              }, "sheffutd_logo.png",      "sheffutd_background.jpg");
        public TeamName SheffWeds        = new(new[] { "Sheffield Wednesday FC",    "Owls",               "Sheffield Wed", "Sheffield Wednesday"           }, "sheffwed_logo.png",      "sheffwed_background.jpg");
        public TeamName StokeCity        = new(new[] { "Stoke City FC",             "The Potters",        "Stoke City", "Stoke"                            }, "stoke_logo.png",         "stoke_background.jpg");
        public TeamName Sunderland       = new(new[] { "Sunderland AFC",            "The Black Cats",     "Sunderland"                                     }, "sunderland_logo.png",    "sunderland_background.jpg");
        public TeamName SwanseaCity      = new(new[] { "Swansea City AFC",          "The Swans",          "Swansea City", "Swansea"                        }, "swansea_logo.png",       "swansea_background.jpg");
        public TeamName Watford          = new(new[] { "Watford FC",                "The Hornets",        "Watford"                                        }, "watford_logo.png",       "watford_background.jpg");
        public TeamName WestBrom         = new(new[] { "West Bromwich Albion FC",   "The Baggies",        "West Bromwich Albion", "West Brom", "WBA"        }, "westbrom_logo.png",      "westbrom_background.jpg");

        // ── League One ──────────────────────────────────────────────────────────
        public TeamName Barnsley         = new(new[] { "Barnsley FC",               "The Tykes",          "Barnsley"                                       }, "barnsley_logo.png",      "barnsley_background.jpg");
        public TeamName BirminghamCity   = new(new[] { "Birmingham City FC",        "Blues",              "Birmingham City", "Birmingham"                   }, "Birmingham_logo.png");
        public TeamName Blackpool        = new(new[] { "Blackpool FC",              "The Tangerines",     "Blackpool"                                      }, "blackpool_logo.png",     "blackpool_background.jpg");
        public TeamName Bolton           = new(new[] { "Bolton Wanderers FC",       "The Trotters",       "Bolton Wanderers", "Bolton"                     }, "bolton_logo.png",        "bolton_background.jpg");
        public TeamName BristolRovers    = new(new[] { "Bristol Rovers FC",         "The Gas",            "Bristol Rovers"                                 }, "bristolrovers_logo.png", "bristolrovers_background.jpg");
        public TeamName BurtonAlbion     = new(new[] { "Burton Albion FC",          "The Brewers",        "Burton Albion", "Burton"                        }, "burton_logo.png",        "burton_background.jpg");
        public TeamName CambridgeUnited  = new(new[] { "Cambridge United FC",       "The U's",            "Cambridge United", "Cambridge"                  }, "cambridge_logo.png",     "cambridge_background.jpg");
        public TeamName CharltonAthletic = new(new[] { "Charlton Athletic FC",      "The Addicks",        "Charlton Athletic", "Charlton"                  }, "charlton_logo.png",      "charlton_background.jpg");
        public TeamName CrawleyTown      = new(new[] { "Crawley Town FC",           "The Reds",           "Crawley Town", "Crawley"                        }, "crawley_logo.png",       "crawley_background.jpg");
        public TeamName ExeterCity       = new(new[] { "Exeter City FC",            "The Grecians",       "Exeter City", "Exeter"                          }, "exeter_logo.png",        "exeter_background.jpg");
        public TeamName Huddersfield     = new(new[] { "Huddersfield Town AFC",     "The Terriers",       "Huddersfield Town", "Huddersfield"               }, "huddersfield_logo.png",  "huddersfield_background.jpg");
        public TeamName LeytonOrient     = new(new[] { "Leyton Orient FC",          "The O's",            "Leyton Orient"                                  }, "leytonorient_logo.png",  "leytonorient_background.jpg");
        public TeamName LincolnCity      = new(new[] { "Lincoln City FC",           "The Imps",           "Lincoln City", "Lincoln"                        }, "lincoln_logo.png",       "lincoln_background.jpg");
        public TeamName MKDons           = new(new[] { "MK Dons FC",                "The Dons",           "MK Dons", "Milton Keynes Dons"                  }, "mkdons_logo.png",        "mkdons_background.jpg");
        public TeamName NorthamptonTown  = new(new[] { "Northampton Town FC",       "The Cobblers",       "Northampton Town", "Northampton"                 }, "northampton_logo.png",   "northampton_background.jpg");
        public TeamName Peterborough     = new(new[] { "Peterborough United FC",    "The Posh",           "Peterborough United", "Peterborough"              }, "peterborough_logo.png",  "peterborough_background.jpg");
        public TeamName Reading          = new(new[] { "Reading FC",                "The Royals",         "Reading"                                        }, "reading_logo.png",       "reading_background.jpg");
        public TeamName Rotherham        = new(new[] { "Rotherham United FC",       "The Millers",        "Rotherham United", "Rotherham"                   }, "rotherham_logo.png",     "rotherham_background.jpg");
        public TeamName ShrewsburyTown   = new(new[] { "Shrewsbury Town FC",        "The Shrews",         "Shrewsbury Town", "Shrewsbury"                   }, "shrewsbury_logo.png",    "shrewsbury_background.jpg");
        public TeamName Stevenage        = new(new[] { "Stevenage FC",              "The Boro",           "Stevenage"                                      }, "stevenage_logo.png",     "stevenage_background.jpg");
        public TeamName StockportCounty  = new(new[] { "Stockport County FC",       "The Hatters",        "Stockport County", "Stockport"                  }, "stockport_logo.png",     "stockport_background.jpg");
        public TeamName WiganAthletic    = new(new[] { "Wigan Athletic FC",         "The Latics",         "Wigan Athletic", "Wigan"                        }, "wigan_logo.png",         "wigan_background.jpg");
        public TeamName Wrexham          = new(new[] { "Wrexham AFC",               "The Reds",           "Wrexham"                                        }, "wrexham_logo.png",       "wrexham_background.jpg");
        public TeamName WycombeWanderers = new(new[] { "Wycombe Wanderers FC",      "The Chairboys",      "Wycombe Wanderers", "Wycombe"                   }, "wycombe_logo.png",       "wycombe_background.jpg");

        // ── League Two ──────────────────────────────────────────────────────────
        public TeamName AFCWimbledon     = new(new[] { "AFC Wimbledon",              "The Dons",           "Wimbledon"                                      }, "wimbledon_logo.png",     "wimbledon_background.jpg");
        public TeamName AccringtonStanley= new(new[] { "Accrington Stanley FC",     "Stanley",            "Accrington Stanley", "Accrington"               }, "accrington_logo.png",    "accrington_background.jpg");
        public TeamName Barrow           = new(new[] { "Barrow AFC",                 "The Bluebirds",      "Barrow"                                         }, "barrow_logo.png",        "barrow_background.jpg");
        public TeamName BradfordCity     = new(new[] { "Bradford City AFC",          "The Bantams",        "Bradford City", "Bradford"                      }, "bradford_logo.png",      "bradford_background.jpg");
        public TeamName Bromley          = new(new[] { "Bromley FC",                 "The Ravens",         "Bromley"                                        }, "bromley_logo.png",       "bromley_background.jpg");
        public TeamName CarlisleUnited   = new(new[] { "Carlisle United FC",         "The Cumbrians",      "Carlisle United", "Carlisle"                    }, "carlisle_logo.png",      "carlisle_background.jpg");
        public TeamName CheltenhamTown   = new(new[] { "Cheltenham Town FC",         "The Robins",         "Cheltenham Town", "Cheltenham"                  }, "cheltenham_logo.png",    "cheltenham_background.jpg");
        public TeamName ColchesterUnited = new(new[] { "Colchester United FC",       "The U's",            "Colchester United", "Colchester"                }, "colchester_logo.png",    "colchester_background.jpg");
        public TeamName CreweAlexandra   = new(new[] { "Crewe Alexandra FC",         "The Railwaymen",     "Crewe Alexandra", "Crewe"                       }, "crewe_logo.png",         "crewe_background.jpg");
        public TeamName DoncasterRovers  = new(new[] { "Doncaster Rovers FC",        "The Rovers",         "Doncaster Rovers", "Doncaster"                  }, "doncaster_logo.png",     "doncaster_background.jpg");
        public TeamName FleetwoodTown    = new(new[] { "Fleetwood Town FC",          "The Cod Army",       "Fleetwood Town", "Fleetwood"                    }, "fleetwood_logo.png",     "fleetwood_background.jpg");
        public TeamName Gillingham       = new(new[] { "Gillingham FC",              "The Gills",          "Gillingham"                                     }, "gillingham_logo.png",    "gillingham_background.jpg");
        public TeamName GrimsbyTown      = new(new[] { "Grimsby Town FC",            "The Mariners",       "Grimsby Town", "Grimsby"                        }, "grimsby_logo.png",       "grimsby_background.jpg");
        public TeamName HarrogateTown    = new(new[] { "Harrogate Town AFC",         "Town",               "Harrogate Town", "Harrogate"                    }, "harrogate_logo.png",     "harrogate_background.jpg");
        public TeamName MansfieldTown    = new(new[] { "Mansfield Town FC",          "The Stags",          "Mansfield Town", "Mansfield"                    }, "mansfield_logo.png",     "mansfield_background.jpg");
        public TeamName Morecambe        = new(new[] { "Morecambe FC",               "The Shrimps",        "Morecambe"                                      }, "morecambe_logo.png",     "morecambe_background.jpg");
        public TeamName NewportCounty    = new(new[] { "Newport County AFC",         "The Exiles",         "Newport County", "Newport"                      }, "newport_logo.png",       "newport_background.jpg");
        public TeamName NottsCounty      = new(new[] { "Notts County FC",            "The Magpies",        "Notts County"                                   }, "nottscounty_logo.png",   "nottscounty_background.jpg");
        public TeamName PortVale         = new(new[] { "Port Vale FC",               "The Valiants",       "Port Vale"                                      }, "portvale_logo.png",      "portvale_background.jpg");
        public TeamName SalfordCity      = new(new[] { "Salford City FC",            "The Ammies",         "Salford City", "Salford"                        }, "salford_logo.png",       "salford_background.jpg");
        public TeamName SwindonTown      = new(new[] { "Swindon Town FC",            "The Robins",         "Swindon Town", "Swindon"                        }, "swindon_logo.png",       "swindon_background.jpg");
        public TeamName TranmereRovers   = new(new[] { "Tranmere Rovers FC",         "Rovers",             "Tranmere Rovers", "Tranmere"                    }, "tranmere_logo.png",      "tranmere_background.jpg");
        public TeamName Walsall          = new(new[] { "Walsall FC",                 "The Saddlers",       "Walsall"                                        }, "walsall_logo.png",       "walsall_background.jpg");
        public TeamName Chesterfield     = new(new[] { "Chesterfield FC",            "The Spireites",      "Chesterfield"                                   }, "chesterfield_logo.png",  "chesterfield_background.jpg");

        /// <summary>
        /// Mapping from a data-file team name to its TeamName object.
        /// Populated from both the hardcoded clubs and all teams found in game data.
        /// </summary>
        readonly Dictionary<string, TeamName> _teamNameLookup;

        public Teams(FootyDataReader dataReader) : this()
        {
            _teamNameLookup = new Dictionary<string, TeamName>();

            foreach (var club in this)
            {
                foreach (var alias in club)
                    _teamNameLookup[alias] = club;
            }

            // Collect all unique team names from game data
            var allTeamNames = dataReader.LeagueSeasons
                .SelectMany(s => s.Divisions)
                .SelectMany(d => d.Games)
                .SelectMany(g => new[] { g.Home, g.Away })
                .Concat(dataReader.FaCupGames.SelectMany(g => new[] { g.Home, g.Away }))
                .Concat(dataReader.LeagueCupGames.SelectMany(g => new[] { g.Home, g.Away }))
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            // Add any team not already registered
            foreach (var name in allTeamNames)
            {
                if (!_teamNameLookup.ContainsKey(name))
                {
                    var team = new TeamName(name);
                    _teamNameLookup[name] = team;
                    Add(team);
                }
            }
        }

        /// <summary>
        /// Parameterless constructor — adds all 92 professional English clubs.
        /// </summary>
        public Teams()
        {
            _teamNameLookup = new Dictionary<string, TeamName>();
            AddRange(new[] {
                // Premier League
                Arsenal, AstonVilla, Bournemouth, Brentford, Brighton,
                Chelsea, CrystalPalace, Everton, Fulham, IpswichTown,
                LeicesterCity, Liverpool, ManCity, ManUtd, Newcastle,
                NottmForest, Southampton, Tottenham, WestHam, Wolves,
                // Championship
                BlackburnRovers, BristolCity, Burnley, CardiffCity, CoventryCity,
                DerbyCounty, HullCity, Leeds, LutonTown, Middlesbrough,
                Millwall, NorwichCity, OxfordUnited, PlymouthArgyle, Portsmouth,
                PrestonNE, QPR, SheffUtd, SheffWeds, StokeCity,
                Sunderland, SwanseaCity, Watford, WestBrom,
                // League One
                Barnsley, BirminghamCity, Blackpool, Bolton, BristolRovers,
                BurtonAlbion, CambridgeUnited, CharltonAthletic, CrawleyTown, ExeterCity,
                Huddersfield, LeytonOrient, LincolnCity, MKDons, NorthamptonTown,
                Peterborough, Reading, Rotherham, ShrewsburyTown, Stevenage,
                StockportCounty, WiganAthletic, Wrexham, WycombeWanderers,
                // League Two
                AFCWimbledon, AccringtonStanley, Barrow, BradfordCity, Bromley,
                CarlisleUnited, CheltenhamTown, ColchesterUnited, CreweAlexandra, DoncasterRovers,
                FleetwoodTown, Gillingham, GrimsbyTown, HarrogateTown, MansfieldTown,
                Morecambe, NewportCounty, NottsCounty, PortVale, SalfordCity,
                SwindonTown, TranmereRovers, Walsall, Chesterfield,
            });
        }
    }
}