using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SCC_Gasso.Core.Application.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity, int Id);
        Task DeleteAsync(TEntity entity);
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity> GetEntityByIdAsync(int Id);
        Task<List<TEntity>> GetAllWithIncludeAsync(Expression<Func<TEntity, bool>> condition);
        Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> filter);
        Task<TEntity?> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
