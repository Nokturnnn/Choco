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
    void GetArticlesByBuyers(Buyer buyer, Admin admin);
}
public class AdminService : IAdmin
{
    private List<Admin> _admins = new();
    private List<Article> _articles = new();
    private List<Buyer> _buyers = new();
    private ILogger _logger;
    private Interaction.IFileRead _fileRead = new Interaction.FileService();
    private Interaction.IFileWrite _fileWrite = new Interaction.FileService();
    private Interaction.IFileAppend _fileAppend = new Interaction.FileService();
    private Interaction.IFileExists _fileExists = new Interaction.FileService();
    private readonly string _pathAdminJson   = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/admin.json";
    private readonly string _pathArticleJson = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/article.json";
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
        string testBill = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/{admin.Login}-SumOfArticlesSold.txt";
        // Read the current content of the file =>
        string? jsonFile = _fileRead.ReadFile(_pathArticleJson);
        // Deserialize the JSON file into a list of Article objects =>
        List<Article>? articles = new List<Article>();
        if (!string.IsNullOrWhiteSpace(jsonFile))
            articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);
        // Add the new article to the list =>
        if (articles != null)
        {
            LogAndConsole($"{admin.Login} add a new Bill with all Articles Sold ==>\n----");
            foreach (var article in articles)
            {
                LogAndConsole($"----\nArticles N°{article.ID}\n- Reference = {article.Reference} \n- Price = {article.Price}\n----");
                total += article.Price;
                _fileAppend.AppendFile(testBill, $"----\n- Reference = {article.Reference} \n- Price = {article.Price}\n----");
            }
            _fileAppend.AppendFile(testBill, $"----\nTotal = {total}\n----");
            LogAndConsole($"----\nTotal = {total}\n----");
        }
    }
    public void GetArticlesByBuyers(Buyer buyer, Admin admin)
    {
        float total = 0;
        string testBillByBuyers = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/{admin.Login}-SumOfArticlesSoldByBuyers.txt";
        // Read the current content of the file =>
        string? jsonFile = _fileRead.ReadFile(_pathArticleJson);
        // Deserialize the JSON file into a list of Article objects =>
        List<Article>? articles = new List<Article>();
        if (!string.IsNullOrWhiteSpace(jsonFile))
            articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);
        // Add the new article to the list =>
        if (articles != null)
        {
            LogAndConsole($"{admin.Login} add a new Bill with all articles sold by each Buyers ==>\n----");
            foreach (var article in articles)
            {
                LogAndConsole($"----\nArticles N°{article.ID}\n- Reference = {article.Reference}\n- Price = {article.Price}\n bought by {buyer.Lastname}{buyer.Firstname}\n----");
                total += article.Price;
                _fileAppend.AppendFile(testBillByBuyers, $"----\n- Reference = {article.Reference} \n- Price = {article.Price}\n bought by {buyer.Lastname}{buyer.Firstname}\n----");
            }
            _fileAppend.AppendFile(testBillByBuyers, $"----\nTotal = {total}\n----");
            LogAndConsole($"----\nTotal = {total}\n----");
        }
    }
}
// PART OF BUYER
public interface IBuyersService
{
    string CreateBuyer(Buyer buyer);
    void AddArticleToLog(Buyer buyer, Article article, int quantity, DateTime DateofOrder);
    float CalculateTotalPrice(Buyer buyer, Article article);
    void FinishOrder(Buyer buyer, Article article);
    void BuyerChooseAnArticleToList(Article article, Buyer buyer, string reference, int quantity);
    bool ValidateBuyerLog(string lastname, string firstname);
    void DisplayListOfArticle();
}
public class BuyerService : IBuyersService
{
    private List<Buyer> _buyers = new();
    private List<Article> _articles = new();
    private List<ItemPurchased> _itemsPurchased = new();
    private ILogger _logger = new FileLogger();
    private Interaction.IFileRead _fileRead = new Interaction.FileService();
    private Interaction.IFileWrite _fileWrite = new Interaction.FileService();
    private Interaction.IFileExists _fileExists = new Interaction.FileService();
    private readonly string _pathBuyerJson   = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/buyer.json";
    private readonly string _pathArticleJson = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/article.json";
    public void LogAndConsole(string message)
    {
        Console.WriteLine(message);
        _logger.Log(message);
    }
    public string CreateBuyer(Buyer buyer)
    {
        // Verify if the file exists =>
        if (!_fileExists.FileExists(_pathBuyerJson))
            // Create the file =>
            _fileWrite.WriteFile(_pathBuyerJson, "");
        // Create a new buyer =>
        Buyer newBuyer = new Buyer(buyer.Firstname , buyer.Lastname, buyer.Adress, buyer.Phone);
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

        return (buyer.Firstname) + (buyer.Lastname);
    }
    public bool ValidateBuyerLog(string lastname, string firstname)
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
        Buyer matchedBuyer = buyers.FirstOrDefault(buyer => buyer.Lastname == buyer.Lastname && buyer.Firstname == buyer.Firstname);
        // Return true if the buyer was found, false otherwise =>
        if (matchedBuyer != null)
        {
            LogAndConsole($"----\n\"Buyer registered =\n Lastname : {matchedBuyer.Lastname} and Firstname : {matchedBuyer.Firstname}\"\n----");
            return true;
        }
        else
        {
            LogAndConsole("Your dont have an buyer account");
            return false;
        }
    }
    public void BuyerChooseAnArticleToList(Article newArticle, Buyer buyer, string reference, int quantity)
    {
        // Read the current content of the file =>
        string jsonFile = _fileRead.ReadFile(_pathArticleJson);
        // Choisir un article de la liste =>
        List<Article>? articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);
        // if (articles != null)
        // {
        //     foreach (var article in articles)
        //     {
        //         LogAndConsole($"----\nArticles N°{article.ID}\n- Reference = {article.Reference} \n- Price = {article.Price}\n----");
        //     }
        // }
        // Choisir une quantité =>
        // Console.WriteLine("Enter the reference of the article you want to buy :");
        // string reference = (Console.ReadLine());
        // Console.WriteLine("Enter the quantity of the article you want to buy :");
        // int quantity = int.Parse(Console.ReadLine());
        // Sauvegarder le fichier =>
        string path = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/{buyer.Lastname}-{buyer.Firstname}-{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-{DateTime.Now.Hour}-{DateTime.Now.Minute}.txt";
        // Créer le fichier vide =>
        _fileWrite.WriteFile(path, "");
        // Calculer le prix total de l'article choisi par le buyer =>
        float totalPrice = 0;
        foreach (var article in articles)
        {
            if (reference == article.Reference)
            {
                totalPrice = article.Price * quantity;
                LogAndConsole($"----\n{buyer.Lastname}-{buyer.Firstname} added a {quantity} of {article.Reference} to his list\nwith a Total price = {totalPrice}\n----");
            }
        }
        // Ajouter l'article choisi par le buyer dans le fichier =>
        _fileWrite.WriteFile(path,
            $"----\n{buyer.Lastname}-{buyer.Firstname} added a {quantity} of {reference} to his list\nWith a Total price = {totalPrice}\n----");
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
                LogAndConsole($"----\nArticles N°{article.ID}\n- Reference = {article.Reference} \n- Price = {article.Price}\n----");
            }
        }
    }
    public void AddArticleToLog(Buyer buyer, Article article, int quantity, DateTime dateofOrder)
    {
        // Create a new item purchased =>
        ItemPurchased itemPurchased = new ItemPurchased(quantity, dateofOrder);
        // Add the new item purchased to the list =>
        _itemsPurchased.Add(itemPurchased);
    }
    public float CalculateTotalPrice(Buyer buyer, Article article)
    {
        float totalPrice = 0;
        foreach (var itemPurchased in _itemsPurchased)
        {
            totalPrice += itemPurchased.Quantity * article.Price;
        }
        return totalPrice;
    }
    public void FinishOrder(Buyer buyer, Article article)
    {
        // Create a new file =>
        string path = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/{buyer.Lastname}-{buyer.Firstname}-{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-{DateTime.Now.Hour}-{DateTime.Now.Minute}.txt";
        // Create the log entry =>
        string logEntry = $"{DateTime.Now.Day}/{DateTime.Now.Month}/{DateTime.Now.Year} -- Add a : {article.Reference} à {DateTime.Now.Hour}h{DateTime.Now.Minute} par {buyer.Lastname} {buyer.Firstname}";
        // Write the log entry to the file =>
        _fileWrite.WriteFile(path, logEntry);
    }
}