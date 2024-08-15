using Application.Abstractions.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Web.Middlewares;

internal sealed class JwtRefreshMiddleware : IMiddleware
{
    private readonly ITokenService _tokenService;
    private JwtRefreshMiddleware() { } // This is use to prevent from creating object

    public JwtRefreshMiddleware(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            if (jwtToken != null)
            {
                var expiration = jwtToken.ValidTo;
                var remainingTime = expiration - DateTime.UtcNow;

                // If less than 2 minutes left, refresh the token
                if (remainingTime.TotalMinutes < 2)
                {
                    var newToken = _tokenService.RefreshToken(token);
                    context.Response.Headers.Add("X-Refreshed-Token", newToken);
                }
            }
        }

        await next(context);
    }
}
