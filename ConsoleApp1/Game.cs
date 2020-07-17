using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    public class CupGame : Game {

        public string year { get; set; }
        public string Type { get; set; }
    }
    public class Game : Fixture
    {
        public DateTime? date { get; set; }

        public int homegoals { get; set; }
        public int awaygoals { get; set; }

        public string Winner 
            => homegoals > awaygoals ? home : awaygoals > homegoals ? away : null;

        public string Loser 
            => homegoals < awaygoals ? home : awaygoals < homegoals ? away : null;

        public bool Drawn 
            => homegoals == awaygoals;

        public int TotalGoals => awaygoals + homegoals;

        public override string ToString() 
            => $"Game: {home} {homegoals} - {awaygoals} {away}";

        internal int PointsFor(string teamName) 
            => Winner == teamName ? 3 : Loser == teamName ? 0 : 1;

        internal int GoalsFor(params string[] name) 
            => name.Contains(home) ? homegoals : awaygoals;

        internal int GoalsAgainst(params string[] name) 
            => name.Contains(home) ? awaygoals : homegoals;
        public static CupGame CupGameFromCsv(string[] x) 
        {
            if (x[4] == "NA")
                return null;

            var reg = Regex.Match(x[4], @"(\d+)-(\d+)");
            
            return new CupGame
            {
                date = DateTime.TryParse(x[0].Replace("\"","").Trim(), out var date) 
                    ? new DateTime?(date) 
                    : null,
                
                year = x[1].Replace("\"", "").Trim(),
                home = x[2].Replace("\"", "").Trim(),
                away = x[3].Replace("\"", "").Trim(),
                homegoals = int.Parse(reg.Groups[1].Value),
                awaygoals = int.Parse(reg.Groups[2].Value),
                Type = x[7],
            };
        }
    }
}