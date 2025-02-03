using Microsoft.EntityFrameworkCore;
using NotificationService.Infrastructure.Cache;
using NotificationService.Infrastructure.Extensions;
using NotificationService.Infrastructure.Messaging;
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

            builder.Services.AddSharedRabbitMQ();


            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllers();

            // Configure Redis
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
            });

            // Add services
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IMessageConsumer, EmailMessageConsumer>();


            // Register cache service
            builder.Services.AddScoped<ICacheService, RedisCacheService>();

            // Register message consumers and start listening
            builder.Services.AddScoped<IMessageConsumer, UserVerificationMessageConsumer>();
            builder.Services.AddScoped<IMessageConsumer, OnboardingReminderMessageConsumer>();
            builder.Services.AddHostedService<NotificationConsumer>();

            // Configure message broker subscriptions
            builder.Services.AddMessageSubscriber(options =>
            {
                options.AddConsumer<UserVerificationMessageConsumer>(MessageQueues.UserVerification);
                options.AddConsumer<OnboardingReminderMessageConsumer>(MessageQueues.OnboardingReminder);
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
