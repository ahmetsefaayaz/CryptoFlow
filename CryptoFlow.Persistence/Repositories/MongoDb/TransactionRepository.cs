using CryptoFlow.Application.Interfaces.ITransaction;
using CryptoFlow.Application.Interfaces.MongoDb;
using CryptoFlow.Domain.Entities;
using MongoDB.Driver;

namespace CryptoFlow.Persistence.Repositories.MongoDb;

public class TransactionRepository: MongoGenericRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(IMongoDatabase database) : base(database)
    {
    }

    public async Task<List<Transaction>> GetRecentTransactionsByUserIdAsync(Guid userId, int limit = 5)
    {
        return await _collection
            .Find(t => t.UserId == userId)
            .SortByDescending(t => t.CreatedAt) 
            .Limit(limit)
            .ToListAsync();
    }
}