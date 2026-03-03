using System.Security.Claims;
using CryptoFlow.Application.Interfaces.IDashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardsController:ControllerBase
{
    private readonly IDashboardService _service;
    public DashboardsController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboardAsync()
    {
        
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdString))
        {
            return Unauthorized("Kullanıcı kimliği doğrulanamadı. Lütfen tekrar giriş yapın.");
        }
        var userId = Guid.Parse(userIdString);
        var result = await _service.GetDashboardAsync(userId);
        return Ok(result);
    }

}