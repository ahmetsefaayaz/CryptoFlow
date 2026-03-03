using CryptoFlow.Application.Dtos.OrderDtos;
using CryptoFlow.Application.Exceptions;
using CryptoFlow.Application.Interfaces.ICrypto;
using CryptoFlow.Application.Interfaces.IOrder;
using CryptoFlow.Application.Interfaces.IUnitOfWork;
using CryptoFlow.Application.Interfaces.MongoDb;
using CryptoFlow.Domain.Entities;
using CryptoFlow.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CryptoFlow.Application.Services.OrderServices;

public class OrderService: IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<Transaction>  _transactionRepository;
    private readonly ICryptoService _cryptoService;
    public OrderService(IUnitOfWork unitOfWork, 
        IGenericRepository<Transaction> transactionRepository,
        ICryptoService cryptoService)
    {
        _unitOfWork = unitOfWork;
        _transactionRepository = transactionRepository;
        _cryptoService = cryptoService;
    }
    
    public async Task<Order> GetOrderAsync(Guid id)
    {
        var order = await _unitOfWork.OrderRepository.GetOrderAsync(id);
        if (order == null) throw new NotFoundException("Order not found");
        return order;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        var orders = await _unitOfWork.OrderRepository.GetOrdersAsync();
        return orders;
    }

    public async Task CreateOrderAsync(Guid userId, CreateOrderDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            if (dto.PaymentCoinId == dto.TargetCoinId)
                throw new WrongInputException("Your target order cannot be your payment");
            var wallet = await _unitOfWork.WalletRepository.GetWalletByUserIdAsync(userId);
            if (wallet is null) throw new NotFoundException("Wallet not found");
            var walletItems = await _unitOfWork.WalletItemRepository.GetWalletItemsByWalletIdAsync(wallet.Id);
            if (walletItems is null) throw new NotFoundException("WalletItems not found");

            var paymentCoin = await _unitOfWork.CoinRepository.GetCoinByIdAsync(dto.PaymentCoinId);
            if (paymentCoin is null) throw new NotFoundException("Payment coin not found");
            var targetCoin = await _unitOfWork.CoinRepository.GetCoinByIdAsync(dto.TargetCoinId);
            if (targetCoin is null) throw new NotFoundException("Target coin not found");

            var paymentWalletItem = walletItems.FirstOrDefault(wi => wi.CoinId == paymentCoin.Id);
            var targetWalletItem = walletItems.FirstOrDefault(wi => wi.CoinId == targetCoin.Id);
            
            var livePrices = await _cryptoService.GetLivePricesAsync();
            
            var targetCurrentPrice = livePrices.ContainsKey(targetCoin.Symbol) ? livePrices[targetCoin.Symbol] : 0;
            var paymentCurrentPrice = livePrices.ContainsKey(paymentCoin.Symbol) ? livePrices[paymentCoin.Symbol] : 0;
            
            if (targetCurrentPrice == 0 || paymentCurrentPrice == 0)
                throw new Exception("Canlı fiyatlar alınamadığı için işlem gerçekleştirilemiyor.");
            
            var totalTargetValue = targetCurrentPrice * dto.Quantity;
            var totalPaymentValue = totalTargetValue / paymentCurrentPrice;
            if (dto.OrderType == OrderType.Buy)
            {
                bool isNewTargetWalletItem = false;
                if (targetWalletItem is null)
                {
                    var walletItem = new WalletItem
                    {
                        CoinId = targetCoin.Id,
                        Balance = 0,
                        WalletId = wallet.Id
                    };
                    isNewTargetWalletItem = true;
                    targetWalletItem = walletItem;
                }
                if (paymentWalletItem == null) throw new NotFoundException("Your coin could not found in your wallet");

                if (totalPaymentValue > paymentWalletItem.Balance)
                    throw new WrongInputException("You dont have sufficient payment");
                paymentWalletItem.Balance -= totalPaymentValue;
                targetWalletItem.Balance += dto.Quantity;
                if (isNewTargetWalletItem)
                    await _unitOfWork.WalletItemRepository.CreateWalletItemAsync(targetWalletItem);
                
            }
            else if (dto.OrderType == OrderType.Sell)
            {
                
                if (targetWalletItem == null || targetWalletItem.Balance < dto.Quantity)
                    throw new WrongInputException("You don't have sufficient target coin to sell.");
                targetWalletItem.Balance -= dto.Quantity;
                
                
                bool isNewPaymentWalletItem = false;
                if (paymentWalletItem == null)
                {
                    var walletItem = new WalletItem
                    {
                        CoinId = paymentCoin.Id,
                        Balance = 0,
                        WalletId = wallet.Id
                    };
                    isNewPaymentWalletItem = true;
                    paymentWalletItem = walletItem;
                }
                paymentWalletItem.Balance += totalPaymentValue;
                if(isNewPaymentWalletItem) await _unitOfWork.WalletItemRepository.CreateWalletItemAsync(paymentWalletItem);
                
            }
            var order = new Order
            {
                TargetCoinId = dto.TargetCoinId,
                PaymentCoinId = dto.PaymentCoinId,
                UserId = userId,
                OrderType = dto.OrderType,
                Quantity = dto.Quantity,
                Price = targetCurrentPrice
            };
            var mongoLog = new Transaction
            {
                CoinId = targetCoin.Id,
                Amount = dto.Quantity,
                WalletId = wallet.Id,
                Description = dto.OrderType == OrderType.Buy ? $"Bought {targetCoin.Symbol} with {paymentCoin.Symbol}" : $"Sold {targetCoin.Symbol} for {paymentCoin.Symbol}",
                TransactionType = dto.OrderType == OrderType.Buy ? TransactionType.Buy : TransactionType.Sell,
                UnitPrice = targetCurrentPrice,
                UserId = userId
            };
            await _transactionRepository.CreateAsync(mongoLog);
            await _unitOfWork.OrderRepository.CreateOrderAsync(order);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}