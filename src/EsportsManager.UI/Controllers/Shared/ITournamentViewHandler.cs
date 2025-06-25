using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.Shared
{
    /// <summary>
    /// Interface cho handler xem danh sách giải đấu
    /// Single Responsibility: Chỉ lo việc hiển thị tournament list
    /// </summary>
    public interface ITournamentViewHandler
    {
        /// <summary>
        /// Xem danh sách giải đấu
        /// </summary>
        Task HandleViewTournamentListAsync();
    }
}
