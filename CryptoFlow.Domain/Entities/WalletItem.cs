namespace CryptoFlow.Domain.Entities;

public class WalletItem: BaseEntity
{
    public Guid WalletId { get; set; }
    public virtual Wallet Wallet { get; set; }
    
    public Guid? CoinId { get; set; }
    public virtual Coin Coin { get; set; }
    
    public decimal Balance { get; set; }
    
    
    
}