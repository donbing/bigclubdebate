using System.IO;
using System.Reflection;

namespace BigClubDebate.Data
{
    public class FootballDataFolderConfig
    {
        readonly string rootPath;

        public FootballDataFolderConfig(string dataFolderPath) 
            => rootPath = dataFolderPath;

        public string FaCupFilePath => Path.Combine(rootPath, "facup.csv.txt");
        public string LeagueDataParentFolder => Path.Combine(rootPath, "england-master");
        public string LeagueCupFilePath => Path.Combine(rootPath, "leaguecup.csv.txt");
        public static FootballDataFolderConfig FromEntryAssemblyPath() 
            => new FootballDataFolderConfig(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "GameData"));
    }
}