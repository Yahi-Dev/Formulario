using Microsoft.EntityFrameworkCore;
using SCC_Gasso.Core.Application.Interfaces.Repositories;
using SCCGasso.Core.Domain.Entities;
using SCCGasso.Infrastructure.Persistence.Context;


namespace SCCGasso.Infrastructure.Persistence.Repositories
{
    public class CuentasBancariasRepository : GenericRepository<CuentasBancarias>, ICuentasBancariasRepository
    {
        private readonly ApplicationContext _context;
        private readonly DbSet<CuentasBancarias> _entities;
        public CuentasBancariasRepository(ApplicationContext context) : base(context)
        {
            _context = context;
            _entities = _context.Set<CuentasBancarias>();
        }
    }
}
