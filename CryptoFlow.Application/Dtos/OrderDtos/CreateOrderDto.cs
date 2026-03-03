using CryptoFlow.Domain.Enums;

namespace CryptoFlow.Application.Dtos.OrderDtos;

public class CreateOrderDto
{
    public Guid PaymentCoinId { get; set; }
    public Guid TargetCoinId { get; set; }
    public OrderType OrderType { get; set; }
    
    public Decimal Quantity { get; set; }
    
    
}