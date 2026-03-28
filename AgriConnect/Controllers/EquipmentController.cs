using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;
    private readonly IValidator<CreateEquipmentDto> _createValidator;
    private readonly IValidator<UpdateEquipmentDto> _updateValidator;

    public EquipmentController(
        IEquipmentService equipmentService,
        IValidator<CreateEquipmentDto> createValidator,
        IValidator<UpdateEquipmentDto> updateValidator)
    {
        _equipmentService = equipmentService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    // POST /api/equipment
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEquipmentDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var (success, message, data) = await _equipmentService.CreateAsync(dto);
        return success ? Ok(new { message, data }) : BadRequest(new { message });
    }

    // GET /api/equipment
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var (_, message, data) = await _equipmentService.GetAllAsync();
        return Ok(new { message, data });
    }

    // GET /api/equipment/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, message, data) = await _equipmentService.GetByIdAsync(id);
        return success ? Ok(new { message, data }) : NotFound(new { message });
    }

    // GET /api/equipment/search?query=tractor
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query = "")
    {
        var (_, message, data) = await _equipmentService.SearchAsync(query);
        return Ok(new { message, data });
    }

    // POST /api/equipment/update/{id}
    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipmentDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var (success, message, data) = await _equipmentService.UpdateAsync(id, dto);
        return success ? Ok(new { message, data }) : NotFound(new { message });
    }

    // DELETE /api/equipment/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _equipmentService.DeleteAsync(id);
        return success ? Ok(new { message }) : NotFound(new { message });
    }
}