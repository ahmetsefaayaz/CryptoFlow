using CryptoFlow.Domain.Entities;

namespace CryptoFlow.Application.Interfaces.ICoin;

public interface ICoinRepository
{
    public Task<Coin?> GetCoinByIdAsync(Guid coinId);
    public Task<Coin?> GetCoinBySymbolAsync(string symbol);
    public Task<List<Coin>> GetCoinsAsync();
    public Task CreateCoinAsync(Coin coin);
    public Task RemoveCoinAsync(Guid id);
}