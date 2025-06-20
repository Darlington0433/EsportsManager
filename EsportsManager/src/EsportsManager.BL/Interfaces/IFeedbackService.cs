using EsportsManager.BL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsportsManager.BL.Interfaces
{
    public interface IFeedbackService
    {
        Task<ServiceResult<List<Feedback>>> GetAllAsync();
        Task<ServiceResult<Feedback>> GetByIdAsync(int id);
        Task<ServiceResult> CreateAsync(Feedback feedback);
        Task<ServiceResult> UpdateAsync(Feedback feedback);
        Task<ServiceResult> DeleteAsync(int id);
        Task<ServiceResult<List<Feedback>>> GetFeedbackByUserIdAsync(int userId);
        Task<ServiceResult<List<Feedback>>> GetRecentFeedbackAsync(int count = 10);
    }
}
