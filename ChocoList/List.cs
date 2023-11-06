using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ChocoLog;
using ChocoInteraction;
using ManagementPeople;

namespace ChocoList;

public interface IAdmin
{
    Task<string> LogAndConsoleAsync(string message);
    Task<bool> ValidateLoginAsync(string login, string password);
    Task<bool> AddArticleAsync(Article article, Admin admin);
    Task<bool> AddLoginToFileAsync(string login, string password);
    Task<string> HashPassword(string password);
    Task<bool> DeleteArticleAsync(Admin admin, string reference);
    Task<bool> GetArticlesAsync(Admin admin);
    Task<Article> GetArticleByReferenceAsync(string reference);
    Task<bool> GetArticlesByBuyersAsync(Buyer buyer, Article article, Admin admin);
    Task<bool> GenerateBillForBuyerByDateAsync(DateTime startDate, DateTime endDate, Article article, Admin admin);
    
}
public class AdminService : IAdmin
{
    // Initialization => //
    private ILogger _logger;
    private Interaction.FileService _fileService;
    private readonly string[] _filePaths;
    // END Initialization //
    
    // Constructor => //
    public AdminService(FileLogger logger, Interaction.FileService fileService) => (_logger, _fileService, _filePaths) = (logger, fileService, new string[]{"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/admin.json","/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/article.json","/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/buyer.json","/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/itemPurchased.json"});
    // END Constructor //
    
    // All methods => //
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
    public async Task<bool> AddLoginToFileAsync(string login, string password)
    {
        try
        {
            // Verify if the file exists =>
            if (!_fileService.FileExistsAsync(_filePaths[0])) 
                await _fileService.WriteFileAsync(_filePaths[0], "");
            // Create a new admin =>
            Admin admin = new Admin(login, await HashPassword(password));
            // Read the current content of the file =>
            string jsonFile = await _fileService.ReadFileAsync(_filePaths[0]);
            // Deserialize the JSON file into a list of Admin objects =>
            var admins = string.IsNullOrWhiteSpace(jsonFile) 
                ? new List<Admin>() 
                : JsonSerializer.Deserialize<List<Admin>>(jsonFile);
            // Add the new admin to the list =>
            admins?.Add(admin);
            // Serialize the list of admins into a JSON string =>
            await _fileService.WriteFileAsync(_filePaths[0], JsonSerializer.Serialize(admins));
            // Display the message =>
            await LogAndConsoleAsync($"\n----Admin added =\n - Login: {login}\n - Password: {password}\n----");
            // Return true if the admin was added =>
            return true;
        }
        catch (Exception e)
        {
            // Log the error =>
            await LogAndConsoleAsync("Error when adding an admin: " + e.Message);
            return false;
        }
    }
    public Task<string> HashPassword(string password)
    {
        return Task.FromResult(BCrypt.Net.BCrypt.HashPassword(password));
    }
    public async Task<bool> ValidateLoginAsync(string login, string password) 
    {
        try
        {
            // Read the current content of the file =>
            string jsonFile = await _fileService.ReadFileAsync(_filePaths[0]);
            // Check if the file is empty =>
            if (string.IsNullOrEmpty(jsonFile)) 
            { 
                await LogAndConsoleAsync("The Json file is empty or missing"); 
                return false; 
            }
            // Create a var to store the list of admins =>
            var admins = JsonSerializer.Deserialize<List<Admin>>(jsonFile) ?? new List<Admin>();
            // Find the admin with matching login and password =>
            var matchedAdmin = admins.FirstOrDefault(admin => admin.Login == login && admin.Password == password);
            // Return true if the admin was found, false otherwise =>
            if (matchedAdmin != null) 
            {
                // Display the message =>
                await LogAndConsoleAsync($"\n----\nAdmin validated =\n - Login: {login}\n - Password: {password}");
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            // Log the error =>
            await LogAndConsoleAsync("Error when validating an admin: " + e.Message);
            return false;
        }
    }
    public async Task<bool> AddArticleAsync(Article article, Admin admin)
    {
        try
        {
            // Verify if the file exists =>
            if (!_fileService.FileExistsAsync(_filePaths[1]))
                // Use WriteAllTextAsync to create the file with empty content if it does not exist =>
                await _fileService.WriteFileAsync(_filePaths[1], "");
            // Create a new article =>
            var newArticle = new Article(article.Reference, article.Price);
            // Read the current content of the file =>
            var jsonFile = await _fileService.ReadFileAsync(_filePaths[1]);
            // Deserialize the JSON file into a list of Article objects =>
            var articles = string.IsNullOrWhiteSpace(jsonFile) 
                ? new List<Article>() 
                : JsonSerializer.Deserialize<List<Article>>(jsonFile) ?? new List<Article>();
            // Add the new article to the list =>
            articles.Add(newArticle);
            // Serialize the list of articles into a JSON string =>
            await _fileService.WriteFileAsync(_filePaths[1], JsonSerializer.Serialize(articles));
            // Display the message =>
            await LogAndConsoleAsync($"---->\n- '{admin.Login}' add an article ==> \n- Reference: {article.Reference} \n- Price: {article.Price}\n----");
            return true;
        }
        catch (Exception e)
        {
            // Log the error =>
            await LogAndConsoleAsync("Error when adding an article: " + e.Message);
            return false;
        }
    }
    public async Task<bool> DeleteArticleAsync(Admin admin, string reference)
    {
        try
        {
            // Verify if the file exists =>
            if (!_fileService.FileExistsAsync(_filePaths[1]))
                // Use WriteAllTextAsync to create the file with empty content if it does not exist =>
                await _fileService.WriteFileAsync(_filePaths[1], "");
            // Read the current content of the file =>
            var jsonFile = await _fileService.ReadFileAsync(_filePaths[1]);
            // Deserialize the JSON file into a list of Article objects =>
            var articles = string.IsNullOrWhiteSpace(jsonFile) 
                ? new List<Article>() 
                : JsonSerializer.Deserialize<List<Article>>(jsonFile) ?? new List<Article>();
            // Find the article with matching reference =>
            var selectedArticle = articles.FirstOrDefault(article => article.Reference == reference);
            // Return true if the article was found, false otherwise =>
            if (selectedArticle != null)
            {
                // Remove the article from the list =>
                articles.Remove(selectedArticle);
                // Serialize the list of articles into a JSON string =>
                await _fileService.WriteFileAsync(_filePaths[1], JsonSerializer.Serialize(articles));
                // Display the message =>
                await LogAndConsoleAsync($"----\nArticle deleted by '{admin.Login}':\n- Reference: {selectedArticle.Reference}\n----");
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            // Log the error =>
            await LogAndConsoleAsync("----\nError when deleting an article: " + e.Message + "\n----");
            return false;
        }
    }
    public async Task<bool> GetArticlesAsync(Admin admin)
    {
        // Initialize a file =>
        string sommeOfArticlesByAdmin = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/SumOfArticlesPurchased/{admin.Login}-SumOfArticles.txt";
        try
        {
            // Initialize a total / billCount =>
            float totalGlobal = 0;
            int billCount = 1;
            // Read the current content of the file =>
            string jsonFile = await _fileService.ReadFileAsync(_filePaths[3]);
            // Deserialize the JSON file into a list of ItemPurchased objects =>
            var itemPurchaseds = string.IsNullOrWhiteSpace(jsonFile) 
                ? new List<ItemPurchased>() 
                : JsonSerializer.Deserialize<List<ItemPurchased>>(jsonFile) ?? new List<ItemPurchased>();
            // Check if the list is empty =>
            if (!itemPurchaseds.Any())
            {
                await LogAndConsoleAsync("You don't have any articles");
                return false;
            }
            foreach (var item in itemPurchaseds)
            {
                Article purchasedArticle = await GetArticleByReferenceAsync(item.ArticleReference);
                if (true)
                {
                    float totalPrice = item.Quantity * purchasedArticle.Price;
                    totalGlobal += totalPrice;

                    string invoiceContent = !_fileService.FileExistsAsync(sommeOfArticlesByAdmin) ? $@"
                        ****************************************
                                     SUM OF ARTICLES            
                        ****************************************

                        Admin : {admin.Login}

                        ****************************************
                                  Details of Purchases :
                        ----------------------------------------" : "";

                    invoiceContent += $@"
                        
                        Reference of article   : {purchasedArticle.Reference}
                        Price per unit         : {purchasedArticle.Price:C}
                        Quantity purchased     : {item.Quantity}
                        Total for this article : {totalPrice:C}

                        ----------------------------------------";

                    await _fileService.AppendFileAsync(sommeOfArticlesByAdmin, invoiceContent);
                    await LogAndConsoleAsync($"\n----> Bill {billCount}\n- Reference : {item.ArticleReference}\n- Quantity : {item.Quantity}\n- Price per unit : {purchasedArticle.Price}\n- Total for this article : {totalPrice} \n- Date of buy : {item.DateofBuy}\n----\n");
                }
            }
            string totalContent = $@" 

                         Total global : {totalGlobal:C}";
            await _fileService.AppendFileAsync(sommeOfArticlesByAdmin, totalContent);
            await LogAndConsoleAsync($"Total global : {totalGlobal}\n");
            return true;
        }
        catch (Exception e)
        {
            await LogAndConsoleAsync("Error when getting articles: " + e.Message);
            return false;
        }
    }
    public async Task<Article> GetArticleByReferenceAsync(string reference)
    {
        try
        {
            // Read the current content of the file =>
            string jsonFile = await _fileService.ReadFileAsync(_filePaths[1]);
            // Deserialize the JSON file into a list of Article objects =>
            var articles = JsonSerializer.Deserialize<List<Article>>(jsonFile) ?? new List<Article>();
            return articles.FirstOrDefault(article => article.Reference == reference)!;
        }
        catch (Exception e)
        {
            // Log the error =>
            await LogAndConsoleAsync("Error when getting an article: " + e.Message);
            return null!;
        }
    }
    public async Task<bool> GetArticlesByBuyersAsync(Buyer buyer, Article article, Admin admin)
    {
        try
        { 
            float totalGlobal = 0;
            int billCount = 1;
            string sumArticleSoldByBuyersPath = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/BillBuyerPerBuyer/{admin.Login}-SumOfArticlesSoldByBuyers.txt";
            string jsonFile = await _fileService.ReadFileAsync(_filePaths[3]);

            var itemPurchaseds = string.IsNullOrWhiteSpace(jsonFile) ? new List<ItemPurchased>() : JsonSerializer.Deserialize<List<ItemPurchased>>(jsonFile);
            if (!itemPurchaseds.Any()) return false;

            await LogAndConsoleAsync($"{admin.Login} is generating a bill for articles bought by {buyer.Firstname}:\n");

            foreach (var item in itemPurchaseds)
            {
                Article purchasedArticle = await GetArticleByReferenceAsync(item.ArticleReference);
                if (purchasedArticle != null)
                {
                    float totalPrice = item.Quantity * purchasedArticle.Price;
                    totalGlobal += totalPrice;

                    string invoiceContent = !_fileService.FileExistsAsync(sumArticleSoldByBuyersPath) ? $@"
                        ****************************************
                                     SUM OF ARTICLES            
                        ****************************************
                        Admin : {admin.Login}
                        Buyer : {buyer.Firstname}
                        Date  : {DateTime.Now.ToShortDateString()}
                        ****************************************
                                  Details of Purchases :
                        ----------------------------------------" : "";

                    invoiceContent += $@"
                        
                        Reference of article   : {purchasedArticle.Reference}
                        Price per unit         : {purchasedArticle.Price:C}
                        Quantity purchased     : {item.Quantity}
                        Total for this article : {totalPrice:C}

                        ----------------------------------------";

                    await _fileService.AppendFileAsync(sumArticleSoldByBuyersPath, invoiceContent);
                    await LogAndConsoleAsync($"\n----> Bill {billCount}\n- Buyer : {buyer.Firstname}\n- Reference : {item.ArticleReference}\n- Quantity : {item.Quantity}\n- Price per unit : {purchasedArticle.Price}\n- Total for this article : {totalPrice} \n- Date of buy : {item.DateofBuy}\n----\n");
                    billCount++;
                }
            }
            string totalContent = $@" 

                         Total global : {totalGlobal:C}" ;
            await _fileService.AppendFileAsync(sumArticleSoldByBuyersPath, totalContent);
            await LogAndConsoleAsync($"Total global : {totalGlobal}\n");
            return true;
        }
        catch (Exception e)
        {
            await LogAndConsoleAsync("Error when getting articles by buyers: " + e.Message);
            return false;
        }
    }
    public async Task<bool> GenerateBillForBuyerByDateAsync(DateTime startDate, DateTime endDate, Article article, Admin admin)
    {
        try
        {
            float totalGlobal = 0;
            int billCount = 1;
            string billByBuyersDateOfBuyPath = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/BillBuyerPerDate/{admin.Login}-SumOfArticlesSoldByBuyersByDate.txt";
            string jsonFile = await _fileService.ReadFileAsync(_filePaths[3]);

            var itemPurchaseds = string.IsNullOrWhiteSpace(jsonFile) ? new List<ItemPurchased>() : JsonSerializer.Deserialize<List<ItemPurchased>>(jsonFile);
            if (!itemPurchaseds.Any()) return false;

            await LogAndConsoleAsync($"{admin.Login} is generating a bill for articles bought between {startDate.ToShortDateString()} and {endDate.ToShortDateString()} :\n");

            foreach (var item in itemPurchaseds)
            {
                if (item.DateofBuy.Date >= startDate.Date && item.DateofBuy.Date <= endDate.Date)
                {
                    Article purchasedArticle = await GetArticleByReferenceAsync(item.ArticleReference);
                    if (purchasedArticle != null)
                    {
                        float totalPrice = item.Quantity * purchasedArticle.Price;
                        totalGlobal += totalPrice;

                        string invoiceContent = !_fileService.FileExistsAsync(billByBuyersDateOfBuyPath) ? $@"
                            ****************************************
                                        SUM OF ARTICLES            
                            ****************************************
                            Admin : {admin.Login}
                            Date  : {DateTime.Now.ToShortDateString()}
                            ****************************************
                                      Details of Purchases :
                            ----------------------------------------" : "";

                        invoiceContent += $@"
                            
                            Reference of article   : {purchasedArticle.Reference}
                            Price per unit         : {purchasedArticle.Price:C}
                            Quantity purchased     : {item.Quantity}
                            Total for this article : {totalPrice:C}
                            Date of Buy            : {item.DateofBuy.ToShortDateString()}

                            ----------------------------------------";
                        
                        await _fileService.AppendFileAsync(billByBuyersDateOfBuyPath, invoiceContent);
                        await LogAndConsoleAsync($"\n----> Bill N°{billCount}\n- Reference: {item.ArticleReference}\n- Quantity: {item.Quantity}\n- Price per unit: {purchasedArticle.Price}\n- Total for this article: {totalPrice} \n- Date of buy: {item.DateofBuy}\n----\n");
                        billCount++;
                    }
                }
            }
            string totalContent = $@" 

                             Total global : {totalGlobal:C}" ;
            await _fileService.AppendFileAsync(billByBuyersDateOfBuyPath, totalContent);
            await LogAndConsoleAsync($"Total global: {totalGlobal}\n");

            return true;
        }
        catch (Exception e)
        {
            await LogAndConsoleAsync("Error when generating a bill for buyers by date: " + e.Message);
            return false;
        }
    }
    // END All methods //
}

// PART OF BUYER
public interface IBuyersService
{
    Task<string> LogAndConsoleAsync(string message);
    Task<bool> CreateBuyerAsync(string firstname, string lastname, string address, string phone);
    Task<bool> ValidateBuyerLogAsync(string firstname, string lastname, string address, string phone);
    Task<bool> BuyerChooseAnArticleToListAsync(Article newArticle, Buyer newBuyer, string reference, int quantity);
    Task FinalizeInvoiceAsync(Buyer newBuyer);
    Task<float> CalculatePriceAtPurchase(string reference, int quantity);
    Task<bool> AddItemPurchasesAsync(ItemPurchased itemPurchased);
    Task<bool> DisplayListOfArticle();
    Task<bool> DisplayOrderInProgress(Buyer newBuyer);
}
public class BuyerService : IBuyersService
{
    // START Initialization
    private readonly ILogger _logger;
    private readonly Interaction.FileService _fileService;
    private readonly string[] _filePaths;
    private float _totalPurchases = 0;
    // END Initialization
    
    // START Constructor
    public BuyerService(FileLogger logger, Interaction.FileService fileService) => (_logger, _fileService, _filePaths) = (logger, fileService, new string[]{"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/admin.json","/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/article.json","/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/buyer.json","/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/JsonsFiles/itemPurchased.json"});
    // END Constructor
    
    // All methods => //
    public async Task<string> LogAndConsoleAsync(string message)
    {
        try
        {
            // Display the message =>
            Console.WriteLine(message);
            // Log the message =>
            await _logger.LogAsync(message);
            return message;
        }
        catch (Exception ex)
        {
            return "Error : " + ex.Message;
        }
    }
    public async Task<bool> CreateBuyerAsync(string firstname, string lastname, string address, string phone)
    {
        try
        {
            // Verify if the file exists =>
            if (!_fileService.FileExistsAsync(_filePaths[2]))
                await _fileService.WriteFileAsync(_filePaths[2], "");
            // Create a new buyer =>
            var newBuyer = new Buyer(firstname, lastname, address, phone);
            // Read the current content of the file =>
            string jsonFile = await _fileService.ReadFileAsync(_filePaths[2]);
            // Deserialize the JSON file into a list of Buyer objects =>
            var buyers = string.IsNullOrWhiteSpace(jsonFile) ? new List<Buyer>() : JsonSerializer.Deserialize<List<Buyer>>(jsonFile);
            // Add the new buyer to the list =>
            buyers.Add(newBuyer);
            // Serialize the list of buyers into a JSON string =>
            await _fileService.WriteFileAsync(_filePaths[2], JsonSerializer.Serialize(buyers));
            // Display the message =>
            await LogAndConsoleAsync($"\n----Buyer added\n- Firstname: {newBuyer.Firstname}\n- Lastname: {newBuyer.Lastname}\n- Address: {newBuyer.Adress}\n- Phone: {newBuyer.Phone}\n----");
            return true;
        }
        catch (Exception e)
        {
            // Log the error =>
            await LogAndConsoleAsync("\n----Error when creating a buyer: " + e.Message);
            return false;
        }
    }
    public async Task<bool> ValidateBuyerLogAsync(string firstname, string lastname, string address, string phone)
    {
        try
        {
            // Read the current content of the file asynchronously =>
            string jsonFile = await _fileService.ReadFileAsync(_filePaths[2]);
            // Deserialize the JSON file into a list of Buyer objects =>
            if (string.IsNullOrEmpty(jsonFile)) 
            { 
                await LogAndConsoleAsync("The Json file is empty or missing"); 
                return false; 
            }
            // Create a var to store the list of buyers =>
            var buyers = JsonSerializer.Deserialize<List<Buyer>>(jsonFile);
            // Find the buyer with matching firstname and lastname =>
            if (buyers != null)
            {
                var matchedBuyer = buyers.FirstOrDefault(buyer => 
                    buyer.Firstname.Equals(firstname, StringComparison.OrdinalIgnoreCase) && 
                    buyer.Lastname.Equals(lastname, StringComparison.OrdinalIgnoreCase) &&
                    buyer.Adress.Equals(address, StringComparison.OrdinalIgnoreCase) && 
                    buyer.Phone == phone);
                // Return true if the buyer was found, false otherwise =>
                if (matchedBuyer != null) 
                { 
                    // Display the message =>
                    await LogAndConsoleAsync($"\n----\n\n----> Buyer registered :\n- Firstname : {matchedBuyer.Firstname}\n- Lastname : {matchedBuyer.Lastname}\n- Address : {matchedBuyer.Adress}\n- Phone : {matchedBuyer.Phone}\n----"); 
                    return true; 
                }
            }
            return false;
        }
        catch (Exception e)
        {
            // Log the error =>
            await LogAndConsoleAsync("Error when validating a buyer : " + e.Message);
            return false;
        }
    }
    public async Task<bool> BuyerChooseAnArticleToListAsync(Article newArticle, Buyer newBuyer, string reference, int quantity)
    {
        try
        {
            // Read the current content of the file =>
            string jsonFile = await _fileService.ReadFileAsync(_filePaths[1]);
            // Deserialize the JSON file into a list of Article objects =>
            var articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);
            // Check that the article list is not null before searching for the > reference =>
            if (articles == null) return false;
            // Find the corresponding article by reference (or other unique property) =>
            var selectedArticle = articles.FirstOrDefault(article => article.Reference == reference);
            // Check that the article was found =>
            if (selectedArticle == null)
            {
                await LogAndConsoleAsync($"\n----> Article with reference '{reference}' not found");
                return false;
            }
            // Initialize a file =>
            string path = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/ArticlesPurchasedBuyer/{newBuyer.Firstname}-{DateTime.Now:dd-MM-yyyy-HH-mm}.txt";
            // Create a new invoice =>
            string invoiceContent;
            // Verify if the file exists =>
            if (!_fileService.FileExistsAsync(path))
            {
                invoiceContent = $@"
                    ****************************************
                                PURCHASE INVOICE            
                    ****************************************

                    Buyer : {newBuyer.Firstname}
                    Date  : {DateTime.Now.ToShortDateString()}

                    ****************************************
                              Details of Purchases :
                    ----------------------------------------";
            }
            else
            {
                invoiceContent = "";
            }
            // Calculate the price at purchase =>
            float priceAtPurchase = await CalculatePriceAtPurchase(reference, quantity);
            // Add the article to the invoice =>
            invoiceContent += $@"
                    
                    Référence of article : {selectedArticle.Reference}
                    Price per unit       : {selectedArticle.Price:C}
                    Quantity purchased   : {quantity}
                    Total payable for this article : {priceAtPurchase:C}

                    ----------------------------------------";
            // Write the invoice to the file =>
            await _fileService.AppendFileAsync(path, invoiceContent);
            // Add the price at purchase to the total =>
            _totalPurchases += priceAtPurchase;
            // Display the message =>
            await LogAndConsoleAsync($"---->\n- Reference : {reference}\n- Quantity : {quantity}\n- Price at purchase : {priceAtPurchase}\n- Date of buy : {DateTime.Now}\n----");
            // Add the item purchased to the list =>
            await AddItemPurchasesAsync(new ItemPurchased(selectedArticle.Reference, quantity, priceAtPurchase, DateTime.Now));
            return true;
        }
        catch (Exception e)
        {
            // Log the error =>
            await LogAndConsoleAsync("\n----> Error when choosing an article to list: " + e.Message);
            return false; 
        }
    }
    public async Task FinalizeInvoiceAsync(Buyer newBuyer)
    {
        // Create a file =>
        string path = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/ArticlesPurchasedBuyer/{newBuyer.Firstname}-{DateTime.Now:dd-MM-yyyy-HH-mm}.txt";
        // Verify if the file exists before attempting to write
        if (_fileService.FileExistsAsync(path))
        {
            // Write the total at the end of the invoice
            string totalContent =
                $@"

                Total payable for all articles : {_totalPurchases:C}";
            await _fileService.AppendFileAsync(path, totalContent);
        }
    }
    public async Task<float> CalculatePriceAtPurchase(string reference, int quantity)
    {
        try
        {
            // Read the current content of the file =>
            string jsonFile = await _fileService.ReadFileAsync(_filePaths[1]);
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
            // Log the error =>
            await LogAndConsoleAsync("Error when calculating the price at purchase : " + e.Message);
            return 0.0f;
        }
    }
    public async Task<bool> AddItemPurchasesAsync(ItemPurchased itemPurchased)
    {
        try
        {
            // Verify if the file exists
            if (!_fileService.FileExistsAsync(_filePaths[3]))
            {
                // Use WriteAllTextAsync to create the file with empty content if it does not exist
                await _fileService.WriteFileAsync(_filePaths[3], "");
            }
            // Read the current content of the file asynchronously
            string jsonFile = await _fileService.ReadFileAsync(_filePaths[3]);
            // Deserialize the JSON file into a list of ItemPurchased objects
            var itemsPurchased = string.IsNullOrWhiteSpace(jsonFile) 
                ? new List<ItemPurchased>() 
                : JsonSerializer.Deserialize<List<ItemPurchased>>(jsonFile);
            // Add the new item purchased to the list; if the list is null initialize it
            itemsPurchased ??= new List<ItemPurchased>();
            itemsPurchased.Add(itemPurchased);
            // Serialize the list of items purchased into a JSON string and write asynchronously
            await _fileService.WriteFileAsync(_filePaths[3], JsonSerializer.Serialize(itemsPurchased));
            // Display the message
            await LogAndConsoleAsync($"----\nItem purchased added\n----");
            return true;
        }
        catch (Exception e)
        {
            // Log the error
            await LogAndConsoleAsync($"Error when adding an item purchased: {e.Message}");
            return false;
        }
    }
    public async Task<bool> DisplayListOfArticle()
    {
        try
        {
            int billCount = 1;
            // Read the current content of the file =>
            string jsonFile = await _fileService.ReadFileAsync(_filePaths[1]);
            // Deserialize the JSON file into a list of Article objects =>
            var articles = JsonSerializer.Deserialize<List<Article>>(jsonFile);
            // Check that the article list is not null before searching for the > reference =>
            if (articles == null) return false;
            // Search for the corresponding article =>
            foreach (var article in articles)
                await LogAndConsoleAsync($"\n---->\nArticles N°{billCount}\n- Reference: {article.Reference}\n- Price: {article.Price}\n----");
            billCount++;
            return true;
        }
        catch (Exception e)
        {
            // Log the error =>
            await LogAndConsoleAsync("Error when displaying the list of articles : " + e.Message);
            return false;
        }
    }
    public async Task<bool> DisplayOrderInProgress(Buyer buyer)
    {
        try
        {
            // Create a file =>
            string orderinprogress = $"/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoModels/{buyer.Firstname}-OrderInProgress.txt";
            // Read the current content of the file =>
            string jsonFile = await _fileService.ReadFileAsync(_filePaths[3]);
            // Deserialize the JSON file into a list of ItemPurchased objects =>
            var itemsPurchased = JsonSerializer.Deserialize<List<ItemPurchased>>(jsonFile);
            // Check that the item list is not null before searching for the > reference =>
            if (itemsPurchased == null || itemsPurchased.Count == 0) return false;
            // Find the last item purchased =>
            var lastItem = itemsPurchased.Last();
            // Display the message =>
            await _fileService.AppendFileAsync(orderinprogress,
                "----\n- Reference: " + lastItem.ArticleReference + "\n- Quantity : " +
                lastItem.Quantity + "\n- Price: " + lastItem.PriceAtPurchase + "\n----");
            await LogAndConsoleAsync($"\n----\nLast item purchased:\n- Reference : {lastItem.ArticleReference}\n- Quantity : {lastItem.Quantity}\n- Price : {lastItem.PriceAtPurchase}\n- Date of Purchase : {lastItem.DateofBuy}\n----");
            return true;
        }
        catch (Exception e)
        {
            await LogAndConsoleAsync("----\n Error when displaying the order in progress : " + e.Message);
            return false;
        }
    }
    // END All methods //
}