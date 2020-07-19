using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model.DataTypes;

namespace BigClubDebate.Data.Model.DataSources
{
    public class LeagueGames
    {
        readonly OpenFootballEnglishLeagueReader data;

        public LeagueGames(OpenFootballEnglishLeagueReader data) 
            => this.data = data;

        public IEnumerable<Game> HeadToHeadGames(TeamName team1, TeamName team2) 
            => data.LeagueSeasons
                .SelectMany(x => x.Divisions)
                .SelectMany(y => y.Games)
                .Where(game => team1.PlayedIn(game) && team2.PlayedIn(game))
                .ToList();

        public List<Game> DivisionGames(int division) 
            => data.LeagueSeasons
                .Select(x => x.GetDivision(division))
                .SelectMany(x => x.Games)
                .ToList();

        public ILookup<string, List<string>> DivisionTables(int division) 
            => DivisionGames(division)
                .GroupBy(x => x.Season)
                .ToLookup(year => year.Key, year => new LeagueTable(year).ToList());
    }
}