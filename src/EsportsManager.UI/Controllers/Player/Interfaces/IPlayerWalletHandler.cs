using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.MenuHandlers
{
    /// <summary>
    /// Interface cho handler quản lý ví điện tử
    /// Single Responsibility: Chỉ lo việc quản lý wallet
    /// </summary>
    public interface IPlayerWalletHandler
    {
        /// <summary>
        /// Quản lý ví điện tử (nạp tiền, chuyển tiền, xem lịch sử)
        /// </summary>
        Task HandleWalletManagementAsync();
    }
}
