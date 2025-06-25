using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.Admin.Interfaces
{
    /// <summary>
    /// Interface for Feedback Management operations
    /// </summary>
    public interface IFeedbackManagementHandler
    {
        Task HandleFeedbackListAsync();
        Task HandleFeedbackResponseAsync();
        Task HandleFeedbackAnalyticsAsync();
        Task HandleFeedbackArchiveAsync();
    }
}
