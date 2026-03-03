
using CryptoFlow.Domain.Enums;

namespace CryptoFlow.Domain.Entities;

public class Order: BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    
    public Guid PaymentCoinId { get; set; }
    public virtual Coin PaymentCoin { get; set; }
    
    public Guid TargetCoinId { get; set; }
    public virtual Coin TargetCoin { get; set; }
    
    public OrderType OrderType { get; set; }
    
    public Decimal Price { get; set; }
    public Decimal Quantity { get; set; }
    
    public OrderStatus Status { get; set; }
    
}