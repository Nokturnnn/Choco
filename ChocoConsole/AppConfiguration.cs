using System;
using System.IO;
using ChocoLog;
using Newtonsoft.Json;

namespace ChocoConsole;

public interface IAppConfiguration
{
    string LogAndConsole(string message);
    bool Initialize();
    bool LoadConfiguration();
    bool SaveConfiguration();
}
    public class AppConfiguration
    {
        // Initialization =>
        public bool IsDatabaseInitialized { get; set; }
        private readonly ILogger _logger = new FileLogger();
        // Path to the configuration file =>
        private string ConfigurationFilePath => "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoConsole/config.json";
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
        public AppConfiguration()
        {
            // Initialize the configuration with default values :>
            IsDatabaseInitialized = false;
        }
        public bool Initialize()
        {
            try
            {
                // Check if the configuration file exists => if it does, load it
                if (File.Exists(ConfigurationFilePath))
                {
                    LoadConfiguration();
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
        public bool LoadConfiguration()
        {
            try
            {
                // Read the configuration file =>
                string configJson = File.ReadAllText(ConfigurationFilePath);
                // Deserialize the JSON into an instance of this class =>
                var config = JsonConvert.DeserializeObject<AppConfiguration>(configJson);
                // Copy the values from the loaded configuration into this instance =>
                IsDatabaseInitialized = config.IsDatabaseInitialized;
                return true;
            }
            catch (Exception ex)
            {
                LogAndConsole("Error loading configuration: " + ex.Message);
                return false;
            }
        }
        public bool SaveConfiguration()
        {
            try
            {
                // Serialize this instance into JSON =>
                var configJson = JsonConvert.SerializeObject(this);
                // Write the JSON to the configuration file =>
                File.WriteAllText(ConfigurationFilePath, configJson);
                return true;
            }
            catch (Exception ex)
            {
                LogAndConsole("Error saving configuration: " + ex.Message);
                return false;
            }
        }
    }