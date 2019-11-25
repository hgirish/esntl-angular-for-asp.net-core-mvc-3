using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Models
{
    public static  class IdentitySeedData
    {
        private const string adminUser = "admin";
        private const string adminPassword = "MySecret123$";
        private const string adminRole = "Administrator";

        public static async Task SeedDatabase(IServiceProvider provider)
        {
          var identityContext=  provider.GetRequiredService<IdentityDataContext>();
            identityContext.Database.Migrate();

            UserManager<IdentityUser> userManager =
                provider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager =
                provider.GetRequiredService<RoleManager<IdentityRole>>();

            var role = await roleManager.FindByNameAsync(adminRole);
            var user = await userManager.FindByNameAsync(adminUser);

            if (role == null)
            {
                role = new IdentityRole(adminRole);
                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception("Cannot create role: " +
                        result.Errors.FirstOrDefault());
                }
            }

            if (user == null)
            {
                user = new IdentityUser(adminUser);
                var result = await userManager.CreateAsync(user, adminPassword);
                if (!result.Succeeded)
                {
                    throw new Exception("Cannot create user: " +
                        result.Errors.FirstOrDefault());
                }
            }

            if (! await userManager.IsInRoleAsync(user, adminRole))
            {
                var result =
                    await userManager.AddToRoleAsync(user, adminRole);
                if (!result.Succeeded)
                {
                    throw new Exception("Cannot add user to role: " +
                        result.Errors.FirstOrDefault());
                }
            }

        }
    }
}
