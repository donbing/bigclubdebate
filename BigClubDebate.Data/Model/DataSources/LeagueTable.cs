﻿using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model.DataTypes;

namespace BigClubDebate.Data.Model.DataSources
{
    public class LeagueTable : List<string>, ITable
    {
        public IEnumerable<Game> Games { get; }
        public string Winner => this[0];
        public string RunnerUp => this[1];
        public int StartDate { get; }

        public LeagueTable(IEnumerable<Game> games) : base(CalculateLeagueTable(games))
        {
            Games = games;
            StartDate = games.Min(g => g.Date.Year);
        }

        public static List<string> CalculateLeagueTable(IEnumerable<Game> games)
        {
            var orderByDescending = TableAndPoints(games);

            var enumerable = orderByDescending.Select(x => x.Item1);

            return enumerable.ToList();
        }

        public static IOrderedEnumerable<(string, int)> TableAndPoints(IEnumerable<Game> games)
        {
            var teams = games
                .Select(g => g.Home)
                .Distinct();

            var st = teams
                .ToDictionary(teamName => teamName, startCount => 0);

            foreach (var game in games)
            {
                st[game.Home] += game.PointsFor(game.Home);
                st[game.Away] += game.PointsFor(game.Away);
            }

            return st.Select(s=>(s.Key,s.Value))
                .OrderByDescending(x => x.Item2);
        }
    }

    public interface ITable : IEnumerable<string>
    {
        IEnumerable<Game> Games { get; }
        string Winner { get; }
        string RunnerUp { get; }
        int StartDate { get; }
    }
}