using System;
using System.Collections.Generic;
using System.Linq;

namespace BigClubDebate.Data.Model
{
    public class TeamName : HashSet<string>
    {
        public string ImageName { get; }
        public string BackGroundName { get; }

        public string MainName 
            => this.First();

        public string NickName 
            => this.ElementAt(1);

        public TeamName(IEnumerable<string> names, string imageName, string backGroundName) : base(names, StringComparer.OrdinalIgnoreCase)
        {
            ImageName = imageName;
            BackGroundName = backGroundName;
        }

        public bool Matches(string otherName) 
            => this.Contains(otherName);
        
        public override bool Equals(object obj)
            => obj is TeamName name && MainName == name.MainName;

        public override int GetHashCode()
            => HashCode.Combine(MainName);

        public bool PlayedIn(Game game) 
            => game.Teams.Any(Matches);
    }
}