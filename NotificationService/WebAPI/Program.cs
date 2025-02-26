using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Common.Interface.Repositories;
using NotificationService.Application.Common.Interface.Services;
using NotificationService.Infrastructure.Cache;
using NotificationService.Infrastructure.MessageConsumers;
using NotificationService.Infrastructure.Middleware;
using NotificationService.Infrastructure.Persistence.Context;
using NotificationService.Infrastructure.Persistence.Repositories;
using NotificationService.Infrastructure.Services;
using NotificationService.Services;
using Serilog;
using System.Reflection;

namespace NotificationService.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Configure Serilog
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    //.Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(
                        path: "Logs/rfq-.log",
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();

                builder.Host.UseSerilog();

                Log.Information("Starting NotificationService");

                // Add services to the container.
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                // Add DbContext
                builder.Services.AddDbContext<NotificationDbContext>(options =>
                    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

                // Add Redis Cache
                builder.Services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = builder.Configuration.GetConnectionString("Redis");
                    options.InstanceName = "NotificationService_";
                });

                // Register MediatR
                builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

                // Add Services
                builder.Services.AddScoped<IEmailService, EmailService>();
                builder.Services.AddScoped<ICacheService, RedisCacheService>();
                builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
                builder.Services.AddScoped<ITemplateService, TemplateService>();

                builder.Services.AddMassTransit(x =>
                {
                    // Add consumers
                    x.AddConsumer<BidEventConsumer>();
                    x.AddConsumer<RfqEventConsumer>();
                    x.AddConsumer<UserVerificationStatusConsumer>();
                    x.AddConsumer<OnboardingReminderConsumer>();
                    x.AddConsumer<EmailNotificationConsumer>();
                    x.AddConsumer<NotificationConsumer>();
                    x.AddConsumer<RfqEmailNotificationConsumer>();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(builder.Configuration["RabbitMQ:HostName"] ?? "localhost", "/", h =>
                        {
                            h.Username(builder.Configuration["RabbitMQ:UserName"] ?? "guest");
                            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
                        });

                        cfg.ConfigureEndpoints(context);
                    });
                });

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                // Add request logging middleware
                app.UseMiddleware<RequestLoggingMiddleware>();

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "NotificationService terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
