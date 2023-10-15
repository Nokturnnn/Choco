using System;
using System.IO;
using ChocoInteraction;
using ChocoLog;

namespace ChocoList
{
    public class ClearDB
    {
        private readonly ILogger _logger;
        private readonly Interaction.IFileWrite _fileWrite;
        private readonly string _pathAdmin = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/admin.json";
        private readonly string _pathArticles = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/article.json";
        private readonly string _pathLog = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoLog/log.txt";
        public ClearDB(ILogger logger, Interaction.IFileWrite fileWrite) => (_logger, _fileWrite) = (logger, fileWrite);
        private void LogAndConsole(string message)
        {
            Console.WriteLine(message);
            _logger.Log(message);
        }
        public void ClearFileJson()
        {
            if (File.Exists(_pathAdmin) && File.Exists(_pathArticles) && File.Exists(_pathLog))
            {
                try
                {
                    _fileWrite.WriteFile(_pathAdmin, "");
                    _fileWrite.WriteFile(_pathArticles, "");
                    _fileWrite.WriteFile(_pathLog, "");
                    LogAndConsole("----\n.The files admin.json, articles.json, log have been cleared");
                }
                catch (Exception ex)
                {
                    LogAndConsole("An error occurred while clearing the files: " + ex.Message);
                }
            }
            else
                LogAndConsole("----\n.The files admin.json and articles.json do not exist or are already empty.");
        }
    }
}