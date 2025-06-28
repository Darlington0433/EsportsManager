using System;
using System.Threading.Tasks;
using EsportsManager.UI.Constants;
using EsportsManager.UI.ConsoleUI.Utilities;

namespace EsportsManager.UI.Utilities;

/// <summary>
/// Helper class cho các thao tác UI chung
/// </summary>
public static class UIHelper
{
    /// <summary>
    /// Hiển thị thông báo thành công
    /// </summary>
    public static void ShowSuccess(string message, int timeout = UIConstants.MessageTimeout.NORMAL)
    {
        ConsoleRenderingService.ShowMessageBox(
            $"{UIConstants.Icons.SUCCESS} {message}",
            false,
            timeout);
    }

    /// <summary>
    /// Hiển thị thông báo lỗi
    /// </summary>
    public static void ShowError(string message, int timeout = UIConstants.MessageTimeout.ERROR)
    {
        ConsoleRenderingService.ShowMessageBox(
            $"{UIConstants.Icons.ERROR} {message}",
            true,
            timeout);
    }

    /// <summary>
    /// Hiển thị thông báo cảnh báo
    /// </summary>
    public static void ShowWarning(string message, int timeout = UIConstants.MessageTimeout.NORMAL)
    {
        ConsoleRenderingService.ShowMessageBox(
            $"{UIConstants.Icons.WARNING} {message}",
            true,
            timeout);
    }

    /// <summary>
    /// Hiển thị thông báo thông tin
    /// </summary>
    public static void ShowInfo(string message, int timeout = UIConstants.MessageTimeout.NORMAL)
    {
        ConsoleRenderingService.ShowMessageBox(
            $"{UIConstants.Icons.INFO} {message}",
            false,
            timeout);
    }

    /// <summary>
    /// Hiển thị loading message
    /// </summary>
    public static void ShowLoading(string message = UIConstants.Messages.LOADING)
    {
        ConsoleRenderingService.ShowMessageBox(message, false, 0);
    }

    /// <summary>
    /// Hiển thị hộp thoại xác nhận
    /// </summary>
    public static bool ShowConfirmDialog(string message)
    {
        Console.WriteLine($"\n{UIConstants.Icons.WARNING} {message}");
        Console.Write("Bạn có chắc chắn muốn tiếp tục? (y/N): ");
        var input = Console.ReadLine()?.Trim().ToLower();
        return input == "y" || input == "yes";
    }

    /// <summary>
    /// Hiển thị hộp thoại xác nhận xóa
    /// </summary>
    public static bool ShowDeleteConfirmDialog()
    {
        return ShowConfirmDialog(UIConstants.Messages.CONFIRM_DELETE);
    }

    /// <summary>
    /// Hiển thị hộp thoại xác nhận cập nhật
    /// </summary>
    public static bool ShowUpdateConfirmDialog()
    {
        return ShowConfirmDialog(UIConstants.Messages.CONFIRM_UPDATE);
    }

    /// <summary>
    /// Vẽ border với tiêu đề
    /// </summary>
    public static void DrawTitledBorder(string title, int width = UIConstants.Border.DEFAULT_WIDTH, int height = UIConstants.Border.DEFAULT_HEIGHT)
    {
        ConsoleRenderingService.DrawBorder(title, width, height);
    }

    /// <summary>
    /// Xử lý lỗi chung cho các thao tác
    /// </summary>
    public static void HandleError(Exception ex, string operation)
    {
        ShowError($"Lỗi trong {operation}: {ex.Message}");
        // Log error for debugging
        Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {operation}: {ex}");
    }

    /// <summary>
    /// Thực thi một thao tác async với xử lý lỗi
    /// </summary>
    public static async Task ExecuteWithErrorHandlingAsync(Func<Task> operation, string operationName)
    {
        try
        {
            ShowLoading();
            await operation();
            ShowSuccess(UIConstants.Messages.OPERATION_SUCCESS);
        }
        catch (Exception ex)
        {
            HandleError(ex, operationName);
        }
    }

    /// <summary>
    /// Đọc input số nguyên từ console với validation
    /// </summary>
    public static bool TryReadInt(string prompt, out int value, string fieldName = "")
    {
        Console.Write(prompt);
        if (!int.TryParse(Console.ReadLine(), out value))
        {
            ShowError($"{fieldName} phải là số nguyên!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Đọc input số thực từ console với validation
    /// </summary>
    public static bool TryReadDecimal(string prompt, out decimal value, string fieldName = "")
    {
        Console.Write(prompt);
        if (!decimal.TryParse(Console.ReadLine(), out value))
        {
            ShowError($"{fieldName} phải là số!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Đọc input string từ console với validation độ dài
    /// </summary>
    public static string? ReadString(string prompt, int minLength = 0, int maxLength = int.MaxValue, bool allowEmpty = true)
    {
        Console.Write(prompt);
        var input = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(input))
        {
            if (!allowEmpty)
            {
                ShowError(UIConstants.Messages.INVALID_INPUT);
                return null;
            }
            return input;
        }

        if (input.Length < minLength || input.Length > maxLength)
        {
            ShowError($"Độ dài phải từ {minLength} đến {maxLength} ký tự!");
            return null;
        }

        return input;
    }
} 