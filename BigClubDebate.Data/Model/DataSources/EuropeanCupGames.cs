using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model.DataTypes;
using BigClubDebate.Data.Model.Reader;

namespace BigClubDebate.Data.Model.DataSources
{
    /// <summary>
    /// Simple ITable for a European cup season, derived from the final match.
    /// </summary>
    class EuroCupTable : List<string>, ITable
    {
        public string Name { get; }
        public IEnumerable<Game> Games { get; }
        public string Winner => Count > 0 ? this[0] : null;
        public string RunnerUp => Count > 1 ? this[1] : null;
        public int StartDate => int.TryParse(Name, out var y) ? y : 0;

        public EuroCupTable(string season, Game final, IEnumerable<Game> seasonGames)
            : base(new[] { final.Winner, final.Loser }.Where(w => w != null).Concat(
                seasonGames.SelectMany(g => g.Teams).Distinct().Except(new[] { final.Winner, final.Loser })).ToList())
        {
            Name = season;
            Games = seasonGames;
        }
    }

    /// <summary>
    /// Queries European cup games (Champions League, Europa League, Conference League).
    /// Mirrors the pattern of CupGames but for UEFA club competitions.
    /// </summary>
    public class EuropeanCupGames
    {
        readonly IList<Game> _allGames;
        readonly ConcurrentDictionary<long?, List<Game>> _clCache = new();
        readonly ConcurrentDictionary<long?, List<Game>> _elCache = new();
        readonly ConcurrentDictionary<long?, List<Game>> _ucolCache = new();
        readonly ConcurrentDictionary<long?, List<ITable>> _clTableCache = new();
        readonly ConcurrentDictionary<long?, List<ITable>> _elTableCache = new();
        readonly ConcurrentDictionary<long?, List<ITable>> _ucolTableCache = new();

        public EuropeanCupGames(TransfermarktCsvReader transfermarktReader, ChampsCsvReader champsReader)
        {
            var games = new List<Game>();

            // Historical CL from engsoccerdata (1955-2017)
            games.AddRange(champsReader.GetAllGames());

            // Recent CL/EL/UCOL from Transfermarkt (2012-present), deduped against historical
            var existingKeys = new HashSet<(DateTime, string, string)>();
            foreach (var g in games)
                existingKeys.Add((g.Date, g.Home, g.Away));

            foreach (var g in transfermarktReader.GetAllEuropeanGames())
            {
                var key = (g.Date, g.Home, g.Away);
                if (existingKeys.Add(key))
                    games.Add(g);
            }

            _allGames = games;
        }

        public IEnumerable<Game> GetChampionsLeagueGames(DateTime? startDate = null)
            => _clCache.GetOrAdd(startDate?.Ticks, _ =>
                _allGames.Where(g => g.Competition == CompetitionType.ChampionsLeague
                                  || g.Competition == CompetitionType.ChampionsLeagueQualifying)
                        .Where(g => !startDate.HasValue || g.Date >= startDate)
                        .ToList());

        public IEnumerable<Game> GetEuropaLeagueGames(DateTime? startDate = null)
            => _elCache.GetOrAdd(startDate?.Ticks, _ =>
                _allGames.Where(g => g.Competition == CompetitionType.EuropaLeague
                                  || g.Competition == CompetitionType.EuropaLeagueQualifying)
                        .Where(g => !startDate.HasValue || g.Date >= startDate)
                        .ToList());

        public IEnumerable<Game> GetConferenceLeagueGames(DateTime? startDate = null)
            => _ucolCache.GetOrAdd(startDate?.Ticks, _ =>
                _allGames.Where(g => g.Competition == CompetitionType.ConferenceLeague
                                  || g.Competition == CompetitionType.ConferenceLeagueQualifying)
                        .Where(g => !startDate.HasValue || g.Date >= startDate)
                        .ToList());

        public IEnumerable<ITable> GetChampionsLeagueTables(DateTime? startDate = null)
            => _clTableCache.GetOrAdd(startDate?.Ticks, _ =>
                BuildSeasonTables(GetChampionsLeagueGames(startDate), startDate));

        public IEnumerable<ITable> GetEuropaLeagueTables(DateTime? startDate = null)
            => _elTableCache.GetOrAdd(startDate?.Ticks, _ =>
                BuildSeasonTables(GetEuropaLeagueGames(startDate), startDate));

        public IEnumerable<ITable> GetConferenceLeagueTables(DateTime? startDate = null)
            => _ucolTableCache.GetOrAdd(startDate?.Ticks, _ =>
                BuildSeasonTables(GetConferenceLeagueGames(startDate), startDate));

        public IEnumerable<ITable> GetChampionsLeagueTables(IEnumerable<Game> games, DateTime? startDate = null)
            => BuildSeasonTables(games, startDate);

        public IEnumerable<ITable> GetEuropaLeagueTables(IEnumerable<Game> games, DateTime? startDate = null)
            => BuildSeasonTables(games, startDate);

        public IEnumerable<ITable> GetConferenceLeagueTables(IEnumerable<Game> games, DateTime? startDate = null)
            => BuildSeasonTables(games, startDate);

        public IEnumerable<Game> HeadToHeadGames(TeamName team1, TeamName team2,
            DateTime? dateFilter = null)
            => _allGames.Where(g => team1.PlayedIn(g) && team2.PlayedIn(g))
                        .Where(g => !dateFilter.HasValue || g.Date >= dateFilter);

        public IEnumerable<Game> AllEuropeanGames(TeamName team,
            DateTime? dateFilter = null)
            => _allGames.Where(g => team.PlayedIn(g))
                        .Where(g => !dateFilter.HasValue || g.Date >= dateFilter);

        static List<ITable> BuildSeasonTables(IEnumerable<Game> games, DateTime? startDate)
        {
            var filtered = startDate.HasValue
                ? games.Where(g => g.Date >= startDate.Value)
                : games;

            var finalRound = new[] { "Final", "final", "Finale" };

            return filtered
                .GroupBy(g => g.Season)
                .Select(seasonGroup =>
                {
                    var finals = seasonGroup
                        .Where(g => finalRound.Any(f => g.Round?.Contains(f, StringComparison.OrdinalIgnoreCase) == true))
                        .OrderByDescending(g => g.Date)
                        .ToList();

                    var final = finals.FirstOrDefault();
                    if (final == null) return null;

                    return (ITable)new EuroCupTable(seasonGroup.Key, final, seasonGroup);
                })
                .Where(t => t != null)
                .ToList();
        }
    }
}
