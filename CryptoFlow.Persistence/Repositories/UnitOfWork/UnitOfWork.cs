using CryptoFlow.Application.Interfaces.ICoin;
using CryptoFlow.Application.Interfaces.IOrder;
using CryptoFlow.Application.Interfaces.IUnitOfWork;
using CryptoFlow.Application.Interfaces.IWallet;
using CryptoFlow.Application.Interfaces.IWalletItem;
using CryptoFlow.Persistence.DbContexts;
using CryptoFlow.Persistence.Repositories.SqlRepositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace CryptoFlow.Persistence.Repositories.UnitOfWork;

public class UnitOfWork: IUnitOfWork
{
    private readonly CryptoDbContext _context;
    private IDbContextTransaction _currentTransaction;
    public UnitOfWork(CryptoDbContext context)
    {
        _context = context;
        WalletRepository = new WalletRepository(_context);
        WalletItemRepository = new WalletItemRepository(_context);
        CoinRepository = new CoinRepository(_context);
        OrderRepository = new OrderRepository(_context);
        
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public IWalletRepository WalletRepository { get; }
    public IWalletItemRepository WalletItemRepository { get; }
    public ICoinRepository CoinRepository { get; }
    public IOrderRepository OrderRepository { get; }

    public Task<int> CommitAsync()
    {
        return _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null) return;
        _currentTransaction = await _context.Database.BeginTransactionAsync();
        
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await CommitAsync(); 
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(); 
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw; 
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
        
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}