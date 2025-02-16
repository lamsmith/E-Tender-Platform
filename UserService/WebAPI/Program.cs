using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using UserService.Application.Common.Interface.Repositories;
using UserService.Application.Common.Interface.Services;
using UserService.Infrastructure.Persistence.Context;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Services;
using SharedLibrary.MessageBroker;
using UserService.Infrastructure.Messaging;
using UserService.Infrastructure.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserService.Infrastructure.MessageConsumers;
using Microsoft.Extensions.Logging;
using UserService.Infrastructure.Persistence.Seeds;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("sqlConnection")));

        // Add services to the container.
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserService, UserServices>();

        // JWT Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
            options.MapInboundClaims = false;
        });

        // Configure MassTransit
        builder.Services.AddMassTransit(x =>
        {
            // Add consumers
            x.AddConsumer<UserCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration["RabbitMQ:HostName"] ?? "localhost", "/", h =>
                {
                    h.Username(builder.Configuration["RabbitMQ:UserName"] ?? "guest");
                    h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
                });

                // Configure endpoints
                cfg.ConfigureEndpoints(context);
            });
        });




        builder.Services.AddHttpContextAccessor();

        // Redis Caching
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Redis");
        });

        // Register cache service
        builder.Services.AddScoped<ICacheService, RedisCacheService>();

        // Register Message Consumer
        builder.Services.AddScoped<UserCreatedConsumer>();

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

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // ? Ensure database migration and seeding runs asynchronously
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<UserDbContext>();
                var logger = services.GetRequiredService<ILogger<UserDbContextSeed>>();
                var seeder = new UserDbContextSeed(logger);

                await context.Database.MigrateAsync();
                await seeder.SeedAsync(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating or seeding the database.");
            }
        }

        await app.RunAsync(); // ? Use RunAsync() instead of Run()
    }
}
