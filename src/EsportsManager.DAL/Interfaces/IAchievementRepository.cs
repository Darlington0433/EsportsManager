using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.DAL.Models;

namespace EsportsManager.DAL.Interfaces
{
    /// <summary>
    /// Interface cho Achievement Repository
    /// </summary>
    public interface IAchievementRepository
    {
        /// <summary>
        /// Lấy danh sách thành tích của user
        /// </summary>
        Task<List<Achievement>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Tạo thành tích mới
        /// </summary>
        Task<Achievement> CreateAsync(Achievement achievement);

        /// <summary>
        /// Cập nhật thành tích
        /// </summary>
        Task<Achievement> UpdateAsync(Achievement achievement);

        /// <summary>
        /// Xóa thành tích
        /// </summary>
        Task<bool> DeleteAsync(int achievementId);

        /// <summary>
        /// Lấy thành tích theo ID
        /// </summary>
        Task<Achievement?> GetByIdAsync(int achievementId);

        /// <summary>
        /// Lấy danh sách thành tích theo loại
        /// </summary>
        Task<List<Achievement>> GetByTypeAsync(string achievementType);
    }
}
