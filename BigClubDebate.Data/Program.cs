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
            var config = FootballDataFolderConfig.FromEntryAssemblyPath();
            var openFootballEnglishLeagueReader = new FootyDataReader(config);

            var leagueSeasons = openFootballEnglishLeagueReader.LeagueSeasons;
            var facup = openFootballEnglishLeagueReader.FaCupGames;
            
            var utd = new TeamStats(Teams.SheffUtd, leagueSeasons.SelectMany(x => x), Standings(facup));
            var weds = new TeamStats(Teams.SheffWeds, leagueSeasons.SelectMany(x => x), Standings(facup));

            Console.WriteLine($"{utd.Name} wins: {utd.CompetitionWins}");
            Console.WriteLine($"{utd.Name} win dates: {string.Join(", ", utd.Last10CompetitionWinDates)}");
            Console.WriteLine($"{weds.Name} wins: {weds.CompetitionWins}");
        }
        
        static IEnumerable<CupTable> Standings(IEnumerable<CupGame> cupGames) =>
            cupGames
                .GroupBy(x => x.Season)
                .Select(year => new CupTable(year));
        
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
