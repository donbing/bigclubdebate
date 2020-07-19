using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model.DataTypes;

namespace BigClubDebate.Data.Model
{
    public class TeamStats
    {
        public TeamName Name { get; set; }
        readonly IEnumerable<Game> teamsGames;
        readonly ILookup<string, List<string>> tables;

        public TeamStats(TeamName teamName, IEnumerable<Game> allCompetitionGames, ILookup<string, List<string>> tables)
        {
            Name = teamName;
            teamsGames = allCompetitionGames.Where(Name.PlayedIn).ToList();
            this.tables = tables;
        }

        public int Games
            => teamsGames.Count();

        public int Wins
            => teamsGames.Count(g => Name.Matches(g.Winner));

        public int CleanWins
            => teamsGames.Count(g => Name.Matches(g.Winner) && g.GoalsAgainst(g.Winner) == 0);

        public int AwayWins
            => teamsGames.Count(g => Name.Matches(g.Winner) && Name.Matches(g.Away));

        public int WonBy5OrMore
            => teamsGames.Count(g => Name.Matches(g.Winner) && g.GoalsFor(g.Winner) - g.GoalsAgainst(g.Winner) >= 5);

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
            => tables?
                .SelectMany(t => t)
                .Count(t => Name.Matches(t[0]));

        public int? RunnersUp 
            => tables?
                .SelectMany(t => t)
                .Count(t => Name.Matches(t[1]));

        public int? Years 
            => tables?
                .SelectMany(t => t)
                .Count(t => t.Any(Name.Matches));

        public int CompetitionWinsInLast10Years 
            => tables
                .Where(t=> (DateTime.UtcNow.Year - int.Parse(t.Key)) <= 10)
                .SelectMany(t => t)
                .Count(t => Name.Matches(t[0]));

        public string LastCompetitionWinDate =>
            tables
                .Where(seasons => seasons.Any(table => Name.Matches(table[0])))
                .Max(x => x.Key);

        public int CompetitionEntriesInLast10Years =>
            tables
                .Where(t => (DateTime.UtcNow.Year - int.Parse(t.Key)) <= 10)
                .SelectMany(t => t)
                .Count(t => t.Any(Name.Matches));
    }
}