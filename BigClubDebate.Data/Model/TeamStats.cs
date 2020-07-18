using System;
using System.Collections.Generic;
using System.Linq;

namespace BigClubDebate.Data.Model
{
    public class TeamStats
    {
        public TeamName name;
        private IEnumerable<Game> games;
        private readonly ILookup<string, List<string>> tables;

        public TeamStats(TeamName teamName, IEnumerable<Game> games, ILookup<string, List<string>> tables)
        {
            this.name = teamName;
            this.games = games.Where(g => name.Matches(g.Away) || name.Matches(g.Home)).ToList();
            this.tables = tables;
        }
        public int Games
            => games.Count();

        public int Wins
            => games.Where(g => name.Matches(g.Winner)).Count();

        public int CleanWins
            => games.Where(g => name.Matches(g.Winner) && g.GoalsAgainst(g.Winner) == 0).Count();

        public int AwayWins
            => games.Where(g => name.Matches(g.Winner) && name.Matches(g.Away)).Count();

        public int WonBy5OrMore
            => games.Where(g => name.Matches(g.Winner) && g.GoalsFor(g.Winner) - g.GoalsAgainst(g.Winner) >= 5).Count();

        public int Losses
            => games.Where(g => name.Matches(g.Loser)).Count();

        public int Draws
            => games.Where(g => g.Drawn).Count();
        public int NoScoreDraws
            => games.Where(g => g.Drawn).Where(g => g.TotalGoals == 0).Count();
        public int AwayDraws
            => games.Where(g => g.Drawn).Where(g => name.Matches(g.Away)).Count();

        public int Goals
            => games.Sum(x => x.GoalsFor(name.ToArray()));
        public int AwayGoals
            => games.Where(g => name.Matches(g.Away)).Select(g => g.GoalsFor(name.ToArray())).Sum();
        public int MostGoalsInOneGame
            => games.Select(g => g.GoalsFor(name.ToArray())).DefaultIfEmpty().Max();
        public int GoalsInLast10Years
            => games.Where(g => g.Date.Value.Year >= DateTime.UtcNow.AddYears(-10).Year).Sum(x => x.GoalsFor(name.ToArray()));

        public int CleanSheets
            => games.Count(g => g.GoalsAgainst(name.ToArray()) == 0);

        public int Conceded
            => games.Sum(x => x.GoalsAgainst(name.ToArray()));

        public int CompetitionWins 
            => tables.SelectMany(t => t).Count(t => name.Matches(t[0]));
        public DateTime? CompetitionStart => games
            .Select(x => x.Date)
            .OrderBy(x => x)
            .Take(1)
            .Min();

        public int RunnersUp => tables.SelectMany(t => t).Count(t => name.Matches(t[1]));

        public int Years => tables.SelectMany(t => t).Count(t => t.Any(c => name.Matches(c)));
        public int CompetitionWinsInLast10Years 
            => tables
                .Where(t=> (DateTime.UtcNow.Year - int.Parse(t.Key)) <= 10)
                .SelectMany(t => t)
                .Count(t => name.Matches(t[0]));

        public string LastCompetitionWinDate =>
            tables
                .Where(t => t.Any(table => name.Matches(table[0])))
                .Max(x => x.Key);

        public int CompetitionEntriesInLast10Years =>
            tables
                .Where(t => (DateTime.UtcNow.Year - int.Parse(t.Key)) <= 10)
                .SelectMany(t => t)
                .Count(t => t.Any(name.Matches));
    }
}