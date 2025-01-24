using Microsoft.EntityFrameworkCore;
using SCC_Gasso.Core.Application.Interfaces.Repositories;
using SCCGasso.Core.Domain.Entities;
using SCCGasso.Infrastructure.Persistence.Context;


namespace SCCGasso.Infrastructure.Persistence.Repositories
{
    public class ReferenciasComercialesRepository : GenericRepository<ReferenciasComerciales>, IReferenciasComercialesRepository
    {
        private readonly ApplicationContext _context;
        private readonly DbSet<ReferenciasComerciales> _entities;
        public ReferenciasComercialesRepository(ApplicationContext context) : base(context)
        {
            _context = context;
            _entities = _context.Set<ReferenciasComerciales>();
        }
    }
}
