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

        private static List<string> CalculateLeagueTable(IEnumerable<Game> games)
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