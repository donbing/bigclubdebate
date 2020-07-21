using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model.DataTypes;

namespace BigClubDebate.Data.Model.DataSources
{
    public class LeagueTable : List<string>
    {
        public IEnumerable<Game> Games { get; }

        public LeagueTable(IEnumerable<Game> games) : base(CalculateLeagueTable(games))
        {
            Games = games;
        }

        public static List<string> CalculateLeagueTable(IEnumerable<Game> games)
        {
            var orderByDescending = TableAndPoints(games);

            var enumerable = orderByDescending.Select(x => x.Item1);

            return enumerable.ToList();
        }

        public static IOrderedEnumerable<(string, int, List<string>)> TableAndPoints(IEnumerable<Game> games)
        {
            var teams = games
                .Select(g => g.Home)
                .Distinct();

            var st = teams
                .ToDictionary(teamName => teamName, startCount => 0);
            var pl = teams
                .ToDictionary(teamName => teamName, startCount => new List<string>());

            var fails = st.Where(s => games.Count(g => s.Key == g.Home) != games.Count(g => s.Key == g.Away)).ToList();
            var c = games.Count();
            //var d = games.Min(x => x.Date);
            //var e = games.Max(x => x.Date);

            var losers = games.Where(g => g.Loser == "Sheffield Wednesday").ToList();
            var winner = games.Where(g => g.Winner == "Sheffield Wednesday").ToList();
            var draw = games.Where(g => g.Drawn && g.Teams.Contains("Sheffield Wednesday")).ToList();

            foreach (var game in games)
            {
                st[game.Home] += game.PointsFor(game.Home);
                st[game.Away] += game.PointsFor(game.Away);
                pl[game.Home].Add($"{game.PointsFor(game.Home)}:{game.HomeGoals}-{game.AwayGoals}");
                pl[game.Away].Add($"{game.PointsFor(game.Away)}:{game.HomeGoals}-{game.AwayGoals}");
                
            }

            return st.Select(s=>(s.Key,s.Value,pl[s.Key]))
                .OrderByDescending(x => x.Item2);
        }
    }
}