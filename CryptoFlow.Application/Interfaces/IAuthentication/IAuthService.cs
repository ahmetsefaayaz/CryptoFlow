using CryptoFlow.Application.Dtos.AuthDtos;

namespace CryptoFlow.Application.Interfaces.IAuthentication;

public interface IAuthService
{
    public Task <string > LoginAsync(LoginDto dto);
    public Task <string > RegisterAsync(RegisterDto dto);
    
    
}