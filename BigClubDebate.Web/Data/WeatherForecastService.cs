using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BigClubDebate.Data;
using Microsoft.Extensions.PlatformAbstractions;

namespace BigClubDebate.Web.Data
{
    public class WeatherForecastService
    {
        //string path = @"C:\Users\chris\source\repos\ConsoleApp1\ConsoleApp1\GameData";
        private IEnumerable<Year> years;
        private IEnumerable<CupGame> leagueCup;
        private IEnumerable<CupGame> facup;
        private IEnumerable<Game> allLeagueGames;

        public WeatherForecastService()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "Content", "GameData");
            var r = new OpenFootballEnglishLeagueReader(path);

            years = r.LeagueYears;
            facup = r.CupGames;
            leagueCup = r.LeagueCup;

            var allLeagues = years.SelectMany(x => x.Leagues);
            allLeagueGames = allLeagues.SelectMany(y => y.games);
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
            var headtoHeadGames = allLeagueGames.Where(g => g.Teams.All(t => team1.Matches(t) || team2.Matches(t)));

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
