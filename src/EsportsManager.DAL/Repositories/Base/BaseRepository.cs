using System.Data;
using System.Data.SqlClient;
using EsportsManager.DAL.Context;
using EsportsManager.DAL.Interfaces;

namespace EsportsManager.DAL.Repositories.Base
{
    public abstract class BaseRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
    {
        protected readonly DataContext _context;

        protected BaseRepository(DataContext context)
        {
            _context = context;
        }        public virtual Task<TEntity?> GetByIdAsync(TKey id)
        {
            return Task.FromException<TEntity?>(new NotImplementedException());
        }

        public virtual Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return Task.FromException<IEnumerable<TEntity>>(new NotImplementedException());
        }

        public virtual Task<TEntity> AddAsync(TEntity entity)
        {
            return Task.FromException<TEntity>(new NotImplementedException());
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            return Task.FromException<TEntity>(new NotImplementedException());
        }

        public virtual Task<bool> DeleteAsync(TKey id)
        {
            return Task.FromException<bool>(new NotImplementedException());
        }

        public virtual Task<bool> ExistsAsync(TKey id)
        {
            return Task.FromException<bool>(new NotImplementedException());
        }

        public virtual Task<int> CountAsync()
        {
            return Task.FromException<int>(new NotImplementedException());
        }
    }
}