// Controller xử lý chức năng Player

using System;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Utilities;
using EsportsManager.BL.DTOs;

namespace EsportsManager.UI.Controllers;

public class PlayerController
{
    private readonly UserProfileDto _currentUser;

    public PlayerController(UserProfileDto currentUser)
    {
        _currentUser = currentUser;
    }

    public void ShowPlayerMenu()
    {
        while (true)
        {
            var menuOptions = new[]
            {
                "Đăng ký tham gia giải đấu",
                "Quản lý team",
                "Xem thông tin cá nhân",
                "Cập nhật thông tin cá nhân", 
                "Xem danh sách giải đấu",
                "Gửi feedback giải đấu",
                "Quản lý ví điện tử",
                "Thành tích cá nhân",
                "Đăng xuất"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu($"MENU PLAYER - {_currentUser.Username}", menuOptions);

            switch (selection)
            {
                case 0:
                    RegisterForTournament();
                    break;
                case 1:
                    ManageTeam();
                    break;
                case 2:
                    ViewPersonalInfo();
                    break;
                case 3:
                    UpdatePersonalInfo();
                    break;
                case 4:
                    ViewTournamentList();
                    break;
                case 5:
                    SubmitTournamentFeedback();
                    break;
                case 6:
                    ManageWallet();
                    break;
                case 7:
                    ViewPlayerAchievements();
                    break;
                case 8:
                case -1:
                    return; // Đăng xuất
            }
        }
    }

    private void RegisterForTournament()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng đăng ký giải đấu đang được phát triển", false, 2000);
    }

    private void ManageTeam()
    {
        var teamOptions = new[]
        {
            "Tạo team mới",
            "Tham gia team",
            "Rời khỏi team",
            "Xem thông tin team",
            "Quản lý thành viên team"
        };

        int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ TEAM", teamOptions);
        
        switch (selection)
        {
            case 0:
                ConsoleRenderingService.ShowMessageBox("Chức năng tạo team đang được phát triển", false, 2000);
                break;
            case 1:
                ConsoleRenderingService.ShowMessageBox("Chức năng tham gia team đang được phát triển", false, 2000);
                break;
            case 2:
                ConsoleRenderingService.ShowMessageBox("Chức năng rời team đang được phát triển", false, 2000);
                break;
            case 3:
                ConsoleRenderingService.ShowMessageBox("Chức năng xem thông tin team đang được phát triển", false, 2000);
                break;
            case 4:
                ConsoleRenderingService.ShowMessageBox("Chức năng quản lý thành viên đang được phát triển", false, 2000);
                break;
        }
    }    private void ViewPersonalInfo()
    {
        Console.Clear();
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        
        // Vẽ khung thông tin
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        int boxWidth = Math.Min(70, windowWidth - 6);
        int boxHeight = Math.Min(20, windowHeight - 4);
        int left = Math.Max(1, (windowWidth - boxWidth) / 2);
        int top = Math.Max(1, (windowHeight - boxHeight) / 2);
        
        ConsoleRenderingService.DrawBorder(left, top, boxWidth, boxHeight, "[THÔNG TIN CÁ NHÂN]", true);
        
        // Hiển thị thông tin
        int contentY = top + 3;
        var infoLines = new[]
        {
            $"Tên đăng nhập: {_currentUser.Username}",
            $"Email: {_currentUser.Email}",
            $"Vai trò: {_currentUser.Role}",
            $"Ngày tạo tài khoản: {_currentUser.CreatedAt:dd/MM/yyyy HH:mm}",
            $"Lần đăng nhập cuối: {_currentUser.LastLoginAt?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa có"}",
            $"Trạng thái tài khoản: {_currentUser.Status}",
            "",
            "Nhấn phím bất kỳ để quay lại..."
        };
        
        for (int i = 0; i < infoLines.Length; i++)
        {
            Console.SetCursorPosition(left + 3, contentY + i);
            if (i == infoLines.Length - 1) // Dòng cuối cùng (hướng dẫn)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.Write(infoLines[i]);
        }
        
        Console.ResetColor();
        Console.ReadKey(true);
    }

    private void UpdatePersonalInfo()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng cập nhật thông tin đang được phát triển", false, 2000);
    }

    private void ViewTournamentList()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng xem danh sách giải đấu đang được phát triển", false, 2000);
    }

    private void SubmitTournamentFeedback()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng gửi feedback đang được phát triển", false, 2000);
    }    private void ManageWallet()
    {
        // Mock data cho wallet
        decimal currentBalance = 250000; // Mock số dư hiện tại
        
        while (true)
        {
            var walletOptions = new[]
            {
                $"Xem số dư ví (Hiện tại: {currentBalance:N0} VND)",
                "Nạp tiền vào ví",
                "Rút tiền từ ví",
                "Lịch sử giao dịch",
                "Donate cho player khác",
                "Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ VÍ ĐIỆN TỬ", walletOptions);
            
            switch (selection)
            {
                case 0:
                    ViewWalletBalance(currentBalance);
                    break;
                case 1:
                    currentBalance = DepositMoney(currentBalance);
                    break;
                case 2:
                    currentBalance = WithdrawMoney(currentBalance);
                    break;
                case 3:
                    ViewTransactionHistory();
                    break;
                case 4:
                    DonateMoney(currentBalance);
                    break;
                case 5:
                case -1:
                    return;
            }
        }
    }
    
    private void ViewWalletBalance(decimal balance)
    {
        Console.Clear();
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        int boxWidth = Math.Min(50, windowWidth - 6);
        int boxHeight = Math.Min(10, windowHeight - 4);
        int left = Math.Max(1, (windowWidth - boxWidth) / 2);
        int top = Math.Max(1, (windowHeight - boxHeight) / 2);
        
        ConsoleRenderingService.DrawBorder(left, top, boxWidth, boxHeight, "[SỐ DƯ VÍ]", true);
        
        Console.SetCursorPosition(left + 3, top + 3);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Số dư hiện tại: {balance:N0} VND");
        
        Console.SetCursorPosition(left + 3, top + 6);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("Nhấn phím bất kỳ để tiếp tục...");
        
        Console.ResetColor();
        Console.ReadKey(true);
    }
    
    private decimal DepositMoney(decimal currentBalance)
    {
        Console.Clear();
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        int boxWidth = Math.Min(60, windowWidth - 6);
        int boxHeight = Math.Min(12, windowHeight - 4);
        int left = Math.Max(1, (windowWidth - boxWidth) / 2);
        int top = Math.Max(1, (windowHeight - boxHeight) / 2);
        
        ConsoleRenderingService.DrawBorder(left, top, boxWidth, boxHeight, "[NẠP TIỀN VÀO VÍ]", true);
        
        Console.SetCursorPosition(left + 3, top + 3);
        Console.Write($"Số dư hiện tại: {currentBalance:N0} VND");
        
        Console.SetCursorPosition(left + 3, top + 5);
        Console.Write("Nhập số tiền muốn nạp (VND): ");
        
        Console.SetCursorPosition(left + 32, top + 5);
        string? amountStr = UnifiedInputService.ReadText(20, c => char.IsDigit(c));
        
        if (decimal.TryParse(amountStr, out decimal amount) && amount > 0)
        {
            decimal newBalance = currentBalance + amount;
            ConsoleRenderingService.ShowMessageBox($"Nạp tiền thành công! Số dư mới: {newBalance:N0} VND", false, 2000);
            return newBalance;
        }
        else
        {
            ConsoleRenderingService.ShowMessageBox("Số tiền không hợp lệ!", true, 2000);
            return currentBalance;
        }
    }
    
    private decimal WithdrawMoney(decimal currentBalance)
    {
        Console.Clear();
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        int boxWidth = Math.Min(60, windowWidth - 6);
        int boxHeight = Math.Min(12, windowHeight - 4);
        int left = Math.Max(1, (windowWidth - boxWidth) / 2);
        int top = Math.Max(1, (windowHeight - boxHeight) / 2);
        
        ConsoleRenderingService.DrawBorder(left, top, boxWidth, boxHeight, "[RÚT TIỀN TỪ VÍ]", true);
        
        Console.SetCursorPosition(left + 3, top + 3);
        Console.Write($"Số dư hiện tại: {currentBalance:N0} VND");
        
        Console.SetCursorPosition(left + 3, top + 5);
        Console.Write("Nhập số tiền muốn rút (VND): ");
        
        Console.SetCursorPosition(left + 32, top + 5);
        string? amountStr = UnifiedInputService.ReadText(20, c => char.IsDigit(c));
        
        if (decimal.TryParse(amountStr, out decimal amount) && amount > 0)
        {
            if (amount <= currentBalance)
            {
                decimal newBalance = currentBalance - amount;
                ConsoleRenderingService.ShowMessageBox($"Rút tiền thành công! Số dư còn lại: {newBalance:N0} VND", false, 2000);
                return newBalance;
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("Số dư không đủ để rút!", true, 2000);
                return currentBalance;
            }
        }
        else
        {
            ConsoleRenderingService.ShowMessageBox("Số tiền không hợp lệ!", true, 2000);
            return currentBalance;
        }
    }
    
    private void ViewTransactionHistory()
    {
        // Mock data cho lịch sử giao dịch
        var transactions = new[]
        {
            "15/06/2024 09:30 - Nạp tiền: +100,000 VND",
            "12/06/2024 14:15 - Rút tiền: -50,000 VND", 
            "10/06/2024 11:20 - Donate: -25,000 VND (→ PlayerXYZ)",
            "08/06/2024 16:45 - Nạp tiền: +200,000 VND",
            "05/06/2024 13:30 - Phí giải đấu: -30,000 VND"
        };
        
        Console.Clear();
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        int boxWidth = Math.Min(70, windowWidth - 6);
        int boxHeight = Math.Min(15, windowHeight - 4);
        int left = Math.Max(1, (windowWidth - boxWidth) / 2);
        int top = Math.Max(1, (windowHeight - boxHeight) / 2);
        
        ConsoleRenderingService.DrawBorder(left, top, boxWidth, boxHeight, "[LỊCH SỬ GIAO DỊCH]", true);
        
        Console.SetCursorPosition(left + 3, top + 3);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("5 giao dịch gần nhất:");
        
        for (int i = 0; i < transactions.Length; i++)
        {
            Console.SetCursorPosition(left + 3, top + 5 + i);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{i + 1}. {transactions[i]}");
        }
        
        Console.SetCursorPosition(left + 3, top + boxHeight - 3);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("Nhấn phím bất kỳ để quay lại...");
        
        Console.ResetColor();
        Console.ReadKey(true);
    }
    
    private void DonateMoney(decimal currentBalance)
    {
        ConsoleRenderingService.ShowMessageBox($"Chức năng donate đang được phát triển.\nSố dư hiện tại: {currentBalance:N0} VND", false, 2000);
    }

    private void ViewPlayerAchievements()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng xem thành tích đang được phát triển", false, 2000);
    }
}
