using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BigClubDebate.Data.Model.DataTypes;

namespace BigClubDebate.Data.Model.Reader
{
    /// <summary>
    /// Parses the engsoccerdata champs.csv (European Cup / Champions League 1955-2017)
    /// into our Game model. Source: https://github.com/jalapic/engsoccerdata
    /// </summary>
    public class ChampsCsvReader
    {
        readonly string _csvPath;

        List<Game>? _allGames;

        public ChampsCsvReader(string csvPath)
        {
            _csvPath = csvPath;
        }

        public IList<Game> GetAllGames()
        {
            EnsureLoaded();
            return _allGames!;
        }

        void EnsureLoaded()
        {
            if (_allGames != null) return;

            if (!File.Exists(_csvPath))
            {
                _allGames = new List<Game>();
                return;
            }

            _allGames = new List<Game>();
            var lines = File.ReadAllLines(_csvPath);

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                try
                {
                    var game = ParseLine(line);
                    if (game != null)
                        _allGames.Add(game);
                }
                catch
                {
                    // Skip malformed lines — data is best-effort
                }
            }
        }

        Game? ParseLine(string line)
        {
            var cols = SplitCsvLine(line);
            if (cols.Length < 12) return null;

            var dateStr = cols[0].Trim();
            if (!DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return null;

            var season = cols[1].Trim();
            var round = cols[2].Trim();

            var home = NormalizeTeamName(cols[4]);
            var away = NormalizeTeamName(cols[5]);

            var homeGoals = ParseInt(cols[10]);
            var awayGoals = ParseInt(cols[11]);

            return new Game
            {
                Date        = date,
                Season      = season,
                Home        = home,
                Away        = away,
                HomeGoals   = homeGoals,
                AwayGoals   = awayGoals,
                Division    = "CL",
                Competition = CompetitionType.ChampionsLeague,
                Round       = round
            };
        }

        /// <summary>
        /// Strips common suffixes so names match our existing alias system.
        /// </summary>
        static string NormalizeTeamName(string raw)
        {
            var name = raw.Trim();
            if (name.EndsWith(" Football Club", StringComparison.OrdinalIgnoreCase))
                name = name.Substring(0, name.Length - " Football Club".Length);
            return name;
        }

        static int ParseInt(string s)
        {
            var trimmed = s.Trim();
            return int.TryParse(trimmed, out var val) ? val : 0;
        }

        /// <summary>
        /// Splits a CSV line respecting quoted fields.
        /// </summary>
        static string[] SplitCsvLine(string line)
        {
            var result = new List<string>();
            var current = new System.Text.StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString());
            return result.ToArray();
        }
    }
}
