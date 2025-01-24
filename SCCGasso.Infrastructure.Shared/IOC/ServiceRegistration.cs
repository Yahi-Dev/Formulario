using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SCC_Gasso.Core.Application.Interfaces.Services;
using SCCGasso.Core.Domain.Settings;
using SCCGasso.Infrastructure.Shared.Services;


namespace SCCGasso.Infrastructure.Shared.IOC
{
    public static class ServiceRegistration
    {
        public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddTransient<IEmailService, EmailService>();
        }
    }
}
