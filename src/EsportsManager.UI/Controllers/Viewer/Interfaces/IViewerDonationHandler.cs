using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.MenuHandlers
{
    /// <summary>
    /// Interface cho handler donation (Viewer)
    /// Single Responsibility: Chỉ lo việc donate
    /// </summary>
    public interface IViewerDonationHandler
    {
        /// <summary>
        /// Donate cho player
        /// </summary>
        Task HandleDonateToPlayerAsync();
    }
}
