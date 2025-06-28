using System;
using System.Collections.Generic;

namespace EsportsManager.BL.DTOs
{

    /// <summary>
    /// User Data Transfer Object - áp dụng Single Responsibility Principle
    /// Chỉ chứa dữ liệu cần thiết để transfer giữa các layers
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public required string Role { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public bool IsEmailVerified { get; set; }
        public string? EmailVerificationToken { get; set; }
        public int TotalLogins { get; set; }
        public TimeSpan TotalTimeOnline { get; set; }
    }

    /// <summary>
    /// DTO cho user profile
    /// </summary>
    public class UserProfileDto
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public required string Role { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public int TotalLogins { get; set; }
        public TimeSpan TotalTimeOnline { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public bool IsEmailVerified { get; set; }
    }

    /// <summary>
    /// Kết quả xác thực
    /// </summary>
    public class AuthenticationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserDto? User { get; set; }
        public string? Token { get; set; }
        public List<string> Errors { get; set; } = new();

        // Factory methods for clean code and consistency
        public static AuthenticationResult Success(UserDto user, string? token = null, string? message = null)
        {
            return new AuthenticationResult
            {
                IsSuccess = true,
                User = user,
                Token = token,
                Message = message ?? string.Empty,
                Errors = new List<string>()
            };
        }

        public static AuthenticationResult Failure(string message, List<string>? errors = null)
        {
            return new AuthenticationResult
            {
                IsSuccess = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }

}
