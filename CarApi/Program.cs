using CarApi.Repositories;

namespace CarApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Add CORS policy to allow all origins, headers, and methods.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    corsBuilder =>
                    {
                        corsBuilder.AllowAnyOrigin()   // Allows all origins
                                   .AllowAnyHeader()   // Allows all headers
                                   .AllowAnyMethod();  // Allows all HTTP methods
                    });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Use HTTPS redirection (ensure your frontend also uses https if this is active)
            app.UseHttpsRedirection();

            // Enable the CORS policy
            app.UseCors("AllowAll");

            // Use authorization middleware
            app.UseAuthorization();

            // Map controllers to endpoints
            app.MapControllers();

            app.Run();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<CarRepository>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration["ConnectionStrings:SqlServerDB"];
                return new CarRepository(connectionString);
            });
        }
    }
}
