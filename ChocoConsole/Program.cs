using ChocoList;
using ChocoLog;
using ChocoInteraction;
using ChocoProject.Core;

namespace ChocoConsole;

class Program
{
    static void Main(string[] args)
    {
        var logger = new FileLogger();
        var adminService = new AdminService(logger, new Interaction.FileService(), new Interaction.FileService(), new Interaction.FileService());
        var buyerService = new BuyerService();
        var core = new Core(adminService, buyerService, logger);

        if (core.Clear())
        {
            core.Start();
        }
    }
}