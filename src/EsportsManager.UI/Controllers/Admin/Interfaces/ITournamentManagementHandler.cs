using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.Admin.Interfaces
{
    /// <summary>
    /// Interface for Tournament Management operations
    /// </summary>
    public interface ITournamentManagementHandler
    {
        Task HandleTournamentCreationAsync();
        Task HandleTournamentUpdateAsync();
        Task HandleTournamentDeletionAsync();
        Task HandleTournamentListAsync();
        Task HandleTournamentDetailViewAsync();
    }
}
