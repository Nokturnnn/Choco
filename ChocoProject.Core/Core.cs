using ChocoInteraction;
using ChocoList;
using ChocoLog;
using ManagementPeople;

namespace ChocoProject.Core;

public interface ICore
{
    Task<string> LogAndConsoleAsync(string message);
    Task<bool> ClearAsync();
    Task<bool> InitializeDBAsync();
    Task<bool> Start();
    Task<bool> HandleBuyerMenuAsync();
    Task<bool> HandleBuyerRegisteredAsync(string buyerLogin);
    Task<bool> HandleAdminConnectedAsync(string adminLogin);
    Task<bool> HandleAdminMenuAsync();
    Task<bool> RegisterAdminAccountAsync();
    Task<bool> AdminLoginAsync();
    Task<bool> BuyerLoginAsync();
    
}
    public class Core : ICore
    {
        private readonly IAdmin _adminService;
        private readonly IBuyersService _buyerService;
        private readonly ILogger _logger;
        private readonly MAdministrator _adminMenu;
        private readonly MBuyer _buyerMenu;
        private readonly MStart _startMenu;
        private readonly ClearDB _clearDB;
        private string currentBuyerLogin;
        private (string reference, float price) article;
        private (string reference, int quantity) articleToBuy;
        private (DateTime startDate, DateTime endDate) date;
        private bool isCleared = false;
        public Core(IAdmin adminService, IBuyersService buyerService, ILogger logger) => (_adminService, _buyerService, _logger, _adminMenu, _buyerMenu, _startMenu, _clearDB) = (adminService, buyerService, logger, new MAdministrator(), new MBuyer(), new MStart(), new ClearDB(logger, new Interaction.FileService()));
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
        public async Task<bool> ClearAsync()
        {
            if (!isCleared) 
            {
                // Call the InitializeDB method =>
                await InitializeDBAsync();
                // Initialize the isCleared variable =>
                isCleared = true;
                // Call the DisplayFirstStart method =>
                await _startMenu.DisplayFirstStart();
                return true;
            }
            else
            {
                await LogAndConsoleAsync("\nDatabase already initialized");
                return false;
            }
        }
        public async Task<bool> InitializeDBAsync()
        {
            await LogAndConsoleAsync("\nDatabase initialized :\n");
            // Call the Initialization method =>
            await _clearDB.Initialization();
            return true;
        }
        public async Task<bool> Start()
        {
            // Call the DisplayMenuStart method =>
            await _startMenu.DisplayMenuStart();
            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    // Call the HandleBuyerMenu method =>
                    await HandleBuyerMenuAsync();
                    return true;
                case "2":
                    // Call the HandleAdminMenu method =>
                    await HandleAdminMenuAsync();
                    return true;
                case "E":
                    await LogAndConsoleAsync("\nGoodbye");
                    return false;
                case "cl":
                    Console.Clear();
                    await Start();
                    return true;
                case "FI":
                    // Call the Clear method =>
                    await ClearAsync();
                    await Start();
                    return true;
                default:
                    await LogAndConsoleAsync("\n----\nInvalid choice, Please try again !\n----\n");
                    // Call the Start method =>
                    await Start();
                    return true;
            }
        }
        public async Task<bool> HandleBuyerMenuAsync()
        {
            // Call the DisplayMenuBuyer method =>
            await _buyerMenu.DisplayMenuBuyer();
            string? buyerChoice = Console.ReadLine();
            switch (buyerChoice)
            {
                case "1":
                    // Call the BuyerLogin method =>
                    await BuyerLoginAsync();
                    return true;
                case "2":
                    // Call the RegisterBuyerAccount method =>
                    await RegisterBuyerAccountAsync();
                    return true;
                case "cl":
                    Console.Clear();
                    await HandleBuyerMenuAsync();
                    return true;
                case "B":
                    // Call the Start method =>
                    await Start();
                    return true;
                default:
                    await LogAndConsoleAsync("\n----\nInvalid choice, Please try again !\n----");
                    // Call the HandleBuyerMenu method =>
                    await HandleBuyerMenuAsync();
                    return false;
            }
        }
        public async Task<bool> HandleBuyerRegisteredAsync(string buyerLogin)
        {
            // Initialize the buying variable =>
            bool buying = true;
            // Initialize the newBuyer variable =>
            Buyer newBuyer = new Buyer(buyerLogin, buyerLogin, buyerLogin, buyerLogin);
            // While the buyer is buying =>
            while (buying)
            {
                // Call the DisplayBuyerRegistered method =>
                await _buyerMenu.DisplayBuyerRegistered();
                // Initialize the buyerChoice variable with the user input =>
                string? buyerChoice = Console.ReadLine();
                switch (buyerChoice)
                {
                    case "1":
                        // Call the DisplayListOfArticle method =>
                        await _buyerService.DisplayListOfArticle();
                        break;
                    case "2":
                        // Initialize the articleToBuy variable with the return of the AddToList method =>
                        articleToBuy = await _buyerMenu.AddToListAsync();
                        // Call the BuyerChooseAnArticleToList method with the article, newBuyer, articleToBuy.reference and articleToBuy.quantity =>
                        await _buyerService.BuyerChooseAnArticleToListAsync(new Article(article.reference, article.price), newBuyer, articleToBuy.reference, articleToBuy.quantity);
                        break;
                    case "cl":
                        // Clear the console =>
                        Console.Clear();
                        // Call the HandleBuyerRegistered method with the buyerLogin =>
                        await HandleBuyerRegisteredAsync(buyerLogin);
                        return true;
                    case"B":
                        // Call the Start method =>
                        await Start();
                        return true;
                    case "F":
                        // Call the FinalizeInvoice method with the newBuyer =>
                        await _buyerService.FinalizeInvoiceAsync(newBuyer);
                        // Purchase is finished =>
                        buying = false;
                        break;
                    case "P":
                        // Call the DisplayOrderInProgress method =>
                        await _buyerService.DisplayOrderInProgress(new Buyer(buyerLogin, buyerLogin, buyerLogin, buyerLogin));
                        // Call the HandleBuyerRegistered method with the buyerLogin =>
                        await HandleBuyerRegisteredAsync(buyerLogin);
                        return true;
                    default:
                        await LogAndConsoleAsync("\n----\nInvalid choice, Please try again !\n----\n");
                        // Call the HandleBuyerRegistered method with the buyerLogin =>
                        await HandleBuyerRegisteredAsync(buyerLogin);
                        return true;
                }
            }

            return true;
        }
        public async Task<bool> HandleAdminConnectedAsync(string adminLogin)
        {
            // Call the DisplayMenuAdminConnected method =>
            await _adminMenu.DisplayMenuAdminConnectedAsync();
            string? adminChoice = Console.ReadLine();
            switch (adminChoice)
            {
                case "1":
                    // Initialize the article variable with the return of the AdminAddArticle method =>
                    article = await _adminMenu.AdminAddArticleAsync();
                    // Call the AddArticle method with the article and adminLogin =>
                    await _adminService.AddArticleAsync(new Article(article.reference, article.price), new Admin(adminLogin, adminLogin));
                    // Call the HandleAdminConnected method with the adminLogin =>
                    await HandleAdminConnectedAsync(adminLogin);
                    return true;
                case "2":
                    // Initialize the article variable with the return of the AdminRemoveArticle method =>
                    var adminRemoveArticle = await _adminMenu.AdminRemoveArticle();
                    // Call the RemoveArticle method with the adminRemoveArticle and adminLogin =>
                    await _adminService.DeleteArticleAsync(new Admin(adminLogin, adminLogin), adminRemoveArticle);
                    await HandleAdminConnectedAsync(adminLogin);
                    return true;
                case "3":
                    // Call the GetArticles method with the adminLogin =>
                    await _adminService.GetArticlesAsync(new Admin(adminLogin, adminLogin));
                    // Call the HandleAdminConnected method with the adminLogin =>
                    await HandleAdminConnectedAsync(adminLogin);
                    return true;
                case "4":
                    // Call the GetArticlesByBuyers method with the adminLogin =>
                    await _adminService.GetArticlesByBuyersAsync(new Buyer(currentBuyerLogin, currentBuyerLogin, currentBuyerLogin, currentBuyerLogin), new Article(article.reference, article.price), new Admin(adminLogin, adminLogin));
                    // Call the HandleAdminConnected method with the adminLogin =>
                    await HandleAdminConnectedAsync(adminLogin);
                    return true;
                case "5":
                    // Initialize the date variable with the return of the AdminAddDate method =>
                    date = await _adminMenu.AdminAddDateAsync();
                    // Call the GenerateBillForBuyerByDate method with the date, article and adminLogin =>
                    await _adminService.GenerateBillForBuyerByDateAsync(date.startDate, date.endDate, new Article(article.reference, article.price), new Admin(adminLogin, adminLogin));
                    // Call the HandleAdminConnected method with the adminLogin =>
                    await HandleAdminConnectedAsync(adminLogin);
                    return true;
                case "cl":
                    Console.Clear();
                    await HandleAdminConnectedAsync(adminLogin);
                    return true;
                case "B":
                    // Call the Start method =>
                    await Start();
                    return true;
                case "E":
                    await LogAndConsoleAsync("\nGoodbye");
                    return false;
                default:
                    await LogAndConsoleAsync("\nInvalid choice, Please try again !");
                    // Call the HandleAdminConnected method with the adminLogin =>
                    await HandleAdminConnectedAsync(adminLogin); 
                    return true;
            }
        }
        public async Task<bool> HandleAdminMenuAsync()
        {
            // Call the DisplayMenuAdmin method =>
            await _adminMenu.DisplayMenuAdminAsync();
            // Initialize the adminChoice variable with the user input =>
            string? adminChoice = Console.ReadLine();
            // Check the user input =>
            switch (adminChoice)
            {
                case "1":
                    // Call the AdminLogin method =>
                    await AdminLoginAsync();
                    return true;
                case "2":
                    // Call the RegisterAdminAccount method =>
                    await RegisterAdminAccountAsync();
                    return true;
                case "cl":
                    Console.Clear();
                    await HandleAdminMenuAsync();
                    return true;
                case "B":
                    // Call the Start method =>
                    await Start();
                    return true;
                default:
                    await LogAndConsoleAsync("\n----\nInvalid choice, Please try again !\n----\n");
                    return false;
            }
        }
        public async Task<bool> RegisterAdminAccountAsync()
        {
            try
            {
                // Initialize the registerCredentials variable with the return of the RegisterAdmin method =>
                var registerCredentials = await _adminMenu.RegisterAdminAsync();
                // Check if the login and password are not null =>
                if (registerCredentials.login != null && registerCredentials.password != null)
                {
                    // Initialize the checkPassword variable with the return of the CheckPassword method =>
                    bool checkPassword = await _adminMenu.CheckPasswordAsync(registerCredentials.password);
                    // Check if the password is valid =>
                    while (!checkPassword)
                    {
                        // Display an error message if the password is not valid =>
                        registerCredentials = await _adminMenu.RegisterAdminAsync();
                        // Check if the password is valid =>
                        checkPassword = await _adminMenu.CheckPasswordAsync(registerCredentials.password);
                    }
                    // Check if the password is valid =>
                    if (checkPassword)
                    {
                        // Call the AddLoginToFile method with the login and password =>
                        await _adminService.AddLoginToFileAsync(registerCredentials.login, registerCredentials.password);
                        // Call the Start method =>
                        await Start();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                await LogAndConsoleAsync("Error : " + e.Message);
                return false;
            }
        }
        public async Task<bool> AdminLoginAsync()
        {
            try
            {
                // Initialize the adminCredentials variable with the return of the AdminConnecting method =>
                var adminCredentials = await _adminMenu.AdminConnectingAsync();
                // Check if the login and password are valid =>
                if (await _adminService.ValidateLoginAsync(adminCredentials.login, adminCredentials.password))
                {
                    // Call the HandleAdminConnected method with the adminLogin =>
                    await HandleAdminConnectedAsync(adminLogin: adminCredentials.login);
                }
                else
                {
                    // Display an error message if the login or password are not valid =>
                    await LogAndConsoleAsync("\n----\n Invalid login or password");
                    // Call the HandleAdminMenu method =>
                    await HandleAdminMenuAsync();
                }
                return true;
            }
            catch (Exception e)
            {
                await LogAndConsoleAsync("Error : " + e.Message + "\nYou probably have not registered yet. Please try again");
                await HandleAdminMenuAsync();
            }
            return false;
        }
        public async Task<bool> BuyerLoginAsync()
        {
            try
            {
                // Initialize the buyerCredentials variable with the return of the BuyerInfosConnecting method =>
                var buyerCredentials = await _buyerMenu.BuyerInfosConnectingAsync();
                // Check if the firstname and lastname are valid =>
                if (await _buyerService.ValidateBuyerLogAsync(buyerCredentials.firstname, buyerCredentials.lastname, buyerCredentials.adress, buyerCredentials.phone))
                {
                    // Initialize the currentBuyerLogin variable with the firstname and lastname =>
                    currentBuyerLogin = buyerCredentials.firstname + " " + buyerCredentials.lastname;
                    // Call the HandleBuyerRegistered method with the currentBuyerLogin =>
                    await HandleBuyerRegisteredAsync(buyerLogin: currentBuyerLogin);
                }
                else
                {
                    // Display an error message if the firstname or lastname are not valid =>
                    await LogAndConsoleAsync("\n----\n Invalid firstname or lastname");
                    // Call the HandleBuyerMenu method =>
                    await HandleBuyerMenuAsync();
                }
                return true;
            }
            catch (Exception e)
            {
                await LogAndConsoleAsync("Error : " + e.Message + "\nYou probably have not registered yet. Please try again");
                await HandleBuyerMenuAsync();
            }
            return false;
        }
        public async Task<bool> RegisterBuyerAccountAsync()
        {
            try
            {
                // Initialize the registerCredentials variable with the return of the RegisterBuyer method =>
                var registerCredentials = await _buyerMenu.AddRegisterAsync();
                // Check if the firstname and lastname are not null =>
                if (registerCredentials.firstname != null && registerCredentials.lastname != null)
                {
                    // Call the AddLoginToFile method with the firstname and lastname =>
                    await _buyerService.CreateBuyerAsync(registerCredentials.firstname, registerCredentials.lastname, registerCredentials.adress, registerCredentials.phone);
                    // Call the Start method =>
                    await Start();
                }
                return true;
            }
            catch (Exception e)
            {
                await LogAndConsoleAsync("Error : " + e.Message);
                return false;
            }
        }
        
    }
