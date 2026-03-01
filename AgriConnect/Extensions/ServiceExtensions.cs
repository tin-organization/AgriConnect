public static class ServiceExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        // Future: services.AddScoped<IProductService, ProductService>();
        // Future: services.AddScoped<IOrderService, OrderService>();

        return services;
    }
}