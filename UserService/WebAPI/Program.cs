using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UserService.Application.Common.Interface.Repositories;
using UserService.Application.Common.Interface.Services;
using UserService.Infrastructure.Cache;
using UserService.Infrastructure.MessageConsumers;
using UserService.Infrastructure.Persistence.Context;
using UserService.Infrastructure.Persistence.Seeds;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .WriteTo.Console()
            .WriteTo.File("Logs/user_service_.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"));

        // Add services to the container.
        builder.Services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("sqlConnection")));

        // Register Services
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserService, UserServices>();

        // Add Redis Cache
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Redis");
            options.InstanceName = "UserService_";
        });
        builder.Services.AddScoped<ICacheService, RedisCacheService>();

        // Configure MassTransit
        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<UserCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration["RabbitMQ:HostName"] ?? "localhost", "/", h =>
                {
                    h.Username(builder.Configuration["RabbitMQ:UserName"] ?? "guest");
                    h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
                });

                cfg.UseJsonSerializer();

                cfg.ConfigureEndpoints(context);
            });
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Ensure database migration and seeding
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<UserDbContext>();

                // First apply migrations
                await context.Database.MigrateAsync();

                // Then seed
                var logger = services.GetRequiredService<ILogger<UserDbContextSeed>>();
                var seeder = new UserDbContextSeed(logger);
                await seeder.SeedAsync(context);

                logger.LogInformation("Database migration and seeding completed successfully.");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                throw; // This will stop the application if database setup fails
            }
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        await app.RunAsync();
    }
}
