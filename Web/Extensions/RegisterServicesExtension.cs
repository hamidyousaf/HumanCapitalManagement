using Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Web.Middlewares;

namespace Web.Extensions;
internal static class RegisterServicesExtension
{
    public static IServiceCollection Register(this IServiceCollection services, ConfigurationManager _configuration)
    {
        #region For serilog.
        var connectionString = _configuration.GetConnectionString("DBConnection");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo
            .MSSqlServer(
                connectionString: connectionString,
                sinkOptions: new MSSqlServerSinkOptions { TableName = "LogEvents", AutoCreateSqlTable = true })
            .CreateLogger();
        #endregion

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

        #region For Swagger
        services
              .AddSwaggerGen(c =>
              {
                  c.AddSecurityDefinition("Bearer", //Name the security scheme
                  new OpenApiSecurityScheme
                  {
                      Description = "JWT Authorization header using the Bearer scheme.",
                      Type = SecuritySchemeType.Http, //We set the scheme type to http since we're using bearer authentication
                      Scheme = "bearer" //The name of the HTTP Authorization scheme to be used in the Authorization header. In this case "bearer".
                  });

                  c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                 {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = "Bearer", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },new List<string>()
                    }
                });
                  c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
              });
        #endregion 

        #region For Middlewares
        services.AddTransient<ExceptionHandlingMiddleware>();
        services.AddTransient<HtmlSanitizationMiddleware>();
        services.AddTransient<JwtRefreshMiddleware>();
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
