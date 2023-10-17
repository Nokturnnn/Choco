namespace ManagementPeople;

public class MBuyer
{
    public void DisplayMenuBuyer()
    {
        Console.WriteLine("----");
        Console.WriteLine("1. Enter your informations");
        Console.WriteLine("2. Make your shopping");
        Console.WriteLine("3. Exit");
        Console.WriteLine("Your choice :");
    }
    public void DisplayBuyerRegistered()
    {
        Console.WriteLine("----> Menu Buyer :");
        Console.WriteLine("1. Buy an article");
        Console.WriteLine("2. Display article");
        Console.WriteLine("3. Back to the Menu");
        Console.WriteLine("4. Exit");
        Console.WriteLine("Your choice :");
    }
    public (string reference, int quantity) AddToList()
    {
        Console.WriteLine("----");
        Console.WriteLine("Enter the reference of the article you want to buy :");
        string reference = Console.ReadLine();
        Console.WriteLine("Enter the quantity of the article you want to buy :");
        int quantity = int.Parse(Console.ReadLine());
        return (reference, quantity);
    }
    public (string firstname, string lastname, string adress, int phone) AddRegister()
    {
        Console.WriteLine("----");
        Console.WriteLine("Enter your firstname: ");
        string firstname = Console.ReadLine();
        Console.WriteLine("Enter your lastname :");
        string lastname = Console.ReadLine();
        Console.WriteLine("Enter your adress :");
        string adress = Console.ReadLine();
        Console.WriteLine("Enter your phone :");
        string phone = Console.ReadLine();
        return (firstname, lastname, adress, int.Parse(phone));
    }
}