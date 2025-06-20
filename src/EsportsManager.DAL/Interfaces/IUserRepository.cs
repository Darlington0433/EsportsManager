using EsportsManager.DAL.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EsportsManager.DAL.Interfaces;

/// <summary>
/// User repository interface - áp dụng Interface Segregation Principle
/// Chỉ chứa các phương thức liên quan đến User
/// </summary>
public interface IUserRepository : IRepository<User, int>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetByRoleAsync(string role);
    Task<bool> IsUsernameExistsAsync(string username);
    Task<bool> IsEmailExistsAsync(string email);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<bool> UpdatePasswordAsync(int userId, string passwordHash);
    Task<bool> UpdateStatusAsync(int userId, string status);
}
