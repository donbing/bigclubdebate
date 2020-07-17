using System;
using System.Linq;

namespace BigClubDebate.Data
{
    public class Game : Fixture
    {
        public DateTime? date { get; set; }

        public int Homegoals { get; set; }
        public int Awaygoals { get; set; }

        public string Winner 
            => Homegoals > Awaygoals ? Home : Awaygoals > Homegoals ? Away : null;

        public string Loser 
            => Homegoals < Awaygoals ? Home : Awaygoals < Homegoals ? Away : null;

        public bool Drawn 
            => Homegoals == Awaygoals;

        public int TotalGoals => Awaygoals + Homegoals;

        internal int PointsFor(string teamName) 
            => Winner == teamName ? 3 : Loser == teamName ? 0 : 1;

        internal int GoalsFor(params string[] name) 
            => name.Contains(Home) ? Homegoals : Awaygoals;

        internal int GoalsAgainst(params string[] name) 
            => name.Contains(Home) ? Awaygoals : Homegoals;

        public override string ToString()
            => $"Game: {Home} {Homegoals} - {Awaygoals} {Away}";

    }
}