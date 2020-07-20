using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BigClubDebate.Data.Model.DataTypes;

namespace BigClubDebate.Data.Model.Reader
{
    public class FootyDataReader 
    {
        readonly FootballDataFolderConfig _config;
        public readonly IList<CupGame> FaCupGames;
        public readonly IList<CupGame> LeagueCupGames;
        public readonly IList<Season> LeagueSeasons;

        public FootyDataReader(FootballDataFolderConfig config)
        {
            _config = config;
            LeagueSeasons = ReadLeagueSeasons(config.LeagueDataParentFolder);
            LeagueCupGames = ReadCupGames(config.LeagueCupFilePath);
            FaCupGames = ReadCupGames(config.FaCupFilePath);
        }

        List<Season> ReadLeagueSeasons(string leagueDataParentFolder)
        {
            var older = File.ReadAllLines(_config.OlderLeagueCupFilePath)
                .Skip(1)
                .Select(x => x.Split(","))
                .Select(LeagueGameFrom)
                .ToLookup(s => s.Season)
                .ToLookup(g => g.Key, g => g.GroupBy(gg => gg.Division))
                .Select(Thing)
                .ToList();
            return older.ToList();
            // codre for parsing the better maintained league file... only goes back to 1991, old data goes to 1880
            return Directory
                .EnumerateDirectories(leagueDataParentFolder, "????-??", SearchOption.AllDirectories)
                .Select(ReadFilesForYearFolder)
                .OrderBy(x => x.Name)
                .ToList();
        }

        Season Thing(IGrouping<string, IEnumerable<IGrouping<string, Game>>> s)
        {
            var leagues = s
                .SelectMany(x => SeasonDiv(x,s.Key))
                .ToList();

            return new Season(s.Key, leagues);
        }

        List<DivisionSeason> SeasonDiv(IEnumerable<IGrouping<string, Game>> arg, string sKey) 
            => arg.Select(dss => new DivisionSeason(sKey, dss.Key, dss.ToList(), Int32.Parse(dss.Key == "NA" ? "1" : dss.Key))).ToList();

        Game LeagueGameFrom(string[] x)
        {
            return new Game
            {
                Date = DateTime.TryParse(x[0].Replace("\"", "").Trim(), out var date)
                    ? date
                    : throw new Exception("cant read date"),

                Season = x[1].Replace("\"", "").Trim(),
                Home = x[2].Replace("\"", "").Trim(),
                Away = x[3].Replace("\"", "").Trim(),
                HomeGoals = int.Parse(x[5]),
                AwayGoals = int.Parse(x[6]),
                Division = x[7], // int in old file
            };
        }

        static List<CupGame> ReadCupGames(string csvFilePath) =>
            File.ReadAllText(csvFilePath)
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(x => x.Split(","))
                .Select(CupGameFrom)
                .Where(x => x != null)
                .ToList();

        static Season ReadFilesForYearFolder(string yearPath)
        {
            var seasonName = Path.GetFileNameWithoutExtension(yearPath);
            var year = seasonName.Substring(0, 4);

            var divisions = Directory.EnumerateFiles(yearPath, "*.txt")
                .Where(f => !f.EndsWith(".conf.txt"))
                .Where(f => !f.EndsWith("playoffs.txt"))
                .Select(f => ReadYearFromFilePath(f, year))
                .ToList();

            return new Season(seasonName, divisions);
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
        static DivisionSeason ReadYearFromFilePath(string filePath, string year)
        {
            var fileInfo = new FileInfo(filePath);
            var fileNameMatch = Regex.Match(fileInfo.Name, LeagueYearFileNamePattern);

            var leagueName = fileNameMatch.Groups[2].Value.Trim();
            var priority = int.Parse(fileNameMatch.Groups[1].Value);

            return ReadSeasonFromText(priority, leagueName, File.ReadAllText(fileInfo.FullName), year);
        }

        static DivisionSeason ReadSeasonFromText(int priority, string divisionName, string fileText, string season)
        {
            var fileLines = fileText.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var games = new List<Game>();
            var date = new DateTime();
            foreach (var line in fileLines)
            {
                if (line.StartsWith("["))
                {
                    date = GuessGameDate(season, line);

                }
                var gameMatch = Regex.Match(line, @"^  (.*)(\d)-(\d)(.*)");

                if (gameMatch.Success)
                {
                    var g = ParseGameFrom(gameMatch.Groups, date, season, divisionName);
                    games.Add(g);
                }
            }

            var fixtures = fileLines
                .Select(g => Regex.Match(g, @"^  (.*)(\D) - (\D)(.*)"))
                .Where(rx => rx.Success)
                .Select(rx => rx.Groups)
                .Select(ParseFixtureFrom)
                .ToList();

            return new DivisionSeason(season, divisionName, games, priority)
            {
                Fixtures = fixtures,
            };
        }

        static DateTime GuessGameDate(string season, string line)
        {
            var dateLine = line.Replace("[", "").Replace("]", "") + "/";
            return DateTime.TryParseExact(dateLine + season, "ddd MMM/d/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out var date1)
                ? date1
                : DateTime.TryParseExact(dateLine + (int.Parse(season) + 1), "ddd MMM/d/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out var date2)
                    ? date2
                    : throw new Exception("DT fail");
        }

        static Game ParseGameFrom(GroupCollection m, DateTime date, string season, string division)
            => new Game
            {
                Home = m[1].Value.Trim(),
                HomeGoals = int.Parse(m[2].Value.Trim()),
                Away = m[4].Value.Trim(),
                AwayGoals = int.Parse(m[3].Value.Trim()),
                Date = date,
                Season = season,
                Division = division,
            };

        static Fixture ParseFixtureFrom(GroupCollection m) => new Fixture
        {
            Home = m[1].Value.Trim(),
            Away = m[4].Value.Trim(),
        };
    }
}