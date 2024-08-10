using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using Infrastructure.Common.ExtensionMethods;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DatabaseInitializers
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;

        public DatabaseInitializer(IConfiguration config,
                    ApplicationDbContext dbContext,
                    IServiceProvider serviceProvider
                    )
        {
            _configuration = config;
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
        }
        public async Task MigrateDbsAsync()
        {
            #region [Update the license db schema]
            await _dbContext.Database.MigrateAsync();
            #endregion
        }
        public async Task SeedDataAsync()
        {
            bool res = await AddRolesMeta();
            await AddSuperadminAsync();
        }

        private async Task<bool> RoleNameExists(string Name, RoleManager<Roles> RoleService)
        {
            var role_obj = await RoleService.FindByNameAsync(Name);
            if (role_obj != null)
                return true;
            else
                return false;
        }
        public async Task<bool> AddRolesMeta()
        {
            var isSuccessfull = true;
            var _roleManager = _serviceProvider.GetRequiredService<RoleManager<Roles>>();
            var rolesList = Enum.GetValues(typeof(RoleEnum)).Cast<RoleEnum>().ToList();

            foreach (var role in rolesList)
            {
                var roleExist = await RoleNameExists(role.GetEnumDescription(), _roleManager);
                if (!roleExist)
                {
                    var result = await _roleManager.CreateAsync(new Roles()
                    {
                        Name = role.GetEnumDescription(),
                        Description = role.GetEnumDescription()
                    });
                    if (!result.Succeeded)
                    {
                        isSuccessfull = false;
                        break;
                    }
                }
            }
            return isSuccessfull;
        }
        public async Task AddSuperadminAsync()
        {
            var _userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
            var uniqueGuid = Guid.NewGuid();
            var superUserName = _configuration["SystemadminUser:UserName"];
#pragma warning disable CS8604 // Possible null reference argument.
            var user = await _userManager.FindByNameAsync(superUserName);
#pragma warning restore CS8604 // Possible null reference argument.
            if (user == null)
            {
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8604 // Possible null reference argument.
                var userResult = await _userManager.CreateAsync(new User()
                {
                    Id = uniqueGuid.ToString(),
                    FirstName = _configuration["SystemadminUser:FirstName"],
                    LastName = _configuration["SystemadminUser:LastName"],
                    Email = _configuration["SystemadminUser:Email"],
                    UserName = superUserName,
                    EmailConfirmed = true
                }, _configuration["SystemadminUser:Password"]);
#pragma warning restore CS8604 // Possible null reference argument.
                if (userResult.Succeeded)
                {
                    user = await _userManager.FindByNameAsync(superUserName);
                    var addToRoleResult = await _userManager.AddToRoleAsync(user, "SuperAdmin");
                }
#pragma warning restore CS8601 // Possible null reference assignment.

            }
        }

    }
}
