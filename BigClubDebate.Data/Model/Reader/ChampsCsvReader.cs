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
            if (cols.Length < 21) return null;

            var dateStr = cols[0].Trim();
            if (!DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return null;

            var season = cols[1].Trim();
            var round = cols[2].Trim();

            var home = NormalizeTeamName(cols[4]);
            var away = NormalizeTeamName(cols[5]);

            // Use total goals (cols 16/17) when available (includes extra time),
            // falling back to FT goals (cols 10/11) for older data where total == NA.
            var ftHome = ParseInt(cols[10]);
            var ftAway = ParseInt(cols[11]);
            var totHome = ParseInt(cols[16]);
            var totAway = ParseInt(cols[17]);

            // totHome/totAway may be 0 if the CSV had "NA" (ParseInt returns 0 for non-numeric).
            // Detect: if tot is 0 and ft is not, use ft. Also, tot could legitimately be 0.
            // Reliable heuristic: if both tot values are 0 and both ft values are non-zero,
            // the total columns were NA, so use ft.
            int homeGoals, awayGoals;
            if (totHome == 0 && totAway == 0 && (ftHome != 0 || ftAway != 0))
            {
                // tot columns were NA — use FT goals
                homeGoals = ftHome;
                awayGoals = ftAway;
            }
            else
            {
                homeGoals = totHome;
                awayGoals = totAway;
            }

            // tiewinner (col 20) — the team that advanced/won the tie.
            // Only relevant when the individual match was a draw but the tie was decided.
            var tieWinner = cols.Length > 20 ? cols[20].Trim() : null;
            if (string.IsNullOrEmpty(tieWinner) || tieWinner == "NA")
                tieWinner = null;
            else
                tieWinner = NormalizeTeamName(tieWinner);

            return new Game
            {
                Date           = date,
                Season         = season,
                Home           = home,
                Away           = away,
                HomeGoals      = homeGoals,
                AwayGoals      = awayGoals,
                TieWinner      = tieWinner,
                Division       = "CL",
                Competition    = CompetitionType.ChampionsLeague,
                Round          = round,
                SourcePriority = 1 // lower priority than Transfermarkt (which has 2)
            };
        }

        /// <summary>
        /// Strips common suffixes and normalizes names so they match across data sources.
        /// </summary>
        static string NormalizeTeamName(string raw)
        {
            var name = raw.Trim();
            if (name.EndsWith(" Football Club", StringComparison.OrdinalIgnoreCase))
                name = name.Substring(0, name.Length - " Football Club".Length);
            if (name.EndsWith(" FC", StringComparison.OrdinalIgnoreCase))
                name = name.Substring(0, name.Length - " FC".Length);
            // Remove trailing periods from abbreviations
            name = name.TrimEnd('.');
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
