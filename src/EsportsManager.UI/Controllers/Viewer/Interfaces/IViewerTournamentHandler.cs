using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.MenuHandlers
{
    /// <summary>
    /// Interface cho handler xem thông tin giải đấu (Viewer)
    /// Single Responsibility: Chỉ lo việc hiển thị tournament info cho viewer
    /// </summary>
    public interface IViewerTournamentHandler
    {
        /// <summary>
        /// Xem danh sách giải đấu
        /// </summary>
        Task HandleViewTournamentListAsync();

        /// <summary>
        /// Xem bảng xếp hạng giải đấu
        /// </summary>
        Task HandleViewTournamentStandingsAsync();
    }
}
