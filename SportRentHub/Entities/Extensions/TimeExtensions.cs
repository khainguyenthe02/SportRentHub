using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ERBeeVisionMaster.Entities.Extensions
{
    public static class TimeExtensions
    {
        public static void ConfigureDateTime(this SwaggerGenOptions options)
        {
            var localTime = DateTimeOffset.Now;
            var exampleDateTime = localTime.ToString("yyyy-MM-ddTHH:mm:ss.fffK");

            options.MapType<DateTime>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "date-time",
                Example = new OpenApiString(exampleDateTime)
            });
        }
    }
}
