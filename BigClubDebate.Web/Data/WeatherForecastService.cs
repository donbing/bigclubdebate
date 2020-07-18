using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BigClubDebate.Data;
using BigClubDebate.Data.Model;

namespace BigClubDebate.Web.Data
{
    public class WeatherForecastService
    {
        private IEnumerable<LeagueYear> years;
        private IEnumerable<CupGame> leagueCup;
        private IEnumerable<CupGame> facup;
        private IEnumerable<Game> allLeagueGames;

        public WeatherForecastService()
        {
            var binDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var path = Path.Combine(binDir, "GameData");

            var r = new OpenFootballEnglishLeagueReader(path);

            years = r.LeagueYears;
            facup = r.FaCupGames;
            leagueCup = r.LeagueCupGames;

            var allLeagues = years.SelectMany(x => x.Leagues);
            allLeagueGames = allLeagues.SelectMany(y => y.Games);
        }

        public Task<(TeamStats, TeamStats)> GetDivision(int division, TeamName team1, TeamName team2)
        {
            var topLeague = years.Select(x => x.GetLeague(division));
            var allGames = topLeague.SelectMany(y => y.Games);
            var tables = topLeague.ToLookup(x =>x.Year,x => (List<string>) new LeagueTable(x.Games));

            return FromResult(team1, team2, allGames, tables);
        }

        public Task<(TeamStats, TeamStats)> HeadToHead(TeamName team1, TeamName team2)
        {
            var headtoHeadGames = allLeagueGames
                .Where(game => team1.PlayedIn(game) && team2.PlayedIn(game));

            return FromResult(team1, team2, headtoHeadGames, null);
        }

        public Task<(TeamStats, TeamStats)> FaCup(TeamName team1, TeamName team2)
        {
            var standings = Standings(facup);

            return FromResult(team1, team2, facup, standings);
        }

        public  Task<(TeamStats, TeamStats)> LeagueCup(TeamName team1, TeamName team2)
        {
            var standings = Standings(leagueCup);

            return FromResult(team1, team2, leagueCup, standings);
        }

        private ILookup<string, List<string>> Standings(IEnumerable<CupGame> cupGames)
        {
            return cupGames.GroupBy(x => x.Year)
                .ToLookup(
                    year => year.Key,
                    year => year.SelectMany(x => x.Teams)
                        .Distinct()
                        .OrderByDescending(t => year.Count(g => g.Winner == t))
                        .ToList()
                );
        }

        private Task<(TeamStats, TeamStats)> FromResult(TeamName team1, TeamName team2, IEnumerable<Game> cupGames, ILookup<string, List<string>> yearlyTables)
        {
            var utd = new TeamStats(team1, cupGames, yearlyTables);
            var weds = new TeamStats(team2, cupGames, yearlyTables);

            return Task.FromResult((utd, weds));
        }
    }
}
