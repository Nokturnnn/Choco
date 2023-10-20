using ChocoInteraction;
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
        private MBuyer _buyerMenu;
        private MStart _startMenu;
        private ClearDB _clearDB;
        private string currentBuyerLogin;
        private (string referance, float price) article;
        
        public Core(IAdmin adminService, IBuyersService buyerService, ILogger logger)
        {
            _adminService = adminService;
            _buyerService = buyerService;
            _logger = logger;
            _adminMenu = new MAdministrator();
            _buyerMenu = new MBuyer();
            _startMenu = new MStart();
            _clearDB = new ClearDB(logger, new Interaction.FileService());
        }
        public bool Clear()
        {
            _startMenu.DisplayMenuClearDB();
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    _clearDB.ClearFileJson();
                    Start();
                    break;
                case "2":
                    Start();
                    break;
                case "3":
                    Console.WriteLine("\nGoodbye");
                    break;
                default:
                    Console.WriteLine("\nInvalid choice, Please try again !");
                    break;
            }
            return false;
        }
        public void Start()
        {
            _startMenu.DisplayMenuStart();
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    HandleBuyerMenu();
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
        private void HandleBuyerMenu()
        {
            _buyerMenu.DisplayMenuBuyer();
            string buyerChoice = Console.ReadLine();
            switch (buyerChoice)
            {
                case "1":
                    BuyerLogin();
                    break;
                case "2":
                    RegisterBuyerAccount();
                    break;
                case "3":
                    Start();
                    break;
                default:
                    Console.WriteLine("\nInvalid choice, Please try again !");
                    HandleBuyerMenu();
                    break;
            }
        }
        private void HandleBuyerRegistered(string buyerLogin)
        {
            _buyerMenu.DisplayBuyerRegistered();
            string buyerChoice = Console.ReadLine();
            switch (buyerChoice)
            {
                case "1":
                    _buyerService.DisplayListOfArticle();
                    HandleBuyerRegistered(buyerLogin);
                    break;
                case "2":
                    var article = _buyerMenu.AddToList();
                    _buyerService.BuyerChooseAnArticleToList(new Article(article.reference, article.quantity), new Buyer(buyerLogin, buyerLogin, buyerLogin, buyerLogin), article.reference, article.quantity);
                    HandleBuyerRegistered(buyerLogin);
                    break;
                case"3":
                    Start();
                    break;
                case "4":
                    Console.WriteLine("\nGoodbye");
                    break;
                default:
                    Console.WriteLine("\nInvalid choice, Please try again !");
                    HandleBuyerRegistered(buyerLogin);
                    break;
            }
        }
        private void HandleAdminConnected(string adminLogin)
        {
            _adminMenu.DisplayMenuAdminConnected();
            string adminChoice = Console.ReadLine();
            switch (adminChoice)
            {
                case "1":
                    article = _adminMenu.AdminAddArticle();
                    _adminService.AddArticle(new Article(article.referance, article.price), new Admin(adminLogin, adminLogin));
                    HandleAdminConnected(adminLogin);
                    break;
                case "2":
                    _adminService.GetArticles(new Admin(adminLogin, adminLogin));
                    HandleAdminConnected(adminLogin);
                    break;
                case "3":
                    _adminService.GetArticlesByBuyers(new Buyer(currentBuyerLogin, currentBuyerLogin, currentBuyerLogin, currentBuyerLogin), new Article(article.referance, article.price), new Admin(adminLogin, adminLogin));
                    HandleAdminConnected(adminLogin);
                    break;
                case "4":
                    break;
                case "5":
                    Start();
                    break;
                case "6":
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
                    break;
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
        private void AdminLogin()
        {
            bool loginSuccessful = false;
            if (!loginSuccessful)
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
                    HandleAdminMenu();
                }
            }
        }
        private void BuyerLogin()
        {
            bool loginSuccessful = false;
            if (!loginSuccessful)
            {
                var buyerCredentials = _buyerMenu.BuyerInfosConnecting();
                if (_buyerService.ValidateBuyerLog(buyerCredentials.firstname, buyerCredentials.lastname))
                {
                    currentBuyerLogin = buyerCredentials.firstname + " " + buyerCredentials.lastname;
                    HandleBuyerRegistered(buyerLogin: currentBuyerLogin);
                    loginSuccessful = true; 
                }
                else
                {
                    Console.WriteLine("Invalid firstname or lastname. Please try again !");
                    HandleBuyerMenu();
                }
            }
        }
        private void RegisterBuyerAccount()
        {
            var registerCredentials = _buyerMenu.AddRegister();
            if (registerCredentials.firstname != null && registerCredentials.lastname != null)
            {
                _buyerService.CreateBuyer(registerCredentials.firstname, registerCredentials.lastname, registerCredentials.adress, registerCredentials.phone);
                Start();
            }
        }
    }
}