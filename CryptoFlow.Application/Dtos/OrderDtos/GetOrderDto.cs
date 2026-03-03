using CryptoFlow.Domain.Enums;

namespace CryptoFlow.Application.Dtos.OrderDtos;

public class GetOrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PaymentCoinId { get; set; }
    public Guid TargetCoinId { get; set; }
    public OrderType OrderType { get; set; }
    public Decimal Price { get; set; }
    public Decimal Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
}