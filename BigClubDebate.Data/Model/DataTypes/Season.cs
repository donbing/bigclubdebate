using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BigClubDebate.Data.Model.DataTypes
{
    public class Season : IEnumerable<Game>
    {
        public IEnumerable<Division> Divisions { get; set; }
        
        public string Name;

        public Season(string yearName, IEnumerable<Division> leagues) 
        { 
            Name = yearName;
            Divisions = leagues;
        }

        public Division GetDivision(int division) 
            => Divisions.First(x => x.DivisionPriority == division);

        public IEnumerator<Game> GetEnumerator()
            => Divisions.SelectMany(x => x.Games).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public override string ToString() 
            => $"{Name+Environment.NewLine}{string.Join(Environment.NewLine, Divisions)}";
    }
}