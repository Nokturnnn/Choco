using System;
using System.IO;
using ChocoLog;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace ChocoConsole;

public interface IAppConfiguration
{
    Task<string> LogAndConsoleAsync(string message);
    Task<bool> InitializeAsync();
    Task<bool> LoadConfigurationAsync();
    Task<bool> SaveConfigurationAsync();
}
    public class AppConfiguration
    {
        // Initialization =>
        public bool IsDatabaseInitialized { get; set; }
        private readonly ILogger _logger;
        // End of initialization =>
        
        // Path to the configuration file =>
        private string ConfigurationFilePath => "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoConsole/config.json";
        private async Task<string> LogAndConsoleAsync(string message)
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
        public AppConfiguration(ILogger logger)
        {
            _logger = logger;
            // Initialize the configuration with default values :>
            IsDatabaseInitialized = false;
        }
        public async Task<bool>InitializeAsync()
        {
            try
            {
                // Check if the configuration file exists => if it does, load it
                if (File.Exists(ConfigurationFilePath))
                {
                    await LoadConfigurationAsync();
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
        private async Task<bool> LoadConfigurationAsync()
        {
            try
            {
                // Read the configuration file =>
                string configJson = File.ReadAllText(ConfigurationFilePath);
                // Deserialize the JSON into an instance of this class =>
                var config = JsonConvert.DeserializeObject<AppConfiguration>(configJson);
                // Copy the values from the loaded configuration into this instance =>
                if (config != null) IsDatabaseInitialized = config.IsDatabaseInitialized;
                return true;
            }
            catch (Exception ex)
            {
                await LogAndConsoleAsync("Error loading configuration: " + ex.Message);
                return false;
            }
        }
        public async Task<bool> SaveConfigurationAsync()
        {
            try
            {
                // Serialize this instance into JSON =>
                var configJson = JsonConvert.SerializeObject(this);
                // Write the JSON to the configuration file =>
                await File.WriteAllTextAsync(ConfigurationFilePath, configJson);
                return true;
            }
            catch (Exception ex)
            {
                await LogAndConsoleAsync("Error saving configuration: " + ex.Message);
                return false;
            }
        }
    }