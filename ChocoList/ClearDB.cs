using System;
using System.IO;
using System.Linq.Expressions;
using ChocoInteraction;
using ChocoLog;

namespace ChocoList
{
    public class ClearDB
    {
        private readonly ILogger _logger;
        private readonly Interaction.IFileWrite _fileWrite    = new Interaction.FileService();
        private readonly Interaction.IFileExists _fileExists  = new Interaction.FileService();
        private readonly Interaction.IFileDelete _fileDelete  = new Interaction.FileService();
        private readonly string _pathAdmin                    = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/admin.json";
        private readonly string _pathArticle                  = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/article.json";
        private readonly string _pathLog                      = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoLog/log.txt";
        private readonly string _pathBuyer                    = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/buyer.json";
        private readonly string _pathItemPurchased            = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/itemPurchased.json";
        public ClearDB(ILogger logger, Interaction.IFileWrite fileWrite) => (_logger, _fileWrite) = (logger, fileWrite);
        private string LogAndConsole(string message)
        {
            try
            {
                // Display the message in the console
                Console.WriteLine(message);
                // Call the method Log from the class FileLogger
                _logger.Log(message);
                // Return the message
                return message;
            }
            catch (Exception ex)
            {
                // Return an error message or code =>
                return "Error : " + ex.Message;
            }
        }
        public bool ClearFileJson() {
            string[] filePaths = { _pathAdmin, _pathArticle, _pathLog, _pathBuyer, _pathItemPurchased };
            bool atLeastOneFileNotFound = false;
            try
            {
                foreach (var filePath in filePaths)
                {
                    if (_fileExists.FileExists(filePath))
                        _fileDelete.DeleteFile(filePath);
                    else
                    {
                        LogAndConsole("----> The file " + filePath + " doesn't exist");
                        atLeastOneFileNotFound = true;
                    }
                }
                // If at least one file doesn't exist, return false
                if (atLeastOneFileNotFound)
                    return false; 
                else
                {
                    LogAndConsole("----> All files have been deleted");
                    return true;
                }
            }
            catch (Exception e)
            {
                // Return an error message or code =>
                LogAndConsole("Error : " + e.Message);
                return false;
            }
        }
    }
}