using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.MenuHandlers
{
    /// <summary>
    /// Interface cho handler quản lý profile (Viewer)
    /// Single Responsibility: Chỉ lo việc quản lý thông tin cá nhân viewer
    /// </summary>
    public interface IViewerProfileHandler
    {
        /// <summary>
        /// Xem thông tin cá nhân
        /// </summary>
        Task HandleViewProfileAsync();

        /// <summary>
        /// Cập nhật thông tin cá nhân
        /// </summary>
        Task HandleUpdateProfileAsync();
    }
}
