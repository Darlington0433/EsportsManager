using EsportsManager.BL.Constants;
using EsportsManager.BL.Utilities;
using System.Collections.Generic;

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
        var errors = new List<string>();
        if (ValidationHelper.IsNullOrEmpty(title, "Tiêu đề", errors) ||
            !ValidationHelper.ValidateLength(title!, "Tiêu đề", 1, 100, errors))
        {
            return (false, string.Join("; ", errors));
        }
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Validates user input for achievement description
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateAchievementDescription(string? description)
    {
        var errors = new List<string>();
        if (ValidationHelper.IsNullOrEmpty(description, "Mô tả", errors) ||
            !ValidationHelper.ValidateLength(description!, "Mô tả", 1, 500, errors))
        {
            return (false, string.Join("; ", errors));
        }
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Validates user ID input
    /// </summary>
    public static (bool IsValid, string ErrorMessage, int UserId) ValidateUserId(string? input)
    {
        var errors = new List<string>();
        if (ValidationHelper.IsNullOrEmpty(input, "User ID", errors))
        {
            return (false, string.Join("; ", errors), 0);
        }

        if (!int.TryParse(input, out int userId) || userId <= 0)
        {
            return (false, "User ID phải là số nguyên dương", 0);
        }
        
        return (true, string.Empty, userId);
    }
    
    /// <summary>
    /// Validates user status
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateUserStatus(string? status)
    {
        var errors = new List<string>();
        if (ValidationHelper.IsNullOrEmpty(status, "Trạng thái", errors) ||
            !ValidationHelper.IsValidValue(status!, UserConstants.VALID_USER_STATUSES, "Trạng thái", errors))
        {
            return (false, string.Join("; ", errors));
        }
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Validates user role
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateUserRole(string? role)
    {
        var errors = new List<string>();
        if (ValidationHelper.IsNullOrEmpty(role, "Vai trò", errors) ||
            !ValidationHelper.IsValidValue(role!, UserConstants.VALID_USER_ROLES, "Vai trò", errors))
        {
            return (false, string.Join("; ", errors));
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

    /// <summary>
    /// Validates user profile image
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateProfileImage(string? fileName, long fileSize)
    {
        var errors = new List<string>();
        var validExtensions = new[] { ".jpg", ".jpeg", ".png" };
        
        if (!string.IsNullOrEmpty(fileName))
        {
            ValidationHelper.IsValidFileExtension(fileName, validExtensions, errors);
            ValidationHelper.IsValidFileSize(fileSize, 5 * 1024 * 1024, errors); // 5MB max
        }
        
        return errors.Count > 0 ? (false, string.Join("; ", errors)) : (true, string.Empty);
    }

    /// <summary>
    /// Validates user contact information
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateContactInfo(string? phone, string? email)
    {
        var errors = new List<string>();
        
        if (!string.IsNullOrEmpty(phone))
        {
            ValidationHelper.IsValidPhone(phone, errors);
        }
        
        if (!string.IsNullOrEmpty(email))
        {
            ValidationHelper.IsValidEmail(email, errors);
        }
        
        return errors.Count > 0 ? (false, string.Join("; ", errors)) : (true, string.Empty);
    }
}
