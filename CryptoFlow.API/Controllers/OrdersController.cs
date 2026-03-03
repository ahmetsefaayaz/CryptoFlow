using System.Security.Claims;
using CryptoFlow.Application.Dtos.OrderDtos;
using CryptoFlow.Application.Interfaces.IOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoFlow.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController: ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderByIdAsync(Guid id)
    {
        var order =  await _orderService.GetOrderAsync(id);
        return Ok(order);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetOrdersAsync()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdString))
        {
            return Unauthorized("Kullanıcı kimliği doğrulanamadı. Lütfen tekrar giriş yapın.");
        }
        var userId = Guid.Parse(userIdString);
        
        await _orderService.CreateOrderAsync(userId, createOrderDto);
        return Ok("işlem başarıyla yapıldı");
    }
    
    
}