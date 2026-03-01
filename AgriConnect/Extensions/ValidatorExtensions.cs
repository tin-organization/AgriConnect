using FluentValidation;

public static class ValidatorExtensions
{
    public static IServiceCollection AddAppValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<RegisterDto>, RegisterValidator>();
        services.AddScoped<IValidator<LoginDto>, LoginValidator>();
        services.AddScoped<IValidator<UpdateProfileDto>, UpdateProfileValidator>();
        // Future: services.AddScoped<IValidator<CreateProductDto>, CreateProductValidator>();

        return services;
    }
}