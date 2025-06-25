using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.MenuHandlers
{
    /// <summary>
    /// Interface cho handler quản lý thông tin cá nhân của player
    /// Single Responsibility: Chỉ lo việc quản lý profile
    /// </summary>
    public interface IPlayerProfileHandler
    {
        /// <summary>
        /// Xem thông tin cá nhân
        /// </summary>
        Task HandleViewPersonalInfoAsync();

        /// <summary>
        /// Cập nhật thông tin cá nhân
        /// </summary>
        Task HandleUpdatePersonalInfoAsync();
    }
}
