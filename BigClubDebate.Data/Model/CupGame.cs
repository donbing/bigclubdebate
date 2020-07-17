using System;
using System.Text.RegularExpressions;

namespace BigClubDebate.Data
{
    public class CupGame : Game {

        public string year { get; set; }
        public string Type { get; set; }

        public static CupGame FromCsv(string[] x)
        {
            if (x[4] == "NA")
                return null;

            var reg = Regex.Match(x[4], @"(\d+)-(\d+)");

            return new CupGame
            {
                date = DateTime.TryParse(x[0].Replace("\"", "").Trim(), out var date)
                    ? new DateTime?(date)
                    : null,

                year = x[1].Replace("\"", "").Trim(),
                Home = x[2].Replace("\"", "").Trim(),
                Away = x[3].Replace("\"", "").Trim(),
                Homegoals = int.Parse(reg.Groups[1].Value),
                Awaygoals = int.Parse(reg.Groups[2].Value),
                Type = x[7],
            };
        }
    }
}