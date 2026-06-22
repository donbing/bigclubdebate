using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model;
using BigClubDebate.Data.Model.DataTypes;
using BigClubDebate.Data.Model.Reader;

namespace BigClubDebate.Data
{
    public class Teams : List<TeamName>
    {
        public TeamName SheffUtd = new TeamName(new[] { "Sheffield United FC", "Blades", "Sheffield Utd", "Sheffield United" }, "sheffutd_logo.png", "sheffutd_background.jpg");
        public TeamName SheffWeds = new TeamName(new[] { "Sheffield Wednesday FC", "Owls", "Sheffield Wed", "Sheffield Wednesday" }, "sheffwed_logo.png", "sheffwed_background.jpg");
        public TeamName Leeds = new TeamName(new[] { "Leeds United FC", "Leeds", "Leeds United", "Leeds Utd" }, "leeds_logo.png", "leeds_background.jpg");
        public TeamName ManUtd = new TeamName(new[] { "Manchester United FC", "Red Devils", "Manchester United", "Manchester Utd", "Man Utd" }, "manu_logo.png", "manu_background.jpg");
        public TeamName ManCity = new TeamName(new[] { "Manchester City FC", "Man City", "Manchester City" }, "manc_logo.png", "manc_background.jpg");
        public TeamName LeicesterCity = new TeamName(new[] { "Leicester City FC", "The Foxes", "Leicester City" }, "leicester_logo.png", "Leicester_background.jpg");
        public TeamName Liverpool = new TeamName(new[] { "Liverpool FC", "The Reds", "Liverpool" }, "Liverpool_Logo.png");
        public TeamName Everton = new TeamName(new[] { "Everton FC", "The Toffees", "Everton" }, "Everton_logo.png");
        public TeamName Chelsea = new TeamName(new[] { "Chelsea FC", "Chelsea", "Chelsea" }, "Chelsea_Logo.png");
        public TeamName Arsenal = new TeamName(new[] { "Arsenal FC", "The Gunners", "Arsenal" }, "Arsenal_Logo.png");
        
        public TeamName Tottenham = new TeamName(new[] { "Tottenham Hotspur FC","Lilly Whites","Tottenham"}, "tottenham_logo.png");
        public TeamName AstonVilla = new TeamName(new[] { "Aston Villa FC", "Villa", "Aston Villa" }, "villa_logo.png");
        public TeamName BirminghamCity = new TeamName(new[] { "Birmingham City FC", "Birmingham", "Birmingham City" }, "Birmingham_logo.png");
        public TeamName WestHam = new TeamName(new[] { "West Ham United FC", "The Hammers", "West Ham United", "West Ham" }, "WestHam_Logo.png");
        public TeamName BlackburnRovers = new TeamName(new[] { "Blackburn Rovers FC", "Rovers", "Blackburn Rovers", "Blackburn" }, "Blackburn_logo.png");

        /// <summary>
        /// Mapping from a data-file team name to its TeamName object.
        /// Populated from both the hardcoded "big clubs" and all teams found in game data.
        /// </summary>
        readonly Dictionary<string, TeamName> _teamNameLookup;

        public Teams(FootyDataReader dataReader) : this()
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