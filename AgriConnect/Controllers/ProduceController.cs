using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProduceController : ControllerBase
{
    private readonly IProduceService _produceService;
    private readonly IValidator<CreateProduceDto> _createValidator;
    private readonly IValidator<UpdateProduceDto> _updateValidator;

    public ProduceController(
        IProduceService produceService,
        IValidator<CreateProduceDto> createValidator,
        IValidator<UpdateProduceDto> updateValidator)
    {
        _produceService = produceService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    // POST /api/produce
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProduceDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var (success, message, data) = await _produceService.CreateAsync(dto);
        return success ? Ok(new { message, data }) : BadRequest(new { message });
    }

    // GET /api/produce
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var (_, message, data) = await _produceService.GetAllAsync();
        return Ok(new { message, data });
    }

    // GET /api/produce/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, message, data) = await _produceService.GetByIdAsync(id);
        return success ? Ok(new { message, data }) : NotFound(new { message });
    }

    // GET /api/produce/search?query=tomato
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query = "")
    {
        var (_, message, data) = await _produceService.SearchAsync(query);
        return Ok(new { message, data });
    }

    // PUT /api/produce/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProduceDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var (success, message, data) = await _produceService.UpdateAsync(id, dto);
        return success ? Ok(new { message, data }) : NotFound(new { message });
    }

    // DELETE /api/produce/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _produceService.DeleteAsync(id);
        return success ? Ok(new { message }) : NotFound(new { message });
    }
}