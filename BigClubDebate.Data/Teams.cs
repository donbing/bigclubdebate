using System.Collections.Generic;

namespace BigClubDebate.Data
{
    public class Teams : List<TeamName>
    {
        public TeamName team5 = new TeamName(new[] { "Sheffield United FC", "Blades", "Sheffield Utd", "Sheffield United" }, "sheffutd_logo.png", "sheffutd_background.jpg");
        public TeamName team6 = new TeamName(new[] { "Sheffield Wednesday FC", "Owls", "Sheffield Wed", "Sheffield Wednesday" }, "sheffwed_logo.png", "sheffwed_background.jpg");
        public TeamName team3 = new TeamName(new[] { "Leeds United FC", "Leeds", "Leeds United", "Leeds Utd" }, "leeds_logo.png", "leeds_background.jpg");
        public TeamName team4 = new TeamName(new[] { "Manchester United FC", "Red Devils", "Manchester United", "Manchester Utd", "Man Utd" }, "manu_logo.png", "manu_background.jpg");
        public TeamName team7 = new TeamName(new[] { "Manchester City FC", "Man City", "Manchester City" }, "manc_logo.png", "manc_background.jpg");

        public Teams()
        {
            AddRange(new[] { team5, team6, team3, team4, team7 });
        }
    }
}