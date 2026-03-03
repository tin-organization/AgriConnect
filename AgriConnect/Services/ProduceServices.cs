using Microsoft.EntityFrameworkCore;

public interface IProduceService
{
    Task<(bool Success, string Message, ProduceResponseDto? Data)> CreateAsync(CreateProduceDto dto);
    Task<(bool Success, string Message, ProduceResponseDto? Data)> GetByIdAsync(int id);
    Task<(bool Success, string Message, List<ProduceResponseDto>? Data)> GetAllAsync();
    Task<(bool Success, string Message, List<ProduceResponseDto>? Data)> SearchAsync(string query);
    Task<(bool Success, string Message, ProduceResponseDto? Data)> UpdateAsync(int id, UpdateProduceDto dto);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}

public class ProduceService : IProduceService
{
    private readonly AppDbContext _db;

    public ProduceService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(bool, string, ProduceResponseDto?)> CreateAsync(CreateProduceDto dto)
    {
        var produce = new Produce
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            Price = dto.Price,
            Unit = dto.Unit,
            AvailableUnitsLeft = dto.AvailableUnitsLeft,
            Location = dto.Location,
            HarvestDate = dto.HarvestDate,
            ExpiryDate = dto.ExpiryDate
        };

        _db.Produces.Add(produce);
        await _db.SaveChangesAsync();
        return (true, "Produce listed successfully.", ToDto(produce));
    }

    public async Task<(bool, string, ProduceResponseDto?)> GetByIdAsync(int id)
    {
        var produce = await _db.Produces.FindAsync(id);
        if (produce is null) return (false, "Produce not found.", null);

        return (true, "Success.", ToDto(produce));
    }

    public async Task<(bool, string, List<ProduceResponseDto>?)> GetAllAsync()
    {
        var produces = await _db.Produces
            .Select(p => ToDto(p))
            .ToListAsync();

        return (true, "Success.", produces);
    }

    public async Task<(bool, string, List<ProduceResponseDto>?)> SearchAsync(string query)
    {
        var produces = await _db.Produces
            .Where(p => p.Title.Contains(query)
                     || p.Description.Contains(query)
                     || p.Location.Contains(query))
            .Select(p => ToDto(p))
            .ToListAsync();

        return (true, "Success.", produces);
    }

    public async Task<(bool, string, ProduceResponseDto?)> UpdateAsync(int id, UpdateProduceDto dto)
    {
        var produce = await _db.Produces.FindAsync(id);
        if (produce is null) return (false, "Produce not found.", null);

        produce.Title = dto.Title;
        produce.Description = dto.Description;
        produce.Category = dto.Category;
        produce.Price = dto.Price;
        produce.Unit = dto.Unit;
        produce.AvailableUnitsLeft = dto.AvailableUnitsLeft;
        produce.Location = dto.Location;
        produce.HarvestDate = dto.HarvestDate;
        produce.ExpiryDate = dto.ExpiryDate;
        produce.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return (true, "Produce updated successfully.", ToDto(produce));
    }

    public async Task<(bool, string)> DeleteAsync(int id)
    {
        var produce = await _db.Produces.FindAsync(id);
        if (produce is null) return (false, "Produce not found.");

        _db.Produces.Remove(produce);
        await _db.SaveChangesAsync();
        return (true, "Produce deleted successfully.");
    }

    private static ProduceResponseDto ToDto(Produce p) => new()
    {
        Id = p.Id,
        Title = p.Title,
        Description = p.Description,
        Category = p.Category,
        Price = p.Price,
        Unit = p.Unit,
        AvailableUnitsLeft = p.AvailableUnitsLeft,
        Location = p.Location,
        HarvestDate = p.HarvestDate,
        ExpiryDate = p.ExpiryDate,
        CreatedAt = p.CreatedAt
    };
}