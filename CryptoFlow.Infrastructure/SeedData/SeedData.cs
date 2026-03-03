using CryptoFlow.Application.Interfaces.IUnitOfWork;
using CryptoFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CryptoFlow.Infrastructure.SeedData;

public static class SeedData
{
    public static async Task SeedRolesAsync(RoleManager<Role> roleManager)
    {
        string[] roleNames = {"Admin", "Trader"};
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new Role
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };
                await roleManager.CreateAsync(role);
            }
        }
    }
    public static async Task SeedAdminAsync(UserManager<User> userManager, IUnitOfWork unitOfWork)
    {
        var existingAdmin = await userManager.FindByEmailAsync("admin@gmail.com");
        if (existingAdmin == null)
        {
            
            var admin = new User
            {
                UserName = "Admin",
                Email = "admin@gmail.com"
            };
            await userManager.CreateAsync(admin, "_AdminPassword0");
            Wallet wallet = new Wallet
            {
                UserId = admin.Id
            };
            await unitOfWork.WalletRepository.CreateWalletAsync(wallet);
            
            await  userManager.AddToRoleAsync(admin, "Admin");
        }
        
    }

    public static async Task SeedCoinsAsync(IUnitOfWork unitOfWork)
    {
        Coin coin = new Coin();
        Coin coin2 = new Coin();
        Coin coin3 = new Coin();
        Coin coin4 = new Coin();
        Coin coin5 = new Coin();

        
        
        if (await unitOfWork.CoinRepository.GetCoinBySymbolAsync("BTC") is  null)
        {
            coin = new Coin
            {
                Symbol = "BTC",
                Name = "Bitcoin"
            };
            await unitOfWork.CoinRepository.CreateCoinAsync(coin);
        }

        if (await unitOfWork.CoinRepository.GetCoinBySymbolAsync("ETH") is null)
        {
            coin2 = new Coin
            {
                Symbol = "ETH",
                Name = "Ethereum"
            };
            await unitOfWork.CoinRepository.CreateCoinAsync(coin2);
        }

        if (await unitOfWork.CoinRepository.GetCoinBySymbolAsync("TRY") is null)
        {
            coin3 = new Coin
            {
                Symbol = "TRY",
                Name = "Turk lirasi"
            };
            await unitOfWork.CoinRepository.CreateCoinAsync(coin3);
        }

        if (await unitOfWork.CoinRepository.GetCoinBySymbolAsync("USD") is null)
        {
            coin4 = new Coin
            {
                Symbol = "USD",
                Name = "Dollar"
            };
            await unitOfWork.CoinRepository.CreateCoinAsync(coin4);
        }
        if (await unitOfWork.CoinRepository.GetCoinBySymbolAsync("SOL") is null)
        {
            coin5 = new Coin
            {
                Symbol = "SOL",
                Name = "Solana"
            };
            await unitOfWork.CoinRepository.CreateCoinAsync(coin5);
        }
        await unitOfWork.CommitAsync();
    }
}