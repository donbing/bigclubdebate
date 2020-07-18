using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BigClubDebate.Data
{
    public class Year
    {
        public IEnumerable<League> Leagues { get; set; }

        public IEnumerable<Game> Games => Leagues.SelectMany(x => x.games);

        public string name;

        public Year(string yearName, IEnumerable<League> leagues) 
        { 
            name = yearName;
            Leagues = leagues;
        }

        public League GetLeague(int v) => Leagues.First(x => x.Priroriry == v);

        public override string ToString() 
            => $"{name+Environment.NewLine}{string.Join(Environment.NewLine, Leagues)}";
    }
}