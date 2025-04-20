using SportRentHub.Services.Interfaces;

namespace SportRentHub.Entities.Extensions
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceManager _serviceManager;
        private readonly IConfiguration _config;

        public TokenValidationMiddleware(RequestDelegate next, IServiceManager serviceManager, IConfiguration config)
        {
            _next = next;
            _serviceManager = serviceManager;
            _config = config;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == "OPTIONS")
            {
                await _next(context);
                return;
            }

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (!string.IsNullOrEmpty(token))
            {
                var principal = JWTExtensions.DecodeJwtToken(token, _config);
                var employeeIdClaim = principal?.FindFirst("userId")?.Value;

                if (!string.IsNullOrEmpty(employeeIdClaim) && int.TryParse(employeeIdClaim, out var employeeId))
                {
                    var user = await _serviceManager.UserService.GetById(employeeId);
                    if (user == null || string.IsNullOrEmpty(user.Token) || user.Token != token)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
