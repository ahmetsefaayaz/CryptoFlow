using CryptoFlow.Domain.Entities;

namespace CryptoFlow.Application.Interfaces.IDashboard;

public interface IDashboardRepository
{
    Task<List<WalletItem>> GetUserWalletItemsReadOnlyAsync(Guid userId);
    Task<List<Coin>> GetMarketCoinsReadOnlyAsync();
}