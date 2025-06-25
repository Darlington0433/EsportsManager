using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.MenuHandlers
{
    /// <summary>
    /// Interface cho handler xem thành tích cá nhân
    /// Single Responsibility: Chỉ lo việc hiển thị achievements
    /// </summary>
    public interface IPlayerAchievementHandler
    {
        /// <summary>
        /// Xem thành tích và bảng xếp hạng cá nhân
        /// </summary>
        Task HandleViewAchievementsAsync();
    }
}
