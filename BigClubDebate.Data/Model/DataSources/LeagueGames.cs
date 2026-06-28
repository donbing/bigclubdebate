using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model.DataTypes;
using BigClubDebate.Data.Model.Reader;

namespace BigClubDebate.Data.Model.DataSources
{
    public class LeagueGames
    {
        readonly IGameDataProvider _data;
        readonly ConcurrentDictionary<(int division, long? ticks), List<Game>> _divisionCache = new();
        readonly ConcurrentDictionary<(string t1, string t2, long? ticks), List<Game>> _h2hCache = new();
        readonly ConcurrentDictionary<(int division, long? ticks), List<ITable>> _tableCache = new();

        public LeagueGames(IGameDataProvider data) 
            => _data = data;

        public IEnumerable<Game> HeadToHeadGames(TeamName team1, TeamName team2, DateTime? dateFilter = null)
        {
            var key = (team1.MainName, team2.MainName, dateFilter?.Ticks);
            return _h2hCache.GetOrAdd(key, _ =>
                _data.GetLeagueSeasons()
                    .SelectMany(x => x.Divisions)
                    .Where(d => !dateFilter.HasValue || d.StartDate >= dateFilter)
                    .SelectMany(y => y.Games)
                    .Where(game => team1.PlayedIn(game) && team2.PlayedIn(game))
                    .ToList());
        }

        public List<Game> DivisionGames(int division, DateTime? startDate = null)
        {
            var key = (division, startDate?.Ticks);
            return _divisionCache.GetOrAdd(key, _ =>
                _data.GetLeagueSeasons()
                    .Select(x => x.GetDivision(division))
                    .Where(d => !startDate.HasValue || d.StartDate >= startDate)
                    .SelectMany(x => x.Games)
                    .ToList());
        }

        public IEnumerable<ITable> DivisionTables(int division, DateTime? startDate = null)
        {
            var key = (division, startDate?.Ticks);
            return _tableCache.GetOrAdd(key, _ =>
                BuildTables(DivisionGames(division, startDate)));
        }

        public IEnumerable<ITable> DivisionTables(IEnumerable<Game> games)
            => BuildTables(games);

        static List<ITable> BuildTables(IEnumerable<Game> games)
            => games.GroupBy(x => x.Season)
                    .Select(year => new LeagueTable(year))
                    .Cast<ITable>()
                    .ToList();
    }
}