using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SCC_Gasso.Core.Application.Interfaces.Repositories;
using SCCGasso.Infrastructure.Persistence.Context;
using SCCGasso.Infrastructure.Persistence.Interceptor;
using SCCGasso.Infrastructure.Persistence.Repositories;

namespace SCCGasso.Infrastructure.Persistence.IOC
{
    public static class ServiceRegistration
    {
        public static void PersistenceLayerRegistration(this IServiceCollection service, IConfiguration configuration)
        {
            #region Context
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                service.AddDbContext<ApplicationContext>(options =>
                                                        options.UseInMemoryDatabase("SCCGasso"));
            }
            else
            {
                service.AddSingleton<UpdateAuditableEntitiesInterceptor>();
                service.AddDbContext<ApplicationContext>((sp, options) =>
                {
                    var interceptor = sp.GetService<UpdateAuditableEntitiesInterceptor>();
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    m => m.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName)).AddInterceptors(interceptor);
                });

            }
            #endregion

            #region Repositories 
            service.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            service.AddTransient<IFormularioRepository, FormularioRepository>();
            service.AddTransient<ICuentasBancariasRepository, CuentasBancariasRepository>();
            service.AddTransient<IPersonasAutorizadasRepository, PersonasAutorizadasRepository>();
            service.AddTransient<IReferenciasComercialesRepository, ReferenciasComercialesRepository>();
            service.AddTransient<ISugerenciasRepository, SugerenciasRepository>();
            #endregion
        }
    }
}

