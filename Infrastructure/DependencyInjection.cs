using Application.Abstractions.Services;
using Domain;
using Domain.Abstractions.GenericRepository;
using Domain.Abstractions.UnitOfWork;
using Domain.Entities;
using Infrastructure.DatabaseInitializers;
using Infrastructure.Repositories.Generic;
using Infrastructure.Repositories.UnitOfWork;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var connectionString = configuration.GetConnectionString("DBConnection");
        services.AddDbContextPool<ApplicationDbContext>(o => o.UseSqlServer(connectionString, op =>
        {
            op.CommandTimeout(120); // It means if query takes time more than 120 ms, than it will throw timeout.
        }));
        services.AddIdentity<User, Roles>(option =>
        {
            option.User.RequireUniqueEmail = true;
            option.Password.RequireDigit = false;
            option.Password.RequiredLength = 8;
            option.Password.RequireNonAlphanumeric = true;
            option.Password.RequireUppercase = false;
            option.Password.RequireLowercase = false;
            option.SignIn.RequireConfirmedEmail = true;
        }).AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

        services.AddAuthentication(auth =>
        {
            auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
#pragma warning disable CS8604 // Possible null reference argument.
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidIssuer = configuration["Jwt:Issuer"],
                RequireExpirationTime = true,
                ValidateLifetime = true, // Ensure this is true to validate the token expiration
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])),
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero // Ensure immediate rejection of expired tokens
            };
#pragma warning restore CS8604 // Possible null reference argument.
        });

        #region Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        #endregion

        #region Registered UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        #endregion

        #region Services
#pragma warning disable CS8604 // Possible null reference argument.
        var secretKey = configuration["Jwt:SecretKey"];
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var validity = Convert.ToInt32(configuration["Jwt:Validity"]);

        services.AddSingleton<ITokenService>(new TokenService(secretKey, issuer, audience, validity));
#pragma warning restore CS8604 // Possible null reference argument.
        #endregion

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ApplicationRegisteration).Assembly));

        return services;
    }
}