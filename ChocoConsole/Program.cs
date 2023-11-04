using ChocoList;
using ChocoLog;
using ChocoInteraction;
using ChocoProject.Core;
using ManagementPeople;
using System;

namespace ChocoConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize the logger =>
            var logger = new FileLogger();
            // Initialize the services =>
            var adminService = new AdminService(logger, new Interaction.FileService());
            var buyerService = new BuyerService(logger, new Interaction.FileService());
            // Initialize the core =>
            var core = new Core(adminService, buyerService, logger);
            // Initialize the configuration =>
            var appConfig = new AppConfiguration();
            // Initialize the menu =>
            // Initialize the menu =>
            await appConfig.InitializeAsync();
            // Check if the database is initialized =>
            if (!appConfig.IsDatabaseInitialized)
            {
                // Clear the database =>
                await core.ClearAsync();
                // Initialize the database =>
                appConfig.IsDatabaseInitialized = true;
                // Save the configuration =>
                await appConfig.SaveConfigurationAsync();
            }
            // Start the application =>
            await core.Start();
        }
    }
}