using CryptoFlow.Application.Dtos.WalletDtos;
using CryptoFlow.Application.Dtos.WalletItemDto;

namespace CryptoFlow.Application.Interfaces.IWallet;

public interface IWalletService
{
    public Task<GetWalletDto> GetUsersWalletAsync (Guid userId);
}