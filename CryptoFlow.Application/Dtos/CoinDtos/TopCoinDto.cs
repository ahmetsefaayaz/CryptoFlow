namespace CryptoFlow.Application.Dtos.CoinDtos;

public class TopCoinDto
{
    public Guid Id  { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public decimal CurrentPrice { get; set; }
}