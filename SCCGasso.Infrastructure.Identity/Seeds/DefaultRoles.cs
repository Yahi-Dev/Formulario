using Microsoft.AspNetCore.Identity;
using SCC_Gasso.Core.Application.Enums;
using SCCGasso.Infrastructure.Identity.Entities;


namespace SCCGasso.Infrastructure.Identity.Seeds
{
    public class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(RolesEnum.Client.ToString()));
            await roleManager.CreateAsync(new IdentityRole(RolesEnum.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(RolesEnum.Vendedor.ToString()));
        }
    }
}
