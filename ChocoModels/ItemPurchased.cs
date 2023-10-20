namespace ManagementPeople;

public class ItemPurchased
{
    public Guid IDAcheteur { get; set; }
    public Guid IDChocolate { get; set; }
    public string ArticleReference { get; set; }
    public int Quantity { get; set; }
    public float PriceAtPurchase { get; set; }
    public DateTime DateofBuy { get; set; }
    
    public ItemPurchased(string articleReference, int quantity, float priceAtPurchase, DateTime dateofBuy) => (IDAcheteur, IDChocolate, ArticleReference, Quantity, PriceAtPurchase, DateofBuy) = (Guid.NewGuid(), Guid.NewGuid(), articleReference, quantity, priceAtPurchase, dateofBuy);
}