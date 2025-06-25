namespace EsportsManager.UI.Controllers.Interfaces
{
    /// <summary>
    /// Interface định nghĩa contract cho tất cả Controllers
    /// Áp dụng Interface Segregation Principle
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// Hiển thị menu chính của controller
        /// </summary>
        void ShowMenu();
    }

    /// <summary>
    /// Interface cho controllers có thể validate permissions
    /// </summary>
    public interface ISecureController : IController
    {
        /// <summary>
        /// Kiểm tra quyền truy cập
        /// </summary>
        bool ValidateAccess();
    }

    /// <summary>
    /// Interface cho controllers có thể log activities
    /// </summary>
    public interface IAuditableController : IController
    {
        /// <summary>
        /// Log hoạt động của user
        /// </summary>
        void LogActivity(string action);
    }
}
