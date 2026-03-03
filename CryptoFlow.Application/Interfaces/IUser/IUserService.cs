using CryptoFlow.Application.Dtos.UserDtos;

namespace CryptoFlow.Application.Interfaces.IUser;

public interface IUserService
{
    public Task<GetUserDto> GetUserById(Guid id);
    public Task<List<GetUserDto>> GetAllUsers();
}