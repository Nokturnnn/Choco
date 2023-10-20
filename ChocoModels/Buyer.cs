namespace ManagementPeople;

public class Buyer
{
    public Guid ID { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Adress { get; set; }
    public string Phone { get; set; }
    
    public Buyer(string firstname, string lastname, string adress, string phone) => (ID, Firstname, Lastname, Adress, Phone) = (Guid.NewGuid(), firstname, lastname, adress, phone);
    
}