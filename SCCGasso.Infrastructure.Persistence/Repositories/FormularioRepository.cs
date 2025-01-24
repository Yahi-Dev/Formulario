using Microsoft.EntityFrameworkCore;
using SCC_Gasso.Core.Application.Interfaces.Repositories;
using SCCGasso.Core.Domain.Entities;
using SCCGasso.Infrastructure.Persistence.Context;

namespace SCCGasso.Infrastructure.Persistence.Repositories
{
    public class FormularioRepository : GenericRepository<Formulario>, IFormularioRepository
    {
        private readonly ApplicationContext _context;
        private readonly DbSet<Formulario> _entities;
        public FormularioRepository(ApplicationContext context) : base(context)
        {
            _context = context;
            _entities = _context.Set<Formulario>();
        }
    }
}
