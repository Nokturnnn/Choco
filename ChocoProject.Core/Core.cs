using ChocoInteraction;
using ChocoList;
using ChocoLog;
using ManagementPeople;

namespace ChocoProject.Core;

public interface ICore
{
    bool Clear();
    bool Start();
    void InitializeDB();
    bool HandleBuyerMenu();
    bool HandleBuyerRegistered(string buyerLogin);
    bool HandleAdminConnected(string adminLogin);
    bool HandleAdminMenu();
    bool RegisterAdminAccount();
    bool AdminLogin();
    bool BuyerLogin();
    bool RegisterBuyerAccount();
    string LogAndConsole(string message);
}

    public class Core : ICore
    {
        private readonly IAdmin _adminService;
        private readonly IBuyersService _buyerService;
        private readonly ILogger _logger;
        private MAdministrator _adminMenu;
        private MBuyer _buyerMenu;
        private MStart _startMenu;
        private ClearDB _clearDB;
        private string currentBuyerLogin;
        private (string reference, float price) article;
        private (string reference, int quantity) articleToBuy;
        private (DateTime startDate, DateTime endDate) date;
        private bool isCleared = false;
        public Core(IAdmin adminService, IBuyersService buyerService, ILogger logger) => (_adminService, _buyerService, _logger, _adminMenu, _buyerMenu, _startMenu, _clearDB) = (adminService, buyerService, logger, new MAdministrator(), new MBuyer(), new MStart(), new ClearDB(logger, new Interaction.FileService()));
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
        public bool Clear()
        {
            if (!isCleared) 
            {
                // Call the InitializeDB method =>
                InitializeDB();
                // Initialize the isCleared variable =>
                isCleared = true;
                // Call the DisplayFirstStart method =>
                _startMenu.DisplayFirstStart();
                return true;
            }
            else
            {
                LogAndConsole("\nDatabase already initialized");
                return false;
            }
        }
        public void InitializeDB()
        {
            LogAndConsole("\nDatabase initialized :\n");
            // Call the Initialization method =>
            _clearDB.Initialization();
        }
        public bool Start()
        {
            // Call the DisplayMenuStart method =>
            _startMenu.DisplayMenuStart();
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    // Call the HandleBuyerMenu method =>
                    HandleBuyerMenu();
                    return true;
                case "2":
                    // Call the HandleAdminMenu method =>
                    HandleAdminMenu();
                    return true;
                case "E":
                    LogAndConsole("\nGoodbye");
                    return false;
                case "cl":
                    Console.Clear();
                    Start();
                    return true;
                case "FI":
                    // Call the Clear method =>
                    Clear();
                    Start();
                    return true;
                default:
                    LogAndConsole("\n----\nInvalid choice, Please try again !\n----\n");
                    // Call the Start method =>
                    Start();
                    return true;
            }
        }
        public bool HandleBuyerMenu()
        {
            // Call the DisplayMenuBuyer method =>
            _buyerMenu.DisplayMenuBuyer();
            string buyerChoice = Console.ReadLine();
            switch (buyerChoice)
            {
                case "1":
                    // Call the BuyerLogin method =>
                    BuyerLogin();
                    return true;
                case "2":
                    // Call the RegisterBuyerAccount method =>
                    RegisterBuyerAccount();
                    return true;
                case "cl":
                    Console.Clear();
                    HandleBuyerMenu();
                    return true;
                case "B":
                    // Call the Start method =>
                    Start();
                    return true;
                default:
                    LogAndConsole("\n----\nInvalid choice, Please try again !\n----");
                    // Call the HandleBuyerMenu method =>
                    HandleBuyerMenu();
                    return false;
            }
        }
        public bool HandleBuyerRegistered(string buyerLogin)
        {
            // Initialize the buying variable =>
            bool buying = true;
            // Initialize the newBuyer variable =>
            Buyer newBuyer = new Buyer(buyerLogin, buyerLogin, buyerLogin, buyerLogin);
            // While the buyer is buying =>
            while (buying)
            {
                // Call the DisplayBuyerRegistered method =>
                _buyerMenu.DisplayBuyerRegistered();
                // Initialize the buyerChoice variable with the user input =>
                string buyerChoice = Console.ReadLine();
                switch (buyerChoice)
                {
                    case "1":
                        // Call the DisplayListOfArticle method =>
                        _buyerService.DisplayListOfArticle();
                        break;
                    case "2":
                        // Initialize the articleToBuy variable with the return of the AddToList method =>
                        articleToBuy = _buyerMenu.AddToList();
                        // Call the BuyerChooseAnArticleToList method with the article, newBuyer, articleToBuy.reference and articleToBuy.quantity =>
                        _buyerService.BuyerChooseAnArticleToList(new Article(article.reference, article.price), newBuyer, articleToBuy.reference, articleToBuy.quantity);
                        break;
                    case "cl":
                        // Clear the console =>
                        Console.Clear();
                        // Call the HandleBuyerRegistered method with the buyerLogin =>
                        HandleBuyerRegistered(buyerLogin);
                        return true;
                    case"B":
                        // Call the Start method =>
                        Start();
                        return true;
                    case "F":
                        // Call the FinalizeInvoice method with the newBuyer =>
                        _buyerService.FinalizeInvoice(newBuyer);
                        // Purchase is finished =>
                        buying = false;
                        break;
                    case "P":
                        // Call the DisplayOrderInProgress method =>
                        _buyerService.DisplayOrderInProgress(new Buyer(buyerLogin, buyerLogin, buyerLogin, buyerLogin));
                        // Call the HandleBuyerRegistered method with the buyerLogin =>
                        HandleBuyerRegistered(buyerLogin);
                        return true;
                    default:
                        LogAndConsole("\n----\nInvalid choice, Please try again !\n----\n");
                        // Call the HandleBuyerRegistered method with the buyerLogin =>
                        HandleBuyerRegistered(buyerLogin);
                        return true;
                }
            }

            return true;
        }
        public bool HandleAdminConnected(string adminLogin)
        {
            // Call the DisplayMenuAdminConnected method =>
            _adminMenu.DisplayMenuAdminConnected();
            string adminChoice = Console.ReadLine();
            switch (adminChoice)
            {
                case "1":
                    // Initialize the article variable with the return of the AdminAddArticle method =>
                    article = _adminMenu.AdminAddArticle();
                    // Call the AddArticle method with the article and adminLogin =>
                    _adminService.AddArticle(new Article(article.reference, article.price), new Admin(adminLogin, adminLogin));
                    // Call the HandleAdminConnected method with the adminLogin =>
                    HandleAdminConnected(adminLogin);
                    return true;
                case "2":
                    // Initialize the article variable with the return of the AdminRemoveArticle method =>
                    var adminRemoveArticle = _adminMenu.AdminRemoveArticle();
                    // Call the RemoveArticle method with the adminRemoveArticle and adminLogin =>
                    _adminService.DeleteArticle(new Admin(adminLogin, adminLogin), adminRemoveArticle);
                    HandleAdminConnected(adminLogin);
                    return true;
                case "3":
                    // Call the GetArticles method with the adminLogin =>
                    _adminService.GetArticles(new Admin(adminLogin, adminLogin));
                    // Call the HandleAdminConnected method with the adminLogin =>
                    HandleAdminConnected(adminLogin);
                    return true;
                case "4":
                    // Call the GetArticlesByBuyers method with the adminLogin =>
                    _adminService.GetArticlesByBuyers(new Buyer(currentBuyerLogin, currentBuyerLogin, currentBuyerLogin, currentBuyerLogin), new Article(article.reference, article.price), new Admin(adminLogin, adminLogin));
                    // Call the HandleAdminConnected method with the adminLogin =>
                    HandleAdminConnected(adminLogin);
                    return true;
                case "5":
                    // Initialize the date variable with the return of the AdminAddDate method =>
                    date = _adminMenu.AdminAddDate();
                    // Call the GenerateBillForBuyerByDate method with the date, article and adminLogin =>
                    _adminService.GenerateBillForBuyerByDate(date.startDate, date.endDate, new Article(article.reference, article.price), new Admin(adminLogin, adminLogin));
                    // Call the HandleAdminConnected method with the adminLogin =>
                    HandleAdminConnected(adminLogin);
                    return true;
                case "cl":
                    Console.Clear();
                    HandleAdminConnected(adminLogin);
                    return true;
                case "B":
                    // Call the Start method =>
                    Start();
                    return true;
                case "E":
                    LogAndConsole("\nGoodbye");
                    return false;
                default:
                    LogAndConsole("\nInvalid choice, Please try again !");
                    // Call the HandleAdminConnected method with the adminLogin =>
                    HandleAdminConnected(adminLogin); 
                    return true;
            }
        }
        public bool HandleAdminMenu()
        {
            // Call the DisplayMenuAdmin method =>
            _adminMenu.DisplayMenuAdmin();
            // Initialize the adminChoice variable with the user input =>
            string adminChoice = Console.ReadLine();
            // Check the user input =>
            switch (adminChoice)
            {
                case "1":
                    // Call the AdminLogin method =>
                    AdminLogin();
                    return true;
                case "2":
                    // Call the RegisterAdminAccount method =>
                    RegisterAdminAccount();
                    return true;
                case "cl":
                    Console.Clear();
                    HandleAdminMenu();
                    return true;
                case "B":
                    // Call the Start method =>
                    Start();
                    return true;
                default:
                    LogAndConsole("\n----\nInvalid choice, Please try again !\n----\n");
                    return false;
            }
        }
        public bool RegisterAdminAccount()
        {
            try
            {
                // Initialize the registerCredentials variable with the return of the RegisterAdmin method =>
                var registerCredentials = _adminMenu.RegisterAdmin();
                // Check if the login and password are not null =>
                if (registerCredentials.login != null && registerCredentials.password != null)
                {
                    // Initialize the checkPassword variable with the return of the CheckPassword method =>
                    bool checkPassword = _adminMenu.CheckPassword(registerCredentials.password);
                    // Check if the password is valid =>
                    while (!checkPassword)
                    {
                        // Display an error message if the password is not valid =>
                        registerCredentials = _adminMenu.RegisterAdmin();
                        // Check if the password is valid =>
                        checkPassword = _adminMenu.CheckPassword(registerCredentials.password);
                    }
                    // Check if the password is valid =>
                    if (checkPassword)
                    {
                        // Call the AddLoginToFile method with the login and password =>
                        _adminService.AddLoginToFile(registerCredentials.login, registerCredentials.password);
                        // Call the Start method =>
                        Start();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                LogAndConsole("Error : " + e.Message);
                return false;
            }
        }
        public bool AdminLogin()
        {
            try
            {
                // Initialize the adminCredentials variable with the return of the AdminConnecting method =>
                var adminCredentials = _adminMenu.AdminConnecting();
                // Check if the login and password are valid =>
                if (_adminService.ValidateLogin(adminCredentials.login, adminCredentials.password))
                {
                    // Call the HandleAdminConnected method with the adminLogin =>
                    HandleAdminConnected(adminLogin: adminCredentials.login);
                }
                else
                {
                    // Display an error message if the login or password are not valid =>
                    LogAndConsole("\n----\n Invalid login or password");
                    // Call the HandleAdminMenu method =>
                    HandleAdminMenu();
                }
                return true;
            }
            catch (Exception e)
            {
                LogAndConsole("Error : " + e.Message + "\nYou probably have not registered yet. Please try again");
                HandleAdminMenu();
            }
            return false;
        }
        public bool BuyerLogin()
        {
            try
            {
                // Initialize the buyerCredentials variable with the return of the BuyerInfosConnecting method =>
                var buyerCredentials = _buyerMenu.BuyerInfosConnecting();
                // Check if the firstname and lastname are valid =>
                if (_buyerService.ValidateBuyerLog(buyerCredentials.firstname, buyerCredentials.lastname, buyerCredentials.adress, buyerCredentials.phone))
                {
                    // Initialize the currentBuyerLogin variable with the firstname and lastname =>
                    currentBuyerLogin = buyerCredentials.firstname + " " + buyerCredentials.lastname;
                    // Call the HandleBuyerRegistered method with the currentBuyerLogin =>
                    HandleBuyerRegistered(buyerLogin: currentBuyerLogin);
                }
                else
                {
                    // Display an error message if the firstname or lastname are not valid =>
                    LogAndConsole("\n----\n Invalid firstname or lastname");
                    // Call the HandleBuyerMenu method =>
                    HandleBuyerMenu();
                }
                return true;
            }
            catch (Exception e)
            {
                LogAndConsole("Error : " + e.Message + "\nYou probably have not registered yet. Please try again");
                HandleBuyerMenu();
            }
            return false;
        }
        public bool RegisterBuyerAccount()
        {
            try
            {
                // Initialize the registerCredentials variable with the return of the RegisterBuyer method =>
                var registerCredentials = _buyerMenu.AddRegister();
                // Check if the firstname and lastname are not null =>
                if (registerCredentials.firstname != null && registerCredentials.lastname != null)
                {
                    // Call the AddLoginToFile method with the firstname and lastname =>
                    _buyerService.CreateBuyer(registerCredentials.firstname, registerCredentials.lastname, registerCredentials.adress, registerCredentials.phone);
                    // Call the Start method =>
                    Start();
                }
                return true;
            }
            catch (Exception e)
            {
                LogAndConsole("Error : " + e.Message);
                return false;
            }
        }
        
    }
