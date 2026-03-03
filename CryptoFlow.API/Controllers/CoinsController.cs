using CryptoFlow.Application.Dtos.CoinDtos;
using CryptoFlow.Application.Interfaces.ICoin;
using Microsoft.AspNetCore.Mvc;

namespace CryptoFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoinsController: ControllerBase
{
    private readonly ICoinService _coinService;
    public CoinsController(ICoinService coinService)
    {
        _coinService = coinService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCoinByIdAsync(Guid id)
    {
        var result = await _coinService.GetCoinByIdAsync(id);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetCoinsAsync()
    {
        var result = await _coinService.GetAllCoinsAsync();
        return Ok(result);
    }

    [HttpGet("parallel")]
    public IAsyncEnumerable<GetCoinDto> GetCoinsParallelAsync(CancellationToken token)
    {
        return _coinService.GetCoinsStreamAsync(token);
    }
    
}