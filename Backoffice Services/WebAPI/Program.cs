using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Backoffice_Services.Infrastructure.Authorization;
using Backoffice_Services.Infrastructure.Cache;
using Backoffice_Services.Infrastructure.ExternalServices;
using Backoffice_Services.Infrastructure.MessageConsumers;
using Backoffice_Services.Infrastructure.Persistence.Context;
using SharedLibrary.Constants;
using SharedLibrary.MessageBroker;
using Backoffice_Services.Domain.Enums;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NotificationService.Infrastructure.Extensions;

namespace Backoffice_Services.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog for Logging
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("Logs/backoffice_service_.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            builder.Host.UseSerilog();

            // Add services to the container
            builder.Services.AddDbContext<BackofficeDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure Redis
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
                options.InstanceName = "BackofficeServices_";
            });

            builder.Services.TryAddSingleton<ICacheService, RedisCacheService>();

            // Configure Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
                    };
                });

            // Configure Authorization
            builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequirePermission", policy =>
                    policy.Requirements.Add(new PermissionRequirement(PermissionType.ManageStaff)));
            });

            // Register RabbitMQ Message Broker
            builder.Services.AddRabbitMQ();

            // Register Message Consumers as SINGLETONS
            builder.Services.AddSingleton<UserProfileCompletedConsumer>();

            // Register Message Broker Subscriptions
            builder.Services.AddMessageSubscriber(options =>
            {
                options.AddConsumer<UserProfileCompletedConsumer>(MessageQueues.UserProfileCompleted);
            });

            // Add HTTP Clients
            builder.Services.AddHttpClient<IAuthServiceClient, AuthServiceClient>();
            builder.Services.AddHttpClient<IBidServiceClient, BidServiceClient>();

            // Register MediatR for CQRS
            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
