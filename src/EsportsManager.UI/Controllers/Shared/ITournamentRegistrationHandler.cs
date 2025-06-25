using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.MenuHandlers
{
    /// <summary>
    /// Interface cho handler quản lý đăng ký giải đấu
    /// Single Responsibility: Chỉ lo việc đăng ký tournament
    /// </summary>
    public interface ITournamentRegistrationHandler
    {
        /// <summary>
        /// Xử lý đăng ký tham gia giải đấu
        /// </summary>
        Task HandleTournamentRegistrationAsync();
    }
}
