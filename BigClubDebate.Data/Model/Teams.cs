using System.Collections.Generic;
using BigClubDebate.Data.Model;
using BigClubDebate.Data.Model.DataTypes;

namespace BigClubDebate.Data
{
    public class Teams : List<TeamName>
    {
        public TeamName SheffUtd = new TeamName(new[] { "Sheffield United FC", "Blades", "Sheffield Utd", "Sheffield United" }, "sheffutd_logo.png", "sheffutd_background.jpg");
        public TeamName SheffWeds = new TeamName(new[] { "Sheffield Wednesday FC", "Owls", "Sheffield Wed", "Sheffield Wednesday" }, "sheffwed_logo.png", "sheffwed_background.jpg");
        public TeamName Leeds = new TeamName(new[] { "Leeds United FC", "Leeds", "Leeds United", "Leeds Utd" }, "leeds_logo.png", "leeds_background.jpg");
        public TeamName ManUtd = new TeamName(new[] { "Manchester United FC", "Red Devils", "Manchester United", "Manchester Utd", "Man Utd" }, "manu_logo.png", "manu_background.jpg");
        public TeamName ManCity = new TeamName(new[] { "Manchester City FC", "Man City", "Manchester City" }, "manc_logo.png", "manc_background.jpg");
        public TeamName LeicesterCity = new TeamName(new[] { "Leicester City FC", "The Foxes", "Manchester City" }, "Leicester_logo.png", "Leicester_background.jpg");
        public TeamName Liverpool = new TeamName(new[] { "Liverpool FC", "The Reds", "Liverpool" }, "Liverpool_logo.png");
        public TeamName Everton = new TeamName(new[] { "Everton FC", "The Toffees", "Everton" }, "Everton_logo.png");
        public TeamName Chelsea = new TeamName(new[] { "Chelsea FC", "Chelsea", "Chelsea" }, "Chelsea_logo.png");
        public TeamName Arsenal = new TeamName(new[] { "Arsenal FC", "The Gunners", "Arsenal" }, "Arsenal_logo.png");
        
        public TeamName Tottenham = new TeamName(new[] { "Tottenham Hotspur FC","Lilly Whites","Tottenham"}, "Tottenham_logo.png");
        public TeamName AstonVilla = new TeamName(new[] { "Aston Villa FC", "Villa", "Aston Villa" }, "Villa_logo.png");
        public TeamName BirminghamCity = new TeamName(new[] { "Birmingham City FC", "Birmingham", "Birmingham City" }, "Birmingham_logo.png");
        public TeamName WestHam = new TeamName(new[] { "West Ham United FC", "The Hammers", "West Ham United", "West Ham" }, "WestHam_logo.png");
        public TeamName BlackburnRovers = new TeamName(new[] { "Blackburn Rovers FC", "Rovers", "Blackburn Rovers", "Blackburn" }, "Blackburn_logo.png");

        public Teams()
        {
            AddRange(new[] { 
                SheffUtd, SheffWeds, Leeds, ManUtd, ManCity, LeicesterCity, Liverpool,
                Everton, Chelsea, Arsenal, Tottenham, AstonVilla, BirminghamCity, WestHam, BlackburnRovers,
            });
        }
    }
}