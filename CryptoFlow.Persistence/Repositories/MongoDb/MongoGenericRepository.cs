using CryptoFlow.Application.Interfaces.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CryptoFlow.Persistence.Repositories.MongoDb;

public class MongoGenericRepository<T>: IGenericRepository<T> where T: class
{
    public readonly IMongoCollection<T> _collection;
    public MongoGenericRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<T>(typeof(T).Name + "s");
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        return await _collection.Find(Builders<T>.Filter.Eq("_id", ObjectId.Parse(id))).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task CreateAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(string id, T entity)
    {
        await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", ObjectId.Parse(id)), entity);
    }

    public async Task DeleteAsync(string id)
    {
        await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", ObjectId.Parse(id)));
    }
}