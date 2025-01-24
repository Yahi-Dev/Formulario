using Microsoft.EntityFrameworkCore;
using SCC_Gasso.Core.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SCCGasso.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _entities;
        public GenericRepository(DbContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }
        public virtual async Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _entities.AnyAsync(filter);
        }

        public virtual async Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _entities.Where(filter).ToListAsync();
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }


        public virtual async Task<List<TEntity>> GetAllWithIncludeAsync(Expression<Func<TEntity, bool>> condition)
        {
            return await _context.Set<TEntity>().Where(condition).ToListAsync();
        }

        public virtual async Task<TEntity> GetEntityByIdAsync(int Id)
        {
            return await _entities.FindAsync(Id);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            try
            {
                await _entities.AddAsync(entity);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return entity;
        }

        public async Task<TEntity?> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public virtual async Task UpdateAsync(TEntity entity, int Id)
        {
            try
            {
                var entry = await _context.Set<TEntity>().FindAsync(Id);
                _context.Entry(entry).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
