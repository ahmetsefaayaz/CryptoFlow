using CryptoFlow.Domain.Entities;

namespace CryptoFlow.Application.Interfaces.IWalletItem;

public interface IWalletItemRepository
{
    public Task<WalletItem?> GetWalletItemAsync(Guid id);
    public Task<List<WalletItem>> GetWalletItemsByWalletIdAsync(Guid walletId);
    public Task<IEnumerable<WalletItem>> GetWalletItemsAsync();
    public Task CreateWalletItemAsync(WalletItem item);
    public Task DeleteWalletItemAsync(Guid id);
}