using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP1_ARQWEB.Areas.Identity.Data;

namespace TP1_ARQWEB.Data
{
    public static class ContextSeed
    {


        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles 
            await roleManager.CreateAsync(new IdentityRole("Admin"));

            await roleManager.CreateAsync(new IdentityRole("Basic"));
        }

        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                FirstName = "Admin",
                LastName = "Admin",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "adminadmin");
                    await userManager.AddToRoleAsync(defaultUser, "Basic");

                    await userManager.AddToRoleAsync(defaultUser, "Admin");

                }

            }
        }
    }
}
