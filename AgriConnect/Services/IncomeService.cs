using Microsoft.EntityFrameworkCore;

public interface IIncomeService
{
    Task<(bool Success, string Message, List<IncomeResponseDto>? Data)> GetAllAsync();
    Task<(bool Success, string Message, decimal TotalIncome)> GetTotalAsync();
}

public class IncomeService : IIncomeService
{
    private readonly AppDbContext _db;

    public IncomeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(bool, string, List<IncomeResponseDto>?)> GetAllAsync()
    {
        var incomes = await _db.Incomes
            .Include(i => i.Order)
                .ThenInclude(o => o.Produce)
            .Include(i => i.Order)
                .ThenInclude(o => o.Equipment)
            .ToListAsync();

        var result = incomes.Select(i => new IncomeResponseDto
        {
            Id = i.Id,
            OrderId = i.OrderId,
            ItemTitle = i.Order.ItemType == ItemType.Produce
                ? i.Order.Produce?.Title ?? ""
                : i.Order.Equipment?.Title ?? "",
            OrderTotal = i.Order.TotalAmount,
            Amount = i.Amount,
            CreatedAt = i.CreatedAt
        }).ToList();

        return (true, "Success.", result);
    }

    public async Task<(bool, string, decimal)> GetTotalAsync()
    {
        var total = await _db.Incomes.SumAsync(i => i.Amount);
        return (true, "Success.", total);
    }
}