using CryptoFlow.Application.Dtos.WalletItemDto;

namespace CryptoFlow.Application.Dtos.WalletDtos;

public class GetWalletDto
{
    public Guid Id { get; set; }
    public IEnumerable<GetWalletItemDto> WalletItems { get; set; }
}