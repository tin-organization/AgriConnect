using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConsultationController : ControllerBase
{
    private readonly IConsultationService _consultationService;
    private readonly IValidator<AskQuestionDto> _askValidator;

    public ConsultationController(IConsultationService consultationService, IValidator<AskQuestionDto> askValidator)
    {
        _consultationService = consultationService;
        _askValidator = askValidator;
    }

    // POST /api/consultation/ask
    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AskQuestionDto dto)
    {
        var validation = await _askValidator.ValidateAsync(dto);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (success, message, data) = await _consultationService.AskAsync(userId, dto);
        return success ? Ok(new { message, data }) : StatusCode(500, new { message });
    }

    // GET /api/consultation/my
    [HttpGet("my")]
    public async Task<IActionResult> GetMyConsultations()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (_, message, data) = await _consultationService.GetMyConsultationsAsync(userId);
        return Ok(new { message, data });
    }

    // DELETE /api/consultation/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (success, message) = await _consultationService.DeleteConsultationAsync(userId, id);
        return success ? Ok(new { message }) : NotFound(new { message });
    }
}