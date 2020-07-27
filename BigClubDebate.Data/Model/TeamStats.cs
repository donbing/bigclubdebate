using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BigClubDebate.Data.Model.DataSources;
using BigClubDebate.Data.Model.DataTypes;

namespace BigClubDebate.Data.Model
{
    public class TeamStats
    {
        public TeamName Name { get; }
        readonly IEnumerable<Game> teamsGames;
        readonly IEnumerable<ITable> seasonEndTables;
        readonly IEnumerable<Game> wins;

        public TeamStats(TeamName teamName, IEnumerable<Game> allCompetitionGames, IEnumerable<ITable> seasonEndTables)
        {
            Name = teamName;
            teamsGames = allCompetitionGames.Where(Name.PlayedIn).ToList();
            this.seasonEndTables = seasonEndTables;
            wins = teamsGames.Where(g => Name.Matches(g.Winner));
        }

        public int Games
            => teamsGames.Count();

        public int Wins
            => wins.Count();

        public int CleanWins
            => wins.Count(g => g.GoalsAgainst(g.Winner) == 0);

        public int AwayWins
            => wins.Count(g => Name.Matches(g.Away));

        public int WonBy5OrMore
            => wins.Count(g => g.GoalsFor(g.Winner) - g.GoalsAgainst(g.Winner) >= 5);

        public int Losses
            => teamsGames.Count(g => Name.Matches(g.Loser));

        public int Draws
            => teamsGames.Count(g => g.Drawn);
        public int NoScoreDraws
            => teamsGames.Where(g => g.Drawn).Count(g => g.TotalGoals == 0);
        public int AwayDraws
            => teamsGames.Where(g => g.Drawn).Count(g => Name.Matches(g.Away));

        public int Goals
            => teamsGames.Sum(x => x.GoalsFor(Name.ToArray()));
        public int AwayGoals
            => teamsGames.Where(g => Name.Matches(g.Away)).Select(g => g.GoalsFor(Name.ToArray())).Sum();
        public int MostGoalsInOneGame
            => teamsGames.Select(g => g.GoalsFor(Name.ToArray())).DefaultIfEmpty().Max();
        public int GoalsInLast10Years
            => teamsGames.Where(g => g.Date.Year >= DateTime.UtcNow.AddYears(-10).Year).Sum(x => x.GoalsFor(Name.ToArray()));

        public int CleanSheets
            => teamsGames.Count(g => g.GoalsAgainst(Name.ToArray()) == 0);

        public int Conceded
            => teamsGames.Sum(x => x.GoalsAgainst(Name.ToArray()));

        public DateTime? CompetitionStart
            => teamsGames
                .OrderBy(x => x.Date)
                .Take(1)
                .Select(x => new DateTime(int.Parse(x.Season),1,1))
                .FirstOrDefault();

        public int? CompetitionWins 
            => seasonEndTables?
                .Count(t => Name.Matches(t.Winner));

        public int? RunnersUp 
            => seasonEndTables?
                .Count(t => Name.Matches(t.RunnerUp));

        public int? Years 
            => seasonEndTables?
                .Count(t => t.Any(Name.Matches));

        public int CompetitionWinsInLast10Years 
            => seasonEndTables
                .Where(t=> DateTime.UtcNow.Year - t.StartDate <= 10)
                .Count(t => Name.Matches(t.Winner));

        public IEnumerable<string> Last10CompetitionWinDates =>
            seasonEndTables
                .Where(table => Name.Matches(table.Winner))
                .OrderByDescending(x => x.StartDate)
                .Select(x => x.StartDate.ToString())
                .Take(10);

        public int CompetitionEntriesInLast10Years =>
            seasonEndTables
                .Where(t => (DateTime.UtcNow.Year - t.StartDate) <= 10)
                .Count(t => t.Any(Name.Matches));
    }
}