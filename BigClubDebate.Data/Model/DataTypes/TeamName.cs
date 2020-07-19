using System;
using System.Collections.Generic;
using System.Linq;

namespace BigClubDebate.Data.Model.DataTypes
{
    public class TeamName : HashSet<string>
    {
        public string ImageName { get; }
        public string BackGroundName { get; }

        public string MainName 
            => this.First();

        public string NickName 
            => this.ElementAt(1);

        public TeamName(IEnumerable<string> names, string imageName) 
            : base(names, StringComparer.OrdinalIgnoreCase) 
                => ImageName = imageName;

        public TeamName(IEnumerable<string> names, string imageName, string backGroundName) 
            : base(names, StringComparer.OrdinalIgnoreCase) 
                => (ImageName, BackGroundName) = (imageName,backGroundName);

        public bool PlayedIn(Game game) 
            => game.Teams.Any(Matches);

        public bool Matches(string otherName) 
            => Contains(otherName);

        public override bool Equals(object obj)
            => obj is TeamName name && MainName == name.MainName;

        public override int GetHashCode()
            => HashCode.Combine(MainName);
    }
}