using Serilog;

namespace SportRentHub.Entities.Extensions
{
    public static class LoggingExtension
    {
        public static void ConfigureLogging(this WebApplicationBuilder builder)
        {
            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    Path.Combine(logDirectory, "log-.txt"),
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();
        }
    }
}
