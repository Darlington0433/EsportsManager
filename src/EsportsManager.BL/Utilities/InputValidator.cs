using System.Text.RegularExpressions;
using EsportsManager.BL.Models;
using System.Linq;
using System.Collections.Generic;

namespace EsportsManager.BL.Utilities;

/// <summary>
/// Input validator utility - áp dụng Single Responsibility Principle
/// Chỉ lo về việc validate input data
/// </summary>
public static class InputValidator
{
    /// <summary>
    /// Validate username
    /// </summary>
    public static ValidationResult ValidateUsername(string username)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(username))
        {
            errors.Add("Username is required");
        }
        else
        {
            if (username.Length < 3)
                errors.Add("Username must be at least 3 characters long");

            if (username.Length > 50)
                errors.Add("Username cannot exceed 50 characters");

            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
                errors.Add("Username can only contain letters, numbers, and underscores");

            if (username.StartsWith("_") || username.EndsWith("_"))
                errors.Add("Username cannot start or end with underscore");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    /// <summary>
    /// Validate email
    /// </summary>
    public static ValidationResult ValidateEmail(string? email)
    {
        var errors = new List<string>();

        if (!string.IsNullOrWhiteSpace(email))
        {
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("Invalid email format");

            if (email.Length > 255)
                errors.Add("Email cannot exceed 255 characters");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    /// <summary>
    /// Validate password
    /// </summary>
    public static ValidationResult ValidatePassword(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required");
        }
        else
        {
            if (!PasswordHasher.IsPasswordStrong(password))
            {
                errors.Add(PasswordHasher.GetPasswordRequirements());
            }
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    /// <summary>
    /// Validate password confirmation
    /// </summary>
    public static ValidationResult ValidatePasswordConfirmation(string password, string confirmPassword)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(confirmPassword))
        {
            errors.Add("Password confirmation is required");
        }
        else if (password != confirmPassword)
        {
            errors.Add("Passwords do not match");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    /// <summary>
    /// Validate user role
    /// </summary>
    public static ValidationResult ValidateRole(string role)
    {
        var errors = new List<string>();
        var validRoles = new[] { "Admin", "Player", "Viewer" };

        if (string.IsNullOrWhiteSpace(role))
        {
            errors.Add("Role is required");
        }
        else if (!validRoles.Contains(role))
        {
            errors.Add($"Invalid role. Valid roles are: {string.Join(", ", validRoles)}");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    /// <summary>
    /// Validate user status
    /// </summary>
    public static ValidationResult ValidateStatus(string status)
    {
        var errors = new List<string>();
        var validStatuses = new[] { "Active", "Suspended", "Inactive" };

        if (string.IsNullOrWhiteSpace(status))
        {
            errors.Add("Status is required");
        }
        else if (!validStatuses.Contains(status))
        {
            errors.Add($"Invalid status. Valid statuses are: {string.Join(", ", validStatuses)}");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    /// <summary>
    /// Validate user ID
    /// </summary>
    public static ValidationResult ValidateUserId(int userId)
    {
        var errors = new List<string>();

        if (userId <= 0)
        {
            errors.Add("User ID must be a positive integer");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    /// <summary>
    /// Validate string length
    /// </summary>
    public static ValidationResult ValidateStringLength(string value, string fieldName, int minLength = 0, int maxLength = int.MaxValue)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(value))
        {
            if (minLength > 0)
                errors.Add($"{fieldName} is required");
        }
        else
        {
            if (value.Length < minLength)
                errors.Add($"{fieldName} must be at least {minLength} characters long");

            if (value.Length > maxLength)
                errors.Add($"{fieldName} cannot exceed {maxLength} characters");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    /// <summary>
    /// Combine multiple validation results
    /// </summary>
    public static ValidationResult CombineResults(params ValidationResult[] results)
    {
        var allErrors = new List<string>();

        foreach (var result in results)
        {
            if (!result.IsValid)
            {
                allErrors.AddRange(result.Errors);
            }
        }

        return allErrors.Any() ? ValidationResult.Failure(allErrors) : ValidationResult.Success();
    }
}
