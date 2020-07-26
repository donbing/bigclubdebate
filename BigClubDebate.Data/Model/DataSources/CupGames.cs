using System;
using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model.DataTypes;
using BigClubDebate.Data.Model.Reader;

namespace BigClubDebate.Data.Model.DataSources
{
    public class CupGames
    {
        readonly IEnumerable<CupGame> leagueCupGames;
        readonly ILookup<string, List<string>> _leagueCupTables;
        readonly IEnumerable<CupGame> _faCupGames;
        readonly ILookup<string, List<string>> _faCupTables;

        public IEnumerable<CupGame> GetFaCupGames(DateTime? startDate = null)
        {
            return _faCupGames
                .Where(x => !startDate.HasValue || x.Date >= startDate);
        }

        public IEnumerable<CupGame> GetLeagueCupGames(DateTime? startDate)
        {
            return leagueCupGames
                .Where(x => !startDate.HasValue || x.Date >= startDate);
        }

        public ILookup<string, List<string>> GetFaCupTables(DateTime? startDate = null)
        {
            var faCupTables = _faCupTables
                .Where(x => !startDate.HasValue || int.Parse(x.Key) >= startDate.Value.Year)
                .ToLookup(x => x.Key, x => x.SelectMany(y => y).ToList());

            return faCupTables;
        }

        public ILookup<string, List<string>> GetLeagueCupTables(DateTime? startDate = null)
        {
            return _leagueCupTables
                .Where(x => !startDate.HasValue || int.Parse(x.Key) >= startDate.Value.Year)
                .ToLookup(x => x.Key, x=> x.SelectMany(y => y).ToList());
        }

        public CupGames(FootyDataReader data)
        {
            _faCupGames = data.FaCupGames; 
            leagueCupGames = data.LeagueCupGames;
            _faCupTables = GetSeasonsTables(GetFaCupGames());
            _leagueCupTables = GetSeasonsTables(leagueCupGames);
        }

        static ILookup<string, List<string>> GetSeasonsTables(IEnumerable<CupGame> cupGames) =>
            cupGames
                .GroupBy(x => x.Season)
                .ToLookup(year => year.Key, year => new CupTable(year).ToList());
    }
}