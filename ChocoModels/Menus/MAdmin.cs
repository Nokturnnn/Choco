using System.Text;
using ChocoLog;

namespace ManagementPeople;

public interface IMAdmin
{
    Task<string> GetUserInputAsync(string prompt);
    Task<string> LogAndConsoleAsync(string message);
    Task<string> DisplayMenuAdminAsync();
    Task<string> DisplayMenuAdminConnectedAsync();
    Task<(string login, string password)> AdminConnectingAsync();
    Task<(string login, string password)> RegisterAdminAsync();
    Task<bool> CheckPasswordAsync(string password);
    Task<(string reference, float price)> AdminAddArticleAsync();
    Task<string> AdminRemoveArticle();
    Task<(DateTime startDate, DateTime endDate)> AdminAddDateAsync();
}
public class MAdministrator : IMAdmin
{
    private readonly ILogger _logger;
    public async Task<string> GetUserInputAsync(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }
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
    public async Task<string> DisplayMenuAdminAsync()
    {
        Console.WriteLine("----");
        try
        {
            StringBuilder menu = new StringBuilder();
            Console.WriteLine("\nConnect or register =>");
            Console.WriteLine("1. Connection");
            Console.WriteLine("2. Register");
            Console.WriteLine("Your choice :");
            return menu.ToString();
        }
        catch (Exception e)
        {
            return "Error : " + e.Message;
        }
    }
    public async Task<string> DisplayMenuAdminConnectedAsync()
    {
        Console.WriteLine("\n----> Menu :");
        try
        {
            StringBuilder menu = new StringBuilder();
            Console.WriteLine("1. Add a article");
            Console.WriteLine("2. Remove a article");
            Console.WriteLine("3. Create a bill");
            Console.WriteLine("4. Create a bill for each Buyers");
            Console.WriteLine("5. Create a bill by date of Purchase");
            Console.WriteLine("Your choice :");
            return menu.ToString();
        }
        catch (Exception e)
        {
            return "Error : " + e.Message;
        }
    }
    public async Task<(string login, string password)> AdminConnectingAsync()
    {
        Console.WriteLine("----");
        try
        {
            string login = await GetUserInputAsync("Enter your login: ");
            string password = await GetUserInputAsync("Enter your password: ");
            return (login, password);
        }
        catch (Exception e)
        {
            await LogAndConsoleAsync("Error : " + e.Message);
            return ("", "");
        }
    }
    public async Task<(string login, string password)> RegisterAdminAsync()
    {
        Console.WriteLine("----");
        try
        {
            string login = await GetUserInputAsync("Enter your login: ");
            string password = await GetUserInputAsync("Enter your password: ");
            return (login, password);
        }
        catch (Exception e)
        {
            await LogAndConsoleAsync("Error : " + e.Message);
            return ("", "");
        }
    }
    public async Task<bool> CheckPasswordAsync(string password)
    {
        try
        {
            string specialCaracter = "@#%&*()-+!";
            if (password.Length >= 6 && password.Any(specialCaracter.Contains))
            {
                await LogAndConsoleAsync("\n----\n Your password is valid :)");
                return true;
            }
            else
            {
                await LogAndConsoleAsync("\n----\n Your password is not valid - Retry:");
            }
            return false;
        }
        catch (Exception e)
        {
            await LogAndConsoleAsync("Error : " + e.Message);
            return false;
        }
    }
    public async Task<(string reference, float price)> AdminAddArticleAsync()
    {
        Console.WriteLine("----");
        try
        {
            string reference = await GetUserInputAsync("Enter the reference of the article: ");
        
            string priceInput = await GetUserInputAsync("Enter the price of the article: ");
            bool isValidPrice = float.TryParse(priceInput, out float price);
            if (!isValidPrice)
            {
                await LogAndConsoleAsync("Invalid price input. Please enter a valid number.");
                return (reference, 0);
            }

            return (reference, price);
        }
        catch (Exception e)
        {
            await LogAndConsoleAsync("Error : " + e.Message);
            return ("", 0);
        }
    }
    public async Task<string> AdminRemoveArticle()
    {
        try
        {
            string reference = await GetUserInputAsync("Enter the reference of the article: ");
            return (reference);
        }
        catch (Exception e)
        {
            await LogAndConsoleAsync("Error : " + e.Message);
            return ("");
        }
    }
    public async Task<(DateTime startDate, DateTime endDate)> AdminAddDateAsync()
    {
        Console.WriteLine("----");
        try
        {
            string startDateInput = await GetUserInputAsync("Enter the start date (yyyy/MM/dd): ");
            bool isStartValid = DateTime.TryParse(startDateInput, out DateTime startDate);
            if (!isStartValid)
            {
                await LogAndConsoleAsync("Invalid start date input. Please enter the date in the format yyyy/MM/dd.");
                return (DateTime.Now, DateTime.Now);
            }

            string endDateInput = await GetUserInputAsync("Enter the end date (yyyy/MM/dd): ");
            bool isEndValid = DateTime.TryParse(endDateInput, out DateTime endDate);
            if (!isEndValid)
            {
                await LogAndConsoleAsync("Invalid end date input. Please enter the date in the format yyyy/MM/dd.");
                return (DateTime.Now, DateTime.Now);
            }

            return (startDate, endDate);
        }
        catch (Exception e)
        {
            await LogAndConsoleAsync("Error : " + e.Message);
            return (DateTime.Now, DateTime.Now);
        }
    }

}