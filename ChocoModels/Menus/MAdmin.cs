using System.Text;

namespace ManagementPeople;
public class MAdministrator
{
    public string GetUserInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }
    public string DisplayMenuAdmin()
    {
        Console.WriteLine("----");
        try
        {
            StringBuilder menu = new StringBuilder();
            Console.WriteLine("Connect or register =>");
            Console.WriteLine("1. Connection");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. Back to menu");
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
            Console.WriteLine("6. Back to menu");
            Console.WriteLine("7. Exit");
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
            string login = GetUserInput("Enter your login : ");
            string password = GetUserInput("Enter your password : ");
            return (login, password);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return ("", "");
        }
    }
    public (string login, string password) RegisterAdmin()
    {
        Console.WriteLine("----");
        try
        {
            string login = GetUserInput("Enter your login : ");
            string password = GetUserInput("Enter your password : ");
            return (login, password);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
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
                Console.WriteLine("----");
                Console.WriteLine("Your password is valid");
                return true;
            }
            else
            {
                Console.WriteLine("----");
                Console.WriteLine("Your password is not valid - Retry :");
            }
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return false;
        }
    }
    public (string reference, float price) AdminAddArticle()
    {
        Console.WriteLine("----");
        try
        {
            string reference = GetUserInput("Enter the reference of the article : ");
            float price = float.Parse(GetUserInput("Enter the price of the article : "));
            return (reference, price);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return ("", 0);
        }
    }
    public string AdminRemoveArticle()
    {
        try
        {
            string reference = GetUserInput("Enter the reference of the article : ");
            return (reference);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return ("");
        }
    }
    public (DateTime startDate, DateTime endDate) AdminAddDate()
    {
        Console.WriteLine("----");
        try
        {
            DateTime startDate = DateTime.Parse(GetUserInput("Enter the start date : "));
            DateTime endDate = DateTime.Parse(GetUserInput("Enter the end date : "));
            string startDateFormatted = startDate.ToString("yyyy/MM/dd");
            string endDateFormatted = endDate.ToString("yyyy/MM/dd");
            return (DateTime.Parse(startDateFormatted), DateTime.Parse(endDateFormatted));
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return (DateTime.Now, DateTime.Now);
        }
    }
}