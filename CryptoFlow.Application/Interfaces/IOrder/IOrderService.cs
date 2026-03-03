using CryptoFlow.Application.Dtos.OrderDtos;
using CryptoFlow.Domain.Entities;

namespace CryptoFlow.Application.Interfaces.IOrder;

public interface IOrderService
{
    public Task<Order> GetOrderAsync(Guid id);
    public Task<List<Order>> GetAllOrdersAsync();
    public Task CreateOrderAsync(Guid userId, CreateOrderDto dto);
}