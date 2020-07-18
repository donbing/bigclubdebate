using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BigClubDebate.Data
{
    public class OpenFootballEnglishLeagueReader
    {
        private readonly string _path;
        private string FaCupFilePath => Path.Combine(_path, "facup.csv.txt");
        private string LeagueDataParentFolder => Path.Combine(_path, "england-master");
        private string LeagueCupFilePath => Path.Combine(_path, "leaguecup.csv.txt");

        public IList<CupGame> CupGames;

        public IList<CupGame> LeagueCup;

        public IList<Year> LeagueYears;

        //https://github.com/openfootball/england
        public OpenFootballEnglishLeagueReader(string path)
        {
            _path = path;
            LeagueYears = Directory
                .EnumerateDirectories(LeagueDataParentFolder, "????-??", SearchOption.AllDirectories)
                .Select(ReadFilesForYearFolder)
                .OrderBy(x => x.name)
                .ToList();
            ;
            LeagueCup = File.ReadAllText(LeagueCupFilePath)
                        .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Skip(1)
                        .Select(x => x.Split(","))
                        .Select(CupGameFrom)
                        .Where(x => x != null)
                        .ToList();

            CupGames =
                File.ReadAllText(FaCupFilePath)
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1)
                    .Select(x => x.Split(","))
                    .Select(CupGameFrom)
                    .Where(x => x != null)
                    .ToList();
        }
        static Year ReadFilesForYearFolder(string yearPath)
        {
            var yearFolder = new DirectoryInfo(yearPath);
            var year = yearFolder.Name.Substring(0, 4);

            var leagues = yearFolder.EnumerateFiles("*.txt")
                .Where(f => !f.Name.EndsWith(".conf.txt"))
                .Where(f => !f.Name.EndsWith("playoffs.txt"))
                .Select(f => ReadYearFromFilePath(f.FullName, year))
                .ToList();

            return new Year(yearFolder.Name, leagues);
        }

        static CupGame CupGameFrom(string[] x)
        {
            // deals with a few games from 1800s
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

        const string LeagueYearFileNamePattern = @"^(\d)-(.*).txt";
        static League ReadYearFromFilePath(string filePath, string year)
        {
            var fileInfo = new FileInfo(filePath);
            var fileNameMatch = Regex.Match(fileInfo.Name, LeagueYearFileNamePattern);

            var leagueName = fileNameMatch.Groups[2].Value.Trim();
            var priority = int.Parse(fileNameMatch.Groups[1].Value);

            return ReadYearFromText(priority, leagueName, File.ReadAllText(fileInfo.FullName), year);
        }

        static League ReadYearFromText(int priority, string name, string fileText, string year)
        {
            var v = fileText.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            IList<Game> games = new List<Game>();
            var date = new DateTime?();
            foreach (var line in v)
            {
                if (line.StartsWith("["))
                {
                    date = DateTime.TryParseExact(line.Replace("[", "").Replace("]", "") + "/" + year, "ddd MMM/d/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out var date1)
                        ? date1
                        : DateTime.TryParseExact(line.Replace("[", "").Replace("]", "") + "/" + (int.Parse(year) + 1), "ddd MMM/d/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out var date2)
                            ? new DateTime?(date2)
                            : throw new Exception("DT fail");

                }
                var gameMatch = Regex.Match(line, @"^  (.*)(\d)-(\d)(.*)");

                if (gameMatch.Success)
                {
                    var g = ParseGameFrom(gameMatch.Groups);
                    g.date = date;
                    games.Add(g);
                }
            }

            var fixtures = v
                .Select(g => Regex.Match(g, @"^  (.*)(\D) - (\D)(.*)"))
                .Where(rx => rx.Success)
                .Select(rx => rx.Groups)
                .Select(ParseFixtureFrom)
                .ToList();

            return new League
            {
                Priroriry = priority,
                name = name,
                games = games,
                fixtures = fixtures
            };
        }

        static Game ParseGameFrom(GroupCollection m) => new Game
        {
            Home = m[1].Value.Trim(),
            Homegoals = int.Parse(m[2].Value.Trim()),
            Away = m[4].Value.Trim(),
            Awaygoals = int.Parse(m[3].Value.Trim()),
            //date = DateTime.Parse(m[3].Value.Trim()), // needs grouping
        };

         static Fixture ParseFixtureFrom(GroupCollection m) => new Fixture
        {
            Home = m[1].Value.Trim(),
            Away = m[4].Value.Trim(),
        };
    }
}