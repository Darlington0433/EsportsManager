using System;
using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.MenuHandlers.Interfaces.Shared
{
    /// <summary>
    /// Base interface for all menu handlers
    /// Provides common error handling and validation patterns
    /// </summary>
    public interface IBaseHandler
    {
        /// <summary>
        /// Validates input before processing
        /// </summary>
        Task<bool> ValidateInputAsync(object input);

        /// <summary>
        /// Handles exceptions in a consistent way
        /// </summary>
        void HandleException(Exception ex, string operation);

        /// <summary>
        /// Logs operations for audit trail
        /// </summary>
        Task LogOperationAsync(string operation, bool success, string? details = null);
    }
}
