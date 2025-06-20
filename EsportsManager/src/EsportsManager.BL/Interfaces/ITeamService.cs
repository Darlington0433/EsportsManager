using EsportsManager.BL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsportsManager.BL.Interfaces
{
    public interface ITeamService
    {
        Task<ServiceResult<List<Team>>> GetAllAsync();
        Task<ServiceResult<Team>> GetByIdAsync(int id);
        Task<ServiceResult> CreateAsync(Team team);
        Task<ServiceResult> UpdateAsync(Team team);
        Task<ServiceResult> DeleteAsync(int id);
        Task<ServiceResult<Team>> GetTeamByUserIdAsync(int userId);
        Task<ServiceResult> AddMemberAsync(int teamId, int userId);
        Task<ServiceResult> RemoveMemberAsync(int teamId, int userId);
        Task<ServiceResult<bool>> IsUserTeamCaptainAsync(int userId);
        Task<ServiceResult<List<Team>>> GetTeamsForTournamentAsync(int tournamentId);
    }
}
