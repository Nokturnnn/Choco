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
        static void Main(string[] args)
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
            appConfig.Initialize();
            // Check if the database is initialized =>
            if (!appConfig.IsDatabaseInitialized)
            {
                // Clear the database =>
                core.Clear();
                // Initialize the database =>
                appConfig.IsDatabaseInitialized = true;
                // Save the configuration =>
                appConfig.SaveConfiguration();
            }
            // Start the application =>
            core.Start();
        }
    }
}