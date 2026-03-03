namespace CryptoFlow.Application.Dtos.CoinDtos;

public class GetCoinDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public decimal Price { get; set; }
}