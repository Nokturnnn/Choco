namespace ManagementPeople;

public class Article
{
    public Guid ID { get; set; }
    public string Reference { get; set; }
    public float Price { get; set; }
    
    public Article(string reference, float price)
    {
        ID = Guid.NewGuid();
        Reference = reference;
        Price = price;
    }
}