using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SCCGasso.Infrastructure.Identity.Contexts;
using SCCGasso.Infrastructure.Identity.Entities;

namespace SCCGasso.Infrastructure.Identity.IOC
{
    public static class ServiceRegistration
    {
        public static void IdentityLayerRegistration(this IServiceCollection service, IConfiguration configuration)
        {
            #region Context
            if (configuration.GetValue<bool>("UserInMemoryDatabase"))
            {
                service.AddDbContext<IdentityContext>(options => options.UseInMemoryDatabase("InMemoryIdentity"));
            }
            else
            {
                service.AddDbContext<IdentityContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"),
                    m => m.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName));
                });
            }
            #endregion

            #region Services


            service.AddIdentity<ApplicationUser, IdentityRole>()
              .AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();

            service.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/User";
                options.AccessDeniedPath = "/User/AccessDenied";
            });

            service.AddAuthentication();

            #endregion

            //service.AddTransient<IAccountService, AccountService>();
        }
    }
}
