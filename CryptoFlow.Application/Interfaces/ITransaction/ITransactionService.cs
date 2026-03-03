using CryptoFlow.Application.Dtos.TransactionDtos;
using CryptoFlow.Domain.Entities;
using CryptoFlow.Domain.Enums;

namespace CryptoFlow.Application.Interfaces.ITransaction;

public interface ITransactionService
{
    public Task <Transaction> GetTransactionByIdAsync(string transactionId);
    public Task<List<Transaction>> GetAllTransactionsAsync();
    public Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(int page, int pageSize, DateTime startDate, DateTime endDate, TransactionType  type);
    public Task CreateTransactionAsync(CreateTransactionDto dto);
    
}