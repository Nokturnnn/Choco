﻿using System.Text.Json;
using ChocoLog;
using ChocoInteraction;
using ManagementPeople;

namespace ChocoList;

public interface IAdmin
{
    bool ValidateLogin(string login, string password);
    void AddArticle(Article article, Admin admin);
    void AddLoginToFile(string login2, string password2);
    // bool VerifiedFileAdminJson(string login, string password);
}
public class AdminService : IAdmin
{
    private List<Admin> _admins = new();
    private List<Article> _articles = new();
    private List<Buyer> _buyers = new();
    private ILogger _logger;
    private Interaction.IFileRead _fileRead;
    private Interaction.IFileWrite _fileWrite;
    private Interaction.IFileAppend _fileAppend;
    private readonly string _pathAdminJson = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/admin.json";
    public AdminService(FileLogger logger, Interaction.IFileRead fileRead, Interaction.IFileWrite fileWrite, Interaction.IFileAppend fileAppend) => (_logger, _fileRead, _fileWrite, _fileAppend) = (logger, fileRead, fileWrite, fileAppend);

    public void LogAndConsole(string message)
    {
        Console.WriteLine(message);
        _logger.Log(message);
    }
    public void AddLoginToFile(string login, string password)
    {
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
        LogAndConsole($"Admin added = Login : {login} and Password : {password}");
    }

    public bool ValidateLogin(string login, string password)
    {
        string jsonFile = _fileRead.ReadFile(_pathAdminJson);
        // Deserialize the JSON file into a list of Admin objects =>
        List<Admin>? admins = JsonSerializer.Deserialize<List<Admin>>(jsonFile);
        // Find the admin with matching login and password =>
        Admin matchedAdmin = admins.FirstOrDefault(admin => admin.Login == login && admin.Password == password);
        
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
        // Ajouter un article a une liste d'articles sans ecraser la liste existante =>
        _articles.Add(article);
        // Lire le contenu du fichier =>
        string jsonFile = _fileRead.ReadFile(_pathAdminJson);
        // Deserialiser le contenu du fichier en une liste d'articles =>
        List<Article>? articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);
        // Ajouter le nouvel article a la liste =>
        articles.Add(article);
        // Serialiser la liste d'articles en une chaine de caracteres JSON =>
        string updatedJson = JsonSerializer.Serialize(articles);
        // Ecrire la chaine de caracteres JSON dans le fichier =>
        _fileWrite.WriteFile(_pathAdminJson, updatedJson);
        LogAndConsole($"\"[Administrateur : {admin.Login}] add a [{article.Reference}] a {DateTime.Now.Hour}h{DateTime.Now.Minute}m{DateTime.Now.Minute} on {DateTime.Now.Day}/{DateTime.Now.Month}/{DateTime.Now.Year} to the list of articles\n----\"");
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