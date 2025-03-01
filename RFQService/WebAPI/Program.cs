using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using SharedLibrary.MessageBroker;
using StackExchange.Redis;
using RFQService.Infrastructure.Cache;
using MassTransit;
using RFQService.Infrastructure.MessageConsumers;
using RFQService.Infrastructure.Persistence.Context;
using RFQService.Infrastructure.Persistence.Repositories;
using RFQService.Application.Common.Interface.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Serilog;
using Serilog.Events;

namespace RFQService.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
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

            // Add DbContext
            builder.Services.AddDbContext<RFQDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("sqlConnection")));

            // Register Repository
            builder.Services.AddScoped<IRFQRepository, RFQRepository>();

            builder.Services.AddHttpContextAccessor();

            // Configure Redis
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
            });

            // Register cache service
            builder.Services.AddScoped<ICacheService, RedisCacheService>();

            //  Register MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Configure JWT Authentication
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
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
                };
            });

            // Add Authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAuthenticatedUser", policy =>
                    policy.RequireAuthenticatedUser());

                options.AddPolicy("RequireCorporateRole", policy =>
                    policy.RequireRole("Corporate"));

                options.AddPolicy("RequireMSMERole", policy =>
                    policy.RequireRole("MSME"));
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            // Configure Swagger with JWT support
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RFQ Service API", Version = "v1" });

                // Add JWT Authentication
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter 'Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });


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

            // Add this before app.UseAuthentication()
            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

                if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    logger.LogInformation("Auth header present: {Auth}", authHeader.ToString());

                    // Log claims if token is present
                    if (context.User?.Identity?.IsAuthenticated ?? false)
                    {
                        foreach (var claim in context.User.Claims)
                        {
                            logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
                        }
                    }
                }
                else
                {
                    logger.LogWarning("No Authorization header present");
                }

                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            try
            {
                Log.Information("Starting RFQ Service");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "RFQ Service terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
