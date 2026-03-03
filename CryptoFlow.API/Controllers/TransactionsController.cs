using CryptoFlow.Application.Interfaces.ITransaction;
using CryptoFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class TransactionsController: ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }
    [HttpGet("get-transactions")]
    public async Task<IActionResult> GetTransactionsAsync(int page, int pageSize, DateTime startDate, DateTime endDate,
        TransactionType type)
    {
        var result = await _transactionService.GetFilteredTransactionsAsync(page, pageSize, startDate, endDate, type);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllTransactionsAsync()
    {
        var transactions = await _transactionService.GetAllTransactionsAsync();
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransactionByIdAsync(string id)
    {
        var transaction = await _transactionService.GetTransactionByIdAsync(id);
        return Ok(transaction);
    }

    
    
}