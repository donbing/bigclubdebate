using System;
using System.Linq;

namespace BigClubDebate.Data.Model.DataTypes
{
    public enum CompetitionType
    {
        DomesticLeague,
        FACup,
        LeagueCup,
        ChampionsLeague,
        ChampionsLeagueQualifying,
        EuropaLeague,
        EuropaLeagueQualifying,
        ConferenceLeague,
        ConferenceLeagueQualifying
    }

    public class Game : Fixture
    {
        public DateTime Date { get; set; }
        public string Season { get; set; }

        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }

        /// <summary>
        /// For two-legged ties or penalty shootouts, the team that advanced/won the tie.
        /// When set, overrides the goal-based Winner/Loser for ties where the match
        /// was drawn but decided by away goals, extra time, or penalties.
        /// </summary>
        public string TieWinner { get; set; }

        /// <summary>
        /// Data source tag used for dedup priority. Higher value = preferred source.
        /// </summary>
        public int SourcePriority { get; set; }

        public string Winner 
            => TieWinner ?? (HomeGoals > AwayGoals ? Home : AwayGoals > HomeGoals ? Away : null);

        public string Loser 
            => TieWinner != null
                ? (TieWinner == Home ? Away : TieWinner == Away ? Home : null)
                : (HomeGoals < AwayGoals ? Home : AwayGoals < HomeGoals ? Away : null);

        public bool Drawn 
            => TieWinner == null && HomeGoals == AwayGoals;

        public int TotalGoals 
            => AwayGoals + HomeGoals;

        public string Division { get; set; }

        public CompetitionType? Competition { get; set; }

        public string Round { get; set; }

        public int PointsFor(TeamName teamName)
        {
            if (!Teams.Any(teamName.Matches))
            {
                throw new ArgumentException("£");
            }

            return teamName.Matches(Winner) ? (Date.Year < 1981) ? 2 : 3 : teamName.Matches(Loser) ? 0 : 1;
        }

        public int GoalsFor(params string[] name) 
            => name.Contains(Home) ? HomeGoals : name.Contains(Away) ? AwayGoals : throw new ArgumentException("baad");

        public int GoalsAgainst(params string[] name)
            => name.Contains(Home) ? AwayGoals : name.Contains(Away) ? HomeGoals : throw new ArgumentException("baad");

        public override string ToString()
            => $"Game: {Home} {HomeGoals}-{AwayGoals} {Away}";
    }
}