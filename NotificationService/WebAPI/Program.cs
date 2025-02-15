using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationService.Infrastructure.Cache;
using NotificationService.Infrastructure.MessageConsumers;
using NotificationService.Infrastructure.Persistence.Context;
using NotificationService.Services;

namespace NotificationService.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            // Add Services
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ICacheService, RedisCacheService>();

            builder.Services.AddMassTransit(x =>
            {
                // Add consumers
                x.AddConsumer<BidEventConsumer>();
                x.AddConsumer<RfqEventConsumer>();
                x.AddConsumer<UserVerificationStatusConsumer>();
                x.AddConsumer<OnboardingReminderConsumer>();
                x.AddConsumer<EmailNotificationConsumer>();
                x.AddConsumer<NotificationConsumer>();

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

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
