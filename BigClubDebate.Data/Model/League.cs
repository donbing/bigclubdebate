using BigClubDebate.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BigClubDebate.Data
{
    public class League
    {
        public string name;
        public int Priroriry;

        public List<string> Table
        {
            get
            {
                var st = games.Select(g => g.Home).Distinct().ToDictionary(x => x, x => 0);
                foreach (var game in games)
                {
                    st[game.Home] += game.PointsFor(game.Home);
                    st[game.Away] += game.PointsFor(game.Away);
                }

                return st.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
            }
        }

        public IEnumerable<Game> games { get; set; }
        public IEnumerable<Fixture> fixtures { get; set; }


        public override string ToString() 
            => $"{name+Environment.NewLine}{string.Join(Environment.NewLine,games)}{string.Join(Environment.NewLine, fixtures)}";
    }
}