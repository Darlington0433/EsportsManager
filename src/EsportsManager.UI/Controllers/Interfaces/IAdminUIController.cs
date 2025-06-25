using EsportsManager.BL.DTOs;

namespace EsportsManager.UI.Controllers.Interfaces;

public interface IAdminUIController : IController
{
    void ShowAdminMenu();
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserDetailsAsync(int userId);
    Task<List<UserDto>> SearchUsersAsync(string searchTerm);
}
