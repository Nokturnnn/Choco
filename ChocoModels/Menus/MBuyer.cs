using System.Text;
using System.Text.RegularExpressions;

namespace ManagementPeople;

public interface IMBuyer
{
    Task<string> GetUserInputAsync(string prompt);
    Task<string> DisplayMenuBuyer();
    Task<string> DisplayBuyerRegistered();
    Task<(string reference, int quantity)> AddToListAsync();
    Task<(string firstname, string lastname, string adress, string phone)> BuyerInfosConnectingAsync();
    Task<(string firstname, string lastname, string adress, string phone)> AddRegisterAsync();
    Task<bool> CheckEntriesBuyerAsync(string firstname, string lastname, string adress, string phone);
}
public class MBuyer : IMBuyer
{
    public async Task<string> GetUserInputAsync(string prompt)
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
    public async Task<string> DisplayMenuBuyer()
    {
        try
        {
            StringBuilder menu = new StringBuilder();
            Console.WriteLine("\n----> Menu Buyer :");
            Console.WriteLine("1. Enter your informations");
            Console.WriteLine("2. Please Register");
            Console.WriteLine("Your choice :");
            return menu.ToString();
        }
        catch (Exception e)
        {
            return "Error : " + e.Message;
        }
    }
    public async Task<string> DisplayBuyerRegistered()
    {
        try
        {
            StringBuilder menu = new StringBuilder();
            Console.WriteLine("\n----> Menu Buyer :");
            Console.WriteLine("1. Display article");
            Console.WriteLine("2. Buy an article");
            Console.WriteLine("Your choice :");
            return menu.ToString();
        }
        catch (Exception e)
        {
          return "Error : " + e.Message;
        }
    }
    public async Task<(string reference, int quantity)> AddToListAsync()
    {
        Console.WriteLine("----");
        try
        {
            string reference = await GetUserInputAsync("Enter the reference of the article you want to buy: ");
            // We should use int.TryParse for better error handling
            string quantityString = await GetUserInputAsync("Enter the quantity of the article you want to buy: ");
            bool isValidQuantity = int.TryParse(quantityString, out int quantity);
            if (!isValidQuantity)
            {
                Console.WriteLine("Invalid quantity input. Please enter a valid number.");
                return ("", 0);
            }
            return (reference, quantity);
        }
        catch (Exception e)
        {
            Console.WriteLine("\n----> Error : " + e.Message);
            return ("", 0);
        }
    }
    public async Task<(string firstname, string lastname, string adress, string phone)> BuyerInfosConnectingAsync()
    {
        Console.WriteLine("----");
        try
        {
            string firstname = await GetUserInputAsync("Enter your firstname: ");
            string lastname = await GetUserInputAsync("Enter your lastname: ");
            string adress = await GetUserInputAsync("Enter your adress: ");
            string phone = await GetUserInputAsync("Enter your phone: ");
            return (firstname, lastname, adress, phone);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return ("", "", "", "");
        }
    }
    public async Task<(string firstname, string lastname, string adress, string phone)> AddRegisterAsync()
    {
        Console.WriteLine("----");
        try
        {
            string firstname = await GetUserInputAsync("Enter your firstname: ");
            string lastname = await GetUserInputAsync("Enter your lastname: ");
            string adress = await GetUserInputAsync("Enter your adress: ");
            string phone = await GetUserInputAsync("Enter your phone: ");
            return (firstname, lastname, adress, phone);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error : " + e.Message);
            return ("", "", "", "");
        }
    }
    public Task<bool> CheckEntriesBuyerAsync(string firstname, string lastname, string address, string phone)
    {
        try
        {
            // Check if the fields are empty =>
            if (string.IsNullOrWhiteSpace(firstname) || 
                string.IsNullOrWhiteSpace(lastname) || 
                string.IsNullOrWhiteSpace(address) || 
                string.IsNullOrWhiteSpace(phone))
            {
                Console.WriteLine("Error: You must fill all the fields");
                return Task.FromResult(false);
            }
            // Check if there is =>
            var hasNumber = new Regex(@"\d");
            // Check if the firstname and lastname contains numbers =>
            if (hasNumber.IsMatch(firstname) || hasNumber.IsMatch(lastname))
            {
                Console.WriteLine("Error : Firstname, Lastname cannot contain numbers");
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            return Task.FromResult(false);
        }
    }

}