using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using SharedLibrary.MessageBroker;
using StackExchange.Redis;
using RFQService.Infrastructure.Cache;
using MassTransit;
using RFQService.Infrastructure.MessageConsumers;

namespace RFQService.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddHttpContextAccessor();

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



            // Configure MassTransit
            builder.Services.AddMassTransit(x =>
            {
                // Add consumers
                x.AddConsumer<RfqStatusUpdateConsumer>();

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
