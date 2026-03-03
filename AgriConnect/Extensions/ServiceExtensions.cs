public static class ServiceExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IConsultationService, ConsultationService>(); 
        services.AddHttpClient<ConsultationService>();
        services.AddScoped<IProduceService, ProduceService>();

        return services;
    }
}