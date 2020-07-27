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
        readonly IEnumerable<CupTable> _leagueCupTables;

        readonly IEnumerable<CupGame> _faCupGames;
        readonly IEnumerable<CupTable> _faCupTables;

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

        public IEnumerable<CupTable> GetFaCupTables(DateTime? startDate = null) 
            => _faCupTables.Where(x => !startDate.HasValue || x.StartDate >= startDate.Value.Year);

        public IEnumerable<CupTable> GetLeagueCupTables(DateTime? startDate = null) 
            => _leagueCupTables.Where(x => !startDate.HasValue || x.StartDate >= startDate.Value.Year);

        public CupGames(FootyDataReader data)
        {
            _faCupGames = data.FaCupGames; 
            leagueCupGames = data.LeagueCupGames;
            _faCupTables = GetSeasonsTables(GetFaCupGames());
            _leagueCupTables = GetSeasonsTables(leagueCupGames);
        }

        static IEnumerable<CupTable> GetSeasonsTables(IEnumerable<CupGame> cupGames)
            => cupGames
                .GroupBy(x => x.Season)
                .Select(year => new CupTable(year));
    }
}