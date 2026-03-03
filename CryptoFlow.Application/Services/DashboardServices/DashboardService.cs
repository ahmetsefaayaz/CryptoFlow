using CryptoFlow.Application.Dtos.CoinDtos;
using CryptoFlow.Application.Dtos.DashboardDtos;
using CryptoFlow.Application.Dtos.WalletItemDto;
using CryptoFlow.Application.Interfaces.ICrypto;
using CryptoFlow.Application.Interfaces.IDashboard;
using CryptoFlow.Application.Interfaces.ITransaction;
using CryptoFlow.Application.Interfaces.IUnitOfWork;
using CryptoFlow.Application.Interfaces.MongoDb;
using CryptoFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CryptoFlow.Application.Services.DashboardServices;

public class DashboardService: IDashboardService
{
    private readonly ICryptoService _cryptoService;
    private readonly IDashboardRepository _repository;
    private readonly ITransactionRepository _transactionRepository;

    public DashboardService(ITransactionRepository transactionRepository,
        IDashboardRepository repository,
        ICryptoService cryptoService)
    {
        _transactionRepository = transactionRepository;
        _repository = repository;
        _cryptoService = cryptoService;
    }


    public async Task<UserDashboardDto> GetDashboardAsync(Guid userId)
    {
        var walletTask = _repository.GetUserWalletItemsReadOnlyAsync(userId);
        var coinTask = _repository.GetMarketCoinsReadOnlyAsync();
        var transactionsTask = _transactionRepository.GetRecentTransactionsByUserIdAsync(userId);
        
        var pricesTask = _cryptoService.GetLivePricesAsync();
        await Task.WhenAll(walletTask, coinTask, transactionsTask, pricesTask);
        
        var livePrices = pricesTask.Result;
        var walletItems = walletTask.Result;
        var coins = coinTask.Result;
        var transactions =  transactionsTask.Result;

        var walletItemDtos = walletItems.Select(w => new GetWalletItemDto
        {
            Balance = w.Balance,
            CoinId = w.CoinId,
            Id = w.Id,
            Symbol = w.Coin.Symbol
        });
        
        decimal totalRevenue = 0;
        foreach (var walletItem in walletItems)
        {
            var currentPrice = livePrices.ContainsKey(walletItem.Coin.Symbol) ? livePrices[walletItem.Coin.Symbol] : 0;
            totalRevenue += (walletItem.Balance * currentPrice);
        }
        
        var topCoins = coins.Select(c => new TopCoinDto
        {
            Id = c.Id,
            Symbol = c.Symbol,
            Name = c.Name,
            CurrentPrice = livePrices.ContainsKey(c.Symbol) ? livePrices[c.Symbol] : 0
        })
        .OrderByDescending(t => t.CurrentPrice)
        .Take(3)
        .ToList();
        return new UserDashboardDto
        {
            WalletItems = walletItemDtos,
            Top3Coins = topCoins,
            Transactions = transactions,
            TotalRevenue = totalRevenue
        };

    }
}