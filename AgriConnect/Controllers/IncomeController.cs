using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class IncomeController : ControllerBase
{
    private readonly IIncomeService _incomeService;

    public IncomeController(IIncomeService incomeService)
    {
        _incomeService = incomeService;
    }

    private string GetUserRole() =>
        User.FindFirstValue(ClaimTypes.Role)!;

    // GET /api/income
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (GetUserRole() != nameof(Role.Admin))
            return Forbid();

        var (_, message, data) = await _incomeService.GetAllAsync();
        return Ok(new { message, data });
    }

    // GET /api/income/total
    [HttpGet("total")]
    public async Task<IActionResult> GetTotal()
    {
        if (GetUserRole() != nameof(Role.Admin))
            return Forbid();

        var (_, message, total) = await _incomeService.GetTotalAsync();
        return Ok(new { message, total });
    }
}