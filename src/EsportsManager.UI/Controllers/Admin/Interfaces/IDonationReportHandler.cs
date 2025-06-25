using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.Admin.Interfaces
{
    /// <summary>
    /// Interface for Donation Report operations
    /// </summary>
    public interface IDonationReportHandler
    {
        Task HandleDonationOverviewAsync();
        Task HandleTopDonatorsAsync();
        Task HandleDonationTrendsAsync();
        Task HandleDonationAnalyticsAsync();
    }
}
