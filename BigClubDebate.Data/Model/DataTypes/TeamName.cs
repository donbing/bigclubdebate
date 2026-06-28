using System;
using System.Collections.Generic;
using System.Linq;

namespace BigClubDebate.Data.Model.DataTypes
{
    public class TeamName : HashSet<string>
    {
        public string ImageName { get; }
        public string BackGroundName { get; }
        public bool HasImage => ImageName != null;

        public string MainName 
            => this.First();

        public string NickName 
            => this.Skip(1).FirstOrDefault() ?? MainName;

        public TeamName(IEnumerable<string> names, string imageName) 
            : base(names, StringComparer.OrdinalIgnoreCase) 
                => ImageName = imageName;

        public TeamName(IEnumerable<string> names, string imageName, string backGroundName) 
            : base(names, StringComparer.OrdinalIgnoreCase) 
                => (ImageName, BackGroundName) = (imageName,backGroundName);

        /// <summary>
        /// Creates a TeamName without a badge image or background.
        /// </summary>
        public TeamName(string name) 
            : base(new[] { name, name }, StringComparer.OrdinalIgnoreCase) 
        {
            ImageName = null;
            BackGroundName = null;
        }

        public bool PlayedIn(Game game) 
            => game.Teams.Any(Matches);

        public bool Matches(string otherName) 
            => Contains(otherName);

        public override string ToString()
            => MainName;

        public override bool Equals(object obj)
            => obj is TeamName name && MainName == name.MainName;

        public override int GetHashCode()
            => HashCode.Combine(MainName);

        public static implicit operator TeamName(string name) => new TeamName(new []{name},"");
    }
}