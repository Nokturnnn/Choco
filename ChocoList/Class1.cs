using System.Diagnostics;
using System.Text.Json;
using ChocoLog;
using ChocoInteraction;
using ManagementPeople;

namespace ChocoList;

public interface IAdmin
{
    bool ValidateLogin(string login, string password);
    void AddArticle(Article article, Admin admin);
    void AddLoginToFile(string login2, string password2);
    void GetArticles(Admin admin);
    void GetArticlesByBuyers(Buyer buyer, Article article, Admin admin);
    void LogAndConsole(string message);
    void GenerateBillForBuyerByDate(DateTime startDate, DateTime endDate, Article article, Admin admin);
}
public class AdminService : IAdmin
{
    private List<ItemPurchased> _itemsPurchased = new();
    private ILogger _logger;
    private Interaction.IFileRead _fileRead = new Interaction.FileService();
    private Interaction.IFileWrite _fileWrite = new Interaction.FileService();
    private Interaction.IFileAppend _fileAppend = new Interaction.FileService();
    private Interaction.IFileExists _fileExists = new Interaction.FileService();
    private readonly string _pathAdminJson   = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/admin.json";
    private readonly string _pathArticleJson = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/article.json";
    private readonly string _pathItemPurchased =
        "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/itemPurchased.json";
    public AdminService(FileLogger logger, Interaction.IFileRead fileRead, Interaction.IFileWrite fileWrite, Interaction.IFileAppend fileAppend) => (_logger, _fileRead, _fileWrite, _fileAppend) = (logger, fileRead, fileWrite, fileAppend);
    public void LogAndConsole(string message)
    {
        Console.WriteLine(message);
        _logger.Log(message);
    }
    public void AddLoginToFile(string login, string password)
    {
        // Verify if the file exists =>
        if (!_fileExists.FileExists(_pathAdminJson))
            // Create the file =>
            _fileWrite.WriteFile(_pathAdminJson, "");
        // Create a new admin =>
        Admin admin = new Admin(login, password);
        // Read the current content of the file =>
        string? jsonFile = _fileRead.ReadFile(_pathAdminJson);
        // Deserialize the JSON file into a list of Admin objects =>
        List<Admin>? admins = new List<Admin>();
        if (!string.IsNullOrWhiteSpace(jsonFile))
            admins = JsonSerializer.Deserialize<List<Admin>>(jsonFile);
        // Add the new admin to the list =>
        if (admins != null)
        {
            admins.Add(admin);
            // Serialize the list of admins into a JSON string =>
            string updatedJson = JsonSerializer.Serialize(admins);
            // Write the updated JSON string to the file =>
            _fileWrite.WriteFile(_pathAdminJson, updatedJson);
        }
        LogAndConsole($"----\nAdmin added = Login : {login} and Password : {password}");
    }
    public bool ValidateLogin(string login, string password)
    {
        // Read the current content of the file =>
        string jsonFile = _fileRead.ReadFile(_pathAdminJson);
        // Check if the file is empty =>
        if (string.IsNullOrEmpty(jsonFile))
        {
            LogAndConsole("The Json file is empty or missing.");
            return false; 
        }
        // Deserialize the JSON file into a list of Admin objects =>
        List<Admin>? admins = JsonSerializer.Deserialize<List<Admin>>(jsonFile);
        // Find the admin with matching login and password =>
        Admin matchedAdmin = admins.FirstOrDefault(admin => admin.Login == login && admin.Password == password);
        // Return true if the admin was found, false otherwise =>
        if (matchedAdmin != null)
        {
            LogAndConsole($"----\nLogin success\n\"Admin connected = Login : {login} and Password : {password}\"\n----");
            return true;
        }
        else
        {
            LogAndConsole("Your login or password is incorrect");
            return false;
        }
    }
    public void AddArticle(Article article, Admin admin)
    {
        // Verify if the file exists =>
        if (!_fileExists.FileExists(_pathArticleJson))
            // Create the file =>
            _fileWrite.WriteFile(_pathArticleJson, "");
        // Create a new article =>
        Article newArticle = new Article(article.Reference, article.Price);
        // Read the current content of the file =>
        string? jsonFile = _fileRead.ReadFile(_pathArticleJson);
        // Deserialize the JSON file into a list of Article objects =>
        List<Article>? articles = new List<Article>();
        if (!string.IsNullOrWhiteSpace(jsonFile))
            articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);
        // Add the new article to the list =>
        if (articles != null)
        {
            articles.Add(newArticle);
            // Serialize the list of articles into a JSON string =>
            string updatedJson = JsonSerializer.Serialize(articles);
            // Write the updated JSON string to the file =>
            _fileWrite.WriteFile(_pathArticleJson, updatedJson);
        }
        LogAndConsole($"----\n- {admin.Login} add an article ==> \n- Reference = {article.Reference} \n- Price = {article.Price}\n----");
    }
    public void GetArticles(Admin admin)
    {
        float total = 0;
        string invoicePath = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/{admin.Login}-SumOfArticlesSold.txt";
        // Read the current content of the file =>
        string? jsonFile = _fileRead.ReadFile(_pathArticleJson);
        // Deserialize the JSON file into a list of Article objects =>
        List<Article>? articles = new List<Article>();
        if (!string.IsNullOrWhiteSpace(jsonFile))
            articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);
        if (articles != null)
        {
            // Create the file =>
            _fileWrite.WriteFile(invoicePath, $"{admin.Login}'s Invoice\n---->\n");
            foreach (var article in articles)
            {
                // Add the details of the article to the invoice =>
                _fileAppend.AppendFile(invoicePath, $"Article N°{article.ID}\n- Référence : {article.Reference}\n- Price : {article.Price}\n----\n");
                // Calculate the total price =>
                total += article.Price;
            }
            // Add the total to the file =>
            _fileAppend.AppendFile(invoicePath, $"\nTotal : {total}\n");
            // Display the message =>
            LogAndConsole($"{admin.Login} created an invoice for all items purchased");
        }
    }
    // Méthode pour récupérer un article par référence (ou autre propriété unique)
    private Article GetArticleByReference(string reference)
    {
        string? jsonFile = _fileRead.ReadFile(_pathArticleJson);
        List<Article>? articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);
        // Vérifiez que la liste d'articles n'est pas null avant de rechercher la référence.
        if (articles != null)
        {
            return articles.FirstOrDefault(article => article.Reference == reference);
        }
        // Si la liste d'articles est null ou vide, retournez null ou lancez une exception appropriée.
        return null;
    }
    public void GetArticlesByBuyers(Buyer buyer, Article article, Admin admin)
    {
        float total = 0;
        float totalGlobal = 0;
        int billCount = 1;
        string testBillByBuyers = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/{admin.Login}-SumOfArticlesSoldByBuyers.txt";
        // Read the current content of the file =>
        string? jsonFile = _fileRead.ReadFile(_pathItemPurchased);
        // Deserialize the JSON file into a list of Article objects =>
        List<ItemPurchased>? itemPurchaseds = new List<ItemPurchased>();
        if (!string.IsNullOrWhiteSpace(jsonFile))
            itemPurchaseds = JsonSerializer.Deserialize<List<ItemPurchased>>(jsonFile);
        if (itemPurchaseds != null)
        {
            LogAndConsole($"{admin.Login} is generating a bill for articles bought by {buyer.Firstname}:\n");
            foreach (var items in itemPurchaseds)
            {
                // Trouver l'article correspondant par référence (ou autre propriété unique)
                Article purchasedArticle = GetArticleByReference(items.ArticleReference); // Remplacez ArticleReference par la propriété appropriée
                if (purchasedArticle != null)
                {
                    // Calculate the total price =>
                    total += items.Quantity * purchasedArticle.Price;
                    // Add the details of the article to the invoice =>
                    _fileAppend.AppendFile(testBillByBuyers, $"----> Bill {billCount}\n- Buyer : {buyer.Firstname}\n- Reference : {items.ArticleReference}\n- Quantity : {items.Quantity}\n- Price : {purchasedArticle.Price}\n Total purchases : {items.Quantity * purchasedArticle.Price} \n- Date of buy : {items.DateofBuy}\n----\n");
                    LogAndConsole($"\n----> Bill {billCount}\n- Buyer : {buyer.Firstname}\n- Reference : {items.ArticleReference}\n- Quantity : {items.Quantity}\n- Price : {purchasedArticle.Price}\n- Total purchases : {items.Quantity * purchasedArticle.Price} \n- Date of buy : {items.DateofBuy}\n----\n");
                    billCount++;
                }
            }
            totalGlobal += total;
            _fileAppend.AppendFile(testBillByBuyers, $"\n- Total global : {totalGlobal}\n");
            LogAndConsole($"Total global : {totalGlobal}\n");
        }
    }
    public void GenerateBillForBuyerByDate(DateTime startDate, DateTime endDate, Article article, Admin admin)
    {
        float total = 0;
        float totalGlobal = 0;
        int billCount = 1;
        string testBillByBuyersDateOfBuy = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/{admin.Login}-SumOfArticlesSoldByBuyersByDate.txt";

        // Read the current content of the file
        string? jsonFile = _fileRead.ReadFile(_pathItemPurchased);

        // Deserialize the JSON file into a list of ItemPurchased objects
        List<ItemPurchased>? itemPurchaseds = new List<ItemPurchased>();
        if (!string.IsNullOrWhiteSpace(jsonFile))
            itemPurchaseds = JsonSerializer.Deserialize<List<ItemPurchased>>(jsonFile);

        if (itemPurchaseds != null)
        {
            LogAndConsole($"{admin.Login} is generating a bill for articles bought between {startDate.ToShortDateString()} and {endDate.ToShortDateString()}:\n");

            foreach (var items in itemPurchaseds)
            {
                // Check if the purchase date falls within the specified date range
                if (items.DateofBuy.Date >= startDate.Date && items.DateofBuy.Date <= endDate.Date)
                {
                    // Find the corresponding article by reference (or other unique property)
                    Article purchasedArticle = GetArticleByReference(items.ArticleReference);

                    if (purchasedArticle != null)
                    {
                        // Calculate the total price
                        total += items.Quantity * purchasedArticle.Price;
                        
                        // Add the details of the article to the invoice
                        _fileAppend.AppendFile(testBillByBuyersDateOfBuy, $"\n----> Bill {billCount}\n- Reference : {items.ArticleReference}\n- Quantity : {items.Quantity}\n- Price : {purchasedArticle.Price}\n Total purchases : {items.Quantity * purchasedArticle.Price} \n- Date of buy : {items.DateofBuy}\n----\n");
                        LogAndConsole($"\n----> Bill {billCount}\n- Reference : {items.ArticleReference}\n- Quantity : {items.Quantity}\n- Price : {purchasedArticle.Price}\n- Total purchases : {items.Quantity * purchasedArticle.Price} \n- Date of buy : {items.DateofBuy}\n----\n");
                        billCount++;
                    }
                }
            }
            totalGlobal += total;
            _fileAppend.AppendFile(testBillByBuyersDateOfBuy, $"\n- Total global : {totalGlobal}\n");
            LogAndConsole($"Total global : {totalGlobal}\n");
        }
    }
}
// PART OF BUYER
public interface IBuyersService
{
    void CreateBuyer(string firstname, string lastname, string adress, string phone);
    void BuyerChooseAnArticleToList(Article article, Buyer buyer, string reference, int quantity);
    bool ValidateBuyerLog(string firstname, string lastname);
    void DisplayListOfArticle();
}
public class BuyerService : IBuyersService
{
    // private List<Buyer> _buyers = new();
    // private List<Article> _articles = new();
    private List<ItemPurchased> _itemsPurchased = new();
    private ILogger _logger = new FileLogger();
    private Interaction.IFileRead _fileRead = new Interaction.FileService();
    private Interaction.IFileWrite _fileWrite = new Interaction.FileService();
    private Interaction.IFileExists _fileExists = new Interaction.FileService();
    private Interaction.IFileAppend _fileAppend = new Interaction.FileService();
    private readonly string _pathBuyerJson         = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/buyer.json";
    private readonly string _pathArticleJson       = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/article.json";
    private readonly string _pathItemPurchasedJson = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/itemPurchased.json";
    public void LogAndConsole(string message)
    {
        Console.WriteLine(message);
        _logger.Log(message);
    }
    public void CreateBuyer(string firstname, string lastname, string adress, string phone)
    {
        // Verify if the file exists =>
        if (!_fileExists.FileExists(_pathBuyerJson))
            // Create the file =>
            _fileWrite.WriteFile(_pathBuyerJson, "");
        // Create a new buyer =>
        Buyer newBuyer = new Buyer(firstname, lastname, adress, phone);
        // Read the current content of the file =>
        string? jsonFile = _fileRead.ReadFile(_pathBuyerJson);
        // Deserialize the JSON file into a list of Buyer objects =>
        List<Buyer>? buyers = new List<Buyer>();
        if (!string.IsNullOrWhiteSpace(jsonFile))
            buyers = JsonSerializer.Deserialize<List<Buyer>>(jsonFile);
        // Add the new buyer to the list =>
        if (buyers != null)
        {
            buyers.Add(newBuyer);
            // Serialize the list of buyers into a JSON string =>
            string updatedJson = JsonSerializer.Serialize(buyers);
            // Write the updated JSON string to the file =>
            _fileWrite.WriteFile(_pathBuyerJson, updatedJson);
            LogAndConsole(" ----\nBuyer added\n----");
        }
    }
    public bool ValidateBuyerLog(string firstname, string lastname)
    {
        // Read the current content of the file =>
        string jsonFile = _fileRead.ReadFile(_pathBuyerJson);
        // Check if the file is empty =>
        if (string.IsNullOrEmpty(jsonFile))
        {
            LogAndConsole("The Json file is empty or missing.");
            return false; 
        }
        // Deserialize the JSON file into a list of Buyer objects =>
        List<Buyer>? buyers = JsonSerializer.Deserialize<List<Buyer>>(jsonFile);
        // Find the buyer with matching login and password =>
        Buyer matchedBuyer = buyers.FirstOrDefault(buyer => buyer.Firstname == firstname && buyer.Lastname == lastname);
        // Return true if the buyer was found, false otherwise =>
        if (matchedBuyer != null)
        {
            LogAndConsole($"----\n\"Buyer registered =\nFirstname : {matchedBuyer.Firstname}\nLastname : {matchedBuyer.Lastname}\"\n----");
            return true;
        }
        else
        {
            LogAndConsole("Your dont have an buyer account");
            return false;
        }
    }
    public void BuyerChooseAnArticleToList(Article newArticle, Buyer newBuyer, string reference, int quantity)
    {
        // Read the current content of the file =>
        string jsonFile = _fileRead.ReadFile(_pathArticleJson);
        // Choisir un article de la liste =>
        List<Article>? articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);
        if (articles != null)
        {
            // Vérifiez si l'article avec la référence spécifiée existe
            Article selectedArticle = articles.FirstOrDefault(article => article.Reference == reference);
            if (selectedArticle != null)
            {
                // Calculez le prix total de l'article choisi par le buyer
                float totalPrice = selectedArticle.Price * quantity;
                LogAndConsole($"----\n[{newBuyer.Firstname}] added a {quantity} of {selectedArticle.Reference} to his list\nwith a Total price = {totalPrice}\n----");
                // Sauvegardez les détails de l'article dans le fichier
                string path = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/{newBuyer.Firstname}-{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-{DateTime.Now.Hour}-{DateTime.Now.Minute}.txt";
                _fileWrite.WriteFile(path, $" ----\nList of articles purchased by '{newBuyer.Firstname}' :\n----\nReference : {selectedArticle.Reference}\nPrice : {selectedArticle.Price}\nQuantity : {quantity}\nTotal price : {totalPrice}\n----\n");
                // Appeler la fonction AddItemPurchased
                float priceAtPurchase = CalculatePriceAtPurchase(reference, quantity);
                LogAndConsole($"---->\n- Reference : {reference}\n- Quantity : {quantity}\n- Price at purchase : {priceAtPurchase}\n- Date of buy : {DateTime.Now}\n----");
                AddItemPurchases(new ItemPurchased(selectedArticle.Reference, quantity, priceAtPurchase, DateTime.Now));
            }
            else
            {
                LogAndConsole($"Article with reference '{reference}' not found.");
            }
        }
    }
    public float CalculatePriceAtPurchase(string reference, int quantity)
    {
        string jsonFile = _fileRead.ReadFile(_pathArticleJson);
        List<Article>? articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);

        if (articles != null)
        {
            // Recherchez l'article par sa référence
            Article purchasedArticle = articles.FirstOrDefault(article => article.Reference == reference);
            if (purchasedArticle != null)
            {
                // Calculez le prix total en fonction de la quantité
                float totalPrice = purchasedArticle.Price * quantity;
                return totalPrice;
            }
        }
        // Si l'article n'a pas été trouvé ou s'il y a une erreur, renvoyez 0 ou une valeur par défaut appropriée
        return 0.0f;
    }
    public void AddItemPurchases(ItemPurchased itemPurchased)
    {
        // Read the current content of the file =>
        string? jsonFile = _fileRead.ReadFile(_pathItemPurchasedJson);
        // PROB
        List<ItemPurchased>? itemsPurchased = new List<ItemPurchased>();
        if (!string.IsNullOrWhiteSpace(jsonFile))
        {
            itemsPurchased = JsonSerializer.Deserialize<List<ItemPurchased>>(jsonFile);
        }
        // Add the new item purchased to the list =>
        itemsPurchased ??= new List<ItemPurchased>();
        itemsPurchased.Add(itemPurchased);
        // Serialize the list of items purchased into a JSON string =>
        string updatedJson = JsonSerializer.Serialize(itemsPurchased);

        // Write the updated JSON string to the file =>
        _fileWrite.WriteFile(_pathItemPurchasedJson, updatedJson);

        LogAndConsole("----\nItem purchased added\n----");
    }
    public void DisplayListOfArticle()
    {
        // Read the current content of the file =>
        string jsonFile = _fileRead.ReadFile(_pathArticleJson);
        // Choisir un article de la liste =>
        List<Article>? articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);
        if (articles != null)
        {
            foreach (var article in articles)
            {
                LogAndConsole($"---->\nArticles N°{article.ID}\n- Reference = {article.Reference} \n- Price = {article.Price}\n----");
            }
        }
    }
}