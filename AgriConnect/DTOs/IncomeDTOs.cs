public class IncomeResponseDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string ItemTitle { get; set; } = "";
    public decimal OrderTotal { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}