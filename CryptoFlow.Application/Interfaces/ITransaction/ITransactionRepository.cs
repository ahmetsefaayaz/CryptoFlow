using CryptoFlow.Application.Interfaces.MongoDb;
using CryptoFlow.Domain.Entities;
using MongoDB.Driver;

namespace CryptoFlow.Application.Interfaces.ITransaction;

public interface ITransactionRepository : IGenericRepository<Transaction>
{
    Task<List<Transaction>> GetRecentTransactionsByUserIdAsync(Guid userId, int limit = 5);
}