using System.Text;
using ChocoLog;

namespace ManagementPeople;

public interface IMAdmin
{
    string GetUserInput(string prompt);
    bool LogAndConsole(string message);
    string DisplayMenuAdmin();
    string DisplayMenuAdminConnected();
    (string login, string password) AdminConnecting();
    (string login, string password) RegisterAdmin();
    bool CheckPassword(string password);
    (string reference, float price) AdminAddArticle();
    string AdminRemoveArticle();
    (DateTime startDate, DateTime endDate) AdminAddDate();

}
public class MAdministrator : IMAdmin
{
    private readonly ILogger _logger;
    public string GetUserInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }
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
    public string DisplayMenuAdmin()
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
    public string DisplayMenuAdminConnected()
    {
        Console.WriteLine("----> Menu :");
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
    public (string login, string password) AdminConnecting()
    {
        Console.WriteLine("----");
        try
        {
            string login = GetUserInput("Enter your login: ");
            string password = GetUserInput("Enter your password: ");
            return (login, password);
        }
        catch (Exception e)
        {
            LogAndConsole("Error : " + e.Message);
            return ("", "");
        }
    }
    public (string login, string password) RegisterAdmin()
    {
        Console.WriteLine("----");
        try
        {
            string login = GetUserInput("Enter your login: ");
            string password = GetUserInput("Enter your password: ");
            return (login, password);
        }
        catch (Exception e)
        {
            LogAndConsole("Error : " + e.Message);
            return ("", "");
        }
    }
    public bool CheckPassword(string password)
    {
        try
        {
            string specialCaracter = "@#%&*()-+!";
            if (password.Length >= 6 && password.Any(specialCaracter.Contains))
            {
                LogAndConsole("\n----\n Your password is valid :)");
                return true;
            }
            else
            {
                LogAndConsole("\n----\n Your password is not valid - Retry:");
            }
            return false;
        }
        catch (Exception e)
        {
            LogAndConsole("Error : " + e.Message);
            return false;
        }
    }
    public (string reference, float price) AdminAddArticle()
    {
        Console.WriteLine("----");
        try
        {
            string reference = GetUserInput("Enter the reference of the article: ");
            float price = float.Parse(GetUserInput("Enter the price of the article: "));
            return (reference, price);
        }
        catch (Exception e)
        {
            LogAndConsole("Error : " + e.Message);
            return ("", 0);
        }
    }
    public string AdminRemoveArticle()
    {
        try
        {
            string reference = GetUserInput("Enter the reference of the article: ");
            return (reference);
        }
        catch (Exception e)
        {
            LogAndConsole("Error : " + e.Message);
            return ("");
        }
    }
    public (DateTime startDate, DateTime endDate) AdminAddDate()
    {
        Console.WriteLine("----");
        try
        {
            DateTime startDate = DateTime.Parse(GetUserInput("Enter the start date: "));
            DateTime endDate = DateTime.Parse(GetUserInput("Enter the end date: "));
            string startDateFormatted = startDate.ToString("yyyy/MM/dd");
            string endDateFormatted = endDate.ToString("yyyy/MM/dd");
            return (DateTime.Parse(startDateFormatted), DateTime.Parse(endDateFormatted));
        }
        catch (Exception e)
        {
            LogAndConsole("Error : " + e.Message);
            return (DateTime.Now, DateTime.Now);
        }
    }
}