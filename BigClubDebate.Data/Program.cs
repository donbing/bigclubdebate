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


            Console.WriteLine($"{utd.name} wins:{utd.CompetitionWins}");
            Console.WriteLine($"{utd.name} wins:{utd.LastCompetitionWinDate}");
            Console.WriteLine($"{weds.name} wins:{weds.CompetitionWins}");

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

            Console.WriteLine($"{utd.name} won:{utd.CompetitionWins}");
            Console.WriteLine($"{weds.name} won:{weds.CompetitionWins}");

            Console.WriteLine($"{utd.name} runner up:{utd.RunnersUp}");
            Console.WriteLine($"{weds.name} runner up:{weds.RunnersUp}");

            Console.WriteLine($"{utd.name} years:{utd.Years}");
            Console.WriteLine($"{weds.name} years:{weds.Years}");
        }
    }
}
