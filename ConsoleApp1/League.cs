using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    public class League
    {
        private const string Pattern = @"^(\d)-(.*).txt";
        public string name;
        public int Priroriry;

        public List<string> Table
        {
            get
            {
                var st = games.Select(g => g.home).Distinct().ToDictionary(x => x, x => 0);
                foreach (var game in games)
                {
                    st[game.home] += game.PointsFor(game.home);
                    st[game.away] += game.PointsFor(game.away);
                }

                return st.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
            }
        }

        public IEnumerable<Game> games { get; set; }
        public IEnumerable<Fixture> fixtures { get; set; }

        internal static League FromFilePath(string f, string year) 
        {
            var fileInfo = new FileInfo(f);
            var m = Regex.Match(fileInfo.Name, Pattern);
            var priorityMatch = int.Parse(m.Groups[1].Value);
            var leagueName = m.Groups[2].Value.Trim();
            return FromFileText(priorityMatch, leagueName, File.ReadAllText(fileInfo.FullName), year);
        }

        public static League FromFileText(int priority, string name, string filetext, string year)
        {
            var v = filetext.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            IList<Game> games = new List<Game>();
            var date = new DateTime?();
            foreach (var line in v) {
                if (line.StartsWith("[")) {
                    date = DateTime.TryParseExact(line.Replace("[", "").Replace("]", "") + "/" + year, "ddd MMM/d/yyyy", CultureInfo.CurrentCulture,DateTimeStyles.None,out var date1) 
                        ? new DateTime?(date1)
                        : DateTime.TryParseExact(line.Replace("[", "").Replace("]", "") + "/" + (int.Parse(year) + 1), "ddd MMM/d/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out var date2)
                            ? new DateTime?(date2)  
                            :  throw new Exception("DT fail");

                }
                var gameMatch = Regex.Match(line, @"^  (.*)(\d)-(\d)(.*)");

                if (gameMatch.Success) {
                    var g = NewMethod(gameMatch.Groups);
                    g.date = date;
                    games.Add(g);
                }
            }

            //var listgames = v
            //    .Select(g => Regex.Match(g, @"^  (.*)(\d)-(\d)(.*)"))
            //    .Where(rx => rx.Success)
            //    .Select(rx => rx.Groups)
            //    .Select(m => NewMethod(m))
            //    .ToList();

            return new League
            {   
                Priroriry = priority,
                name = name,
                games = games,
                fixtures = v
                .Select(g => Regex.Match(g, @"^  (.*)(\D) - (\D)(.*)"))
                .Where(rx => rx.Success)
                .Select(rx => rx.Groups)
                .Select(m => NewMethodFixture(m))
                .ToList()
            };
        }
        public override string ToString() 
            => $"{name+Environment.NewLine}{string.Join(Environment.NewLine,games)}{string.Join(Environment.NewLine, fixtures)}";
        private static Game NewMethod(GroupCollection m) => new Game
        {
            home = m[1].Value.Trim(),
            homegoals = int.Parse(m[2].Value.Trim()),
            away = m[4].Value.Trim(),
            awaygoals = int.Parse(m[3].Value.Trim()),
            //date = DateTime.Parse(m[3].Value.Trim()), // needs grouping
        };
        private static Fixture NewMethodFixture(GroupCollection m) => new Fixture
        {
            home = m[1].Value.Trim(),
            away = m[4].Value.Trim(),
        };
    }
}