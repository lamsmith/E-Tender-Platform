using Microsoft.Extensions.DependencyInjection;
using NotificationService.Infrastructure.Persistence.Context;
using SharedLibrary.MessageBroker;
using NotificationService.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using NotificationService.Infrastructure.Cache;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace NotificationService.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddRabbitMQ();
            builder.Services.AddHostedService<NotificationConsumer>();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllers();

            // Configure Redis
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
            });

            // Register cache service
            builder.Services.AddScoped<ICacheService, RedisCacheService>();

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
