using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.Interfaces;

namespace EsportsManager.UI.Controllers.Base
{
    /// <summary>
    /// Base Controller - Abstract class chứa common functionality
    /// Áp dụng Template Method Pattern và DRY principle
    /// </summary>
    public abstract class BaseController : IController
    {
        protected readonly UserProfileDto _currentUser;

        protected BaseController(UserProfileDto currentUser)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        }

        /// <summary>
        /// Template method - defines the skeleton of menu operation
        /// Subclasses implement specific menu options
        /// </summary>
        public void ShowMenu()
        {
            try
            {
                BeforeMenuDisplay();
                DisplayMenu();
                AfterMenuDisplay();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        /// <summary>
        /// Hook method - executed before menu display
        /// Subclasses can override for custom initialization
        /// </summary>
        protected virtual void BeforeMenuDisplay()
        {
            Console.Clear();
            DisplayUserWelcome();
        }

        /// <summary>
        /// Abstract method - must be implemented by subclasses
        /// Contains the actual menu logic
        /// </summary>
        protected abstract void DisplayMenu();

        /// <summary>
        /// Hook method - executed after menu display
        /// Subclasses can override for cleanup
        /// </summary>
        protected virtual void AfterMenuDisplay()
        {
            // Default: no action needed
        }

        /// <summary>
        /// Common error handling logic
        /// </summary>
        protected virtual void HandleError(Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox(
                $"Đã xảy ra lỗi: {ex.Message}", 
                true, 
                3000);
        }

        /// <summary>
        /// Display welcome message with user info
        /// </summary>
        protected virtual void DisplayUserWelcome()
        {
            var welcomeMessage = $"Chào mừng {_currentUser.Username} ({_currentUser.Role})";
            ConsoleRenderingService.DrawBorder(welcomeMessage, 80, 3);
        }

        /// <summary>
        /// Common method to handle async operations safely
        /// </summary>
        protected void ExecuteAsyncSafely(Func<Task> asyncOperation)
        {
            try
            {
                asyncOperation().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        /// <summary>
        /// Validate if user has required role
        /// </summary>
        protected bool ValidateUserRole(string requiredRole)
        {
            if (!_currentUser.Role.Equals(requiredRole, StringComparison.OrdinalIgnoreCase))
            {
                ConsoleRenderingService.ShowMessageBox(
                    $"Bạn không có quyền truy cập chức năng này. Yêu cầu: {requiredRole}",
                    true,
                    3000);
                return false;
            }
            return true;
        }
    }
}
