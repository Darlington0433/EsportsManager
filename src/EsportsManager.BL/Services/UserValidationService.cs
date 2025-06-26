using EsportsManager.BL.Constants;

namespace EsportsManager.BL.Services;

/// <summary>
/// User validation and formatting service for business logic
/// </summary>
public class UserValidationService
{
    /// <summary>
    /// Validates user input for achievement title
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateAchievementTitle(string? title)
    {
        if (string.IsNullOrEmpty(title?.Trim()))
        {
            return (false, "Tiêu đề không được rỗng!");
        }
        
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Validates user input for achievement description
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateAchievementDescription(string? description)
    {
        if (string.IsNullOrEmpty(description?.Trim()))
        {
            return (false, "Mô tả không được rỗng!");
        }
        
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Validates user ID input
    /// </summary>
    public static (bool IsValid, string ErrorMessage, int UserId) ValidateUserId(string? input)
    {
        if (!int.TryParse(input, out int userId))
        {
            return (false, "User ID không hợp lệ!", 0);
        }
        
        if (userId <= 0)
        {
            return (false, "User ID phải lớn hơn 0!", 0);
        }
        
        return (true, string.Empty, userId);
    }
    
    /// <summary>
    /// Validates user status
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateUserStatus(string? status)
    {
        if (string.IsNullOrEmpty(status) || !UserConstants.VALID_USER_STATUSES.Contains(status))
        {
            return (false, $"Trạng thái không hợp lệ! Các giá trị cho phép: {string.Join(", ", UserConstants.VALID_USER_STATUSES)}");
        }
        
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Validates user role
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateUserRole(string? role)
    {
        if (string.IsNullOrEmpty(role) || !UserConstants.VALID_USER_ROLES.Contains(role))
        {
            return (false, $"Vai trò không hợp lệ! Các giá trị cho phép: {string.Join(", ", UserConstants.VALID_USER_ROLES)}");
        }
        
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Validates confirmation input for yes/no questions
    /// </summary>
    public static bool IsConfirmationYes(string? input)
    {
        return input?.ToLower() == UserConstants.CONFIRM_YES || 
               input?.ToLower() == UserConstants.CONFIRM_YES_FULL;
    }
    
    /// <summary>
    /// Validates confirmation input for delete operations
    /// </summary>
    public static bool IsDeleteConfirmation(string? input)
    {
        return input?.ToUpper() == UserConstants.CONFIRM_DELETE;
    }
}
