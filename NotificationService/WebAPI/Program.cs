using Microsoft.EntityFrameworkCore;
using NotificationService.Infrastructure.Cache;
using NotificationService.Infrastructure.Extensions;
using NotificationService.Infrastructure.Messaging;
using NotificationService.Infrastructure.Persistence.Context;
using NotificationService.MessageBroker;
using NotificationService.Services;
using SharedLibrary.Constants;
using SharedLibrary.MessageBroker;
using SharedLibrary.MessageBroker.Interfaces;

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

            // Add RabbitMQ
            builder.Services.AddRabbitMQ();

            // Add Message Consumers
            builder.Services.AddMessageConsumer<UserVerificationStatusConsumer>(MessageQueues.UserVerification);
            builder.Services.AddMessageConsumer<OnboardingReminderMessageConsumer>(MessageQueues.OnboardingReminder);

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
