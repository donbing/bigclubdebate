using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BigClubDebate.Data;

namespace BigClubDebate.Web.Data
{
    public class WeatherForecastService
    {
        string path = @"C:\Users\chris\source\repos\ConsoleApp1\ConsoleApp1\GameData";
        private IOrderedEnumerable<Year> years;
        private IEnumerable<CupGame> leagueCup;
        private IEnumerable<CupGame> facup;
        private IEnumerable<Game> allGames;

        public WeatherForecastService()
        {
            years = Directory.EnumerateDirectories(Path.Combine(path, "england-master"), "????-??", SearchOption.AllDirectories)
                .Select(yearPath => Year.FromPath(yearPath))
                .ToList()
                .OrderBy(x => x.name);

            leagueCup = File.ReadAllText(Path.Combine(path, "leaguecup.csv.txt"))
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(x => x.Split(","))
                .Select(x => CupGame.FromCsv(x))
                .Where(x => x != null).ToList();


            facup = File.ReadAllText(Path.Combine(path, "facup.csv.txt"))
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(x => x.Split(","))
                .Select(x => CupGame.FromCsv(x))
                .Where(x => x != null)
                .ToList();


            var allLeagues = years.SelectMany(x => x.Leagues);
            allGames = allLeagues.SelectMany(y => y.games);
        }

        public async Task<(TeamStats, TeamStats)> GetDivision(int division, TeamName team1, TeamName team2)
        {
            var topLeague = years.Select(x => x.GetLeague(division));
            var allGames = topLeague.SelectMany(y => y.games);
            var tables = topLeague.Select(x => x.Table);
            var utd = new TeamStats(team1, allGames, tables);
            var weds = new TeamStats(team2, allGames, tables);

            return (utd, weds);
        }

        public async Task<(TeamStats, TeamStats)> HeadToHead(TeamName team1, TeamName team2)
        {
            var headtoHeadGames = allGames.Where(g => g.Teams.All(t => team1.Matches(t) || team2.Matches(t)));

            var utd = new TeamStats(team1, headtoHeadGames, null);
            var weds = new TeamStats(team2, headtoHeadGames, null);

            return (utd, weds);
        }
        public async Task<(TeamStats, TeamStats)> FaCup(TeamName team1, TeamName team2)
        {
            var standings = facup.GroupBy(x => x.year).Select(year => 
                year.SelectMany(x => x.Teams).Distinct().OrderByDescending(t => year.Count(g => g.Winner == t)).ToList()
            );
            
            var utd = new TeamStats(team1, facup, standings);
            var weds = new TeamStats(team2, facup, standings);

            return (utd, weds);
        }
        public async Task<(TeamStats, TeamStats)> LeagueCup(TeamName team1, TeamName team2)
        {
            var standings = leagueCup.GroupBy(x => x.year).Select(year =>
                year.SelectMany(x => x.Teams).Distinct().OrderByDescending(t => year.Count(g => g.Winner == t)).ToList()
            );
            var utd = new TeamStats(team1, leagueCup, standings);
            var weds = new TeamStats(team2, leagueCup, standings);

            return (utd, weds);
        }
    }
}
