using CryptoFlow.Application.Dtos.DepositDtos;
using CryptoFlow.Application.Exceptions;
using CryptoFlow.Application.Interfaces.ICoin;
using CryptoFlow.Application.Interfaces.ICrypto;
using CryptoFlow.Application.Interfaces.IUnitOfWork;
using CryptoFlow.Application.Interfaces.IWalletItem;
using CryptoFlow.Application.Interfaces.MongoDb;
using CryptoFlow.Domain.Entities;
using CryptoFlow.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CryptoFlow.Application.Services.WalletItemServices;

public class DepositService: IDepositService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<Transaction>  _transactionRepository;
    private readonly ICryptoService _cryptoService;
    
    public DepositService(IUnitOfWork unitOfWork,
        IGenericRepository<Transaction> transactionRepository,
        ICryptoService cryptoService)
    {
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _cryptoService = cryptoService;
    }
    public async Task DepositAsync(Guid userId, DepositDto dto)
    {
        var coin = await _unitOfWork.CoinRepository.GetCoinByIdAsync(dto.CoinId);
        if (coin == null) throw new NotFoundException("Coin not found");

        var wallet = await _unitOfWork.WalletRepository.GetWalletByUserIdAsync(userId);
        if (wallet == null) throw new NotFoundException("Wallet not found for this user");

        var existingWalletItem = wallet.WalletItems.FirstOrDefault(wi => wi.CoinId == dto.CoinId);
        if (existingWalletItem != null)
        {
            existingWalletItem.Balance += dto.Amount;
        }
        else
        {
            var newWalletItem = new WalletItem
            {
                CoinId = coin.Id,
                Balance = dto.Amount,
                WalletId = wallet.Id
            };
            await _unitOfWork.WalletItemRepository.CreateWalletItemAsync(newWalletItem);
        }

        var livePrices = await _cryptoService.GetLivePricesAsync();
        var currentPrice = livePrices.ContainsKey(coin.Symbol) ? livePrices[coin.Symbol] : 0;

        var transaction = new Transaction
        {
            CoinId = coin.Id,
            Amount = dto.Amount,
            WalletId = wallet.Id,
            Description = $"{coin.Symbol} deposited",
            TransactionType = TransactionType.Deposit,
            UnitPrice = currentPrice,
            UserId = userId
        };
    
        await _transactionRepository.CreateAsync(transaction); 
        await _unitOfWork.CommitAsync(); 
    }

    public async Task WithdrawAsync(Guid userId, WithdrawDto dto)
    {
        var coin = await _unitOfWork.CoinRepository.GetCoinByIdAsync(dto.CoinId);
        if (coin == null) throw new NotFoundException("Coin not found");
        var wallet = await _unitOfWork.WalletRepository.GetWalletByUserIdAsync(userId);
        if (wallet == null) throw new NotFoundException("Wallet not found for this user");
        var existingWalletItem = wallet.WalletItems.FirstOrDefault(wi => wi.CoinId == coin.Id);
        if (existingWalletItem != null && existingWalletItem.Balance > dto.Amount)
        {
            existingWalletItem.Balance -= dto.Amount;
        }
        else throw new WrongInputException("Not enough money");
        
        var livePrices = await _cryptoService.GetLivePricesAsync();
        var currentPrice = livePrices.ContainsKey(coin.Symbol) ? livePrices[coin.Symbol] : 0;

        var transaction = new Transaction
        {
            CoinId = coin.Id,
            Amount = dto.Amount,
            WalletId = wallet.Id,
            Description = $"{coin.Symbol} withdrawed",
            TransactionType = TransactionType.Withdraw,
            UnitPrice = currentPrice,
            UserId = userId
        };
    
        await _transactionRepository.CreateAsync(transaction); 
        await _unitOfWork.CommitAsync(); 
        
    }
}