using Microsoft.AspNetCore.Identity;
using SCC_Gasso.Core.Application.Enums;
using SCC_Gasso.Core.Application.Helpers;
using SCCGasso.Infrastructure.Identity.Entities;


namespace SCCGasso.Infrastructure.Identity.Seeds
{
    public class ClientUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser clientUser = new();
            clientUser.UserName = "clientUser";
            clientUser.Email = "clientUser@email.com";
            clientUser.FirstName = "cliente";
            clientUser.LastName = "user";
            clientUser.PhoneNumber = "829-123-9811";
            clientUser.IdCard = "931-1981312-1";
            clientUser.IdRoleAppPermission = 1;
            clientUser.LastLogin = GetDateTime.GetDateTimeInString();
            clientUser.IsActive = true;
            clientUser.EmailConfirmed = true;
            clientUser.PhoneNumberConfirmed = true;

            if (userManager.Users.All(u => u.Id != clientUser.Id))
            {
                var user = await userManager.FindByEmailAsync(clientUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(clientUser, "123Pa$$word");
                    await userManager.AddToRoleAsync(clientUser, RolesEnum.Client.ToString());
                }
            }
        }
    }
}
