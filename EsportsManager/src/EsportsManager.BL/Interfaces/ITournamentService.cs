using EsportsManager.BL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsportsManager.BL.Interfaces
{
    public interface ITournamentService
    {
        Task<ServiceResult<List<Tournament>>> GetAllAsync();
        Task<ServiceResult<Tournament>> GetByIdAsync(int id);
        Task<ServiceResult> CreateAsync(Tournament tournament);
        Task<ServiceResult> UpdateAsync(Tournament tournament);
        Task<ServiceResult> DeleteAsync(int id);
        Task<ServiceResult<List<Tournament>>> GetUpcomingTournamentsAsync();
        Task<ServiceResult<List<Tournament>>> GetActiveTournamentsAsync();
        Task<ServiceResult<List<Tournament>>> GetCompletedTournamentsAsync();
    }
}
