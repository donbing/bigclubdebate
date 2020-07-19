using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BigClubDebate.Data.Model;

namespace BigClubDebate.Data
{
    public class OpenFootballEnglishLeagueReader 
    {
        readonly string dataFolderPath;
        string FaCupFilePath => Path.Combine(dataFolderPath, "facup.csv.txt");
        string LeagueDataParentFolder => Path.Combine(dataFolderPath, "england-master");
        string LeagueCupFilePath => Path.Combine(dataFolderPath, "leaguecup.csv.txt");

        public IList<CupGame> FaCupGames;

        public IList<CupGame> LeagueCupGames;

        public IList<LeagueSeason> LeagueSeasons;

        public OpenFootballEnglishLeagueReader(string path)
        {
            dataFolderPath = path;

            LeagueSeasons = Directory
                .EnumerateDirectories(LeagueDataParentFolder, "????-??", SearchOption.AllDirectories)
                .Select(ReadFilesForYearFolder)
                .OrderBy(x => x.Name)
                .ToList();

            LeagueCupGames = ReadCupGames(LeagueCupFilePath);
            FaCupGames = ReadCupGames(FaCupFilePath);
        }

        static List<CupGame> ReadCupGames(string csvFilePath) =>
            File.ReadAllText(csvFilePath)
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(x => x.Split(","))
                .Select(CupGameFrom)
                .Where(x => x != null)
                .ToList();

        static LeagueSeason ReadFilesForYearFolder(string yearPath)
        {
            var seasonName = Path.GetFileNameWithoutExtension(yearPath);
            var year = seasonName.Substring(0, 4);

            var divisions = Directory.EnumerateFiles(yearPath, "*.txt")
                .Where(f => !f.EndsWith(".conf.txt"))
                .Where(f => !f.EndsWith("playoffs.txt"))
                .Select(f => ReadYearFromFilePath(f, year))
                .ToList();

            return new LeagueSeason(seasonName, divisions);
        }

        static CupGame CupGameFrom(string[] x)
        {
            // deals with a few games from 1800s
            if (x[4] == "NA")
                return null;

            var reg = Regex.Match(x[4], @"(\d+)-(\d+)");

            return new CupGame
            {
                Date = DateTime.TryParse(x[0].Replace("\"", "").Trim(), out var date)
                    ? date
                    : throw new Exception("car read date"),

                Season = x[1].Replace("\"", "").Trim(),
                Home = x[2].Replace("\"", "").Trim(),
                Away = x[3].Replace("\"", "").Trim(),
                HomeGoals = int.Parse(reg.Groups[1].Value),
                AwayGoals = int.Parse(reg.Groups[2].Value),
                Type = x[7].Replace("\"", "").Trim(),
            };
        }

        const string LeagueYearFileNamePattern = @"^(\d)-(.*).txt";
        static Division ReadYearFromFilePath(string filePath, string year)
        {
            var fileInfo = new FileInfo(filePath);
            var fileNameMatch = Regex.Match(fileInfo.Name, LeagueYearFileNamePattern);

            var leagueName = fileNameMatch.Groups[2].Value.Trim();
            var priority = int.Parse(fileNameMatch.Groups[1].Value);

            return ReadSeasonFromText(priority, leagueName, File.ReadAllText(fileInfo.FullName), year);
        }

        static Division ReadSeasonFromText(int priority, string name, string fileText, string season)
        {
            var fileLines = fileText.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var games = new List<Game>();
            var date = new DateTime();
            foreach (var line in fileLines)
            {
                if (line.StartsWith("["))
                {
                    date = GetDate(season, line);

                }
                var gameMatch = Regex.Match(line, @"^  (.*)(\d)-(\d)(.*)");

                if (gameMatch.Success)
                {
                    var g = ParseGameFrom(gameMatch.Groups, date, season);
                    games.Add(g);
                }
            }

            var fixtures = fileLines
                .Select(g => Regex.Match(g, @"^  (.*)(\D) - (\D)(.*)"))
                .Where(rx => rx.Success)
                .Select(rx => rx.Groups)
                .Select(ParseFixtureFrom)
                .ToList();

            return new Division
            {
                DivisionPriority = priority,
                DivisionName = name,
                Games = games,
                Fixtures = fixtures,
                Year = season,
            };
        }

        static DateTime GetDate(string season, string line)
        {
            var dateLine = line.Replace("[", "").Replace("]", "") + "/";
            return DateTime.TryParseExact(dateLine + season, "ddd MMM/d/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out var date1)
                ? date1
                : DateTime.TryParseExact(dateLine + (int.Parse(season) + 1), "ddd MMM/d/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out var date2)
                    ? date2
                    : throw new Exception("DT fail");
        }

        static Game ParseGameFrom(GroupCollection m, DateTime date, string season)
            => new Game
            {
                Home = m[1].Value.Trim(),
                HomeGoals = int.Parse(m[2].Value.Trim()),
                Away = m[4].Value.Trim(),
                AwayGoals = int.Parse(m[3].Value.Trim()),
                Date = date,
                Season = season,
            };

        static Fixture ParseFixtureFrom(GroupCollection m) => new Fixture
        {
            Home = m[1].Value.Trim(),
            Away = m[4].Value.Trim(),
        };
    }
}