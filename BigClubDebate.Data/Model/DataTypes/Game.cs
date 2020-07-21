using System;
using System.Linq;

namespace BigClubDebate.Data.Model.DataTypes
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

        public string Division { get; set; }

        public int PointsFor(TeamName teamName)
        {
            if (!Teams.Any(teamName.Matches))
            {
                throw new ArgumentException("£");
            }

            return teamName.Matches(Winner) ? (Date.Year < 1981) ? 2 : 3 : teamName.Matches(Loser) ? 0 : 1;
        }

        public int GoalsFor(params string[] name) 
            => name.Contains(Home) ? HomeGoals : name.Contains(Away) ? AwayGoals : throw new ArgumentException("baad");

        public int GoalsAgainst(params string[] name)
            => name.Contains(Home) ? AwayGoals : name.Contains(Away) ? HomeGoals : throw new ArgumentException("baad");

        public override string ToString()
            => $"Game: {Home} {HomeGoals}-{AwayGoals} {Away}";
    }
}