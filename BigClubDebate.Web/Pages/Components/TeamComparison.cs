using BigClubDebate.Data.Model;
using BigClubDebate.Data.Model.DataTypes;

namespace BigClubDebate.Web.Pages.Components
{
    public class TeamComparison
    {

        public (TeamName firstTeam, TeamName secondTeam) Names { get; set; }
        public (TeamStats, TeamStats) Teams { get; }
        public TeamStats Item1 => Teams.Item1;
        public TeamStats Item2 => Teams.Item2;

        public TeamComparison(TeamName firstTeam, TeamName secondTeam, TeamStats teamStats, TeamStats teamStats1)
        {
            Teams = (teamStats, teamStats1);
            Names = (firstTeam, secondTeam);
        }
    }
}
