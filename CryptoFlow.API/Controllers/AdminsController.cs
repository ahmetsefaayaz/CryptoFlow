using CryptoFlow.Application.Interfaces.IAuthentication;
using CryptoFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminsController: ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminsController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("get-roles")]
    public async Task<IActionResult> GetRolesAsync()
    {
        var result = await _adminService.GetRolesAsync();
        return Ok(result);
    }

    [HttpGet("get-role/{id}")]
    public async Task<IActionResult> GetRoleAsync(Guid id)
    {
        var result = await _adminService.GetRoleAsync(id);
        return Ok(result);
    }
    [HttpPost("create-role")]
    public async Task<IActionResult> CreateRoleAsync(string roleName)
    {
        var result = await _adminService.CreateRoleAsync(roleName);
        return Ok(result);
    }

    [HttpDelete("delete-role/{id}")]
    public async Task<IActionResult> DeleteRoleAsync(Guid id)
    {
        var result = await _adminService.DeleteRoleAsync(id);
        return Ok(result);
    }

    [HttpDelete("delete-claim/{id}")]
    public async Task<IActionResult> DeleteClaimAsync(Guid id, string type, string value)
    {
        var result = await _adminService.RemoveClaimAsync(id, type, value);
        return Ok(result);
    }

    [HttpPost("add-claim/{id}")]
    public async Task<IActionResult> AddClaimAsync(Guid id, string type, string value)
    {
        var result = await _adminService.AssignClaimAsync(id, type, value);
        return Ok(result);
    }

    [HttpPost("assign-role/{id}")]
    public async Task<IActionResult> AssignRoleAsync(Guid id, string roleName)
    {
        var result = await _adminService.AssignRoleAsync(id, roleName);
        return Ok(result);
    }
    
    
    
    
}