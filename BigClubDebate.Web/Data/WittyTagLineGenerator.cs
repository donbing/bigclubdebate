using System.Linq;
using BigClubDebate.Data;
using BigClubDebate.Data.Model;

namespace BigClubDebate.Web.Data
{
    public class WittyTagLineGenerator
    {
        readonly Teams _teams;

        public WittyTagLineGenerator(Teams teams)
        {
            _teams = teams;
        }
        public string ForTeams(params TeamName[] teams)
        {
            var mancesterTeams = new[]{_teams.ManCity, _teams.ManUtd};
            if (mancesterTeams.All(teams.Contains))
                return "The Manchester Derby";

            var sheffTeams = new[] { _teams.SheffUtd, _teams.SheffWeds };
            if (sheffTeams.All(teams.Contains))
                return "The Steel City Derby";

            var NorthWestTeams = new[] { _teams.Liverpool, _teams.ManUtd };
            if (NorthWestTeams.All(teams.Contains))
                return "The North West Derby";

            return "who is the bigger club?";
        }
    }
}