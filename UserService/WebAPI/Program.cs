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

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<UserDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("sqlConnection")));

        // Add services to the container.

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserService, UserServices>();


        //JWT
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




        //RabbitMQ
        builder.Services.AddSharedRabbitMQ();
        builder.Services.AddScoped<UserMessagePublisher>();
        //builder.Services.AddHostedService<MassTransitConsoleHostedService>();

        builder.Services.AddHttpContextAccessor();


        //REDIS
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Redis");

        });

        // Register cache service
        builder.Services.AddScoped<ICacheService, RedisCacheService>();

        // Register Message Consumer
        builder.Services.AddScoped<UserCreatedConsumer>();

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

        app.UseAuthentication();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}

