using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.MenuHandlers.Interfaces.Shared;

namespace EsportsManager.UI.Controllers.MenuHandlers.Shared
{
    /// <summary>
    /// Base abstract class for all menu handlers
    /// Implements Template Method Pattern and provides common functionality
    /// </summary>
    public abstract class BaseHandler : IBaseHandler
    {
        protected readonly UserProfileDto _currentUser;

        protected BaseHandler(UserProfileDto currentUser)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        }

        /// <summary>
        /// Template method for executing operations with common error handling
        /// </summary>
        protected async Task ExecuteOperationAsync(Func<Task> operation, string operationName)
        {
            try
            {
                await operation();
                await LogOperationAsync(operationName, true);
            }
            catch (Exception ex)
            {
                HandleException(ex, operationName);
                await LogOperationAsync(operationName, false, ex.Message);
            }
        }

        /// <summary>
        /// Validates input before processing
        /// Override in derived classes for specific validation
        /// </summary>
        public virtual async Task<bool> ValidateInputAsync(object input)
        {
            if (input == null)
            {
                ConsoleRenderingService.ShowMessageBox("❌ Dữ liệu đầu vào không hợp lệ!", true, 2000);
                return false;
            }
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Handles exceptions in a consistent way
        /// </summary>
        public virtual void HandleException(Exception ex, string operation)
        {
            var errorMessage = $"❌ Lỗi trong {operation}: {ex.Message}";
            ConsoleRenderingService.ShowMessageBox(errorMessage, true, 3000);
            
            // Log detailed error for debugging (in real app, use proper logging)
            Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {operation}: {ex}");
        }

        /// <summary>
        /// Logs operations for audit trail
        /// </summary>
        public virtual async Task LogOperationAsync(string operation, bool success, string? details = null)
        {
            var logMessage = $"[{(success ? "SUCCESS" : "FAILED")}] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - " +
                           $"User: {_currentUser.Username} - Operation: {operation}";
            
            if (!string.IsNullOrEmpty(details))
            {
                logMessage += $" - Details: {details}";
            }

            // In a real application, this would write to a proper logging system
            Console.WriteLine(logMessage);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Common method to show confirmation dialog
        /// </summary>
        protected bool ShowConfirmationDialog(string message)
        {
            Console.WriteLine($"\n⚠️  {message}");
            Console.Write("Bạn có chắc chắn muốn tiếp tục? (y/N): ");
            var input = Console.ReadLine()?.Trim().ToLower();
            return input == "y" || input == "yes";
        }

        /// <summary>
        /// Common method to display success message
        /// </summary>
        protected void ShowSuccessMessage(string message)
        {
            ConsoleRenderingService.ShowMessageBox($"✅ {message}", false, 2000);
        }

        /// <summary>
        /// Common method to display error message
        /// </summary>
        protected void ShowErrorMessage(string message)
        {
            ConsoleRenderingService.ShowMessageBox($"❌ {message}", true, 2000);
        }
    }
}
