using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace NotificationService.Infrastructure.Logging
{
    public static class LoggingConfiguration
    {
        public static void ConfigureLogging(WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.WithExceptionDetails()
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.Console(new CompactJsonFormatter())
                .WriteTo.File(
                    new CompactJsonFormatter(),
                    "Logs/notification-service-.json",
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();

            builder.Host.UseSerilog();
        }
    }
}
