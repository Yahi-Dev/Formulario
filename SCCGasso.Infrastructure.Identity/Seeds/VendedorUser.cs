using Microsoft.AspNetCore.Identity;
using SCC_Gasso.Core.Application.Enums;
using SCC_Gasso.Core.Application.Helpers;
using SCCGasso.Infrastructure.Identity.Entities;


namespace SCCGasso.Infrastructure.Identity.Seeds
{
    public class VendedorUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser vendedorUser = new();
            vendedorUser.UserName = "vendedorUser";
            vendedorUser.Email = "vendedorUser@email.com";
            vendedorUser.FirstName = "vendedor";
            vendedorUser.LastName = "user";
            vendedorUser.PhoneNumber = "829-123-9811";
            vendedorUser.IdCard = "931-1981312-1";
            vendedorUser.IdRoleAppPermission = 1;
            vendedorUser.LastLogin = GetDateTime.GetDateTimeInString();
            vendedorUser.IsActive = true;
            vendedorUser.EmailConfirmed = true;
            vendedorUser.PhoneNumberConfirmed = true;

            if (userManager.Users.All(u => u.Id != vendedorUser.Id))
            {
                var user = await userManager.FindByEmailAsync(vendedorUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(vendedorUser, "123Pa$$word");
                    await userManager.AddToRoleAsync(vendedorUser, RolesEnum.Client.ToString());
                }
            }
        }
    }
}
