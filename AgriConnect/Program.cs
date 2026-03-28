namespace AgriConnect
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDatabase(builder.Configuration);
            builder.Services.AddJwtAuth(builder.Configuration);
            builder.Services.AddAppServices();
            builder.Services.AddAppValidators();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            // Removed UseHttpsRedirection — Render handles HTTPS at proxy level
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            app.Run($"http://0.0.0.0:{port}");
        }
    }
}