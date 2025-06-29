using System;
using System.Threading.Tasks;
using EsportsManager.UI.Constants;
using EsportsManager.UI.ConsoleUI.Utilities;
using System.Linq;
using System.Text;

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

    /// <summary>
    /// In bảng dữ liệu trong border (header + rows), cho phép truyền colWidths để căn đều từng cột
    /// </summary>
    public static void PrintTableInBorder(
        string[] headers,
        List<string[]> rows,
        int borderWidth,
        int borderHeight,
        int left,
        int top,
        int[]? colWidths = null)
    {
        int contentWidth = borderWidth - 10; // Tăng padding để đảm bảo không tràn ra ngoài
        int colCount = headers.Length;
        
        // Nếu không truyền colWidths thì chia đều
        if (colWidths == null || colWidths.Length != colCount)
        {
            colWidths = Enumerable.Repeat(contentWidth / colCount, colCount).ToArray();
        }
        
        // Đảm bảo tổng độ rộng các cột không vượt quá contentWidth
        int totalColWidth = colWidths.Sum() + ((colCount - 1) * 3); // Tính cả separator
        if (totalColWidth > contentWidth)
        {
            // Giảm độ rộng các cột một cách tỷ lệ
            double ratio = (double)(contentWidth - ((colCount - 1) * 3)) / colWidths.Sum();
            for (int i = 0; i < colWidths.Length; i++)
            {
                colWidths[i] = Math.Max(3, (int)(colWidths[i] * ratio));
            }
        }
        
        int currentY = top + 2;

        // In header với màu nổi bật
        Console.SetCursorPosition(left + 2, currentY++);
        Console.ForegroundColor = ConsoleColor.White;
        
        // Tạo dòng header
        StringBuilder headerLine = new StringBuilder();
        for (int i = 0; i < headers.Length; i++)
        {
            string headerText = headers[i];
            if (headerText.Length > colWidths[i] - 1)
            {
                headerText = headerText.Substring(0, colWidths[i] - 3) + "...";
            }
            headerLine.Append(headerText.PadRight(colWidths[i]));
            if (i < headers.Length - 1) headerLine.Append(" | ");
        }
        
        // Đảm bảo headerLine không vượt quá contentWidth
        string headerLineStr = headerLine.ToString();
        if (headerLineStr.Length > contentWidth)
        {
            headerLineStr = headerLineStr.Substring(0, contentWidth);
        }
        Console.WriteLine(headerLineStr);

        // Separator line với ký tự đặc biệt để tạo đường phân cách rõ ràng
        Console.SetCursorPosition(left + 2, currentY++);
        int sepLen = Math.Min(totalColWidth, contentWidth);
        Console.WriteLine(new string('═', sepLen));
        
        // In từng dòng data với màu thường
        Console.ForegroundColor = ConsoleColor.Gray;
        foreach (var row in rows)
        {
            if (currentY >= top + borderHeight - 2) break; // Đảm bảo không vượt quá chiều cao border
            
            Console.SetCursorPosition(left + 2, currentY++);
            StringBuilder rowLine = new StringBuilder();
            
            for (int i = 0; i < Math.Min(row.Length, colWidths.Length); i++)
            {
                string cellText = row[i] ?? "";
                if (cellText.Length > colWidths[i] - 1)
                {
                    cellText = cellText.Substring(0, colWidths[i] - 3) + "...";
                }
                rowLine.Append(cellText.PadRight(colWidths[i]));
                if (i < colWidths.Length - 1) rowLine.Append(" | ");
            }
            
            // Đảm bảo rowLine không vượt quá contentWidth
            string rowLineStr = rowLine.ToString();
            if (rowLineStr.Length > contentWidth)
            {
                rowLineStr = rowLineStr.Substring(0, contentWidth);
            }
            Console.WriteLine(rowLineStr);
        }
        
        // Separator line dưới cùng để tạo viền hoàn chỉnh
        if (rows.Count > 0 && currentY < top + borderHeight - 2)
        {
            Console.SetCursorPosition(left + 2, currentY++);
            Console.WriteLine(new string('─', sepLen));
        }
        
        Console.ResetColor();
    }

    /// <summary>
    /// In prompt hoặc thông báo trong border
    /// </summary>
    public static void PrintPromptInBorder(string message, int left, int y, int width)
    {
        Console.SetCursorPosition(left + 2, y);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message.PadRight(width));
        Console.ResetColor();
    }

    /// <summary>
    /// In thông báo màu trong border
    /// </summary>
    public static void PrintMessageInBorder(string message, int left, int y, int width, ConsoleColor color)
    {
        Console.SetCursorPosition(left, y);
        Console.ForegroundColor = color;
        Console.WriteLine(message.PadRight(width));
        Console.ResetColor();
    }
} 