using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.Admin.Interfaces
{
    /// <summary>
    /// Interface for System Settings operations
    /// </summary>
    public interface ISystemSettingsHandler
    {
        Task HandleSystemConfigurationAsync();
        Task HandleSecuritySettingsAsync();
        Task HandleMaintenanceModeAsync();
        Task HandleBackupSettingsAsync();
        Task HandleNotificationSettingsAsync();
    }
}
