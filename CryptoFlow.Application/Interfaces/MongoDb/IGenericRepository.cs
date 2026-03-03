namespace CryptoFlow.Application.Interfaces.MongoDb;

public interface IGenericRepository<T> where T : class
{
    Task<T?>GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task CreateAsync(T entity);
    Task UpdateAsync(string id, T entity);
    Task DeleteAsync(string id);
}