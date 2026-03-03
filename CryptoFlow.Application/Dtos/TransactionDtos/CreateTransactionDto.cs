using CryptoFlow.Domain.Enums;

namespace CryptoFlow.Application.Dtos.TransactionDtos;

public class CreateTransactionDto
{
    
    
    public Guid WalletId { get; set; }
    public Guid? CoinId { get; set; }
    public decimal Amount { get; set; }
    public decimal UnitPrice { get; set; }
    
    public TransactionType TransactionType { get; set; }
    public string Description { get; set; }
}