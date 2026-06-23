using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model.DataSources;
using BigClubDebate.Data.Model.DataTypes;

namespace BigClubDebate.Data.Model
{
    public class TeamStats
    {
        public TeamName Name { get; }

        public int Games { get; }
        public int Wins { get; }
        public int CleanWins { get; }
        public int AwayWins { get; }
        public int WonBy5OrMore { get; }
        public int Losses { get; }
        public int Draws { get; }
        public int NoScoreDraws { get; }
        public int AwayDraws { get; }
        public int Goals { get; }
        public int AwayGoals { get; }
        public int MostGoalsInOneGame { get; }
        public int GoalsInLast10Years { get; }
        public int CleanSheets { get; }
        public int Conceded { get; }
        public DateTime? CompetitionStart { get; }
        public int? CompetitionWins { get; }
        public int? RunnersUp { get; }
        public int? Years { get; }
        public int CompetitionWinsInLast10Years { get; }
        public List<string> Last10CompetitionWinDates { get; }
        public int CompetitionEntriesInLast10Years { get; }

        public TeamStats(TeamName teamName, IEnumerable<Game> allCompetitionGames, IEnumerable<ITable> seasonEndTables)
        {
            Name = teamName;

            var teamsGames = allCompetitionGames.Where(Name.PlayedIn).ToList();
            var tablesList = seasonEndTables?.ToList();

            Games = teamsGames.Count;
            var tenYearsAgo = DateTime.UtcNow.AddYears(-10).Year;

            int wins = 0, cleanWins = 0, awayWins = 0, wonBy5OrMore = 0;
            int losses = 0, draws = 0, noScoreDraws = 0, awayDraws = 0;
            int goals = 0, awayGoals = 0, mostGoals = 0, goalsInLast10 = 0;
            int cleanSheets = 0, conceded = 0;
            DateTime? earliest = null;

            foreach (var g in teamsGames)
            {
                var isHome = Name.Matches(g.Home);
                var goalsFor = isHome ? g.HomeGoals : g.AwayGoals;
                var goalsAgainst = isHome ? g.AwayGoals : g.HomeGoals;

                goals += goalsFor;
                conceded += goalsAgainst;
                if (!isHome) awayGoals += goalsFor;
                if (goalsFor > mostGoals) mostGoals = goalsFor;
                if (g.Date.Year >= tenYearsAgo) goalsInLast10 += goalsFor;
                if (goalsAgainst == 0) cleanSheets++;
                if (!earliest.HasValue || g.Date < earliest) earliest = g.Date;

                if (g.Drawn)
                {
                    draws++;
                    if (g.TotalGoals == 0) noScoreDraws++;
                    if (!isHome) awayDraws++;
                }
                else if (Name.Matches(g.Winner))
                {
                    wins++;
                    if (goalsAgainst == 0) cleanWins++;
                    if (!isHome) awayWins++;
                    if (goalsFor - goalsAgainst >= 5) wonBy5OrMore++;
                }
                else
                {
                    losses++;
                }
            }

            Wins = wins;
            CleanWins = cleanWins;
            AwayWins = awayWins;
            WonBy5OrMore = wonBy5OrMore;
            Losses = losses;
            Draws = draws;
            NoScoreDraws = noScoreDraws;
            AwayDraws = awayDraws;
            Goals = goals;
            AwayGoals = awayGoals;
            MostGoalsInOneGame = mostGoals;
            GoalsInLast10Years = goalsInLast10;
            CleanSheets = cleanSheets;
            Conceded = conceded;
            CompetitionStart = earliest;

            if (tablesList != null)
            {
                CompetitionWins = tablesList.Count(t => Name.Matches(t.Winner));
                RunnersUp = tablesList.Count(t => Name.Matches(t.RunnerUp));
                Years = tablesList.Count(t => t.Any(Name.Matches));
                CompetitionWinsInLast10Years = tablesList
                    .Count(t => DateTime.UtcNow.Year - t.StartDate <= 10 && Name.Matches(t.Winner));
                Last10CompetitionWinDates = tablesList
                    .Where(t => Name.Matches(t.Winner))
                    .OrderByDescending(x => x.StartDate)
                    .Select(x => x.StartDate.ToString())
                    .Take(10)
                    .ToList();
                CompetitionEntriesInLast10Years = tablesList
                    .Count(t => (DateTime.UtcNow.Year - t.StartDate) <= 10 && t.Any(Name.Matches));
            }
        }
    }
}