using CryptoFlow.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CryptoFlow.Domain.Entities;

public class Transaction
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public Guid UserId { get; set; }
    public Guid WalletId { get; set; }
    
    public Guid? CoinId { get; set; }
    
    public decimal Amount { get; set; }
    public decimal UnitPrice { get; set; }
    
    public TransactionType TransactionType { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}