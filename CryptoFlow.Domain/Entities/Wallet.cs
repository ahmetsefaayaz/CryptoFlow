namespace CryptoFlow.Domain.Entities;

public class Wallet: BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    
    public virtual ICollection<WalletItem> WalletItems { get; set; }
    
    
}