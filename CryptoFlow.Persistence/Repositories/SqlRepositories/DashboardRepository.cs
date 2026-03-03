using CryptoFlow.Application.Interfaces.IDashboard;
using CryptoFlow.Domain.Entities;
using CryptoFlow.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CryptoFlow.Persistence.Repositories.SqlRepositories;

public class DashboardRepository: IDashboardRepository
{
    private readonly IDbContextFactory<CryptoDbContext> _contextFactory;

    public DashboardRepository(IDbContextFactory<CryptoDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<List<WalletItem>> GetUserWalletItemsReadOnlyAsync(Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.WalletItems
            .AsNoTracking()
            .Include(w => w.Coin)
            .Where(w => w.Wallet.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Coin>> GetMarketCoinsReadOnlyAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Coins
            .AsNoTracking()
            .ToListAsync();
    }
}