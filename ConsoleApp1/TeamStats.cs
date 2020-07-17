using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    public class TeamName : HashSet<string>
    {
        public string MainName => this.First();
        public string SlangName => this.ElementAt(1);

        public string ImageName { get; }

        public TeamName(IEnumerable<string> collection, string imageName) : base(collection)
        {
            ImageName = imageName;
        }

        public static implicit operator TeamName(string[] names) => new TeamName(names, "");

        public bool Matches(string otherName) => this.Contains(otherName, StringComparer.OrdinalIgnoreCase);
        public override bool Equals(object obj) => obj is TeamName name && MainName == name.MainName;
        public override int GetHashCode() => HashCode.Combine(MainName);
    }

    public class TeamStats
    {
        public TeamName name;
        private IEnumerable<Game> games;
        private readonly IEnumerable<List<string>> tables;

        public TeamStats(TeamName teamName, IEnumerable<Game> games, IEnumerable<List<string>> tables)
        {
            this.name = teamName;
            this.games = games.Where(g => name.Matches(g.away) || name.Matches(g.home)).ToList();
            this.tables = tables?.ToList() ?? new List<List<string>>();
        }
        public int Games
            => games.Count();

        public int Wins
            => games.Where(g => name.Matches(g.Winner)).Count();

        public int CleanWins
            => games.Where(g => name.Matches(g.Winner) && g.GoalsAgainst(g.Winner) == 0).Count();

        public int AwayWins
            => games.Where(g => name.Matches(g.Winner) && name.Matches(g.away)).Count();

        public int WonBy5OrMore
            => games.Where(g => name.Matches(g.Winner) && g.GoalsFor(g.Winner) - g.GoalsAgainst(g.Winner) >= 5).Count();

        public int Losses
            => games.Where(g => name.Matches(g.Loser)).Count();

        public int Draws
            => games.Where(g => g.Drawn).Count();
        public int NoScoreDraws
            => games.Where(g => g.Drawn).Where(g => g.TotalGoals == 0).Count();
        public int AwayDraws
            => games.Where(g => g.Drawn).Where(g => name.Matches(g.away)).Count();

        public int Goals
            => games.Sum(x => x.GoalsFor(name.ToArray()));
        public int AwayGoals
            => games.Where(g => name.Matches(g.away)).Select(g => g.GoalsFor(name.ToArray())).Sum();
        public int MostGoalsInOneGame
            => games.Select(g => g.GoalsFor(name.ToArray())).DefaultIfEmpty().Max();
        public int GoalsInLast10Years
            => games.Where(g => g.date.Value.Year >= DateTime.UtcNow.AddYears(-10).Year).Sum(x => x.GoalsFor(name.ToArray()));

        public int CleanSheets
            => games.Count(g => g.GoalsAgainst(name.ToArray()) == 0);

        public int Conceded
            => games.Sum(x => x.GoalsAgainst(name.ToArray()));

        public int Chapions => tables.Count(t => name.Matches(t[0]));
        public DateTime? CompetitionStart => games
            .Select(x => x.date)
            .OrderBy(x => x)
            .Take(1)
            .Min();

        public int RunnersUp => tables.Count(t => name.Matches(t[1]));

        public int Years => tables.Count(t => t.Any(c => name.Matches(c)));
    }
}