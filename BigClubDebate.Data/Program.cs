using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BigClubDebate.Data.Model;
using BigClubDebate.Data.Model.DataSources;
using BigClubDebate.Data.Model.DataTypes;
using BigClubDebate.Data.Model.Reader;

namespace BigClubDebate.Data
{
    class Program
    {
        private static readonly Teams Teams = new Teams();

        static void Main(string[] args)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "content", "GameData");

            var openFootballEnglishLeagueReader = new FootyDataReader(new FootballDataFolderConfig(path));

            var leagueGames = openFootballEnglishLeagueReader.LeagueSeasons;
            var facup = openFootballEnglishLeagueReader.FaCupGames;

            var utd = new TeamStats(Teams.SheffUtd, facup, Standings(facup));
            var weds = new TeamStats(Teams.SheffWeds, facup, Standings(facup));

            Console.WriteLine($"{utd.Name} wins:{utd.CompetitionWins}");
            Console.WriteLine($"{utd.Name} wins:{utd.LastCompetitionWinDate}");
            Console.WriteLine($"{weds.Name} wins:{weds.CompetitionWins}");

            //ShowStats(utd, weds);

            //Console.WriteLine(string.Join(Environment.NewLine, facup));
            //DisplayD1Data(years, team1, team2);
        }

        public static ILookup<string, List<string>> Standings(IEnumerable<CupGame> cupGames) =>
            cupGames.GroupBy(x => x.Season)
                .ToLookup(
                    year => year.Key,
                    year => new CupTable(year).ToList()
                );

        private static void ShowStats(TeamStats utd, TeamStats weds)
        {
            Console.WriteLine($"{utd.Name} wins:{utd.Wins}");
            Console.WriteLine($"{weds.Name} wins:{weds.Wins}");

            Console.WriteLine($"{utd.Name} goals:{utd.Goals}");
            Console.WriteLine($"{weds.Name} goals:{weds.Goals}");

            Console.WriteLine($"{utd.Name} Conceded:{utd.Conceded}");
            Console.WriteLine($"{weds.Name} Conceded:{weds.Conceded}");

            Console.WriteLine($"{utd.Name} c/sheets:{utd.CleanSheets}");
            Console.WriteLine($"{weds.Name} c/sheets:{weds.CleanSheets}");

            Console.WriteLine($"{utd.Name} lost:{utd.Losses}");
            Console.WriteLine($"{weds.Name} lost:{weds.Losses}");

            Console.WriteLine($"{utd.Name} drawn:{utd.Draws}");
            Console.WriteLine($"{weds.Name} drawn:{weds.Draws}");

            Console.WriteLine($"{utd.Name} won:{utd.CompetitionWins}");
            Console.WriteLine($"{weds.Name} won:{weds.CompetitionWins}");

            Console.WriteLine($"{utd.Name} runner up:{utd.RunnersUp}");
            Console.WriteLine($"{weds.Name} runner up:{weds.RunnersUp}");

            Console.WriteLine($"{utd.Name} years:{utd.Years}");
            Console.WriteLine($"{weds.Name} years:{weds.Years}");
        }
    }
}
