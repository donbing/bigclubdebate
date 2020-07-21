using System;
using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model.DataTypes;

namespace BigClubDebate.Data.Model.DataSources
{
    public class CupTable : List<string>
    {
        public static IEnumerable<CupGame> Finals;
        public string Name { get; set; }

        public CupTable(IGrouping<string, CupGame> seasonsCupGames) : base(OrderSeasonCupTable(seasonsCupGames))
        {
            Name = seasonsCupGames.Key;
        }

        private static IEnumerable<string> OrderSeasonCupTable(IGrouping<string, CupGame> seasonsCupGames)
        {
            var finalIds = new[] {"final", "Final", "f"};
            Finals = seasonsCupGames
                .OrderByDescending(x => x.Date)
                .Where(x => finalIds.Contains(x.Type));
            var final = Finals
                .Last();

            if (final == null)
            {
                var c = seasonsCupGames.Select(x => x.Type).Distinct();
                throw new Exception("cant find cup final");
            }
            // dirty hac to not deal with replays, 2leg games and other shenanigans
            var winners = new[] { final.Winner, final.Loser };
            var others = seasonsCupGames
                .SelectMany(x => x.Teams)
                .Distinct()
                .Except(winners);

            return winners.Concat(others).ToList();
        }
    }
}