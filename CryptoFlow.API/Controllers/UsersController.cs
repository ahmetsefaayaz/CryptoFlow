using CryptoFlow.Application.Dtos.UserDtos;
using CryptoFlow.Application.Interfaces.IUser;
using Microsoft.AspNetCore.Mvc;

namespace CryptoFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController: ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserByIdAsync(Guid id)
    {
        var user = await _userService.GetUserById(id);
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }
    
}