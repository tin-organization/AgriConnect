using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IValidator<CreateOrderDto> _createValidator;
    private readonly IValidator<UpdateOrderDto> _updateValidator;

    public OrderController(
        IOrderService orderService,
        IValidator<CreateOrderDto> createValidator,
        IValidator<UpdateOrderDto> updateValidator)
    {
        _orderService = orderService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private string GetUserRole() =>
        User.FindFirstValue(ClaimTypes.Role)!;

    // POST /api/order
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        if (GetUserRole() != nameof(Role.Buyer))
            return Forbid();

        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var (success, message, data) = await _orderService.CreateAsync(GetUserId(), dto);
        return success ? Ok(new { message, data }) : BadRequest(new { message });
    }

    // GET /api/order/my
    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        if (GetUserRole() != nameof(Role.Buyer))
            return Forbid();

        var (_, message, data) = await _orderService.GetMyOrdersAsync(GetUserId());
        return Ok(new { message, data });
    }

    // POST /api/order/update/{id}
    [HttpPost("update/{id}")]
    public async Task<IActionResult> UpdateQuantity(int id, [FromBody] UpdateOrderDto dto)
    {
        if (GetUserRole() != nameof(Role.Buyer))
            return Forbid();

        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var (success, message, data) = await _orderService.UpdateQuantityAsync(GetUserId(), id, dto);
        return success ? Ok(new { message, data }) : BadRequest(new { message });
    }

    // POST /api/order/cancel/{id}
    [HttpPost("cancel/{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        if (GetUserRole() != nameof(Role.Buyer))
            return Forbid();

        var (success, message) = await _orderService.CancelAsync(GetUserId(), id);
        return success ? Ok(new { message }) : BadRequest(new { message });
    }
}