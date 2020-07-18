using System;
using System.Collections.Generic;

namespace BigClubDebate.Data.Model
{
    public class League
    {
        public string Year { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public IEnumerable<Game> Games { get; set; }
        public IEnumerable<Fixture> Fixtures { get; set; }

        public override string ToString() 
            => $"{Name+Environment.NewLine}{string.Join(Environment.NewLine,Games)}{string.Join(Environment.NewLine, Fixtures)}";
    }
}