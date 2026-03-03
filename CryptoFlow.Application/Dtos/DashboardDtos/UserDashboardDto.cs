using CryptoFlow.Application.Dtos.CoinDtos;
using CryptoFlow.Domain.Entities;

namespace CryptoFlow.Application.Dtos.DashboardDtos;

public class UserDashboardDto
{
    public List<TopCoinDto> Top3Coins { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<WalletItem> WalletItems { get; set; }
    public IEnumerable<Transaction> Transactions { get; set; }
    
}