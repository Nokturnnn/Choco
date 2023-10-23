using System.Text;

namespace ManagementPeople;

public class MStart
{
    public string DisplayMenuClearDB()
    {
        StringBuilder menu = new StringBuilder();
        Console.WriteLine("***********************************");
        Console.WriteLine("Welcome to ChocoProject");
        Console.WriteLine("1. Clear All Files");
        Console.WriteLine("2. Access to the menu");
        Console.WriteLine("3. Exit");
        Console.Write("Your choice : ");
        return menu.ToString();
    }
    public string DisplayMenuStart()
    {
        StringBuilder menu = new StringBuilder();
        Console.WriteLine("***********************************");
        Console.WriteLine("Welcome to ChocoProject");
        Console.WriteLine("1. Buyer");
        Console.WriteLine("2. Administrator");
        Console.WriteLine("3. Exit");
        Console.Write("Your choice : ");
        return menu.ToString();
    }
}