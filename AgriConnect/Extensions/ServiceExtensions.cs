using AgriConnect.DTOs;
using AgriConnect.Services;
using AgriConnect.Validators;
using FluentValidation;

public static class ServiceExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        // Future: services.AddScoped<IProductService, ProductService>();
        // Future: services.AddScoped<IOrderService, OrderService>();
        
        // Blog
        services.AddScoped<IBlogService, BlogService>();

        // Validators  (add alongside existing validator registrations)
        services.AddScoped<IValidator<CreateBlogDto>, CreateBlogValidator>();
        services.AddScoped<IValidator<UpdateBlogDto>, UpdateBlogValidator>();
        services.AddScoped<IValidator<AddCommentDto>, AddCommentValidator>();

        services.AddScoped<IConsultationService, ConsultationService>(); 
        services.AddHttpClient<ConsultationService>();

        services.AddScoped<IProduceService, ProduceService>();
        services.AddScoped<IEquipmentService, EquipmentService>();

        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IIncomeService, IncomeService>();

        services.AddScoped<IInventoryService, InventoryService>();

        return services;
    }
}