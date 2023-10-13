namespace ManagementPeople;

public class Buyer
{
    public Guid ID { get; set; }
    public string Adress { get; set; }
    public string Lastname { get; set; }
    public string Firstname { get; set; }
    public int Phone { get; set; }
    
    public Buyer(string adress, string lastname, string firstname, int phone)
    {
        ID = Guid.NewGuid();
        Adress = adress;
        Lastname = lastname;
        Firstname = firstname;
        Phone = phone;
    }
}