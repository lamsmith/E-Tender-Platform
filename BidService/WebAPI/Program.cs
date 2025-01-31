using BidService.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.MessageBroker;
using BidService.Infrastructure.Messaging;
using BidService.Infrastructure.Cache;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace BidService.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAutoMapper(typeof(Program));


            builder.Services.AddDbContext<BidDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("sqlConnection")));

            // Add RabbitMQ services
            builder.Services.AddSharedRabbitMQ();
            builder.Services.AddScoped<BidMessagePublisher>();

            // Configure Redis
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
            });

            // Register cache service
            builder.Services.AddScoped<ICacheService, RedisCacheService>();

            builder.Services.AddControllers();
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
