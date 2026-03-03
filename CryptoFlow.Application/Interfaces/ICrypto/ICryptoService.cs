using CryptoFlow.Application.Dtos.CoinDtos;

namespace CryptoFlow.Application.Interfaces.ICrypto;

public interface ICryptoService
{
    Task<Dictionary<string, decimal>> GetLivePricesAsync();
}