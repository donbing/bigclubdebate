using System.Collections.Generic;
using System.Linq;

namespace BigClubDebate.Data.Model
{
    public class LeagueTable : List<string>
    {
        public LeagueTable(IEnumerable<Game> games) : base(OrderGames(games))
        {
        }

        private static List<string> OrderGames(IEnumerable<Game> games)
        {
            var st = games
                .Select(g => g.Home)
                .Distinct()
                .ToDictionary(teamName => teamName, startCount => 0);

            foreach (var game in games)
            {
                st[game.Home] += game.PointsFor(game.Home);
                st[game.Away] += game.PointsFor(game.Away);
            }

            return st
                .OrderByDescending(x => x.Value)
                .Select(x => x.Key)
                .ToList();
        }
    }
}