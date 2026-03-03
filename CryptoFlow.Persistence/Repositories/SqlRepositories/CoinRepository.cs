using CryptoFlow.Application.Interfaces.ICoin;
using CryptoFlow.Domain.Entities;
using CryptoFlow.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CryptoFlow.Persistence.Repositories.SqlRepositories;

public class CoinRepository: ICoinRepository
{
    private readonly CryptoDbContext _context;
    public CoinRepository(CryptoDbContext context)
    {
        _context = context;
    }

    public async Task<Coin?> GetCoinByIdAsync(Guid coinId)
    {
        return await _context.Coins.FirstOrDefaultAsync(x => x.Id == coinId);
    }

    public async Task<Coin?> GetCoinBySymbolAsync(string symbol)
    {
        return await _context.Coins.FirstOrDefaultAsync(x => x.Symbol == symbol);
    }

    public async Task<List<Coin>> GetCoinsAsync()
    {
        return await _context.Coins.ToListAsync();
    }

    public async Task CreateCoinAsync(Coin coin)
    {
        await _context.Coins.AddAsync(coin);
    }

    public async Task RemoveCoinAsync(Guid id)
    {
        var  coin = await _context.Coins.FirstOrDefaultAsync(x => x.Id == id);
        _context.Coins.Remove(coin);
    }
}