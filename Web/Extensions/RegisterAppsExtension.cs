using Infrastructure.DatabaseInitializers;
using Web.Middlewares;

namespace Web.Extensions;

internal static class RegisterAppsExtension
{
    internal static void Use(this WebApplication app)
    {
        // Configure the HTTP request pipeline.

        #region Initialization of Database
        IDatabaseInitializer dbInitializer = app.Services.CreateScope().ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        dbInitializer.MigrateDbsAsync().Wait();
        dbInitializer.SeedDataAsync().Wait();
        #endregion

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        #region Registered middlewares
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<HtmlSanitizationMiddleware>();
        app.UseMiddleware<JwtRefreshMiddleware>();
        #endregion

        app.UseAuthorization();

        #region Using rate limiter.
        app.UseRateLimiter();
        #endregion

        app.MapControllers();

        app.Run();
    }
}
