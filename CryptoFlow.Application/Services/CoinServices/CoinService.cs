using System.Runtime.CompilerServices;
using CryptoFlow.Application.Dtos.CoinDtos;
using CryptoFlow.Application.Exceptions;
using CryptoFlow.Application.Interfaces.ICoin;
using CryptoFlow.Application.Interfaces.ICrypto;
using CryptoFlow.Application.Interfaces.IUnitOfWork;

namespace CryptoFlow.Application.Services.CoinServices;

public class CoinService: ICoinService
{
    
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICryptoService _cryptoService;

    public CoinService(IUnitOfWork unitOfWork,  ICryptoService cryptoService)
    {
        _unitOfWork = unitOfWork;
        _cryptoService = cryptoService;
    }

    public async Task<List<GetCoinDto>> GetAllCoinsAsync()
    {
        var coins = await _unitOfWork.CoinRepository.GetCoinsAsync();
        
        var livePrices = await _cryptoService.GetLivePricesAsync();
        
        
        var getCoinDtos = coins.Select(c => new GetCoinDto
            {
                Id = c.Id,
                Name = c.Name,
                Symbol = c.Symbol,
                Price = livePrices.ContainsKey(c.Symbol) ? livePrices[c.Symbol] : 0
            })
            .OrderBy(o => o.Name)
            .ToList();
        return getCoinDtos;
    }

    public async Task<GetCoinDto> GetCoinByIdAsync(Guid id)
    {
        var coin = await _unitOfWork.CoinRepository.GetCoinByIdAsync(id);
        if(coin == null) throw new NotFoundException("Coin not found");
        
        var livePrices = await _cryptoService.GetLivePricesAsync();
        
        var getCoin = new GetCoinDto
        {
            Id = coin.Id,
            Name = coin.Name,
            Symbol = coin.Symbol,
            Price =  livePrices.ContainsKey(coin.Symbol) ? livePrices[coin.Symbol] : 0
        };
        return getCoin;
        
    }

    public async IAsyncEnumerable<GetCoinDto> GetCoinsStreamAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var coins = await _unitOfWork.CoinRepository.GetCoinsAsync();
        while (!cancellationToken.IsCancellationRequested)
        {
            var livePrices = await _cryptoService.GetLivePricesAsync();
            foreach (var coin in coins)
            {
                var dto = new GetCoinDto
                {
                    Id = coin.Id,
                    Name = coin.Name,
                    Symbol = coin.Symbol,
                    Price = livePrices.ContainsKey(coin.Symbol) ? livePrices[coin.Symbol] : 0
                };
                yield return dto;
            }
            try
            {
                await Task.Delay(1000, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
        
    }
}