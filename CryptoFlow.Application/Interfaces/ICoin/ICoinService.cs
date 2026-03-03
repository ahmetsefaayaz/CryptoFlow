using System.Runtime.CompilerServices;
using CryptoFlow.Application.Dtos.CoinDtos;
using CryptoFlow.Domain.Entities;

namespace CryptoFlow.Application.Interfaces.ICoin;

public interface ICoinService
{
    public Task <List<GetCoinDto>> GetAllCoinsAsync();
    public Task<GetCoinDto> GetCoinByIdAsync(Guid id);
    public IAsyncEnumerable<GetCoinDto> GetCoinsStreamAsync([EnumeratorCancellation] CancellationToken cancellationToken = default);
}