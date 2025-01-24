using Microsoft.EntityFrameworkCore;
using SCC_Gasso.Core.Application.Interfaces.Repositories;
using SCCGasso.Core.Domain.Entities;
using SCCGasso.Infrastructure.Persistence.Context;


namespace SCCGasso.Infrastructure.Persistence.Repositories
{
    public class SugerenciasRepository : GenericRepository<Sugerencias>, ISugerenciasRepository
    {
        private readonly ApplicationContext _context;
        private readonly DbSet<Sugerencias> _entities;
        public SugerenciasRepository(ApplicationContext context) : base(context)
        {
            _context = context;
            _entities = _context.Set<Sugerencias>();
        }
    }
}
