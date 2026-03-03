using FluentValidation;

public static class ValidatorExtensions
{
    public static IServiceCollection AddAppValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<RegisterDto>, RegisterValidator>();
        services.AddScoped<IValidator<LoginDto>, LoginValidator>();
        services.AddScoped<IValidator<UpdateProfileDto>, UpdateProfileValidator>();
        services.AddScoped<IValidator<AskQuestionDto>, AskQuestionValidator>();

        return services;
    }
}