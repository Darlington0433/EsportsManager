using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.UI.Constants;
using EsportsManager.UI.Utilities;
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
            await UIHelper.ExecuteWithErrorHandlingAsync(operation, operationName);
        }

        /// <summary>
        /// Validates input before processing
        /// Override in derived classes for specific validation
        /// </summary>
        public virtual async Task<bool> ValidateInputAsync(object input)
        {
            if (input == null)
            {
                UIHelper.ShowError(UIConstants.Messages.INVALID_INPUT);
                return false;
            }
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Handles exceptions in a consistent way
        /// </summary>
        public virtual void HandleException(Exception ex, string operation)
        {
            UIHelper.HandleError(ex, operation);
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
            return UIHelper.ShowConfirmDialog(message);
        }

        /// <summary>
        /// Common method to display success message
        /// </summary>
        protected void ShowSuccessMessage(string message)
        {
            UIHelper.ShowSuccess(message);
        }

        /// <summary>
        /// Common method to display error message
        /// </summary>
        protected void ShowErrorMessage(string message)
        {
            UIHelper.ShowError(message);
        }

        /// <summary>
        /// Common method to display warning message
        /// </summary>
        protected void ShowWarningMessage(string message)
        {
            UIHelper.ShowWarning(message);
        }

        /// <summary>
        /// Common method to display info message
        /// </summary>
        protected void ShowInfoMessage(string message)
        {
            UIHelper.ShowInfo(message);
        }

        /// <summary>
        /// Common method to display delete confirmation dialog
        /// </summary>
        protected bool ShowDeleteConfirmationDialog()
        {
            return UIHelper.ShowDeleteConfirmDialog();
        }

        /// <summary>
        /// Common method to display update confirmation dialog
        /// </summary>
        protected bool ShowUpdateConfirmationDialog()
        {
            return UIHelper.ShowUpdateConfirmDialog();
        }

        /// <summary>
        /// Draws a titled border
        /// </summary>
        protected void DrawTitledBorder(string title, int width = UIConstants.Border.DEFAULT_WIDTH, int height = UIConstants.Border.DEFAULT_HEIGHT)
        {
            UIHelper.DrawTitledBorder(title, width, height);
        }
    }
}
