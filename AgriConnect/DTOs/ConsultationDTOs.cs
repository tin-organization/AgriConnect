public class AskQuestionDto
{
    public string Question { get; set; } = "";
}

public class ConsultationResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Question { get; set; } = "";
    public string Response { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}