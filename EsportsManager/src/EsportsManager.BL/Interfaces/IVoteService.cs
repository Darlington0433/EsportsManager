using EsportsManager.BL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsportsManager.BL.Interfaces
{
    public interface IVoteService
    {
        Task<ServiceResult<List<Vote>>> GetAllAsync();
        Task<ServiceResult<Vote>> GetByIdAsync(int id);
        Task<ServiceResult> CreateAsync(Vote vote);
        Task<ServiceResult> UpdateAsync(Vote vote);
        Task<ServiceResult> DeleteAsync(int id);
        Task<ServiceResult<List<Vote>>> GetVotesByUserIdAsync(int userId);
        Task<ServiceResult<List<Vote>>> GetVotesByEntityIdAsync(string entityType, int entityId);
        Task<ServiceResult<Dictionary<int, int>>> GetVoteCountsByEntityIdsAsync(string entityType, List<int> entityIds);
        Task<ServiceResult<bool>> HasUserVotedForEntityAsync(int userId, string entityType, int entityId);
    }
}
