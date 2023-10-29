using System.Text;

namespace ManagementPeople;
public interface IMStart
{
    string DisplayMenuStart();
    string DisplayFirstStart();
}
public class MStart : IMStart
{
    public string DisplayMenuStart()
    {
        StringBuilder menu = new StringBuilder();
        Console.WriteLine("**************************************");
        Console.WriteLine("       Welcome to ChocoProject");
        Console.WriteLine("**************************************");
        Console.WriteLine("1. Buyer");
        Console.WriteLine("2. Administrator");
        Console.Write("Your choice : ");
        return menu.ToString();
    }
    public string DisplayFirstStart()
    {
        StringBuilder menu = new StringBuilder();
        Console.WriteLine("**************************************");
        Console.WriteLine("The Project kicks off for the first time :)");
        Console.WriteLine("**************************************\n");
        return menu.ToString();
    }
}