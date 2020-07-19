using System.Linq;
using BigClubDebate.Data;
using BigClubDebate.Data.Model;
using BigClubDebate.Data.Model.DataTypes;

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

            var liverpoolTeams = new[] { _teams.Liverpool, _teams.Everton };
            if (liverpoolTeams.All(teams.Contains))
                return "The Merseyside Derby";

            var rosesTeams = new[] { _teams.Leeds, _teams.ManUtd };
            if (rosesTeams.All(teams.Contains))
                return "War of the Roses";

            var ctTeams = new[] { _teams.Chelsea, _teams.Liverpool };
            if (ctTeams.All(teams.Contains))
                return "Chelsea–Liverpool rivalry";

            var clTeams = new[] { _teams.Chelsea, _teams.Leeds};
            if (clTeams.All(teams.Contains))
                return "Chelsea–Leeds rivalry";

            var amTeams = new[] { _teams.Arsenal, _teams.ManUtd};
            if (amTeams.All(teams.Contains))
                return "Arsenal–Manchester United rivalry";

            return "who is the bigger club?";
        }
    }
}