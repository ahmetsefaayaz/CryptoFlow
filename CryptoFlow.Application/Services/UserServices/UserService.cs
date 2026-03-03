using CryptoFlow.Application.Dtos.UserDtos;
using CryptoFlow.Application.Exceptions;
using CryptoFlow.Application.Interfaces.IUser;
using CryptoFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CryptoFlow.Application.Services.UserServices;

public class UserService: IUserService
{
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<GetUserDto> GetUserById(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) throw new NotFoundException("User not found");
        var claims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var userDto = new GetUserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.UserName,
            RoleDto = roles.Select(r => new GetRoleDto
            {
                RoleName = r
            }),
            ClaimDto = claims.Select(c => new GetClaimDto
            {
                ClaimType = c.Type,
                ClaimValue = c.Value
            }),
            CreatedAt = user.CreatedAt
        };
        return userDto;

    }

    public async Task<List<GetUserDto>> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<GetUserDto>();

        foreach (var u in users)
        {
            var claims = await _userManager.GetClaimsAsync(u);
            var roles = await _userManager.GetRolesAsync(u);

            userDtos.Add(new GetUserDto
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.UserName,
                CreatedAt = u.CreatedAt,
            
                ClaimDto = claims.Select(c => new GetClaimDto
                {
                    ClaimType = c.Type,
                    ClaimValue = c.Value
                }),
                RoleDto = roles.Select(r => new GetRoleDto
                {
                    RoleName = r
                })
            });
        }

        return userDtos;
        
    }
}