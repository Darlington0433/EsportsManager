using System.Threading.Tasks;
using EsportsManager.DAL.Models;

namespace EsportsManager.DAL.Interfaces
{
    /// <summary>
    /// Interface cho UserProfile Repository
    /// </summary>
    public interface IUserProfileRepository
    {
        /// <summary>
        /// Lấy profile theo UserID
        /// </summary>
        Task<UserProfile?> GetByUserIdAsync(int userId);

        /// <summary>
        /// Tạo profile mới
        /// </summary>
        Task<UserProfile> CreateAsync(UserProfile profile);

        /// <summary>
        /// Cập nhật profile
        /// </summary>
        Task<UserProfile> UpdateAsync(UserProfile profile);

        /// <summary>
        /// Cập nhật achievements cho user
        /// </summary>
        Task<bool> UpdateAchievementsAsync(int userId, string achievementsJson);
    }
}
