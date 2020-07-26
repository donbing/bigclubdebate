using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model.DataSources;
using BigClubDebate.Data.Model.DataTypes;

namespace BigClubDebate.Data.Model
{
    class BigClubPoints
    {
        public int OverallResult { get; }
        TeamName team;
        public BigClubPoints(IEnumerable<CupTable> faCups, IEnumerable<LeagueTable> leagues)
        {
            // winner, runner up
            // for champs/europa multipy entries by 10
            // - prem multiply by 5


            // number of international players
            // sponsorship money
            // intl games played in their stadium
            // count players with > 50 intl caps * 10

            
            // total silverware
            var multipliers = new Dictionary<string,(int,int)>
            {
                ["ChapmsLeague"] = (100,50),
                ["EurpoaLeague"] = (80,30),
                ["Prem"] = (90,40),
                ["FACup"] = (75,25),
                ["LeagueCup"] = (60,20),
            };

            // biggest stadium = 100
            // highest ave attendance = 100
            // highest spent+bought = 200
            // age of club, older the better= 100

            var premPts = leagues.Count(t => team.Matches(t[0])) * 100;
            //var auropaPts = eurpoa.Count(t => team.Matches(t[0])) * 100;
            var faCupPoints = faCups.Count(t => team.Matches(t[0])) * 100;


        }
    }
}