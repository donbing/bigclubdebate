using System;
using System.Collections.Generic;
using System.Linq;
using BigClubDebate.Data.Model.DataTypes;
using BigClubDebate.Data.Model.Reader;

namespace BigClubDebate.Data.Model.DataSources
{
    public class LeagueGames
    {
        readonly FootyDataReader data;

        public LeagueGames(FootyDataReader data) 
            => this.data = data;

        public IEnumerable<Game> HeadToHeadGames(TeamName team1, TeamName team2, DateTime? dateFilter = null) 
            => data.LeagueSeasons
                .SelectMany(x => x.Divisions)
                .Where(d => !dateFilter.HasValue || d.StartDate >= dateFilter)
                .SelectMany(y => y.Games)
                .Where(game => team1.PlayedIn(game) && team2.PlayedIn(game))
                .ToList();

        public List<Game> DivisionGames(int division, DateTime? startDate = null) 
            => data.LeagueSeasons
                .Select(x => x.GetDivision(division))
                .Where(d => !startDate.HasValue || d.StartDate >= startDate)
                .SelectMany(x => x.Games)
                .ToList();

        public ILookup<string, List<string>> DivisionTables(int division, DateTime? startDate = null) 
            => DivisionGames(division, startDate)
                .GroupBy(x => x.Season)
                .ToLookup(year => year.Key, year => new LeagueTable(year).ToList());
    }
}