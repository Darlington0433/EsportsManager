using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.Admin.Interfaces
{
    /// <summary>
    /// Interface for Voting Results operations
    /// </summary>
    public interface IVotingResultsHandler
    {
        Task HandlePlayerVotingResultsAsync();
        Task HandleTournamentVotingResultsAsync();
        Task HandleVotingSearchAsync();
        Task HandleVotingStatisticsAsync();
    }
}
