using System.Threading.Tasks;
using System.Collections.Generic;

namespace EsportsManager.DAL.Interfaces;

/// <summary>
/// Base repository interface - áp dụng Single Responsibility Principle
/// Chỉ chứa các phương thức CRUD cơ bản
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IRepository<T, TKey> where T : class
{
    Task<T?> GetByIdAsync(TKey id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(TKey id);
    Task<bool> ExistsAsync(TKey id);
    Task<int> CountAsync();
}
