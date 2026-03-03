using System.Security.Claims;
using CryptoFlow.Application.Exceptions;
using CryptoFlow.Application.Interfaces.IWallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletsController: ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletsController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWallet()
    {
        var userIdToString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdToString == null) throw new WrongInputException("Once giris yapmaniz lazim");
        var userId = Guid.Parse(userIdToString);
        var result = await _walletService.GetUsersWalletAsync(userId);
        return Ok(result);
    }
    
}