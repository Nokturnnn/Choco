namespace ManagementPeople;
public class MAdministrator
{
    public void DisplayMenuAdmin()
    {
        Console.WriteLine("----");
        Console.WriteLine("Connect or register =>");
        Console.WriteLine("1. Connection");
        Console.WriteLine("2. Register");
        Console.WriteLine("3. Back to menu");
        Console.WriteLine("Your choice :");
    }
    public void DisplayMenuAdminConnected()
    {
        Console.WriteLine("----> Menu :");
        Console.WriteLine("1. Add a article");
        Console.WriteLine("2. Create a bill");
        Console.WriteLine("3. Create a bill for each Buyers");
        Console.WriteLine("4. Create a bill by date of Purchase");
        Console.WriteLine("5. Back to menu");
        Console.WriteLine("6. Exit");
        Console.WriteLine("Your choice :");
    }
    public (string login, string password) AdminConnecting()
    {
        Console.WriteLine("----");
        Console.WriteLine("Enter your login :");
        string login = Console.ReadLine();
        Console.WriteLine("Enter your password :");
        string password = Console.ReadLine();
        return (login, password);
    }
    public (string login, string password) RegisterAdmin()
    {
        Console.WriteLine("----");
        Console.WriteLine("Save your login :");
        string login = Console.ReadLine();
        Console.WriteLine("Save your password :");
        string password = Console.ReadLine();
        return (login, password);
    }
    public bool CheckPassword(string password)
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
    public (string reference, float price) AdminAddArticle()
    {
        Console.WriteLine("Enter the reference of the article :");
        string reference = Console.ReadLine();
        Console.WriteLine("Enter the price of the article :");
        float price = float.Parse(Console.ReadLine());
        Console.WriteLine("----");
        return (reference, price);
    }
    public (DateTime startDate, DateTime endDate) AdminAddDate()
    {
        Console.WriteLine("Enter start date of the purchase (yyyy-MM-dd)");
        string startDateString = Console.ReadLine();
        DateTime startDate = DateTime.Parse(startDateString);
        Console.WriteLine("Enter end date of the purchase (yyyy-MM-dd)");
        string endDateString = Console.ReadLine();
        DateTime endDate = DateTime.Parse(endDateString);
        Console.WriteLine("----");

        // Formate les dates au format "yyyy/MM/dd"
        string startDateFormatted = startDate.ToString("yyyy/MM/dd");
        string endDateFormatted = endDate.ToString("yyyy/MM/dd");

        return (DateTime.Parse(startDateFormatted), DateTime.Parse(endDateFormatted));
    }
}