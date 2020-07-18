using System;
using System.IO;
using System.Linq;

namespace BigClubDebate.Data
{
    class Program
    {
        private static readonly Teams Teams = new Teams();

        static void Main(string[] args)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "GameData");

            var openFootballEnglishLeagueReader = new OpenFootballEnglishLeagueReader(path);

            var leagueGames = openFootballEnglishLeagueReader.LeagueYears;
            var facup = openFootballEnglishLeagueReader.CupGames;

            var utd = new TeamStats(Teams.team5, facup, null);
            var weds = new TeamStats(Teams.team6, facup, null);

            ShowStats(utd, weds);

            //Console.WriteLine(string.Join(Environment.NewLine, facup));
            //DisplayD1Data(years, team1, team2);
        }

        private static void DisplayD1Data(IOrderedEnumerable<Year> years, string[] team1, string[] team2)
        {
            var topLeague = years.Select(x => x.GetLeague(1));

            var tables = topLeague.Select(d => d.Table).Skip(1);
            var allGames = topLeague.SelectMany(y => y.games);

            var utd = new TeamStats(team1, allGames, tables);
            var weds = new TeamStats(team2, allGames, tables);
            ShowStats(utd, weds);
        }

        private static void ShowStats(TeamStats utd, TeamStats weds)
        {
            Console.WriteLine($"{utd.name} wins:{utd.Wins}");
            Console.WriteLine($"{weds.name} wins:{weds.Wins}");

            Console.WriteLine($"{utd.name} goals:{utd.Goals}");
            Console.WriteLine($"{weds.name} goals:{weds.Goals}");

            Console.WriteLine($"{utd.name} Conceded:{utd.Conceded}");
            Console.WriteLine($"{weds.name} Conceded:{weds.Conceded}");

            Console.WriteLine($"{utd.name} c/sheets:{utd.CleanSheets}");
            Console.WriteLine($"{weds.name} c/sheets:{weds.CleanSheets}");

            Console.WriteLine($"{utd.name} lost:{utd.Losses}");
            Console.WriteLine($"{weds.name} lost:{weds.Losses}");

            Console.WriteLine($"{utd.name} drawn:{utd.Draws}");
            Console.WriteLine($"{weds.name} drawn:{weds.Draws}");

            Console.WriteLine($"{utd.name} won:{utd.Chapions}");
            Console.WriteLine($"{weds.name} won:{weds.Chapions}");

            Console.WriteLine($"{utd.name} runner up:{utd.RunnersUp}");
            Console.WriteLine($"{weds.name} runner up:{weds.RunnersUp}");

            Console.WriteLine($"{utd.name} years:{utd.Years}");
            Console.WriteLine($"{weds.name} years:{weds.Years}");
        }
    }
}
