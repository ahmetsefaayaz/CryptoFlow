using CryptoFlow.Application.Interfaces.IOrder;
using CryptoFlow.Domain.Entities;
using CryptoFlow.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CryptoFlow.Persistence.Repositories.SqlRepositories;

public class OrderRepository: IOrderRepository
{
    private readonly CryptoDbContext _context;

    public OrderRepository(CryptoDbContext context)
    {
        _context = context;
    }
    
    
    
    
    public async Task<Order?> GetOrderAsync(Guid orderId)
    {
        return await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<List<Order>> GetOrdersAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    public async Task CreateOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task RemoveOrderAsync(Guid orderId)
    {
        var order = await GetOrderAsync(orderId);
        _context.Orders.Remove(order);
    }
}