using System.Collections.Generic;
using BigClubDebate.Data.Model.DataTypes;

namespace BigClubDebate.Data.Model.Reader
{
    public interface IGameDataProvider
    {
        IList<Season> GetLeagueSeasons();
        IList<CupGame> GetFaCupGames();
        IList<CupGame> GetLeagueCupGames();
        IList<Game> GetEuropeanCupGames();
    }
}
