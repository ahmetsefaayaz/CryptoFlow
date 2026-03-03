using CryptoFlow.Domain.Entities;

namespace CryptoFlow.Application.Interfaces.IWallet;

public interface IWalletRepository
{
    public Task<Wallet?> GetWalletByIdAsync(Guid id);
    public Task<Wallet?> GetWalletByUserIdAsync(Guid userId);
    public Task<List<Wallet>> GetWalletsAsync();
    public Task CreateWalletAsync(Wallet wallet);
}