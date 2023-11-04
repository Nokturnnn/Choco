using System;
using System.IO;
using System.Linq.Expressions;
using ChocoInteraction;
using ChocoLog;

namespace ChocoList;

public interface IClearDB
{
    Task<string> LogAndConsoleAsync(string message);
    Task<bool> Initialization();
    Task<bool> ClearFilesJson();
    Task<bool> CreateFilesJson();
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
        public async Task<string> LogAndConsoleAsync(string message)
        {
            try
            {
                Console.WriteLine(message);
                await _logger.LogAsync(message);
                return message;
            }
            catch (Exception ex)
            {
                return "Error : " + ex.Message;
            }
        }
        public async Task<bool>Initialization()
        {
            try
            {
                if (await ClearFilesJson())
                {
                    await CreateFilesJson();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                await LogAndConsoleAsync("Error" + e.Message);
                return false;
            }
        }
        public async Task<bool>ClearFilesJson() 
        {
            try
            {
                // Create a boolean to verify if the file have been deleted =>
                bool verify = false;
                // Search all the files in the path =>
                foreach (var filePath in _filePaths)
                {
                    // Check if the file exist =>
                    if (_fileService.FileExistsAsync(filePath))
                    {
                        // Delete the file =>
                        await _fileService.DeleteFileAsync(filePath);
                        verify = true;
                    }
                }
                if (verify)
                    await LogAndConsoleAsync("---->\n- Admin file have been deleted\n- Article file have been deleted\n- Buyer file have been deleted\n- ItemPurchased file have been deleted\n");
                return true;
            }
            catch (Exception e)
            {
                // Return an error message or code =>
                await LogAndConsoleAsync("Error : " + e.Message);
                return false;
            }
        }
        public async Task<bool> CreateFilesJson()
        {
            try
            {
                // Create a boolean to verify if the file have been created =>
                bool verify = false;
                // Search all the files in the path =>
                foreach (var filePath in _filePaths)
                {
                    // Create the file =>
                    await _fileService.WriteFileAsync(filePath, "");
                    verify = true;
                }
                if (verify)
                    await LogAndConsoleAsync("---->\n- Admin file have been created\n- Article file have been created\n- Buyer file have been created\n- ItemPurchased file have been created\n");
                return true;
            }
            catch (Exception e)
            {
                // Return an error message or code =>
                await LogAndConsoleAsync("Error : " + e.Message);
                return false;
            }
        }
    }