using Microsoft.EntityFrameworkCore;
using SCC_Gasso.Core.Application.Interfaces.Repositories;
using SCCGasso.Core.Domain.Entities;
using SCCGasso.Infrastructure.Persistence.Context;


namespace SCCGasso.Infrastructure.Persistence.Repositories
{
    public class PersonasAutorizadasRepository : GenericRepository<PersonasAutorizadas>, IPersonasAutorizadasRepository
    {
        private readonly ApplicationContext _context;
        private readonly DbSet<PersonasAutorizadas> _entities;
        public PersonasAutorizadasRepository(ApplicationContext context) : base(context)
        {
            _context = context;
            _entities = _context.Set<PersonasAutorizadas>();
        }
    }
}
