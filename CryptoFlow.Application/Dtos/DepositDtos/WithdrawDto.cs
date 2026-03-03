namespace CryptoFlow.Application.Dtos.DepositDtos;

public class WithdrawDto
{
    public Guid CoinId { get; set; }
    public decimal Amount { get; set; }
}