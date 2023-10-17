using System;
using System.IO;
using ChocoInteraction;
using ChocoLog;

namespace ChocoList
{
    public class ClearDB
    {
        private readonly ILogger _logger;
        private readonly Interaction.IFileWrite _fileWrite = new Interaction.FileService();
        private readonly Interaction.IFileExists _fileExists = new Interaction.FileService();
        private readonly Interaction.IFileDelete _fileDelete = new Interaction.FileService();
        private readonly string _pathAdmin   = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/admin.json";
        private readonly string _pathArticle = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/article.json";
        private readonly string _pathLog     = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoLog/log.txt";
        private readonly string _pathBuyer   = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/buyer.json";
        public ClearDB(ILogger logger, Interaction.IFileWrite fileWrite) => (_logger, _fileWrite) = (logger, fileWrite);
        private void LogAndConsole(string message)
        {
            Console.WriteLine(message);
            _logger.Log(message);
        }
        public void ClearFileJson()
        {
            if (_fileExists.FileExists(_pathAdmin) || _fileExists.FileExists(_pathArticle) || _fileExists.FileExists(_pathLog) || _fileExists.FileExists((_pathBuyer)))
            {
                try
                {
                    _fileDelete.DeleteFile(_pathAdmin);
                    _fileDelete.DeleteFile(_pathArticle);
                    _fileDelete.DeleteFile(_pathLog);
                    _fileDelete.DeleteFile(_pathBuyer);
                    LogAndConsole("----\n.The files admin.json, articles.json, buyers, log have been deleted");
                }
                catch (Exception ex)
                {
                    LogAndConsole("An error occurred while clearing the files: " + ex.Message);
                }
            }
            else
                LogAndConsole("----\n.The files admin.json, article.json, buyers.json, log.txt do not exist or are already empty.");
        }
    }
}