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
}

public class AdminService : IAdmin
{
    private List<Admin> _admins = new();
    private List<Article> _articles = new();
    private List<Buyer> _buyers = new();
    private ILogger _logger;
    private Interaction.IFileRead _fileRead;
    private Interaction.IFileWrite _fileWrite;
    public AdminService(FileLogger logger, Interaction.IFileRead fileRead, Interaction.IFileWrite fileWrite)
    {
        _logger = logger;
        _fileRead = fileRead;
        _fileWrite = fileWrite;
    }
    public void AddLoginToFile(string login, string password)
    {
        // Path to the JSON file =>
        string path = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/admin.json";
        // Create a new admin =>
        Admin admin = new Admin(login, password);
        // Read the current content of the file =>
        string? jsonFile = _fileRead.ReadFile(path);
        // Deserialize the JSON file into a list of Admin objects =>
        List<Admin>? admins = new List<Admin>();
        if (!string.IsNullOrWhiteSpace(jsonFile))
        {
            admins = JsonSerializer.Deserialize<List<Admin>>(jsonFile);
        }
        // Add the new admin to the list =>
        if (admins != null)
        {
            admins.Add(admin);
            // Serialize the list of admins into a JSON string =>
            string updatedJson = JsonSerializer.Serialize(admins);
            // Write the updated JSON string to the file =>
            _fileWrite.WriteFile(path, updatedJson);
        }
        // Create the log entry =>
        string logEntry = $"Admin added = Login : {login} and Password : {password},";
        _logger.Log(logEntry);
        Console.WriteLine("----");
        Console.WriteLine("Your account has been created");
    }

    public bool ValidateLogin(string login, string password)
    {
        string pathAdmin = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/admin.json";
        string jsonFile = _fileRead.ReadFile(pathAdmin);
        // Deserialize the JSON file into a list of Admin objects =>
        List<Admin> admins = JsonSerializer.Deserialize<List<Admin>>(jsonFile);
        // Find the admin with matching login and password =>
        Admin matchedAdmin = admins.FirstOrDefault(admin => admin.Login == login && admin.Password == password);
        
        if (matchedAdmin != null)
        {
            Console.WriteLine("----");
            Console.WriteLine("Login success");
            string logEntry = $"Admin connected = Login : {login} and Password : {password},";
            _logger.Log(logEntry);
            Console.WriteLine("----");
            return true;
        }
        else
        {
            Console.WriteLine("Your login or password is incorrect");
            return false;
        }
    }
    public void AddArticle(Article article, Admin admin)
    {
        _articles.Add(article);
        // Create the log entry =>
        string logEntry = $"[Administrateur : {admin.Login}] add a [{article.Reference}] a {DateTime.Now.Hour}h{DateTime.Now.Minute}m{DateTime.Now.Minute} on {DateTime.Now.Day}/{DateTime.Now.Month}/{DateTime.Now.Year} to the list of articles";
        _logger.Log(logEntry);
        Console.WriteLine("----");
        // Create a file with article added =>
        string path = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/articles.json";
        _fileWrite.WriteFile(path, JsonSerializer.Serialize(_articles));
        Console.WriteLine("Article added");
    }
}
public interface IBuyersService
{
    Buyer CreateBuyer(string adress, string lastname, string firstname, int phone );
    void AddArticleToLog(Buyer buyer, Article article, int quantity, DateTime DateofOrder);
    float CalculateTotalPrice(Buyer buyer, Article article);
    void FinishOrder(Buyer buyer, Article article);
}
public class BuyerService : IBuyersService
{
    private List<Buyer> _buyers = new();
    private List<Article> _articles = new();
    private List<ItemPurchased> _itemsPurchased = new();
    private Interaction.IFileRead _fileRead;
    private Interaction.IFileWrite _fileWrite;
    
    public Buyer CreateBuyer(string adress, string lastname, string firstname, int phone)
    {
        // Create a new buyer =>
        Buyer buyer = new Buyer(adress, lastname, firstname, phone);
        // Add the new buyer to the list =>
        _buyers.Add(buyer);
        // Return the new buyer =>
        return buyer;
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