using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.MenuHandlers
{
    /// <summary>
    /// Interface cho handler quản lý team của player
    /// Single Responsibility: Chỉ lo việc quản lý đội
    /// </summary>
    public interface IPlayerTeamManagementHandler
    {
        /// <summary>
        /// Xử lý quản lý team (tạo, tham gia, rời team)
        /// </summary>
        Task HandleTeamManagementAsync();
    }
}
