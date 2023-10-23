using System.Text;

namespace ManagementPeople;

public class MBuyer
{
    private string GetUserInput(string prompt)
    {
        try
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return "";
        }
    }
    public string DisplayMenuBuyer()
    {
        try
        {
            StringBuilder menu = new StringBuilder(); 
            Console.WriteLine("----");
            Console.WriteLine("1. Enter your informations");
            Console.WriteLine("2. Please Register");
            Console.WriteLine("3. Back to menu");
            Console.WriteLine("Your choice :");
            return menu.ToString();
        }
        catch (Exception e)
        {
            return "Error : " + e.Message;
        }
    }
    public string DisplayBuyerRegistered()
    {
        try
        {
            StringBuilder menu = new StringBuilder();
            Console.WriteLine("----> Menu Buyer :");
            Console.WriteLine("1. Display article");
            Console.WriteLine("2. Buy an article");
            Console.WriteLine("3. Back to the Menu");
            Console.WriteLine("Your choice :");
            return menu.ToString();
        }
        catch (Exception e)
        {
          return "Error : " + e.Message;
        }
    }
    public (string reference, int quantity) AddToList()
    {
        Console.WriteLine("----");
        try
        {
            string reference = GetUserInput("Enter the reference of the article you want to buy: ");
            int quantity = int.Parse(GetUserInput("Enter the quantity of the article you want to buy: "));
            return (reference, quantity);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return ("", 0);
        }
    }
    public (string firstname, string lastname) BuyerInfosConnecting()
    {
        Console.WriteLine("----");
        try
        {
            string firstname = GetUserInput("Enter your firstname: ");
            string lastname = GetUserInput("Enter your lastname: ");
            return (firstname, lastname);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return ("", "");
        }
    }
    public (string firstname, string lastname, string adress, string phone) AddRegister()
    {
        Console.WriteLine("----");
        try
        {
            string firstname = GetUserInput("Enter your firstname: ");
            string lastname = GetUserInput("Enter your lastname: ");
            string adress = GetUserInput("Enter your address: ");
            string phone = GetUserInput("Enter your phone: ");
            return (firstname, lastname, adress, phone);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return ("", "", "", "");
        }
    }
}