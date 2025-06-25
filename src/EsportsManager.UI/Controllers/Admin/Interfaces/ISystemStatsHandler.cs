using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.Admin.Interfaces
{
    /// <summary>
    /// Interface for System Statistics operations
    /// </summary>
    public interface ISystemStatsHandler
    {
        Task HandleSystemOverviewAsync();
        Task HandleUserStatisticsAsync();
        Task HandleTournamentStatisticsAsync();
        Task HandleActivityLogsAsync();
        Task HandlePerformanceMetricsAsync();
    }
}
