using Microsoft.AspNetCore.Identity;
using SCC_Gasso.Core.Application.Enums;
using SCC_Gasso.Core.Application.Helpers;
using SCCGasso.Infrastructure.Identity.Entities;

namespace SCCGasso.Infrastructure.Identity.Seeds
{
    public class AdminUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser adminUser = new();
            adminUser.UserName = "adminUser";
            adminUser.Email = "adminUser@email.com";
            adminUser.FirstName = "admin";
            adminUser.LastName = "user";
            adminUser.PhoneNumber = "829-123-9811";
            adminUser.IdCard = "931-1981312-1";
            adminUser.IdRoleAppPermission = 1;
            adminUser.LastLogin = GetDateTime.GetDateTimeInString();
            adminUser.IsActive = true;
            adminUser.EmailConfirmed = true;
            adminUser.PhoneNumberConfirmed = true;

            if (userManager.Users.All(u => u.Id != adminUser.Id))
            {
                var user = await userManager.FindByEmailAsync(adminUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(adminUser, "123Pa$$word");
                    await userManager.AddToRoleAsync(adminUser, RolesEnum.Admin.ToString());
                }
            }
        }
    }
}
