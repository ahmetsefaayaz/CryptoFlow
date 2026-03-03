using CryptoFlow.Application.Dtos.AuthDtos;
using CryptoFlow.Application.Interfaces.IAuthentication;
using Microsoft.AspNetCore.Mvc;

namespace CryptoFlow.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return Ok(result);
    }
    
}