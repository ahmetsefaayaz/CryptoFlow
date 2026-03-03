using CryptoFlow.Application.Interfaces.ICoin;
using CryptoFlow.Application.Interfaces.IOrder;
using CryptoFlow.Application.Interfaces.IWallet;
using CryptoFlow.Application.Interfaces.IWalletItem;
using CryptoFlow.Domain.Entities;

namespace CryptoFlow.Application.Interfaces.IUnitOfWork;

public interface IUnitOfWork: IDisposable
{
    IWalletRepository WalletRepository { get; }
    IWalletItemRepository WalletItemRepository { get; }
    ICoinRepository CoinRepository { get; }
    IOrderRepository OrderRepository { get; }
    public Task<int> CommitAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}