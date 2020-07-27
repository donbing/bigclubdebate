using System;
using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model.DataSources;
using BigClubDebate.Data.Model.DataTypes;

namespace BigClubDebate.Data.Model
{
    public class BigClubStat
    {
        readonly string _name;
        readonly Func<int> _calc;
        public int Result => _calc.Invoke();

        public BigClubStat(string name, Func<int> calc)
        {
            _name = name;
            _calc = calc;
        }

    }


    public class BigClubPoints
    {
        /*
        public static (BigClubPoints, BigClubPoints) Get(TeamName team1, TeamName team2, CupGames cup, LeagueGames league)
        {
            return (
                new BigClubPoints(team1, cup.GetFaCupTables(), cup.GetLeagueCupTables(), league.DivisionTables(1)),
                new BigClubPoints(team2, cup.GetFaCupTables(), cup.GetLeagueCupTables(), league.DivisionTables(1)),



            )

        }
        */

        public int OverallResult { get; }

        public BigClubPoints(TeamName team, IEnumerable<CupTable> faCups, IEnumerable<CupTable> leagueCups, IEnumerable<LeagueTable> leagues, IEnumerable<CupTable> championsLeagueTables = null, IEnumerable<CupTable> europaLeagueTables = null)
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
            // age of club, older the better = 100

            var stats = new[]
            {
               // new BigClubStat("Champions League", () => championsLeagueTables.Sum(t => (team.Matches(t.Winner) ? 100 : 0) + (team.Matches(t.RunnerUp) ? 50 : 0))),
               // new BigClubStat("Europa League", () => europaLeagueTables.Sum(t => (team.Matches(t.Winner) ? 80 : 0) + (team.Matches(t.RunnerUp) ? 30 : 0))),
                new BigClubStat("Premier League", () => leagues.Sum(t => (team.Matches(t[0]) ? 90 : 0) + (team.Matches(t.RunnerUp) ? 40 : 0))),
                new BigClubStat("League Cup", () => leagueCups.Sum(t => (team.Matches(t.Winner) ? 75 : 0) + (team.Matches(t.RunnerUp) ? 25 : 0))),
                new BigClubStat("FA Cup", () => faCups.Sum(t => (team.Matches(t.Winner) ? 60 : 0) + (team.Matches(t.RunnerUp) ? 20 : 0))),
            };

            OverallResult = stats.Sum(s => s.Result);
        }
    }
}