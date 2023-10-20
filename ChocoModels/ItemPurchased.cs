namespace ManagementPeople;

public class ItemPurchased
{
    public Guid IDAcheteur { get; set; }
    public Guid IDChocolate { get; set; }
    public int Quantity { get; set; }
    public DateTime DateofBuy { get; set; }
    
    public ItemPurchased(int quantity, DateTime dateofBuy) => ( IDAcheteur, IDChocolate, Quantity, DateofBuy) = (Guid.NewGuid(), Guid.NewGuid(), quantity, dateofBuy);
}