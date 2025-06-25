using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.MenuHandlers
{
    /// <summary>
    /// Interface cho handler voting (Viewer)
    /// Single Responsibility: Chỉ lo việc voting
    /// </summary>
    public interface IViewerVotingHandler
    {
        /// <summary>
        /// Vote cho player yêu thích
        /// </summary>
        Task HandleVoteForPlayerAsync();

        /// <summary>
        /// Vote cho tournament yêu thích
        /// </summary>
        Task HandleVoteForTournamentAsync();

        /// <summary>
        /// Vote cho sport/game yêu thích
        /// </summary>
        Task HandleVoteForSportAsync();
    }
}
