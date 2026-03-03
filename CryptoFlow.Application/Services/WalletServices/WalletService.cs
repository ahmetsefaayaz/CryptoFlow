using CryptoFlow.Application.Dtos.WalletDtos;
using CryptoFlow.Application.Dtos.WalletItemDto;
using CryptoFlow.Application.Exceptions;
using CryptoFlow.Application.Interfaces.ICrypto;
using CryptoFlow.Application.Interfaces.IDashboard;
using CryptoFlow.Application.Interfaces.IUnitOfWork;
using CryptoFlow.Application.Interfaces.IWallet;
using CryptoFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CryptoFlow.Application.Services.WalletServices;

public class WalletService: IWalletService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User>  _userManager;
    
    public WalletService(IUnitOfWork unitOfWork, UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task <GetWalletDto> GetUsersWalletAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new NotFoundException("User not found");
        var wallet = await _unitOfWork.WalletRepository.GetWalletByUserIdAsync(userId);
        if (wallet == null) throw new NotFoundException("Wallet not found");
        var walletItems = wallet.WalletItems.ToList();
        var walletItemsDto = walletItems.Select(w => new GetWalletItemDto
        {
            Balance = w.Balance,
            CoinId = w.CoinId,
            Id = w.Id,
            Symbol = w.Coin.Symbol
        });
        return new GetWalletDto
        {
            WalletItems = walletItemsDto,
            Id = wallet.Id
        };
        
    }
}