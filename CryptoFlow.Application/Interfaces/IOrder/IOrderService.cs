using CryptoFlow.Application.Dtos.OrderDtos;
using CryptoFlow.Domain.Entities;

namespace CryptoFlow.Application.Interfaces.IOrder;

public interface IOrderService
{
    public Task<GetOrderDto> GetOrderAsync(Guid id);
    public Task<IEnumerable<GetOrderDto>> GetAllOrdersAsync();
    public Task CreateOrderAsync(Guid userId, CreateOrderDto dto);
}