using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.MenuHandlers
{
    /// <summary>
    /// Interface cho handler gửi feedback về giải đấu
    /// Single Responsibility: Chỉ lo việc feedback
    /// </summary>
    public interface IPlayerFeedbackHandler
    {
        /// <summary>
        /// Gửi feedback về giải đấu
        /// </summary>
        Task HandleSubmitFeedbackAsync();
    }
}
