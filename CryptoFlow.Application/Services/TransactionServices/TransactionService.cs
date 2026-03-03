using CryptoFlow.Application.Dtos.TransactionDtos;
using CryptoFlow.Application.Exceptions;
using CryptoFlow.Application.Interfaces.ITransaction;
using CryptoFlow.Application.Interfaces.MongoDb;
using CryptoFlow.Domain.Entities;
using CryptoFlow.Domain.Enums;

namespace CryptoFlow.Application.Services.TransactionServices;

public class TransactionService: ITransactionService
{
    private readonly IGenericRepository<Transaction> _transactionRepository;

    public TransactionService(IGenericRepository<Transaction> transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(int page, int pageSize, DateTime startDate, DateTime endDate, TransactionType type)
    {
        var transactions = await _transactionRepository.GetAllAsync();
        return transactions
            .Where(t => t.TransactionType == type)
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
    
    public async Task<Transaction> GetTransactionByIdAsync(string transactionId)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        if (transaction == null) throw new NotFoundException("Transaction not found");
        return  transaction;
    }

    public async Task<List<Transaction>> GetAllTransactionsAsync()
    {
        var  transactions = await _transactionRepository.GetAllAsync();
        return transactions.ToList();
    }

    
    public async Task CreateTransactionAsync(CreateTransactionDto dto)
    {
        var transaction = new Transaction
        {
            WalletId = dto.WalletId,
            CoinId = dto.CoinId,
            Amount = dto.Amount,
            UnitPrice = dto.UnitPrice,
            TransactionType = dto.TransactionType,
            Description = dto.Description
        };
        await _transactionRepository.CreateAsync(transaction);
    }

}