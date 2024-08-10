using Domain;
using Domain.Abstractions.GenericRepository;
using Domain.Abstractions.UnitOfWork;
using Domain.Entities;
using Infrastructure.DatabaseInitializers;
using Infrastructure.Repositories.Generic;
using Infrastructure.Repositories.UnitOfWork;
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
        services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(connectionString, op =>
        {
            op.CommandTimeout(120);
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
                ValidAudience = configuration["AuthSettings:Audience"],
                ValidIssuer = configuration["AuthSettings:Issuer"],
                RequireExpirationTime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AuthSettings:Key"])),
                ValidateIssuerSigningKey = true
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
        #endregion

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ApplicationRegisteration).Assembly));

        return services;
    }
}