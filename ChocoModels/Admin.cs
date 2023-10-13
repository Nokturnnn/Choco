namespace ManagementPeople;

public class Admin
{
    public Guid ID { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    
    public Admin(string login, string password)
    {
        ID = Guid.NewGuid();
        Login = login;
        Password = password;
    }
}