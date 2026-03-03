using CryptoFlow.Application.Dtos.WalletItemDto;

namespace CryptoFlow.Application.Interfaces.IWallet;

public interface IWalletService
{
    public Task<IEnumerable<GetWalletItemDto>> GetUsersWalletAsync (Guid userId);
}