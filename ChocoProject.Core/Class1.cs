using ChocoList;
using ChocoLog;
using ManagementPeople;

namespace ChocoProject.Core
{
    public class Core : MBuyer
    {
        private readonly IAdmin _adminService;
        private readonly IBuyersService _buyerService;
        private readonly ILogger _logger;
        private MAdministrator _adminMenu;
        private MStart _startMenu;

        public Core(IAdmin adminService, IBuyersService buyerService, ILogger logger)
        {
            _adminService = adminService;
            _buyerService = buyerService;
            _logger = logger;
            _adminMenu = new MAdministrator();
            _startMenu = new MStart();
        }
        public void Start()
        {
            _startMenu.DisplayMenuStart();
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    DisplayMenuBuyer();
                    break;
                case "2":
                    HandleAdminMenu();
                    break;
                case "3":
                    Console.WriteLine("\nGoodbye");
                    break;
                default:
                    Console.WriteLine("\nInvalid choice, Please try again !");
                    Start();
                    break;
            }
        }
        public void HandleAdminConnected(string adminLogin)
        {
            _adminMenu.DisplayMenuAdminConnected();
            string adminChoice = Console.ReadLine();
            switch (adminChoice)
            {
                case "1":
                    var article = _adminMenu.AddArticle();
                    _adminService.AddArticle(new Article(article.reference, article.price), new Admin(adminLogin, adminLogin));
                    HandleAdminConnected(adminLogin);
                    break;
                case "2":
                    Start();
                    break;
                case "3":
                    Console.WriteLine("\nGoodbye");
                    break;
                default:
                    Console.WriteLine("\nInvalid choice, Please try again !");
                    HandleAdminConnected(adminLogin);
                    break;
            }
        }
        private void HandleAdminMenu()
        {
            _adminMenu.DisplayMenuAdmin();
            string adminChoice = Console.ReadLine();
            switch (adminChoice)
            {
                case "1":
                    AdminLogin();
                    break;
                case "2":
                    RegisterAdminAccount();
                    break;
                case "3":
                    Start();
                    break;
                default:
                    Console.WriteLine("Invalid choice, Please try again !");
                    Start();
                    break;
            }
        }
        private void AdminLogin()
        {
            bool loginSuccessful = false;
            while (!loginSuccessful)
            {
                var adminCredentials = _adminMenu.AdminConnecting();
                if (_adminService.ValidateLogin(adminCredentials.login, adminCredentials.password))
                {
                    HandleAdminConnected(adminLogin: adminCredentials.login);
                    loginSuccessful = true; 
                }
                else
                {
                    Console.WriteLine("Invalid login or password. Please try again.");
                }
            }
        }
        private void RegisterAdminAccount()
        {
            var registerCredentials = _adminMenu.RegisterAdmin();
            if (registerCredentials.password != null)
            {
                bool checkPassword = _adminMenu.CheckPassword(registerCredentials.password);
                while (!checkPassword)
                {
                    registerCredentials = _adminMenu.RegisterAdmin();
                    checkPassword = _adminMenu.CheckPassword(registerCredentials.password);
                }
                if (checkPassword)
                {
                    _adminService.AddLoginToFile(registerCredentials.login, registerCredentials.password);
                    Start();
                }
            }
        }
    }
}