using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

public interface IConsultationService
{
    Task<(bool Success, string Message, ConsultationResponseDto? Data)> AskAsync(int userId, AskQuestionDto dto);
    Task<(bool Success, string Message, List<ConsultationResponseDto>? Data)> GetMyConsultationsAsync(int userId);
    Task<(bool Success, string Message)> DeleteConsultationAsync(int userId, int consultationId);
}

public class ConsultationService : IConsultationService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    // Agriculture system prompt — restricts AI to agri topics only
    private const string SystemPrompt = """
        You are an expert agricultural consultant for AgriConnect platform.
        You ONLY answer questions related to:
        - Crop diseases, pests, and treatments
        - Farming tips and best practices
        - Agricultural equipment and tools
        - Agri market prices and trends
        - Weather impact on crops
        - Soil health and fertilizers
        - Livestock and poultry
        - Irrigation and water management
        - Organic farming
        - Seeds and harvesting

        If the question is NOT related to agriculture, respond with exactly:
        "I can only assist with agriculture-related questions. Please ask about farming, crops, livestock, or agri market topics."

        Be concise, practical, and helpful.
        """;

    public ConsultationService(AppDbContext db, IConfiguration config, HttpClient httpClient)
    {
        _db = db;
        _config = config;
        _httpClient = httpClient;
    }

    public async Task<(bool, string, ConsultationResponseDto?)> AskAsync(int userId, AskQuestionDto dto)
    {
        var apiKey = _config["Gemini:ApiKey"];
        var url = $"{_config["Gemini:Url"]}?key={apiKey}";

        // Build Gemini request body
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = SystemPrompt + "\n\nUser Question: " + dto.Question }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(url, content);

        if (!httpResponse.IsSuccessStatusCode)
            return (false, "Failed to get response from AI. Try again later.", null);

        var responseBody = await httpResponse.Content.ReadAsStringAsync();
        var parsed = JsonDocument.Parse(responseBody);

        // Extract text from Gemini response
        var aiResponse = parsed
            .RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "No response received.";

        // Save to database
        var consultation = new Consultation
        {
            UserId = userId,
            Question = dto.Question,
            Response = aiResponse
        };

        _db.Consultations.Add(consultation);
        await _db.SaveChangesAsync();

        return (true, "Success.", ToDto(consultation));
    }

    public async Task<(bool, string, List<ConsultationResponseDto>?)> GetMyConsultationsAsync(int userId)
    {
        var consultations = await _db.Consultations
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => ToDto(c))
            .ToListAsync();

        return (true, "Success.", consultations);
    }

    public async Task<(bool, string)> DeleteConsultationAsync(int userId, int consultationId)
    {
        var consultation = await _db.Consultations
            .FirstOrDefaultAsync(c => c.Id == consultationId && c.UserId == userId);

        if (consultation == null)
            return (false, "Consultation not found or not yours to delete.");

        _db.Consultations.Remove(consultation);
        await _db.SaveChangesAsync();

        return (true, "Consultation deleted.");
    }

    private static ConsultationResponseDto ToDto(Consultation c) => new()
    {
        Id = c.Id,
        UserId = c.UserId,
        Question = c.Question,
        Response = c.Response,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}