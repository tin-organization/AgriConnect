using Microsoft.EntityFrameworkCore;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                Environment.GetEnvironmentVariable("DATABASE_URL")
                ?? config.GetConnectionString("DefaultConnection")
            ));
        return services;
    }
}