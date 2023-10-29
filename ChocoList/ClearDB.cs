using System;
using System.IO;
using System.Linq.Expressions;
using ChocoInteraction;
using ChocoLog;

namespace ChocoList;

public interface IClearDB
{
    string LogAndConsole(string message);
    bool ClearFilesJson();
    bool CreateFilesJson();
    bool Initialization();
}
    public class ClearDB : IClearDB
    {
        // Initialization =>
        private readonly ILogger _logger;
        private readonly Interaction.FileService _fileService = new Interaction.FileService();
        private readonly string[] _filePaths;
        // END Initialization =>
        
        // Constructor =>
        public ClearDB(ILogger logger, Interaction.FileService fileService) => (_logger, _fileService, _filePaths) = (logger, fileService, new string[]{"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/admin.json", "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/article.json", "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/buyer.json", "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/itemPurchased.json"});
        // END Constructor =>
        public string LogAndConsole(string message)
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
        public bool Initialization()
        {
            try
            {
                if (ClearFilesJson())
                {
                    CreateFilesJson();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                LogAndConsole("Error" + e.Message);
                return false;
            }
        }
        public bool ClearFilesJson() 
        {
            try
            {
                // Create a boolean to verify if the file have been deleted =>
                bool verify = false;
                // Search all the files in the path =>
                foreach (var filePath in _filePaths)
                {
                    // Check if the file exist =>
                    if (_fileService.FileExists(filePath))
                    {
                        // Delete the file =>
                        _fileService.DeleteFile(filePath);
                        verify = true;
                    }
                }
                if (verify)
                    LogAndConsole("---->\n- Admin file have been deleted\n- Article file have been deleted\n- Buyer file have been deleted\n- ItemPurchased file have been deleted\n");
                return true;
            }
            catch (Exception e)
            {
                // Return an error message or code =>
                LogAndConsole("Error : " + e.Message);
                return false;
            }
        }
        public bool CreateFilesJson()
        {
            try
            {
                // Create a boolean to verify if the file have been created =>
                bool verify = false;
                // Search all the files in the path =>
                foreach (var filePath in _filePaths)
                {
                    // Create the file =>
                    _fileService.WriteFile(filePath, "");
                    verify = true;
                }
                if (verify)
                    LogAndConsole("---->\n- Admin file have been created\n- Article file have been created\n- Buyer file have been created\n- ItemPurchased file have been created\n");
                return true;
            }
            catch (Exception e)
            {
                // Return an error message or code =>
                LogAndConsole("Error : " + e.Message);
                return false;
            }
        }
    }