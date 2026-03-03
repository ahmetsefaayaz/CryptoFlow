using CryptoFlow.Application.Interfaces.IWallet;
using CryptoFlow.Domain.Entities;
using CryptoFlow.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CryptoFlow.Persistence.Repositories.SqlRepositories;

public class WalletRepository: IWalletRepository
{
    private readonly CryptoDbContext _context;
    public WalletRepository(CryptoDbContext context)
    {
        _context = context;
    }


    public async Task<Wallet?> GetWalletByIdAsync(Guid id)
    {
        return await _context.Wallets.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Wallet?> GetWalletByUserIdAsync(Guid userId)
    {
        return await _context.Wallets
            .Include(w => w.WalletItems)
            .ThenInclude(wi => wi.Coin)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<List<Wallet>> GetWalletsAsync()
    {
        return await _context.Wallets.ToListAsync();
    }

    public async Task CreateWalletAsync(Wallet wallet)
    {
        await _context.Wallets.AddAsync(wallet);
    }
}