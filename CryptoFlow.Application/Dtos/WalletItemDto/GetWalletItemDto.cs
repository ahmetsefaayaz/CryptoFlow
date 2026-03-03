namespace CryptoFlow.Application.Dtos.WalletItemDto;

public class GetWalletItemDto
{
    
    public Guid? CoinId { get; set; }
    public decimal Balance { get; set; }
}