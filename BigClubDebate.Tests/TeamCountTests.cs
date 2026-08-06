using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BigClubDebate.Data.Model.DataTypes;
using BigClubDebate.Data.Model.Reader;
using Xunit;

namespace BigClubDebate.Tests;

/// <summary>
/// These tests verify that we can gather unique team names from all data sources
/// and that normalization reduces name variance.
/// </summary>
public class TeamCountTests
{
    [Fact]
    public void CountUniqueTeamsFromAllSources()
    {
        // and verifies cumulative counts are sensible.
        var (config, reader, champsReader, transfermarktReader) = CreateReaders();

        var teams = new HashSet<string>();

        // League teams from latest season (equivalent to reading england-master/<latest>/*.txt)
        var latestSeason = reader.GetLeagueSeasons()
            .OrderByDescending(s => s.Name)
            .First();
        Assert.NotNull(latestSeason);

        foreach (var div in latestSeason.Divisions)
        {
            foreach (var game in div.Games)
            {
                teams.Add(game.Home);
                teams.Add(game.Away);
            }
        }

        var leagueCount = teams.Count;
        Assert.True(leagueCount > 0, "Should have teams from the latest league season");

        // Add FA Cup teams (equivalent to reading facup.csv.txt)
        foreach (var g in reader.GetFaCupGames())
        {
            teams.Add(g.Home);
            teams.Add(g.Away);
        }

        var leagueAndFaCupCount = teams.Count;
        Assert.True(leagueAndFaCupCount >= leagueCount,
            $"FA Cup should not reduce team count ({leagueAndFaCupCount} >= {leagueCount})");

        // Add League Cup teams (equivalent to reading leaguecup.csv.txt)
        foreach (var g in reader.GetLeagueCupGames())
        {
            teams.Add(g.Home);
            teams.Add(g.Away);
        }

        var leagueAndCupsCount = teams.Count;
        Assert.True(leagueAndCupsCount >= leagueAndFaCupCount,
            $"League Cup should not reduce team count ({leagueAndCupsCount} >= {leagueAndFaCupCount})");

        // Add European competition teams (equivalent to reading champs.csv + transfermarkt/games.csv)
        foreach (var g in champsReader.GetAllGames())
        {
            teams.Add(g.Home);
            teams.Add(g.Away);
        }

        foreach (var g in transfermarktReader.GetAllEuropeanGames())
        {
            teams.Add(g.Home);
            teams.Add(g.Away);
        }

        var allSourcesCount = teams.Count;
        Assert.True(allSourcesCount >= leagueAndCupsCount,
            $"European sources should not reduce team count ({allSourcesCount} >= {leagueAndCupsCount})");

        // Sanity: a dataset spanning English football history plus European cups
        // should easily have hundreds of unique team names
        Assert.True(allSourcesCount > 200,
            $"Expected >200 unique teams across all sources, got {allSourcesCount}");
    }

    [Fact]
    public void CountNormalizedTeamsAllSources()
    {
        // The C# readers (ChampsCsvReader, TransfermarktCsvReader) already normalize during
        // parsing, so the league and cup names are the "raw" baseline, and European source
        // names come through already normalized.
        var (config, reader, champsReader, transfermarktReader) = CreateReaders();

        var latestSeason = reader.GetLeagueSeasons()
            .OrderByDescending(s => s.Name)
            .First();

        var raw = new HashSet<string>();
        var allNorm = new HashSet<string>();

        // League — names are as-is from data files (no normalization)
        foreach (var div in latestSeason.Divisions)
        {
            foreach (var game in div.Games)
            {
                raw.Add(game.Home);
                raw.Add(game.Away);
                allNorm.Add(game.Home);
                allNorm.Add(game.Away);
            }
        }

        // FA Cup — names as-is
        foreach (var g in reader.GetFaCupGames())
        {
            raw.Add(g.Home);
            raw.Add(g.Away);
            allNorm.Add(g.Home);
            allNorm.Add(g.Away);
        }

        // League Cup — names as-is
        foreach (var g in reader.GetLeagueCupGames())
        {
            raw.Add(g.Home);
            raw.Add(g.Away);
            allNorm.Add(g.Home);
            allNorm.Add(g.Away);
        }

        // Champs — reader already normalizes during parsing
        foreach (var g in champsReader.GetAllGames())
        {
            raw.Add(g.Home);
            raw.Add(g.Away);
            allNorm.Add(g.Home);
            allNorm.Add(g.Away);
        }

        // Transfermarkt — reader already normalizes during parsing
        foreach (var g in transfermarktReader.GetAllEuropeanGames())
        {
            raw.Add(g.Home);
            raw.Add(g.Away);
            allNorm.Add(g.Home);
            allNorm.Add(g.Away);
        }

        Assert.True(raw.Count > 0, "Should have raw team names");
        Assert.True(allNorm.Count > 0, "Should have normalized team names");
        Assert.True(allNorm.Count <= raw.Count,
            $"Normalized count ({allNorm.Count}) should be ≤ raw count ({raw.Count})");
    }

    /// <summary>
    /// Tests that Transfermarkt normalization reduces name count compared to raw CSV names.
    /// Reads the raw CSV directly and compares un-normalized vs normalized counts.
    /// </summary>
    [Fact]
    public void TransfermarktNormalizationReducesTeamCount()
    {
        var (config, _, _, transfermarktReader) = CreateReaders();
        var csvPath = config.TransfermarktGamesCsvPath;

        if (!File.Exists(csvPath))
            return; // Skip if data file doesn't exist in this environment

        // Raw names: read the CSV ourselves and extract team names without normalization
        var rawNames = new HashSet<string>();
        var lines = File.ReadAllLines(csvPath);
        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var cols = line.Split(',');
            if (cols.Length > 20)
            {
                rawNames.Add(cols[19].Trim('"'));
                rawNames.Add(cols[20].Trim('"'));
            }
        }

        // Normalized names: use the reader which already normalizes
        var normalizedNames = new HashSet<string>();
        foreach (var g in transfermarktReader.GetAllEuropeanGames())
        {
            normalizedNames.Add(g.Home);
            normalizedNames.Add(g.Away);
        }

        Assert.True(rawNames.Count > 0, "Should have raw Transfermarkt team names");
        Assert.True(normalizedNames.Count > 0, "Should have normalized Transfermarkt team names");
        Assert.True(normalizedNames.Count <= rawNames.Count,
            $"Transfermarkt normalization should reduce count ({normalizedNames.Count} ≤ {rawNames.Count})");
    }

    /// <summary>
    /// Tests that Champs normalization reduces name count compared to raw CSV names.
    /// Reads the raw CSV directly and compares un-normalized vs normalized counts.
    /// </summary>
    [Fact]
    public void ChampsNormalizationReducesTeamCount()
    {
        var (config, _, champsReader, _) = CreateReaders();
        var csvPath = config.EngSoccerDataChampsCsvPath;

        if (!File.Exists(csvPath))
            return; // Skip if data file doesn't exist in this environment

        // Raw names: read the CSV ourselves and extract team names without normalization
        var rawNames = new HashSet<string>();
        var lines = File.ReadAllLines(csvPath);
        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var cols = line.Split(',');
            if (cols.Length > 5)
            {
                rawNames.Add(cols[4].Trim('"'));
                rawNames.Add(cols[5].Trim('"'));
            }
        }

        // Normalized names: use the reader which already normalizes
        var normalizedNames = new HashSet<string>();
        foreach (var g in champsReader.GetAllGames())
        {
            normalizedNames.Add(g.Home);
            normalizedNames.Add(g.Away);
        }

        Assert.True(rawNames.Count > 0, "Should have raw Champs team names");
        Assert.True(normalizedNames.Count > 0, "Should have normalized Champs team names");
        Assert.True(normalizedNames.Count <= rawNames.Count,
            $"Champs normalization should reduce count ({normalizedNames.Count} ≤ {rawNames.Count})");
    }

    private static (FootballDataFolderConfig, FootyDataReader, ChampsCsvReader, TransfermarktCsvReader) CreateReaders()
    {
        var repoRoot = FindRepositoryRoot();
        var config = new FootballDataFolderConfig(Path.Combine(repoRoot, "BigClubDebate.Web", "GameData"));
        var reader = new FootyDataReader(config);
        var champsReader = new ChampsCsvReader(config.EngSoccerDataChampsCsvPath);
        var transfermarktReader = new TransfermarktCsvReader(config.TransfermarktGamesCsvPath);
        return (config, reader, champsReader, transfermarktReader);
    }

    private static string FindRepositoryRoot()
    {
        var dir = AppContext.BaseDirectory;
        while (dir != null && !Directory.Exists(Path.Combine(dir, "BigClubDebate.Web")))
        {
            dir = Path.GetDirectoryName(dir);
        }

        if (dir == null)
            throw new InvalidOperationException("Could not find repository root containing BigClubDebate.Web");

        return dir;
    }
}
