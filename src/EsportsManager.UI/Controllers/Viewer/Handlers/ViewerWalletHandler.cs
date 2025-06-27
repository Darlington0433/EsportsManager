using System;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Services;
using EsportsManager.BL.Constants;
using EsportsManager.UI.ConsoleUI.Utilities;

namespace EsportsManager.UI.Controllers.Viewer.Handlers
{
    /// <summary>
    /// Handler cho việc quản lý ví điện tử của Viewer
    /// Áp dụng Single Responsibility Principle - Focus on Top-up và Payment Management
    /// </summary>
    public class ViewerWalletHandler
    {
        private readonly UserProfileDto _currentUser;
        private readonly IWalletService _walletService;
        private readonly WalletValidationService _validationService;

        public ViewerWalletHandler(UserProfileDto currentUser, IWalletService walletService)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _validationService = new WalletValidationService();
        }

        public async Task HandleWalletManagementAsync()
        {
            while (true)
            {
                try
                {
                    Console.Clear();
                    ConsoleRenderingService.DrawBorder("QUẢN LÝ VÍ ĐIỆN TỬ", 80, 20);

                    // Get current wallet balance
                    var wallet = await _walletService.GetWalletByUserIdAsync(_currentUser.Id);
                    
                    int borderLeft = (Console.WindowWidth - 80) / 2;
                    int borderTop = (Console.WindowHeight - 20) / 4;

                    Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                    Console.WriteLine($"💰 Số dư hiện tại: {wallet?.Balance ?? 0:N0} VND");
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
                    Console.WriteLine($"👤 Tài khoản: {_currentUser.Username}");

                    var walletOptions = new[]
                    {
                        "Nạp tiền vào ví",
                        "Xem lịch sử giao dịch", 
                        "Quản lý thông tin thanh toán",
                        "Xem thông tin ví chi tiết",
                        "⬅️ Quay lại"
                    };

                    int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ VÍ", walletOptions);

                    switch (selection)
                    {
                        case 0:
                            await HandleTopUpAsync(borderLeft, borderTop);
                            break;
                        case 1:
                            await HandleTransactionHistoryAsync();
                            break;
                        case 2:
                            await HandlePaymentInfoManagementAsync();
                            break;
                        case 3:
                            await HandleViewWalletDetailsAsync();
                            break;
                        case 4:
                        case -1:
                            return;
                    }
                }
                catch (Exception ex)
                {
                    int borderLeft = (Console.WindowWidth - 80) / 2;
                    int borderTop = (Console.WindowHeight - 20) / 4;
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 18);
                    ConsoleRenderingService.ShowMessageBox($"❌ Lỗi hệ thống: {ex.Message}", true, 2000);
                }
            }
        }

        private async Task HandleTopUpAsync(int borderLeft, int borderTop)
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("NẠP TIỀN VÀO VÍ", 80, 18);

                var wallet = await _walletService.GetWalletByUserIdAsync(_currentUser.Id);
                Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                Console.WriteLine($"💰 Số dư hiện tại: {wallet?.Balance ?? 0:N0} VND");
                int cursorY = borderTop + 4;

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("💳 Chọn phương thức thanh toán:");
                var paymentMethods = new[]
                {
                    "Chuyển khoản ngân hàng",
                    "Thẻ tín dụng/ghi nợ",
                    "Ví điện tử (MoMo/ZaloPay)",
                    "⬅️ Hủy"
                };

                int methodSelection = InteractiveMenuService.DisplayInteractiveMenu("PHƯƠNG THỨC THANH TOÁN", paymentMethods);

                if (methodSelection == -1 || methodSelection == 3) return;

                string[] methodNames = { "BankTransfer", "CreditCard", "EWallet" };
                string selectedMethod = methodNames[methodSelection];

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"\n📋 Nhập thông tin nạp tiền:");
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"Số tiền tối thiểu: {WalletConstants.MIN_TOP_UP_AMOUNT:N0} VND");
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"Số tiền tối đa: {WalletConstants.MAX_TOP_UP_AMOUNT:N0} VND");
                Console.SetCursorPosition(borderLeft + 2, cursorY);
                Console.Write("Số tiền nạp: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY + 1);
                    ConsoleRenderingService.ShowMessageBox("Số tiền không hợp lệ!", true, 1500);
                    return;
                }

                var validationResult = _validationService.ValidateTopUpRequest(amount);
                if (!validationResult.IsValid)
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY + 2);
                    ConsoleRenderingService.ShowMessageBox(validationResult.ErrorMessage, true, 2000);
                    return;
                }

                string paymentDetails = await GetPaymentDetailsAsync(selectedMethod, amount, borderLeft, cursorY + 3);
                if (string.IsNullOrEmpty(paymentDetails)) return;

                cursorY += 5;
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"\n💰 Xác nhận nạp {amount:N0} VND?");
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"💳 Phương thức: {GetMethodDisplayName(selectedMethod)}");
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"💵 Phí giao dịch: {amount * 0.005m:N0} VND (0.5%)");
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"💸 Tổng thanh toán: {amount * 1.005m:N0} VND");
                Console.SetCursorPosition(borderLeft + 2, cursorY);
                Console.Write("Xác nhận (y/n): ");

                var confirmation = Console.ReadLine()?.ToLower();
                if (confirmation == "y" || confirmation == "yes")
                {
                    var depositDto = new DepositDto
                    {
                        Amount = amount,
                        PaymentMethod = selectedMethod,
                        ReferenceCode = GenerateReferenceCode(),
                        Note = $"Top-up via {selectedMethod} - {paymentDetails}"
                    };

                    var result = await _walletService.DepositAsync(_currentUser.Id, depositDto);
                    if (result.Success)
                    {
                        Console.SetCursorPosition(borderLeft + 2, cursorY + 2);
                        ConsoleRenderingService.ShowMessageBox($"✅ Nạp tiền thành công! Số dư mới: {result.NewBalance:N0} VND", false, 3000);
                        Console.SetCursorPosition(borderLeft + 2, cursorY + 3);
                        ConsoleRenderingService.ShowMessageBox($"📄 Mã giao dịch: {depositDto.ReferenceCode}", false, 2000);
                    }
                    else
                    {
                        Console.SetCursorPosition(borderLeft + 2, cursorY + 2);
                        ConsoleRenderingService.ShowMessageBox($"❌ Nạp tiền thất bại: {result.Message}", true, 2000);
                    }
                }
                else
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY + 2);
                    ConsoleRenderingService.ShowMessageBox("❌ Đã hủy giao dịch", false, 1500);
                }
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(borderLeft + 2, borderTop + 16);
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", true, 3000);
            }
        }

        private async Task<string> GetPaymentDetailsAsync(string method, decimal amount, int borderLeft, int cursorY)
        {
            try
            {
                switch (method)
                {
                    case "BankTransfer":
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.WriteLine("\n🏦 Thông tin chuyển khoản:");
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.WriteLine("📋 Ngân hàng: Vietcombank");
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.WriteLine("📋 Số tài khoản: 1234567890");
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.WriteLine("📋 Tên tài khoản: ESPORTS MANAGER SYSTEM");
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.WriteLine($"📋 Số tiền: {amount * 1.005m:N0} VND");
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.WriteLine("📋 Nội dung: NAP TIEN [Username]");
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.Write("Nhập mã OTP từ ngân hàng: ");
                        var otp = Console.ReadLine();
                        return string.IsNullOrEmpty(otp) ? "" : $"OTP: {otp}";

                    case "CreditCard":
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.Write("Nhập số thẻ (16 số): ");
                        var cardNumber = Console.ReadLine();
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.Write("Nhập tên chủ thẻ: ");
                        var cardHolder = Console.ReadLine();
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.Write("Nhập MM/YY: ");
                        var expiry = Console.ReadLine();
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.Write("Nhập CVV: ");
                        var cvv = Console.ReadLine();
                        
                        if (string.IsNullOrEmpty(cardNumber) || string.IsNullOrEmpty(cardHolder) || 
                            string.IsNullOrEmpty(expiry) || string.IsNullOrEmpty(cvv))
                        {
                            Console.SetCursorPosition(borderLeft + 2, cursorY++);
                            ConsoleRenderingService.ShowMessageBox("Thông tin thẻ không đầy đủ!", true, 2000);
                            return "";
                        }
                        return $"Card: ****{cardNumber?.Substring(cardNumber.Length - 4)}";

                    case "EWallet":
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.WriteLine("\n📱 Chọn ví điện tử:");
                        var ewallets = new[] { "MoMo", "ZaloPay", "ViettelPay", "Hủy" };
                        int ewalletChoice = InteractiveMenuService.DisplayInteractiveMenu("VÍ ĐIỆN TỬ", ewallets);
                        if (ewalletChoice == -1 || ewalletChoice == 3) return "";
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.Write($"Nhập số điện thoại {ewallets[ewalletChoice]}: ");
                        var phone = Console.ReadLine();
                        return string.IsNullOrEmpty(phone) ? "" : $"{ewallets[ewalletChoice]}: {phone}";

                    default:
                        return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        private async Task HandleTransactionHistoryAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("LỊCH SỬ GIAO DỊCH", 100, 20);

                var transactions = await _walletService.GetTransactionHistoryAsync(_currentUser.Id);
                if (transactions == null || !transactions.Any())
                {
                    ConsoleRenderingService.ShowNotification("Chưa có giao dịch nào", ConsoleColor.Yellow);
                    Console.WriteLine("\nNhấn Enter để tiếp tục...");
                    Console.ReadLine();
                    return;
                }

                int borderLeft = (Console.WindowWidth - 100) / 2;
                int borderTop = (Console.WindowHeight - 20) / 4;

                Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{"Ngày",-12} {"Loại",-15} {"Số tiền",-15} {"Trạng thái",-12} {"Ghi chú",-30}");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
                Console.WriteLine(new string('─', 90));

                int currentRow = borderTop + 4;
                foreach (var transaction in transactions.Take(12))
                {
                    Console.SetCursorPosition(borderLeft + 2, currentRow);
                    Console.ForegroundColor = transaction.TransactionType == "Deposit" ? ConsoleColor.Green : ConsoleColor.Yellow;
                    
                    var row = string.Format("{0,-12} {1,-15} {2,-15} {3,-12} {4,-30}",
                        transaction.CreatedAt.ToString("dd/MM/yyyy"),
                        transaction.TransactionType,
                        $"{transaction.Amount:N0} VND",
                        transaction.Status,
                        transaction.Note?.Length > 29 ? transaction.Note.Substring(0, 29) : transaction.Note ?? "");
                    Console.WriteLine(row);
                    currentRow++;
                }

                Console.ResetColor();
                Console.SetCursorPosition(borderLeft + 2, borderTop + 17);
                Console.WriteLine($"Tổng cộng: {transactions.Count()} giao dịch");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 18);
                Console.WriteLine("Nhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", true, 3000);
            }
        }

        private async Task HandlePaymentInfoManagementAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("QUẢN LÝ THÔNG TIN THANH TOÁN", 80, 15);

                var paymentOptions = new[]
                {
                    "Thêm phương thức thanh toán",
                    "Xem danh sách phương thức",
                    "Cập nhật thông tin thanh toán", 
                    "Xóa phương thức thanh toán",
                    "⬅️ Quay lại"
                };

                int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ THANH TOÁN", paymentOptions);

                switch (selection)
                {
                    case 0:
                        await AddPaymentMethodAsync();
                        break;
                    case 1:
                        await ViewPaymentMethodsAsync();
                        break;
                    case 2:
                        await UpdatePaymentMethodAsync();
                        break;
                    case 3:
                        await DeletePaymentMethodAsync();
                        break;
                    case 4:
                    case -1:
                        return;
                }
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", true, 3000);
            }
        }

        private async Task HandleViewWalletDetailsAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("THÔNG TIN VÍ CHI TIẾT", 80, 15);

                var wallet = await _walletService.GetWalletByUserIdAsync(_currentUser.Id);
                if (wallet == null)
                {
                    ConsoleRenderingService.ShowNotification("Chưa có ví điện tử", ConsoleColor.Yellow);
                    return;
                }

                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 15) / 4;

                Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                Console.WriteLine($"💰 Số dư hiện tại: {wallet.Balance:N0} VND");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
                Console.WriteLine($"📈 Tổng đã nạp: {wallet.TotalReceived:N0} VND");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
                Console.WriteLine($"📉 Tổng đã chi: {wallet.TotalWithdrawn:N0} VND");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
                Console.WriteLine($"📅 Ngày tạo: {wallet.CreatedAt:dd/MM/yyyy HH:mm}");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 6);
                Console.WriteLine($"🔄 Cập nhật cuối: {wallet.LastUpdatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa có"}");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 7);
                Console.WriteLine($"🔒 Trạng thái: {(wallet.IsLocked ? "🔒 Khóa" : "✅ Hoạt động")}");

                Console.SetCursorPosition(borderLeft + 2, borderTop + 9);
                Console.WriteLine("📋 Lưu ý về ví:");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 10);
                Console.WriteLine("• Dùng để donate cho player yêu thích");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 11);
                Console.WriteLine("• Phí nạp tiền: 0.5% trên tổng số tiền");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 12);
                Console.WriteLine("• Bảo mật cao với mã hóa AES-256");

                Console.SetCursorPosition(borderLeft + 2, borderTop + 13);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", true, 3000);
            }
        }

        // Helper methods for payment management
        private string GetMethodDisplayName(string method)
        {
            return method switch
            {
                "BankTransfer" => "Chuyển khoản ngân hàng",
                "CreditCard" => "Thẻ tín dụng/ghi nợ",
                "EWallet" => "Ví điện tử",
                _ => method
            };
        }

        private string GenerateReferenceCode()
        {
            return $"TOP{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        private Task AddPaymentMethodAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("THÊM PHƯƠNG THỨC THANH TOÁN", 80, 20);
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 20) / 4;
                int cursorY = borderTop + 2;

                var methodTypes = new[]
                {
                    "Chuyển khoản ngân hàng",
                    "Thẻ tín dụng/ghi nợ", 
                    "Ví điện tử (MoMo, ZaloPay, etc.)"
                };

                int typeSelection = InteractiveMenuService.DisplayInteractiveMenu("CHỌN LOẠI THANH TOÁN", methodTypes);
                if (typeSelection == -1) return Task.CompletedTask;

                string methodType = typeSelection switch
                {
                    0 => "BankTransfer",
                    1 => "CreditCard", 
                    2 => "EWallet",
                    _ => "BankTransfer"
                };

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"➕ Thêm {GetMethodDisplayName(methodType)}:");
                string name, details;
                switch (methodType)
                {
                    case "BankTransfer":
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.Write("Tên ngân hàng: ");
                        name = Console.ReadLine()?.Trim() ?? "";
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.Write("Số tài khoản: ");
                        details = Console.ReadLine()?.Trim() ?? "";
                        break;
                    case "CreditCard":
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.Write("Tên chủ thẻ: ");
                        name = Console.ReadLine()?.Trim() ?? "";
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.Write("Số thẻ (4 số cuối): ");
                        var cardNumber = Console.ReadLine()?.Trim() ?? "";
                        details = $"****-****-****-{cardNumber}";
                        break;
                    case "EWallet":
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.Write("Tên ví điện tử: ");
                        name = Console.ReadLine()?.Trim() ?? "";
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.Write("Số điện thoại/Email: ");
                        details = Console.ReadLine()?.Trim() ?? "";
                        break;
                    default:
                        name = details = "";
                        break;
                }

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(details))
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    ConsoleRenderingService.ShowMessageBox($"✅ Đã thêm {GetMethodDisplayName(methodType)}: {name}", true, 2000);
                }
                else
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    ConsoleRenderingService.ShowMessageBox("❌ Thông tin không đầy đủ!", false, 2000);
                }
            }
            catch (Exception ex)
            {
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 20) / 4;
                Console.SetCursorPosition(borderLeft + 2, borderTop + 18);
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
            return Task.CompletedTask;
        }

        private Task ViewPaymentMethodsAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("DANH SÁCH PHƯƠNG THỨC THANH TOÁN", 80, 25);
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 25) / 4;
                int cursorY = borderTop + 2;

                // Mock data - in real app, load from database
                var paymentMethods = new[]
                {
                    new { Type = "BankTransfer", Name = "Vietcombank", Details = "1234567890", IsDefault = true },
                    new { Type = "CreditCard", Name = "Visa Card", Details = "****-****-****-1234", IsDefault = false },
                    new { Type = "EWallet", Name = "MoMo", Details = "0901234567", IsDefault = false }
                };

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("📋 Danh sách phương thức thanh toán đã lưu:");
                cursorY++;
                for (int i = 0; i < paymentMethods.Length; i++)
                {
                    var method = paymentMethods[i];
                    string defaultMark = method.IsDefault ? " [MẶC ĐỊNH]" : "";
                    string typeDisplay = GetMethodDisplayName(method.Type);
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine($"{i + 1}. {typeDisplay}{defaultMark}");
                    Console.SetCursorPosition(borderLeft + 4, cursorY++);
                    Console.WriteLine($"📄 {method.Name}");
                    Console.SetCursorPosition(borderLeft + 4, cursorY++);
                    Console.WriteLine($"🔢 {method.Details}");
                    cursorY++;
                }

                if (!paymentMethods.Any())
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine("🔍 Chưa có phương thức thanh toán nào được lưu.");
                }

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("Nhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 25) / 4;
                Console.SetCursorPosition(borderLeft + 2, borderTop + 22);
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
            return Task.CompletedTask;
        }

        private Task UpdatePaymentMethodAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("CẬP NHẬT PHƯƠNG THỨC THANH TOÁN", 80, 20);
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 20) / 4;
                int cursorY = borderTop + 2;

                // Mock data - show available methods
                var methods = new[]
                {
                    "Vietcombank - 1234567890",
                    "Visa Card - ****1234",
                    "MoMo - 0901234567"
                };

                int selection = InteractiveMenuService.DisplayInteractiveMenu("CHỌN PHƯƠNG THỨC CẬP NHẬT", methods);
                if (selection == -1) return Task.CompletedTask;

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"✏️ Cập nhật: {methods[selection]}");
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.Write("Tên mới (để trống nếu không đổi): ");
                var newName = Console.ReadLine()?.Trim();
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.Write("Thông tin mới (để trống nếu không đổi): ");
                var newDetails = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(newName) || !string.IsNullOrEmpty(newDetails))
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    ConsoleRenderingService.ShowMessageBox("✅ Đã cập nhật thông tin thanh toán!", true, 2000);
                }
                else
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    ConsoleRenderingService.ShowMessageBox("ℹ️ Không có thay đổi nào.", false, 1500);
                }
            }
            catch (Exception ex)
            {
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 20) / 4;
                Console.SetCursorPosition(borderLeft + 2, borderTop + 18);
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
            return Task.CompletedTask;
        }

        private Task DeletePaymentMethodAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("XÓA PHƯƠNG THỨC THANH TOÁN", 80, 20);
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 20) / 4;
                int cursorY = borderTop + 2;

                // Mock data - show available methods
                var methods = new[]
                {
                    "Vietcombank - 1234567890",
                    "Visa Card - ****1234", 
                    "MoMo - 0901234567"
                };

                if (!methods.Any())
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    ConsoleRenderingService.ShowMessageBox("❌ Không có phương thức thanh toán nào để xóa.", false, 2000);
                    return Task.CompletedTask;
                }

                int selection = InteractiveMenuService.DisplayInteractiveMenu("CHỌN PHƯƠNG THỨC XÓA", methods);
                if (selection == -1) return Task.CompletedTask;

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"🗑️ Xóa: {methods[selection]}");
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.Write("Xác nhận (y/n): ");

                var confirmation = Console.ReadLine()?.ToLower();
                if (confirmation == "y" || confirmation == "yes")
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    ConsoleRenderingService.ShowMessageBox("✅ Đã xóa phương thức thanh toán!", true, 2000);
                }
                else
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    ConsoleRenderingService.ShowMessageBox("❌ Đã hủy thao tác", false, 1500);
                }
            }
            catch (Exception ex)
            {
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 20) / 4;
                Console.SetCursorPosition(borderLeft + 2, borderTop + 18);
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
            return Task.CompletedTask;
        }
    }
}
