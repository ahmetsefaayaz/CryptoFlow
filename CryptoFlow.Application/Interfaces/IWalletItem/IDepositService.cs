using CryptoFlow.Application.Dtos.DepositDtos;

namespace CryptoFlow.Application.Interfaces.IWalletItem;

public interface IDepositService
{
    public Task DepositAsync(Guid userId, DepositDto dto);
    public Task WithdrawAsync(Guid userId, WithdrawDto dto);
}