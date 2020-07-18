using System;
using System.Collections.Generic;
using System.Linq;

namespace BigClubDebate.Data
{
    public class TeamName : HashSet<string>
    {
        public string MainName => this.First();
        public string SlangName => this.ElementAt(1);

        public string ImageName { get; }
        public string BackGroundName { get; }

        public TeamName(IEnumerable<string> collection, string imageName, string backGroundName) : base(collection)
        {
            ImageName = imageName;
            BackGroundName = backGroundName;
        }

        public static implicit operator TeamName(string[] names) => new TeamName(names, "", "");

        public bool Matches(string otherName) => this.Contains(otherName, StringComparer.OrdinalIgnoreCase);
        public override bool Equals(object obj) => obj is TeamName name && MainName == name.MainName;
        public override int GetHashCode() => HashCode.Combine(MainName);
    }
}