using System;
using System.Linq;

namespace BigClubDebate.Data.Model
{
    public class Game : Fixture
    {
        public DateTime Date { get; set; }
        public string Season { get; set; }

        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }

        public string Winner 
            => HomeGoals > AwayGoals ? Home : AwayGoals > HomeGoals ? Away : null;

        public string Loser 
            => HomeGoals < AwayGoals ? Home : AwayGoals < HomeGoals ? Away : null;

        public bool Drawn 
            => HomeGoals == AwayGoals;

        public int TotalGoals 
            => AwayGoals + HomeGoals;

        public int PointsFor(string teamName) 
            => Winner == teamName ? 3 : Loser == teamName ? 0 : 1;

        public int GoalsFor(params string[] name) 
            => name.Contains(Home) ? HomeGoals : AwayGoals;

        public int GoalsAgainst(params string[] name) 
            => name.Contains(Home) ? AwayGoals : HomeGoals;

        public override string ToString()
            => $"Game: {Home} {HomeGoals}-{AwayGoals} {Away}";
    }
}