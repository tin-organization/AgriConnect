using Microsoft.EntityFrameworkCore;

public interface IInventoryService
{
    Task<(bool Success, string Message, InventoryDashboardDto? Data)> GetDashboardAsync(int sellerId);
    Task<(bool Success, string Message, List<InventoryProduceDto>? Data)> GetMyProduceAsync(int sellerId);
    Task<(bool Success, string Message, List<InventoryEquipmentDto>? Data)> GetMyEquipmentAsync(int sellerId);
    Task<(bool Success, string Message, List<InventoryOrderDto>? Data)> GetMyItemOrdersAsync(int sellerId);
    Task<(bool Success, string Message)> ToggleProduceAvailabilityAsync(int sellerId, int produceId, ToggleProduceAvailabilityDto dto);
    Task<(bool Success, string Message)> ToggleEquipmentAvailabilityAsync(int sellerId, int equipmentId);
}

public class InventoryService : IInventoryService
{
    private readonly AppDbContext _db;

    public InventoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(bool, string, InventoryDashboardDto?)> GetDashboardAsync(int sellerId)
    {
        var produceCount = await _db.Produces
            .CountAsync(p => p.SellerId == sellerId);

        var equipmentCount = await _db.Equipments
            .CountAsync(e => e.SellerId == sellerId);

        var activeOrders = await _db.Orders
            .CountAsync(o => o.SellerId == sellerId && o.Status == OrderStatus.Active);

        var grossRevenue = await _db.Orders
            .Where(o => o.SellerId == sellerId && o.Status == OrderStatus.Active)
            .SumAsync(o => o.TotalAmount);

        var sellerOrderIds = await _db.Orders
            .Where(o => o.SellerId == sellerId && o.Status == OrderStatus.Active)
            .Select(o => o.Id)
            .ToListAsync();

        var platformCut = await _db.Incomes
            .Where(i => sellerOrderIds.Contains(i.OrderId))
            .SumAsync(i => i.Amount);

        return (true, "Success.", new InventoryDashboardDto
        {
            TotalProduceListings = produceCount,
            TotalEquipmentListings = equipmentCount,
            TotalActiveOrders = activeOrders,
            GrossRevenue = grossRevenue,
            PlatformCut = platformCut,
            NetRevenue = grossRevenue - platformCut
        });
    }

    public async Task<(bool, string, List<InventoryProduceDto>?)> GetMyProduceAsync(int sellerId)
    {
        var produces = await _db.Produces
            .Where(p => p.SellerId == sellerId)
            .ToListAsync();

        var result = produces.Select(p => new InventoryProduceDto
        {
            Id = p.Id,
            Title = p.Title,
            Category = p.Category,
            Price = p.Price,
            Unit = p.Unit,
            AvailableUnitsLeft = p.AvailableUnitsLeft,
            InitialUnitsLeft = p.InitialUnitsLeft,
            IsAvailable = p.AvailableUnitsLeft > 0 && p.ExpiryDate > DateTime.UtcNow,
            IsLowStock = p.AvailableUnitsLeft > 0 &&
                         p.AvailableUnitsLeft <= (p.InitialUnitsLeft * 0.10),
            IsExpired = p.ExpiryDate <= DateTime.UtcNow,
            Location = p.Location,
            ExpiryDate = p.ExpiryDate,
            CreatedAt = p.CreatedAt
        }).ToList();

        return (true, "Success.", result);
    }

    public async Task<(bool, string, List<InventoryEquipmentDto>?)> GetMyEquipmentAsync(int sellerId)
    {
        var equipments = await _db.Equipments
            .Where(e => e.SellerId == sellerId)
            .ToListAsync();

        var equipmentIds = equipments.Select(e => e.Id).ToList();

        var activeOrderEquipmentIds = await _db.Orders
            .Where(o => o.EquipmentId.HasValue &&
                        equipmentIds.Contains(o.EquipmentId.Value) &&
                        o.Status == OrderStatus.Active)
            .Select(o => o.EquipmentId!.Value)
            .ToListAsync();

        var result = equipments.Select(e => new InventoryEquipmentDto
        {
            Id = e.Id,
            Title = e.Title,
            Type = e.Type,
            Price = e.Price,
            ListingType = e.ListingType,
            Condition = e.Condition,
            Brand = e.Brand,
            Model = e.Model,
            IsAvailable = e.IsAvailable,
            HasActiveOrder = activeOrderEquipmentIds.Contains(e.Id),
            Location = e.Location,
            CreatedAt = e.CreatedAt
        }).ToList();

        return (true, "Success.", result);
    }

    public async Task<(bool, string, List<InventoryOrderDto>?)> GetMyItemOrdersAsync(int sellerId)
    {
        var orders = await _db.Orders
            .Where(o => o.SellerId == sellerId && o.Status == OrderStatus.Active)
            .Include(o => o.Produce)
            .Include(o => o.Equipment)
            .ToListAsync();

        var result = orders.Select(o => new InventoryOrderDto
        {
            Id = o.Id,
            ItemType = o.ItemType,
            ItemTitle = o.ItemType == ItemType.Produce
                ? o.Produce?.Title ?? ""
                : o.Equipment?.Title ?? "",
            Quantity = o.Quantity,
            PriceSnapshot = o.PriceSnapshot,
            TotalAmount = o.TotalAmount,
            ListingType = o.ListingType,
            Status = o.Status,
            CreatedAt = o.CreatedAt
        }).ToList();

        return (true, "Success.", result);
    }

    public async Task<(bool, string)> ToggleProduceAvailabilityAsync(int sellerId, int produceId, ToggleProduceAvailabilityDto dto)
    {
        var produce = await _db.Produces.FindAsync(produceId);

        if (produce is null || produce.SellerId != sellerId)
            return (false, "Produce not found.");

        if (produce.ExpiryDate <= DateTime.UtcNow)
            return (false, "Cannot toggle availability on expired produce.");

        if (dto.MakeAvailable)
        {
            if (produce.AvailableUnitsLeft <= 0)
                return (false, "Cannot make available — no stock left.");
            produce.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return (true, $"Produce is now available with {produce.InitialUnitsLeft} {produce.Unit} in stock.");
        }
        else
        {
            var hasActiveOrders = await _db.Orders
                .AnyAsync(o => o.ProduceId == produceId && o.Status == OrderStatus.Active);

            if (hasActiveOrders)
                return (false, "Cannot mark as unavailable — this produce has active orders.");

            produce.AvailableUnitsLeft = 0;
            produce.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return (true, "Produce marked as unavailable.");
        }
    }

    public async Task<(bool, string)> ToggleEquipmentAvailabilityAsync(int sellerId, int equipmentId)
    {
        var equipment = await _db.Equipments.FindAsync(equipmentId);

        if (equipment is null || equipment.SellerId != sellerId)
            return (false, "Equipment not found.");

        if (!equipment.IsAvailable)
        {
            var hasActiveOrder = await _db.Orders
                .AnyAsync(o => o.EquipmentId == equipmentId && o.Status == OrderStatus.Active);

            if (hasActiveOrder)
                return (false, "Cannot mark as available — this equipment has an active order. Cancel the order first.");
        }

        equipment.IsAvailable = !equipment.IsAvailable;
        equipment.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return (true, equipment.IsAvailable
            ? "Equipment is now available."
            : "Equipment marked as unavailable.");
    }
}