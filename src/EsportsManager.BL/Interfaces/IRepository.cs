using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsportsManager.BL.Interfaces;

/// <summary>
/// Interface cơ sở cho Repository Pattern
/// </summary>
/// <typeparam name="TEntity">Kiểu entity</typeparam>
/// <typeparam name="TKey">Kiểu khóa chính</typeparam>
public interface IRepository<TEntity, TKey> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<bool> AddAsync(TEntity entity);
    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TKey id);
    Task<bool> ExistsAsync(TKey id);
}

/// <summary>
/// Interface cơ sở cho Repository Pattern với khóa chính là int
/// </summary>
/// <typeparam name="TEntity">Kiểu entity</typeparam>
public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class
{
} 