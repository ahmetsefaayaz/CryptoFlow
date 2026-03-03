using CryptoFlow.Domain.Entities;

namespace CryptoFlow.Application.Interfaces.IOrder;

public interface IOrderRepository
{
    public Task<Order?> GetOrderAsync(Guid orderId);
    public Task<List<Order>> GetOrdersAsync();
    public Task CreateOrderAsync(Order order);
    public Task RemoveOrderAsync(Guid orderId);
}