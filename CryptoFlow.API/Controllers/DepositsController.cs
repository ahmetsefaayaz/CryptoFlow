using System.Security.Claims;
using CryptoFlow.Application.Dtos.DepositDtos;
using CryptoFlow.Application.Exceptions;
using CryptoFlow.Application.Interfaces.IUnitOfWork;
using CryptoFlow.Application.Interfaces.IWalletItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepositsController: ControllerBase
{
    private readonly IDepositService _service;

    public DepositsController(IDepositService service)
    {
        _service = service;
    }
    [HttpPost("deposit")]
    public async Task<IActionResult> DepositAsync(DepositDto dto)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdString))
        {
            return Unauthorized("please login first.");
        }
        var userId = Guid.Parse(userIdString);
        await _service.DepositAsync(userId, dto);
        return Ok(new { Message = "Money successfully deposited!" });
    }
    [HttpPost("withdraw")]
    public async Task<IActionResult> WithdrawAsync(WithdrawDto dto)
    {
        var userIdToString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userIdToString == null) throw new WrongInputException("Please login first");
        var userId = Guid.Parse(userIdToString);
        await _service.WithdrawAsync(userId, dto);
        return Ok(new { Message = "Money successfully withdrawed!" });
    }
    
    
}