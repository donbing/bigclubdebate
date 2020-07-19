using System;
using System.Collections;
using System.Collections.Generic;

namespace BigClubDebate.Data.Model.DataTypes
{
    public class DivisionSeason : IEnumerable<Game>
    {
        public string Year { get; set; }
        public DateTime StartDate => new DateTime(int.Parse(Year), 08, 01);
        public string DivisionName { get; set; }
        public int DivisionPriority { get; set; }
        public IEnumerable<Game> Games { get; set; }
        public IEnumerable<Fixture> Fixtures { get; set; }

        public IEnumerator<Game> GetEnumerator() 
            => Games.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public override string ToString() 
            => $"{DivisionName+Environment.NewLine}{string.Join(Environment.NewLine,Games)}{string.Join(Environment.NewLine, Fixtures)}";
    }
}