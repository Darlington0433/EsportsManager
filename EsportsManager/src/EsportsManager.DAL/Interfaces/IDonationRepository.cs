using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsportsManager.DAL.Interfaces
{
    /// <summary>
    /// Interface for Donation Repository
    /// </summary>
    public interface IDonationRepository : IRepository<Models.Donation, int>
    {
        Task<IEnumerable<Models.Donation>> GetByFromUserIdAsync(int fromUserId);
        Task<IEnumerable<Models.Donation>> GetByToUserIdAsync(int toUserId);
        Task<IEnumerable<Models.Donation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalDonatedByUserAsync(int userId);
        Task<decimal> GetTotalReceivedByUserAsync(int userId);
    }
}
