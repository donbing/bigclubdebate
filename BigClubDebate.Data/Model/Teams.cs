using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model;
using BigClubDebate.Data.Model.DataTypes;
using BigClubDebate.Data.Model.Reader;

namespace BigClubDebate.Data
{
    public class Teams : List<TeamName>
    {
        public TeamName SheffUtd = new TeamName(new[] { "Sheffield United Football Club", "Sheffield United FC", "Blades", "Sheffield Utd", "Sheffield United" }, "sheffutd_logo.png", "sheffutd_background.jpg");
        public TeamName SheffWeds = new TeamName(new[] { "Sheffield Wednesday Football Club", "Sheffield Wednesday FC", "Owls", "Sheffield Wed", "Sheffield Wednesday" }, "sheffwed_logo.png", "sheffwed_background.jpg");
        public TeamName Leeds = new TeamName(new[] { "Leeds United Football Club", "Leeds United FC", "Leeds", "Leeds United", "Leeds Utd" }, "leeds_logo.png", "leeds_background.jpg");
        public TeamName ManUtd = new TeamName(new[] { "Manchester United Football Club", "Manchester United FC", "Red Devils", "Manchester United", "Manchester Utd", "Man Utd" }, "manu_logo.png", "manu_background.jpg");
        public TeamName ManCity = new TeamName(new[] { "Manchester City Football Club", "Manchester City FC", "Man City", "Manchester City" }, "manc_logo.png", "manc_background.jpg");
        public TeamName LeicesterCity = new TeamName(new[] { "Leicester City Football Club", "Leicester City FC", "The Foxes", "Leicester City" }, "leicester_logo.png", "Leicester_background.jpg");
        public TeamName Liverpool = new TeamName(new[] { "Liverpool Football Club", "Liverpool FC", "The Reds", "Liverpool" }, "Liverpool_Logo.png");
        public TeamName Everton = new TeamName(new[] { "Everton Football Club", "Everton FC", "The Toffees", "Everton" }, "Everton_logo.png");
        public TeamName Chelsea = new TeamName(new[] { "Chelsea Football Club", "Chelsea FC", "Chelsea", "Chelsea" }, "Chelsea_Logo.png");
        public TeamName Arsenal = new TeamName(new[] { "Arsenal Football Club", "Arsenal FC", "The Gunners", "Arsenal" }, "Arsenal_Logo.png");
        
        public TeamName Tottenham = new TeamName(new[] { "Tottenham Hotspur Football Club", "Tottenham Hotspur FC","Lilly Whites","Tottenham"}, "tottenham_logo.png");
        public TeamName AstonVilla = new TeamName(new[] { "Aston Villa Football Club", "Aston Villa FC", "Villa", "Aston Villa" }, "villa_logo.png");
        public TeamName BirminghamCity = new TeamName(new[] { "Birmingham City Football Club", "Birmingham City FC", "Birmingham", "Birmingham City" }, "Birmingham_logo.png");
        public TeamName WestHam = new TeamName(new[] { "West Ham United Football Club", "West Ham United FC", "The Hammers", "West Ham United", "West Ham" }, "WestHam_Logo.png");
        public TeamName BlackburnRovers = new TeamName(new[] { "Blackburn Rovers Football Club", "Blackburn Rovers FC", "Rovers", "Blackburn Rovers", "Blackburn" }, "Blackburn_logo.png");

        /// <summary>
        /// Mapping from a data-file team name to its TeamName object.
        /// Populated from both the hardcoded "big clubs" and all teams found in game data.
        /// </summary>
        readonly Dictionary<string, TeamName> _teamNameLookup;

        public Teams(IGameDataProvider dataReader) : this()
        {
            _teamNameLookup = new Dictionary<string, TeamName>();

            // Register all hardcoded big clubs with all their aliases
            var bigClubs = new[] {
                SheffUtd, SheffWeds, Leeds, ManUtd, ManCity, LeicesterCity,
                Liverpool, Everton, Chelsea, Arsenal, Tottenham, AstonVilla,
                BirminghamCity, WestHam, BlackburnRovers
            };
            foreach (var club in bigClubs)
            {
                foreach (var alias in club)
                {
                    _teamNameLookup[alias] = club;
                }
                Add(club);
            }

            // Collect all unique team names from all data sources (for name matching)
            var allTeamNames = dataReader.GetLeagueSeasons()
                .SelectMany(s => s.Divisions)
                .SelectMany(d => d.Games)
                .SelectMany(g => new[] { g.Home, g.Away })
                .Concat(dataReader.GetFaCupGames().SelectMany(g => new[] { g.Home, g.Away }))
                .Concat(dataReader.GetLeagueCupGames().SelectMany(g => new[] { g.Home, g.Away }))
                .Concat(dataReader.GetEuropeanCupGames().SelectMany(g => new[] { g.Home, g.Away }))
                .Distinct()
                .ToList();

            // Register all team names in the lookup (for matching any data source)
            foreach (var name in allTeamNames)
            {
                if (!_teamNameLookup.ContainsKey(name))
                {
                    var team = new TeamName(name);
                    _teamNameLookup[name] = team;
                }
            }

            // Only show teams from the top 4 English divisions in the current season
            var latestSeason = dataReader.GetLeagueSeasons()
                .OrderByDescending(s => s.Name)
                .FirstOrDefault();

            if (latestSeason != null)
            {
                var displayTeamNames = latestSeason.Divisions
                    .Where(d => d.DivisionPriority >= 1 && d.DivisionPriority <= 4)
                    .SelectMany(d => d.Games)
                    .SelectMany(g => new[] { g.Home, g.Away })
                    .Distinct()
                    .ToList();

                foreach (var name in displayTeamNames)
                {
                    if (_teamNameLookup.TryGetValue(name, out var team))
                    {
                        if (!Contains(team))
                            Add(team);
                    }
                    else
                    {
                        // Check if any existing team matches this name via its aliases
                        var existing = this.FirstOrDefault(t => t.Matches(name));
                        if (existing != null)
                        {
                            _teamNameLookup[name] = existing;
                        }
                        else
                        {
                            var newTeam = new TeamName(name);
                            _teamNameLookup[name] = newTeam;
                            Add(newTeam);
                        }
                    }
                }
            }

            // Deduplicate by MainName — ensure no two TeamName objects share the same display name
            var seen = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            var deduped = new List<TeamName>();
            foreach (var team in this)
            {
                if (seen.Add(team.MainName))
                    deduped.Add(team);
            }
            Clear();
            AddRange(deduped);
        }

        /// <summary>
        /// Parameterless constructor used for testing / console apps.
        /// </summary>
        public Teams()
        {
            _teamNameLookup = new Dictionary<string, TeamName>();
            AddRange(new[] { 
                SheffUtd, SheffWeds, Leeds, ManUtd, ManCity, LeicesterCity, Liverpool,
                Everton, Chelsea, Arsenal, Tottenham, AstonVilla, BirminghamCity, WestHam, BlackburnRovers,
            });
        }
    }
}