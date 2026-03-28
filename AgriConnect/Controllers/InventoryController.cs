using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private string GetUserRole() =>
        User.FindFirstValue(ClaimTypes.Role)!;

    // GET /api/inventory/dashboard
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        if (GetUserRole() != nameof(Role.Seller))
            return Forbid();

        var (_, message, data) = await _inventoryService.GetDashboardAsync(GetUserId());
        return Ok(new { message, data });
    }

    // GET /api/inventory/produce
    [HttpGet("produce")]
    public async Task<IActionResult> GetMyProduce()
    {
        if (GetUserRole() != nameof(Role.Seller))
            return Forbid();

        var (_, message, data) = await _inventoryService.GetMyProduceAsync(GetUserId());
        return Ok(new { message, data });
    }

    // GET /api/inventory/equipment
    [HttpGet("equipment")]
    public async Task<IActionResult> GetMyEquipment()
    {
        if (GetUserRole() != nameof(Role.Seller))
            return Forbid();

        var (_, message, data) = await _inventoryService.GetMyEquipmentAsync(GetUserId());
        return Ok(new { message, data });
    }

    // GET /api/inventory/orders
    [HttpGet("orders")]
    public async Task<IActionResult> GetMyItemOrders()
    {
        if (GetUserRole() != nameof(Role.Seller))
            return Forbid();

        var (_, message, data) = await _inventoryService.GetMyItemOrdersAsync(GetUserId());
        return Ok(new { message, data });
    }

    // POST /api/inventory/produce/{id}/toggle
    [HttpPost("produce/{id}/toggle")]
    public async Task<IActionResult> ToggleProduceAvailability(int id, [FromBody] ToggleProduceAvailabilityDto dto)
    {
        if (GetUserRole() != nameof(Role.Seller))
            return Forbid();

        var (success, message) = await _inventoryService.ToggleProduceAvailabilityAsync(GetUserId(), id, dto);
        return success ? Ok(new { message }) : BadRequest(new { message });
    }

    // POST /api/inventory/equipment/{id}/toggle
    [HttpPost("equipment/{id}/toggle")]
    public async Task<IActionResult> ToggleEquipmentAvailability(int id)
    {
        if (GetUserRole() != nameof(Role.Seller))
            return Forbid();

        var (success, message) = await _inventoryService.ToggleEquipmentAvailabilityAsync(GetUserId(), id);
        return success ? Ok(new { message }) : BadRequest(new { message });
    }
}