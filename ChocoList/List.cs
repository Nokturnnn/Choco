using System.Diagnostics;
using System.Text.Json;
using ChocoLog;
using ChocoInteraction;
using ManagementPeople;

namespace ChocoList;

public interface IAdmin
{
    bool ValidateLogin(string login, string password);
    bool AddArticle(Article article, Admin admin);
    bool AddLoginToFile(string login, string password);
    bool GetArticles(Admin admin);
    bool DeleteArticle(Admin admin, string reference);
    bool GetArticlesByBuyers(Buyer buyer, Article article, Admin admin);
    string LogAndConsole(string message);
    bool GenerateBillForBuyerByDate(DateTime startDate, DateTime endDate, Article article, Admin admin);
}
public class AdminService : IAdmin
{
    private ILogger _logger;
    private Interaction.IFileRead _fileRead      = new Interaction.FileService();
    private Interaction.IFileWrite _fileWrite    = new Interaction.FileService();
    private Interaction.IFileAppend _fileAppend  = new Interaction.FileService();
    private Interaction.IFileExists _fileExists  = new Interaction.FileService();
    private readonly string _pathAdminJson       = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/admin.json";
    private readonly string _pathArticleJson     = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/article.json";
    private readonly string _pathItemPurchased   =
        "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/itemPurchased.json";
    public AdminService(FileLogger logger, Interaction.IFileRead fileRead, Interaction.IFileWrite fileWrite, Interaction.IFileAppend fileAppend, Interaction.IFileExists fileExists) => (_logger, _fileRead, _fileWrite, _fileAppend, _fileExists) = (logger, fileRead, fileWrite, fileAppend, fileExists);
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
    public bool AddLoginToFile(string login, string password)
    {
        try
        {
            // Verify if the file exists =>
            if (!_fileExists.FileExists(_pathAdminJson)) _fileWrite.WriteFile(_pathAdminJson, "");
            // Create a new admin =>
            Admin admin = new Admin(login, password);
            // Read the current content of the file =>
            string? jsonFile = _fileRead.ReadFile(_pathAdminJson);
            // Deserialize the JSON file into a list of Admin objects =>
            var admins = string.IsNullOrWhiteSpace(jsonFile) ? new List<Admin>() : JsonSerializer.Deserialize<List<Admin>>(jsonFile);
            // Add the new admin to the list =>
            admins.Add(admin);
            // Serialize the list of admins into a JSON string =>
            _fileWrite.WriteFile(_pathAdminJson, JsonSerializer.Serialize(admins));
            // Display the message =>
            LogAndConsole($"----\nAdmin added = Login : {login} and Password : {password}");
            // Return true if the admin was added, false otherwise =>
            return true;
        }
        catch (Exception e)
        {
            // Console.WriteLine("Error : " + e.Message);
            LogAndConsole("Error when adding an item : " + e.Message);
            return false;
        }
    }
    public bool ValidateLogin(string login, string password)
    {
        try
        {
            // Read the current content of the file =>
            string jsonFile = _fileRead.ReadFile(_pathAdminJson);
            // Check if the file is empty =>
            if (string.IsNullOrEmpty(jsonFile)) { LogAndConsole("The Json file is empty or missing"); return false; }
            // Deserialize the JSON file into a list of Admin objects =>
            var admins = JsonSerializer.Deserialize<List<Admin>>(jsonFile);
            // Find the admin with matching login and password =>
            var matchedAdmin = admins.FirstOrDefault(admin => admin.Login == login && admin.Password == password);
            // Return true if the admin was found, false otherwise =>
            if (matchedAdmin != null) { LogAndConsole($"----\nLogin success\n\"Admin connected = Login : {login} and Password : {password}\"\n----"); return true; }
            LogAndConsole("Your login or password is incorrect");
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return false;
        }
    }
    public bool AddArticle(Article article, Admin admin)
    {
        try
        {
            // Verify if the file exists =>
            if (!_fileExists.FileExists(_pathArticleJson)) _fileWrite.WriteFile(_pathArticleJson, "");
            // Create a new article =>
            var newArticle = new Article(article.Reference, article.Price);
            // Read the current content of the file =>
            var jsonFile = _fileRead.ReadFile(_pathArticleJson);
            // Deserialize the JSON file into a list of Article objects =>
            var articles = string.IsNullOrWhiteSpace(jsonFile) ? new List<Article>() : JsonSerializer.Deserialize<List<Article>>(jsonFile);
            // Add the new article to the list =>
            articles.Add(newArticle);
            // Serialize the list of articles into a JSON string =>
            _fileWrite.WriteFile(_pathArticleJson, JsonSerializer.Serialize(articles));
            // Display the message =>
            LogAndConsole($"---->\n- {admin.Login} add an article ==> \n- Reference = {article.Reference} \n- Price = {article.Price}\n----");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return false;
        }
    }
    public bool DeleteArticle(Admin admin, string reference)
    {
        try
        {
            if(!_fileExists.FileExists(_pathArticleJson)) _fileWrite.WriteFile(_pathArticleJson, "");
            var jsonFile = _fileRead.ReadFile(_pathArticleJson);
            var articles = string.IsNullOrWhiteSpace(jsonFile) ? new List<Article>() : JsonSerializer.Deserialize<List<Article>>(jsonFile);
            var selectedArticle = articles.FirstOrDefault(article => article.Reference == reference);
            articles.Remove(selectedArticle);
            _fileWrite.WriteFile(_pathArticleJson, JsonSerializer.Serialize(articles));
            LogAndConsole($"----\nArticle deleted by '{admin.Login}':\n- Reference: {selectedArticle.Reference}\n----");
            return true;
        }
        catch (Exception e)
        {
            LogAndConsole("----\nError when deleting an article : " + e.Message + "\n----");
            return false;
        }
    }
    public bool GetArticles(Admin admin)
    {
        try
        {
            // Create the file =>
            string sommeOfArticlesByAdmin = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/{admin.Login}-SumOfArticles.txt";
            // Read the current content of the file =>
            string jsonFile = _fileRead.ReadFile(_pathItemPurchased);
            // Deserialize the JSON file into a list of ItemPurchased objects =>
            List<ItemPurchased> itemPurchaseds = string.IsNullOrWhiteSpace(jsonFile) ? new List<ItemPurchased>() : JsonSerializer.Deserialize<List<ItemPurchased>>(jsonFile);
            // Check that the item list is not null before searching for the > reference =>
            if (itemPurchaseds.Count == 0) { LogAndConsole("You don't have any articles"); return false; }
            LogAndConsole($"{admin.Login} adds a bill for purchased articles\n");
            // Initialize the total price, total and billcount to '0' =>
            float total = 0, totalGlobal = 0;
            // Search for the corresponding article =>
            foreach (var items in itemPurchaseds)
            {
                // Find the corresponding article by reference (or other unique property) =>
                Article purchasedArticle = GetArticleByReference(items.ArticleReference);
                // Calculate the total price =>
                total += items.Quantity * purchasedArticle.Price;
                // Write the details of the article to the invoice =>
                _fileAppend.AppendFile(sommeOfArticlesByAdmin, $"---->\n- Reference = {purchasedArticle.Reference} \n- Quantity: {items.Quantity}\n- Price = {purchasedArticle.Price}\n----");
                LogAndConsole($"---->\n- Reference = {purchasedArticle.Reference} \n- Quantity: {items.Quantity}\n- Price = {purchasedArticle.Price}\n----");
            }
            // Calculate the total price =>
            totalGlobal += total;
            // Write the total price to the invoice =>
            _fileAppend.AppendFile(sommeOfArticlesByAdmin, $"\n- Total price: {totalGlobal}\n");
            LogAndConsole($"Total price: {totalGlobal}\n");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return false;
        }
    }
    private Article GetArticleByReference(string reference)
    {
        try
        {
            // Read the current content of the file =>
            string? jsonFile = _fileRead.ReadFile(_pathArticleJson);
            // Deserialize the JSON file into a list of Article objects =>
            List<Article>? articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);
            // Check that the article list is not null before searching for the > reference =>
            if (articles != null)
                return articles.FirstOrDefault(article => article.Reference == reference);
            // If the article was not found or there was an error, return null or an appropriate default value =>
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return null;
        }
    }
    public bool GetArticlesByBuyers(Buyer buyer, Article article, Admin admin)
    {
        try
        { 
            // Initialize the total price, total and billcount to '0' =>
            float total = 0, totalGlobal = 0;
            int billCount = 1;
            // Create the file =>
            string sumArticleSoldByBuyers = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/{admin.Login}-SumOfArticlesSoldByBuyers.txt";
            // Read the current content of the file =>
            string jsonFile = _fileRead.ReadFile(_pathItemPurchased);
            // Deserialize the JSON file into a list of ItemPurchased objects =>
            List<ItemPurchased> itemPurchaseds = string.IsNullOrWhiteSpace(jsonFile) ? new List<ItemPurchased>() : JsonSerializer.Deserialize<List<ItemPurchased>>(jsonFile);
            // Check that the item list is not null before searching for the > reference =>
            if (itemPurchaseds.Count == 0) { return false; }
            LogAndConsole($"{admin.Login} is generating a bill for articles bought by {buyer.Firstname}:\n");
            // Search for the corresponding article =>
            foreach (var items in itemPurchaseds)
            {
                // Find the corresponding article by reference (or other unique property) =>
                Article purchasedArticle = GetArticleByReference(items.ArticleReference);
                // Check that the article was found =>
                if (purchasedArticle != null)
                {
                    // Calculate the total price =>
                    total += items.Quantity * purchasedArticle.Price;
                    // Write the details of the article to the invoice =>
                    _fileAppend.AppendFile(sumArticleSoldByBuyers, $"----> Bill {billCount}\n- Buyer : {buyer.Firstname}\n- Reference : {items.ArticleReference}\n- Quantity : {items.Quantity}\n- Price : {purchasedArticle.Price}\n Total purchases : {items.Quantity * purchasedArticle.Price} \n- Date of buy : {items.DateofBuy}\n----\n");
                    LogAndConsole($"\n----> Bill {billCount}\n- Buyer : {buyer.Firstname}\n- Reference : {items.ArticleReference}\n- Quantity : {items.Quantity}\n- Price : {purchasedArticle.Price}\n- Total purchases : {items.Quantity * purchasedArticle.Price} \n- Date of buy : {items.DateofBuy}\n----\n");
                    billCount++;
                }
            }
            // Calculate the total price =>
            totalGlobal += total;
            // Write the total price to the invoice =>
            _fileAppend.AppendFile(sumArticleSoldByBuyers, $"\n- Total global : {totalGlobal}\n");
            LogAndConsole($"Total global : {totalGlobal}\n");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return false;
        }
    }
    public bool GenerateBillForBuyerByDate(DateTime startDate, DateTime endDate, Article article, Admin admin)
    {
        try
        {
            // Initialize the total price, total and billcount to '0' =>
            float total = 0, totalGlobal = 0;
            int billCount = 1;
            // Create the file =>
            string testBillByBuyersDateOfBuy = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/{admin.Login}-SumOfArticlesSoldByBuyersByDate.txt";
            // Read the current content of the file =>
            string jsonFile = _fileRead.ReadFile(_pathItemPurchased);
            // Deserialize the JSON file into a list of ItemPurchased objects =>
            List<ItemPurchased> itemPurchaseds = string.IsNullOrWhiteSpace(jsonFile) ? new List<ItemPurchased>() : JsonSerializer.Deserialize<List<ItemPurchased>>(jsonFile);
            // Check that the item list is not null before searching for the > reference =>
            if (itemPurchaseds.Count == 0) { return false; }
            LogAndConsole($"{admin.Login} is generating a bill for articles bought between {startDate.ToShortDateString()} and {endDate.ToShortDateString()}:\n");
            // Search for the corresponding article =>
            foreach (var items in itemPurchaseds)
            {
                // Find the corresponding article by reference (or other unique property) =>
                if (items.DateofBuy.Date >= startDate.Date && items.DateofBuy.Date <= endDate.Date)
                {
                    // Check that the article was found =>
                    Article purchasedArticle = GetArticleByReference(items.ArticleReference);
                    // Calculate the total price =>
                    if (purchasedArticle != null)
                    {
                        // Calculate the total price =>
                        total += items.Quantity * purchasedArticle.Price;
                        // Write the details of the article to the invoice =>
                        _fileAppend.AppendFile(testBillByBuyersDateOfBuy, $"\n----> Bill {billCount}\n- Reference : {items.ArticleReference}\n- Quantity : {items.Quantity}\n- Price : {purchasedArticle.Price}\n Total purchases : {items.Quantity * purchasedArticle.Price} \n- Date of buy : {items.DateofBuy}\n----\n");
                        LogAndConsole($"\n----> Bill {billCount}\n- Reference : {items.ArticleReference}\n- Quantity : {items.Quantity}\n- Price : {purchasedArticle.Price}\n- Total purchases : {items.Quantity * purchasedArticle.Price} \n- Date of buy : {items.DateofBuy}\n----\n");
                        billCount++;
                    }
                }
            }
            // Calculate the total price =>
            totalGlobal += total;
            // Write the total price to the invoice =>
            _fileAppend.AppendFile(testBillByBuyersDateOfBuy, $"\n- Total global : {totalGlobal}\n");
            LogAndConsole($"Total global : {totalGlobal}\n");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return false;
        }
    }
}

// PART OF BUYER
public interface IBuyersService
{
    bool CreateBuyer(string firstname, string lastname, string adress, string phone);
    bool BuyerChooseAnArticleToList(Article article, Buyer buyer, string reference, int quantity);
    bool ValidateBuyerLog(string firstname, string lastname);
    bool DisplayListOfArticle();
    bool AddItemPurchases(ItemPurchased itemPurchased);
    bool DisplayOrderInProgress();
}
public class BuyerService : IBuyersService
{
    private ILogger _logger = new FileLogger();
    private Interaction.IFileRead _fileRead         = new Interaction.FileService();
    private Interaction.IFileWrite _fileWrite       = new Interaction.FileService();
    private Interaction.IFileExists _fileExists     = new Interaction.FileService();
    private Interaction.IFileAppend _fileAppend     = new Interaction.FileService();
    private readonly string _pathBuyerJson          = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/buyer.json";
    private readonly string _pathArticleJson        = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/article.json";
    private readonly string _pathItemPurchasedJson  = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/itemPurchased.json";
    public bool LogAndConsole(string message)
    {
        try
        {
            // Display the message =>
            Console.WriteLine(message);
            // Call the Log method of the injected logger =>
            _logger.Log(message);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return false;
        }
    }
    public bool CreateBuyer(string firstname, string lastname, string adress, string phone)
    {
        try
        {
            // Verify if the file exists =>
            if (!_fileExists.FileExists(_pathBuyerJson)) _fileWrite.WriteFile(_pathBuyerJson, "");
            // Create a new buyer =>
            var newBuyer = new Buyer(firstname, lastname, adress, phone);
            // Read the current content of the file =>
            string jsonFile = _fileRead.ReadFile(_pathBuyerJson);
            // Deserialize the JSON file into a list of Buyer objects =>
            var buyers = string.IsNullOrWhiteSpace(jsonFile) ? new List<Buyer>() : JsonSerializer.Deserialize<List<Buyer>>(jsonFile);
            // Add the new buyer to the list =>
            buyers.Add(newBuyer);
            // Serialize the list of buyers into a JSON string =>
            _fileWrite.WriteFile(_pathBuyerJson, JsonSerializer.Serialize(buyers));
            // Display the message =>
            LogAndConsole($" ----\nBuyer added\n- Firstname: {newBuyer.Firstname}\n- Lastname: {newBuyer.Lastname}\n- Address: {newBuyer.Adress}\n- Phone: {newBuyer.Phone}\n----");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return false;
        }
    }
    public bool ValidateBuyerLog(string firstname, string lastname)
    {
        try
        {
            // Read the current content of the file =>
            string jsonFile = _fileRead.ReadFile(_pathBuyerJson);
            // Deserialize the JSON file into a list of Buyer objects =>
            if (string.IsNullOrEmpty(jsonFile)) { LogAndConsole("The Json file is empty or missing"); return false; }
            // Create a var to store the list of buyers =>
            var buyers = JsonSerializer.Deserialize<List<Buyer>>(jsonFile);
            // Find the buyer with matching firstname and lastname =>
            var matchedBuyer = buyers.FirstOrDefault(buyer => buyer.Firstname == firstname && buyer.Lastname == lastname);
            // Return true if the buyer was found, false otherwise =>
            if (matchedBuyer != null) { LogAndConsole($"----\n\"Buyer registered =\nFirstname : {matchedBuyer.Firstname}\nLastname : {matchedBuyer.Lastname}\"\n----"); return true; }
            // Display the message =>
            LogAndConsole("You don't have a buyer account");
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return false;
        }
    }
    public bool BuyerChooseAnArticleToList(Article newArticle, Buyer newBuyer, string reference, int quantity)
    {
        try
        {
            // Read the current content of the file =>
            string jsonFile = _fileRead.ReadFile(_pathArticleJson);
            // Deserialize the JSON file into a list of Article objects =>
            var articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);
            // Check that the article list is not null before searching for the > reference =>
            if (articles == null) return false;
            // Find the corresponding article by reference (or other unique property) =>
            var selectedArticle = articles.FirstOrDefault(article => article.Reference == reference);
            // Check that the article was found =>
            if (selectedArticle == null)
            {
                LogAndConsole($"Article with reference '{reference}' not found");
                return false;
            }
            // Calculate the total price =>
            float totalPrice = selectedArticle.Price * quantity;
            // Create the file =>
            string path = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/{newBuyer.Firstname}-{DateTime.Now:dd-MM-yyyy-HH-mm}.txt";
            // Write the details of the article to the invoice =>
            _fileWrite.WriteFile(path, $" ----\nList of articles purchased by '{newBuyer.Firstname}' :\n----\nReference : {selectedArticle.Reference}\nPrice : {selectedArticle.Price}\nQuantity : {quantity}\nTotal price : {totalPrice}\n----\n");
            // Display the message =>
            float priceAtPurchase = CalculatePriceAtPurchase(reference, quantity);
            // Display the message =>
            LogAndConsole($"---->\n- Reference : {reference}\n- Quantity : {quantity}\n- Price at purchase : {priceAtPurchase}\n- Date of buy : {DateTime.Now}\n----");
            // Add the item purchased to the list =>
            AddItemPurchases(new ItemPurchased(selectedArticle.Reference, quantity, priceAtPurchase, DateTime.Now));
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return false;
        }
    }
    public float CalculatePriceAtPurchase(string reference, int quantity)
    {
        try
        {
            // Read the current content of the file =>
            string jsonFile = _fileRead.ReadFile(_pathArticleJson);
            // Deserialize the JSON file into a list of Article objects =>
            var articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);
            // Check that the article list is not null before searching for the > reference =>
            if (articles == null) return 0.0f;
            // Find the corresponding article by reference (or other unique property) =>
            var purchasedArticle = articles.FirstOrDefault(article => article.Reference == reference);
            // Check that the article was found =>
            if (purchasedArticle != null) return purchasedArticle.Price * quantity;
            return 0.0f;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return 0.0f;
        }
    }
    public bool AddItemPurchases(ItemPurchased itemPurchased)
    {
        try
        {
            // Verify if the file exists =>
            if (!_fileExists.FileExists(_pathItemPurchasedJson)) _fileWrite.WriteFile(_pathItemPurchasedJson, "");
            // Read the current content of the file =>
            string jsonFile = _fileRead.ReadFile(_pathItemPurchasedJson);
            // Deserialize the JSON file into a list of ItemPurchased objects =>
            var itemsPurchased = string.IsNullOrWhiteSpace(jsonFile) ? new List<ItemPurchased>() : JsonSerializer.Deserialize<List<ItemPurchased>>(jsonFile);
            // Add the new item purchased to the list / if the list is not null she does nothing =>
            itemsPurchased ??= new List<ItemPurchased>();
            // Add the new item purchased to the list =>
            itemsPurchased.Add(itemPurchased);
            // Serialize the list of items purchased into a JSON string =>
            _fileWrite.WriteFile(_pathItemPurchasedJson, JsonSerializer.Serialize(itemsPurchased));
            // Display the message =>
            LogAndConsole($"----\nItem purchased added\n----");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return false;
        }
    }
    public bool DisplayListOfArticle()
    {
        try
        {
            // Read the current content of the file =>
            string jsonFile = _fileRead.ReadFile(_pathArticleJson);
            // Deserialize the JSON file into a list of Article objects =>
            var articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);
            // Check that the article list is not null before searching for the > reference =>
            if (articles == null) return false;
            // Search for the corresponding article =>
            foreach (var article in articles)
                LogAndConsole($"---->\nArticles N°{article.ID}\n- Reference: {article.Reference}\n- Price: {article.Price}\n----");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return false;
        }
    }
    public bool DisplayOrderInProgress()
    {
        try
        {
            // Read the current content of the file =>
            string jsonFile = _fileRead.ReadFile(_pathItemPurchasedJson);
            // Deserialize the JSON file into a list of ItemPurchased objects =>
            var itemsPurchased = JsonSerializer.Deserialize<List<ItemPurchased>>(jsonFile);
            // Check that the item list is not null before searching for the > reference =>
            if (itemsPurchased == null || itemsPurchased.Count == 0) return false;
            // Find the last item purchased =>
            var lastItem = itemsPurchased.Last();
            // Display the message =>
            LogAndConsole($"----\nLast item purchased:\n- Reference: {lastItem.ArticleReference}\n- Quantity: {lastItem.Quantity}\n- Price: {lastItem.PriceAtPurchase}\n----");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return false;
        }
    }
}