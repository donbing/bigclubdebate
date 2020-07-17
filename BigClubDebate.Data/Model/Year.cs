using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BigClubDebate.Data
{
    public class Year
    {
        public IEnumerable<League> Leagues { get; set; }
        public IEnumerable<Game> Games => Leagues.SelectMany(x => x.games);

        public string name;

        public Year(string name1, IEnumerable<League> leagues) { this.name = name1;
            Leagues = leagues;
        }

        public static Year FromPath(string yearPath) {
            var directoryInfo = new DirectoryInfo(yearPath);
            var leagues = directoryInfo.EnumerateFiles("*.txt")
                .Where(f => !f.Name.EndsWith(".conf.txt"))
                .Where(f => !f.Name.EndsWith("playoffs.txt"))
                .Select(f => League.FromFilePath(f.FullName, directoryInfo.Name.Substring(0, 4)))
                .ToList();
            
            return new Year(directoryInfo.Name, leagues);
        }

        public League GetLeague(int v) => Leagues.First(x => x.Priroriry == v);
        public override string ToString() 
            => $"{name+Environment.NewLine}{string.Join(Environment.NewLine, Leagues)}";
    }
}