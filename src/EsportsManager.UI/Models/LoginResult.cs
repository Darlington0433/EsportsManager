// Lớp lưu trữ thông tin user đăng nhập

using EsportsManager.BL.DTOs;

namespace EsportsManager.UI.Models;

public class LoginResult
{
    public bool IsSuccess { get; set; }
    public UserProfileDto? UserProfile { get; set; }
    public string? ErrorMessage { get; set; }
    
    public static LoginResult Success(UserProfileDto userProfile)
    {
        return new LoginResult
        {
            IsSuccess = true,
            UserProfile = userProfile
        };
    }
    
    public static LoginResult Failure(string errorMessage)
    {
        return new LoginResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}
