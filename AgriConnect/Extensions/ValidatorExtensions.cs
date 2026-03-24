using FluentValidation;

public static class ValidatorExtensions
{
    public static IServiceCollection AddAppValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<RegisterDto>, RegisterValidator>();
        services.AddScoped<IValidator<LoginDto>, LoginValidator>();
        services.AddScoped<IValidator<UpdateProfileDto>, UpdateProfileValidator>();
        services.AddScoped<IValidator<AskQuestionDto>, AskQuestionValidator>();

        services.AddScoped<IValidator<CreateProduceDto>, CreateProduceValidator>();
        services.AddScoped<IValidator<UpdateProduceDto>, UpdateProduceValidator>();
        services.AddScoped<IValidator<CreateEquipmentDto>, CreateEquipmentValidator>();
        services.AddScoped<IValidator<UpdateEquipmentDto>, UpdateEquipmentValidator>();
        return services;
    }
}