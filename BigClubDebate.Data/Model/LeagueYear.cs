using System;
using System.Collections.Generic;
using System.Linq;

namespace BigClubDebate.Data.Model
{
    public class LeagueYear
    {
        public IEnumerable<League> Leagues { get; set; }

        public IEnumerable<Game> Games 
            => Leagues.SelectMany(x => x.Games);

        public string name;

        public LeagueYear(string yearName, IEnumerable<League> leagues) 
        { 
            name = yearName;
            Leagues = leagues;
        }

        public League GetLeague(int v) 
            => Leagues.First(x => x.Priority == v);

        public override string ToString() 
            => $"{name+Environment.NewLine}{string.Join(Environment.NewLine, Leagues)}";
    }
}