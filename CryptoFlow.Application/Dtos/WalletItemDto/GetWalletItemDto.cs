namespace CryptoFlow.Application.Dtos.WalletItemDto;

public class GetWalletItemDto
{
    public Guid Id { get; set; }
    public Guid? CoinId { get; set; }
    public string Symbol { get; set; }
    public decimal Balance { get; set; }
}