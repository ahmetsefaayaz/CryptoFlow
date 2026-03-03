namespace CryptoFlow.Domain.Entities;

public class Coin: BaseEntity
{
    public string Name { get; set; }
    public string Symbol { get; set; }
    public bool IsActive { get; set; } = true;
    
    
    public virtual ICollection<WalletItem> WalletItems { get; set; }    
}