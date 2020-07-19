using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data;
using BigClubDebate.Data.Model;

namespace BigClubDebate.Web.Data
{
    public class LeagueData
    {
        readonly OpenFootballEnglishLeagueReader data;

        public LeagueData(OpenFootballEnglishLeagueReader data) 
            => this.data = data;

        public IEnumerable<Game> HeadToHeadGames(TeamName team1, TeamName team2) 
            => data.LeagueSeasons
                .SelectMany(x => x.Divisions)
                .SelectMany(y => y.Games)
                .Where(game => team1.PlayedIn(game) && team2.PlayedIn(game))
                .ToList();

        public List<Game> GetSeasonsForDivision(int division) 
            => data.LeagueSeasons
                .Select(x => x.GetDivision(division))
                .SelectMany(x => x.Games)
                .ToList();

        public ILookup<string, List<string>> GetTables(IEnumerable<Game> leagueGames) 
            => leagueGames
                .GroupBy(x => x.Season)
                .ToLookup(year => year.Key, year => new LeagueTable(year).ToList());

        public ILookup<string, List<string>> GetTables(int division) 
            => GetTables(GetSeasonsForDivision(division));
    }
}