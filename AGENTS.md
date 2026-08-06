# Big Club Debate - Agent Instructions

This codebase is a Blazor Server application that analyzes historical football data to settle "big club" debates.

## Project Overview
- **Framework**: .NET 10.0 (Blazor Server)
- **Primary Projects**:
  - `BigClubDebate.Web`: UI and Blazor components.
  - `BigClubDebate.Data`: Domain logic, models, and data parsing.
- **Data Source**: Historical football results stored as CSV and custom text files in `GameData/`.

## Key Files & Directories
- `BigClubDebate.Data/Model/Reader/FootyDataReader.cs`: Core logic for parsing game data. Uses complex Regex patterns.
- `BigClubDebate.Data/Model/Teams.cs`: Central registry of "big clubs" and their aliases.
- `BigClubDebate.Data/Model/DataTypes/Game.cs`: Main data model for match results.
- `BigClubDebate.Web/Pages/`: Blazor pages (Index, Data, About).
- `BigClubDebate.Web/Pages/Components/`: Reusable UI sections (LeagueSection, FaCupSection, etc.).

## Build & Test
- **Build**: `dotnet build`
- **Test**: `dotnet test` (standard XUnit/NUnit discovery)
- **Pipeline**: See `azure-pipelines.yml` for CI/CD details.

## Conventions
- **Data Loading**: Data is loaded into singleton services (`FootyDataReader`, `Teams`, `LeagueGames`, `CupGames`) at startup in `Startup.cs`.
- **Parsing**: `FootyDataReader` parses non-standard formats. If you modify parsing logic, ensure you don't break historical context (e.g., 2 vs 3 points for a win).
- **Team Aliases**: Always check `Teams.cs` when matching team names; teams often have multiple aliases (e.g., "Man Utd" vs "Manchester United FC").

## Pitfalls
- **Data Paths**: The app resolves `GameData/` paths relative to the entry assembly. Be careful when running tests or new tools that might change the working directory.
- **Hardcoded Teams**: The "big club" list in `Teams.cs` is currently hardcoded. Adding new teams requires updating this file.

## Documentation
- [README.md](./readme.md) - Project overview.
- [NOTES.md](./BigClubDebate.Web/GameData/england-master/NOTES.md) - Details on source data formatting.

## notes
/dev/null does not exist on windows