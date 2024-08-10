using Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Web.Middlewares;

namespace Web.Extensions;
internal static class RegisterServicesExtension
{
    public static IServiceCollection Register(this IServiceCollection services, ConfigurationManager _configuration)
    {
        // Add services to the container.
        var configuration = _configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();

        #region For MediatR.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        #endregion

        #region For Infrastructure project
        // Adding from Infrastructure project. All repos will be register there.
        services.AddInfrastructure(configuration);
        #endregion

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        #region For Middlewares
        services.AddTransient<ExceptionHandlingMiddleware>();
        #endregion

        #region For Rate Limiting
        // Rate limiting is a technique for restricting the number of requests to our API.
        // Rate limiting is a .Net 7 feature.
        // If we set QueueLimit to 0, it shows 503 error.
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddFixedWindowLimiter("fixedPolicy", opt =>
            {
                opt.Window = TimeSpan.FromSeconds(10);
                opt.PermitLimit = 1;
                opt.QueueLimit = 2;
                opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
            });

            options.AddSlidingWindowLimiter("slidingPolicy", opt =>
            {
                opt.Window = TimeSpan.FromSeconds(30);
                opt.PermitLimit = 3;
                opt.SegmentsPerWindow = 3;
            });

            options.AddConcurrencyLimiter("concurrencyPolicy", opt =>
            {
                opt.PermitLimit = 5;
            });

            options.AddTokenBucketLimiter("tokenPolicy", opt =>
            {
                opt.TokenLimit = 10;
                opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                opt.TokensPerPeriod = 2;
            });
        });

        #endregion

        return services;
    }
}
