using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BigClubDebate.Data.Model.DataTypes;

namespace BigClubDebate.Data.Model.Reader
{
    /// <summary>
    /// Parses the transfermarkt-datasets games CSV into our Game model.
    /// Filters to European club competitions (CL, EL, UCOL and qualifiers).
    /// CSV source: https://github.com/dcaribou/transfermarkt-datasets
    /// </summary>
    public class TransfermarktCsvReader
    {
        readonly string _csvPath;

        List<Game>? _allGames;

        static readonly HashSet<string> TargetCompetitionIds = new(StringComparer.OrdinalIgnoreCase)
        {
            "CL", "CLQ", "EL", "ELQ", "UCOL", "ECLQ"
        };

        static readonly Dictionary<string, CompetitionType> CompetitionTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["CL"]   = CompetitionType.ChampionsLeague,
            ["CLQ"]  = CompetitionType.ChampionsLeagueQualifying,
            ["EL"]   = CompetitionType.EuropaLeague,
            ["ELQ"]  = CompetitionType.EuropaLeagueQualifying,
            ["UCOL"] = CompetitionType.ConferenceLeague,
            ["ECLQ"] = CompetitionType.ConferenceLeagueQualifying
        };

        public TransfermarktCsvReader(string csvPath)
        {
            _csvPath = csvPath;
        }

        public IList<Game> GetAllEuropeanGames()
        {
            EnsureLoaded();
            return _allGames!;
        }

        public IEnumerable<Game> GetChampionsLeagueGames()
            => _allGames!.Where(g => g.Competition == CompetitionType.ChampionsLeague
                                  || g.Competition == CompetitionType.ChampionsLeagueQualifying);

        public IEnumerable<Game> GetEuropaLeagueGames()
            => _allGames!.Where(g => g.Competition == CompetitionType.EuropaLeague
                                  || g.Competition == CompetitionType.EuropaLeagueQualifying);

        public IEnumerable<Game> GetConferenceLeagueGames()
            => _allGames!.Where(g => g.Competition == CompetitionType.ConferenceLeague
                                  || g.Competition == CompetitionType.ConferenceLeagueQualifying);

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

            var competitionId = cols[1].Trim();
            if (!TargetCompetitionIds.Contains(competitionId)) return null;

            if (!CompetitionTypeMap.TryGetValue(competitionId, out var compType))
                return null;

            var dateStr = cols[4].Trim();
            if (!DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return null;

            return new Game
            {
                Date        = date,
                Season      = cols[2].Trim(),
                Home        = NormalizeTeamName(cols[19]),
                Away        = NormalizeTeamName(cols[20]),
                HomeGoals   = ParseInt(cols[7]),
                AwayGoals   = ParseInt(cols[8]),
                Division    = competitionId,
                Competition = compType,
                Round       = cols[3].Trim()
            };
        }

        /// <summary>
        /// Strips Transfermarkt's "Football Club" / "FC" suffixes so names match
        /// our existing alias system (e.g. "Manchester City Football Club" → "Manchester City").
        /// </summary>
        static string NormalizeTeamName(string raw)
        {
            var name = raw.Trim();
            // "Manchester City Football Club" → "Manchester City"
            if (name.EndsWith(" Football Club", StringComparison.OrdinalIgnoreCase))
                name = name.Substring(0, name.Length - " Football Club".Length);
            // "AFC Ajax Amsterdam" → "AFC Ajax" (but not "Liverpool FC" yet)
            // Keep the rest as-is for non-English clubs
            return name;
        }

        static int ParseInt(string s)
        {
            var trimmed = s.Trim();
            return int.TryParse(trimmed, out var val) ? val : 0;
        }

        /// <summary>
        /// Splits a CSV line respecting quoted fields (e.g., "Manchester City, Reserves").
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
                    // Handle escaped quotes ("") inside quoted fields
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++; // skip the second quote
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
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
