using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model.DataTypes;
using BigClubDebate.Data.Model.Reader;

namespace BigClubDebate.Data.Model.DataSources
{
    public class CupGames
    {
        public IEnumerable<CupGame> FaCupGames { get; }

        public IEnumerable<CupGame> LeagueCupGames { get; }

        public ILookup<string, List<string>> FaCupTables { get; }

        public ILookup<string, List<string>> LeagueCupTables { get; }

        public CupGames(FootyDataReader data)
        {
            FaCupGames = data.FaCupGames; 
            LeagueCupGames = data.LeagueCupGames;
            FaCupTables = GetSeasonsTables(FaCupGames);
            LeagueCupTables = GetSeasonsTables(LeagueCupGames);
        }

        static ILookup<string, List<string>> GetSeasonsTables(IEnumerable<CupGame> cupGames) =>
            cupGames
                .GroupBy(x => x.Season)
                .ToLookup(year => year.Key, year => new CupTable(year).ToList());
    }
}