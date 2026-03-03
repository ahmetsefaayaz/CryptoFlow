using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CryptoFlow.Application.Dtos.AuthDtos;
using CryptoFlow.Application.Exceptions;
using CryptoFlow.Application.Interfaces.IAuthentication;
using CryptoFlow.Application.Interfaces.IUnitOfWork;
using CryptoFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CryptoFlow.Application.Services.AuthenticationServices;

public class AuthService: IAuthService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService( UserManager<User> userManager, IPasswordHasher<User> passwordHasher,
        IConfiguration configuration, IUnitOfWork unitOfWork)
    {
         _userManager = userManager;
         _passwordHasher = passwordHasher;
         _configuration = configuration;
         _unitOfWork = unitOfWork;
    }
    
    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if(user == null) throw new NotFoundException("User not found");
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new WrongInputException("Wrong Password");
        }
        return await GenerateJwtTokenAsync(user);
        
    }

    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if(existingUser != null) throw new WrongInputException("Email already exists");
        var user = new User
        {
            Email = dto.Email,
            UserName = dto.UserName
        };
        if(dto.Password != dto.ConfirmPassword) throw new WrongInputException("Passwords do not match");
        Wallet wallet = new Wallet
        {
            UserId = user.Id
        };
        await _unitOfWork.WalletRepository.CreateWalletAsync(wallet);
        
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"User creation failed: {errors}");
        }
        await _userManager.AddToRoleAsync(user, "Trader");
        
        await _unitOfWork.CommitAsync();
        return "User created";
    }

    private async Task<string> GenerateJwtTokenAsync(User user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        claims.AddRange(userClaims);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}