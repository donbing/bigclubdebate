using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BigClubDebate.Data;
using BigClubDebate.Data.Model.DataTypes;
using BigClubDebate.Data.Model.Reader;
using Xunit;

namespace BigClubDebate.Tests;

public class GameTests
{
    [Fact]
    public void GameWithTieWinnerOverrulesGoalDifferenceForWinner()
    {
        var game = CreateGameWithTieWinner("Real Madrid", "Stade Reims", 3, 3, "Real Madrid");

        Assert.Equal("Real Madrid", game.Winner);
        Assert.Equal("Stade Reims", game.Loser);
        Assert.False(game.Drawn);
    }

    [Fact]
    public void GameWithoutTieWinnerDeterminesWinnerByGoals()
    {
        var game = CreateStandardGame("Bayern Munich", "Leeds United", 2, 0);

        Assert.Equal("Bayern Munich", game.Winner);
        Assert.Equal("Leeds United", game.Loser);
        Assert.False(game.Drawn);
    }

    [Fact]
    public void GameWithoutTieWinnerAndEqualGoalsIsDrawn()
    {
        var game = CreateStandardGame("Liverpool", "AC Milan", 3, 3);

        Assert.Null(game.Winner);
        Assert.Null(game.Loser);
        Assert.True(game.Drawn);
    }

    private static Game CreateGameWithTieWinner(string home, string away, int homeGoals, int awayGoals, string tieWinner)
    {
        return new Game
        {
            Home = home,
            Away = away,
            HomeGoals = homeGoals,
            AwayGoals = awayGoals,
            TieWinner = tieWinner
        };
    }

    private static Game CreateStandardGame(string home, string away, int homeGoals, int awayGoals)
    {
        return new Game
        {
            Home = home,
            Away = away,
            HomeGoals = homeGoals,
            AwayGoals = awayGoals
        };
    }
}

public class NormalizationTests
{
    [Theory]
    [InlineData("Manchester City Football Club", "Manchester City")]
    [InlineData("Real Madrid Club de Fútbol", "Real Madrid")]
    [InlineData("Club Atlético de Madrid S.A.D.", "Atletico Madrid")]
    [InlineData("FC Bayern München", "Bayern Munich")]
    [InlineData("Juventus Football Club", "Juventus")]
    [InlineData("Chelsea FC", "Chelsea")]
    public void TransfermarktNamesAreNormalizedCorrectly(string rawName, string expectedNormalized)
    {
        var actualNormalized = InvokeTransfermarktNormalize(rawName);
        Assert.Equal(expectedNormalized, actualNormalized);
    }

    [Theory]
    [InlineData("Real Madrid Football Club", "Real Madrid")]
    [InlineData("SL Benfica FC", "SL Benfica")]
    public void ChampsNamesAreNormalizedCorrectly(string rawName, string expectedNormalized)
    {
        var actualNormalized = InvokeChampsNormalize(rawName);
        Assert.Equal(expectedNormalized, actualNormalized);
    }

    private static string InvokeTransfermarktNormalize(string raw)
    {
        var method = typeof(TransfermarktCsvReader).GetMethod("NormalizeTeamName", BindingFlags.Static | BindingFlags.NonPublic);
        return (string)method!.Invoke(null, new object[] { raw })!;
    }

    private static string InvokeChampsNormalize(string raw)
    {
        var method = typeof(ChampsCsvReader).GetMethod("NormalizeTeamName", BindingFlags.Static | BindingFlags.NonPublic);
        return (string)method!.Invoke(null, new object[] { raw })!;
    }
}

public class TeamsTests
{
    [Fact]
    public void TeamsShouldNotContainDuplicateMainNames()
    {
        var repoRoot = FindRepositoryRoot();
        var config = new FootballDataFolderConfig(Path.Combine(repoRoot, "BigClubDebate.Web", "GameData"));
        var reader = new FootyDataReader(config);
        var teams = new Teams(reader).ToList();

        var distinctMainNames = teams.Select(t => t.MainName).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        Assert.Equal(teams.Count, distinctMainNames.Count);
    }

    [Fact]
    public void TeamsShouldNotShareAliases()
    {
        var repoRoot = FindRepositoryRoot();
        var config = new FootballDataFolderConfig(Path.Combine(repoRoot, "BigClubDebate.Web", "GameData"));
        var reader = new FootyDataReader(config);
        var teams = new Teams(reader).ToList();

        for (var i = 0; i < teams.Count; i++)
        {
            for (var j = i + 1; j < teams.Count; j++)
            {
                var left = new HashSet<string>(teams[i], StringComparer.OrdinalIgnoreCase);
                var right = new HashSet<string>(teams[j], StringComparer.OrdinalIgnoreCase);

                left.IntersectWith(right);
                Assert.Empty(left);
            }
        }
    }

    [Fact]
    public void TeamsShouldHaveExpectedTeamCount()
    {
        var repoRoot = FindRepositoryRoot();
        var config = new FootballDataFolderConfig(Path.Combine(repoRoot, "BigClubDebate.Web", "GameData"));
        var reader = new FootyDataReader(config);
        var teams = new Teams(reader).ToList();

        Assert.Equal(91, teams.Count);
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
