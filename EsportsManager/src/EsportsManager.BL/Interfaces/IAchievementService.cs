using EsportsManager.BL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsportsManager.BL.Interfaces
{
    public interface IAchievementService
    {
        Task<ServiceResult<List<Achievement>>> GetAllAsync();
        Task<ServiceResult<Achievement>> GetByIdAsync(int id);
        Task<ServiceResult> CreateAsync(Achievement achievement);
        Task<ServiceResult> UpdateAsync(Achievement achievement);
        Task<ServiceResult> DeleteAsync(int id);
        Task<ServiceResult<List<Achievement>>> GetAchievementsByUserIdAsync(int userId);
        Task<ServiceResult<List<Achievement>>> GetRecentAchievementsAsync(int count = 10);
    }
}
