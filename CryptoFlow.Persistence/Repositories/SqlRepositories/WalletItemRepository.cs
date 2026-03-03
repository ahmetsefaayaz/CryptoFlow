using CryptoFlow.Application.Interfaces.IWalletItem;
using CryptoFlow.Domain.Entities;
using CryptoFlow.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CryptoFlow.Persistence.Repositories.SqlRepositories;

public class WalletItemRepository: IWalletItemRepository
{
    private readonly CryptoDbContext _context;

    public WalletItemRepository(CryptoDbContext context)
    {
        _context = context;
    }
    
    
    public async Task<WalletItem?> GetWalletItemAsync(Guid id)
    {
        return await _context.WalletItems.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<WalletItem>> GetWalletItemsByWalletIdAsync(Guid walletId)
    {
        return await _context.WalletItems.Where(x => x.WalletId == walletId).ToListAsync();
    }

    public async Task<IEnumerable<WalletItem>> GetWalletItemsAsync()
    {
        return await _context.WalletItems.ToListAsync();
    }

    public async Task CreateWalletItemAsync(WalletItem item)
    {
        await _context.WalletItems.AddAsync(item);
    }

    public async Task DeleteWalletItemAsync(Guid id)
    {
        var item = await GetWalletItemAsync(id);
        _context.WalletItems.Remove(item);
    }
}