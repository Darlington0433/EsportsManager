// Form đăng nhập

using System;
using EsportsManager.UI.Utilities;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Services;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.DTOs;
using Models = EsportsManager.BL.Models;

namespace EsportsManager.UI.Forms
{
    /// <summary>
    /// UserAuthenticationForm - Form đăng nhập với giao diện giống hệt ảnh mẫu
    /// Bao gồm: border xanh lá, label căn trái, input field highlight, hướng dẫn phím
    /// </summary>
    public class UserAuthenticationForm
    {
        #region Private Fields

        private readonly IUserService _userService;

        // Các field cần nhập trong form đăng nhập
        private readonly string[] _fieldLabels = {
            "Tên đăng nhập",
            "Mật khẩu"
        };

        // Giá trị đã nhập cho mỗi field
        private readonly string[] _fieldValues = new string[2];

        // Index của field đang được chọn
        private int _selectedFieldIndex = 0;

        // Lưu trữ kết quả đăng nhập
        private Models.AuthenticationResult? _authResult;

        #endregion

        #region Constructor

        public UserAuthenticationForm(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Hiển thị form đăng nhập với giao diện giống hệt ảnh mẫu
        /// </summary>
        /// <returns>true nếu user hoàn thành form, false nếu hủy bỏ (ESC)</returns>
        public bool Show()
        {
            // Khởi tạo giá trị rỗng cho tất cả fields
            for (int i = 0; i < _fieldValues.Length; i++)
            {
                _fieldValues[i] = "";
            }

            // Vòng lặp chính xử lý form
            while (true)
            {
                DrawForm();

                // Xử lý input từ user
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        _selectedFieldIndex = (_selectedFieldIndex > 0) ? _selectedFieldIndex - 1 : _fieldLabels.Length - 1;
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.Tab:
                        _selectedFieldIndex = (_selectedFieldIndex < _fieldLabels.Length - 1) ? _selectedFieldIndex + 1 : 0;
                        break;

                    case ConsoleKey.Enter:
                        HandleFieldInput();
                        if (_selectedFieldIndex == _fieldLabels.Length - 1 && ValidateForm())
                        {
                            HandleSubmit();
                            if (_authResult != null && _authResult.IsSuccess)
                                return true;
                        }
                        else
                        {
                            if (_selectedFieldIndex < _fieldLabels.Length - 1)
                                _selectedFieldIndex++;
                        }
                        break;

                    case ConsoleKey.F1:
                    case ConsoleKey.S:
                        if (ValidateForm())
                        {
                            HandleSubmit();
                            if (_authResult != null && _authResult.IsSuccess)
                                return true;
                        }
                        break;

                    case ConsoleKey.Escape:
                        return false; // User hủy bỏ
                }
            }
        }

        /// <summary>
        /// Lấy kết quả đăng nhập
        /// </summary>
        public Models.AuthenticationResult? GetAuthResult()
        {
            return _authResult;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Vẽ toàn bộ form với layout giống hệt ảnh mẫu
        /// </summary>
        private void DrawForm()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;

            // Kích thước và vị trí an toàn, không bao giờ lòi ra ngoài màn hình
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            int formWidth = Math.Min(60, windowWidth - 6);  // Tối đa 60, margin 6
            int formHeight = Math.Min(12, windowHeight - 4); // Tối đa 12, margin 4

            // Căn giữa màn hình với vị trí an toàn
            int left = Math.Max(1, (windowWidth - formWidth) / 2);
            int top = Math.Max(1, (windowHeight - formHeight) / 2);

            // Vẽ border xanh lá
            ConsoleRenderingService.DrawBorder(left, top, formWidth, formHeight, "[ĐĂNG NHẬP]", true);

            // Vẽ các input fields
            for (int i = 0; i < _fieldLabels.Length; i++)
            {
                int fieldY = top + 3 + (i * 3);

                // Vẽ label field
                SafeConsole.SetCursorPosition(left + 2, fieldY);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{_fieldLabels[i]}:");

                // Vẽ input field với highlight nếu được chọn
                SafeConsole.SetCursorPosition(left + 18, fieldY);

                if (i == _selectedFieldIndex)
                {
                    // Field được chọn: background xanh
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    // Field không được chọn: background đen
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                // Hiển thị value (password thì hiển thị *)
                string displayValue = "";
                if (!string.IsNullOrEmpty(_fieldValues[i]))
                {
                    if (_fieldLabels[i].Contains("Mật khẩu"))
                    {
                        displayValue = new string('*', _fieldValues[i].Length);
                    }
                    else
                    {
                        displayValue = _fieldValues[i];
                    }
                }

                // Tính chiều rộng field thích ứng với kích thước form
                int fieldWidth = Math.Min(25, formWidth - 25); // Tối đa 25, nhưng không vượt khung

                // Pad để đủ chiều rộng field
                displayValue = displayValue.PadRight(fieldWidth);
                Console.Write(displayValue);
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.Black;
            }

            // Vẽ hướng dẫn phím ở cuối form - căn giữa và cắt nếu quá dài
            string helpText = "↑↓/Tab: Chọn   Enter: Nhập/Đăng nhập   Esc: Thoát";
            int maxHelpWidth = formWidth - 4; // Để trong khung border
            // Tính vị trí căn giữa
            int helpX = left + ((formWidth - helpText.Length) / 2);
            SafeConsole.SetCursorPosition(helpX, top + formHeight - 2);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(helpText);
            Console.ResetColor();
        }

        /// <summary>
        /// Xử lý nhập dữ liệu cho field đang được chọn
        /// </summary>
        private void HandleFieldInput()
        {
            // Tính vị trí input
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            int formWidth = 60;
            int formHeight = 12;
            int left = Math.Max(0, (windowWidth - formWidth) / 2);
            int top = Math.Max(0, (windowHeight - formHeight) / 2);
            int fieldY = top + 3 + (_selectedFieldIndex * 3);

            // Đặt cursor vào vị trí input
            SafeConsole.SetCursorPosition(left + 18, fieldY);
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;

            // Clear field hiện tại
            int fieldWidth = Math.Min(25, formWidth - 25);
            Console.Write(new string(' ', fieldWidth));
            SafeConsole.SetCursorPosition(left + 18, fieldY);

            // Đọc input dựa trên loại field
            if (_fieldLabels[_selectedFieldIndex].Contains("Mật khẩu"))
            {
                _fieldValues[_selectedFieldIndex] = ReadPasswordInput();
            }
            else
            {
                _fieldValues[_selectedFieldIndex] = ReadTextInput();
            }

            Console.ResetColor();
            Console.BackgroundColor = ConsoleColor.Black;
        }

        /// <summary>
        /// Đọc password input (hiển thị dấu *)
        /// </summary>
        private string ReadPasswordInput()
        {
            return UnifiedInputService.ReadPassword() ?? "";
        }

        /// <summary>
        /// Đọc text input thông thường
        /// </summary>
        private string ReadTextInput()
        {
            return UnifiedInputService.ReadUsername() ?? "";
        }

        /// <summary>
        /// Validate form trước khi submit
        /// </summary>
        private bool ValidateForm()
        {
            return ValidationService.ValidateRequiredFields(_fieldValues, _fieldLabels, ShowMessage);
        }        /// <summary>
                 /// Xử lý submit form đăng nhập với UserService
                 /// </summary>
        private void HandleSubmit()
        {
            try
            {
                // Tạo LoginDto từ input
                var loginDto = new LoginDto
                {
                    Username = _fieldValues[0],
                    Password = _fieldValues[1]
                };                // Gọi UserService để authenticate (sử dụng authentication thực tế) - chuyển thành synchronous
                var result = _userService.LoginAsync(loginDto).GetAwaiter().GetResult(); if (result.IsSuccess)
                {
                    // Đăng nhập thành công, hiển thị thông báo
                    ShowMessage($"Đăng nhập thành công! Chào mừng {result.Username}", false);

                    // Lưu kết quả authentication vào _authResult
                    _authResult = result;

                    // Lưu profile thực tế của người dùng (không còn dùng demo)
                    UserSessionManager.CurrentUser = new UserProfileDto
                    {
                        Id = result.UserId ?? 0,
                        Username = result.Username ?? string.Empty,
                        Role = result.Role ?? "Viewer",
                        Status = "Active",
                        CreatedAt = DateTime.Now
                    };

                    UserSessionManager.IsLoggedIn = true;
                }
                else
                {
                    // Hiển thị thông báo chi tiết dựa trên loại lỗi
                    string errorMessage = result.Message ?? "Tên đăng nhập hoặc mật khẩu không đúng!";

                    if (errorMessage.Contains("Account is not active"))
                    {
                        ShowMessage("❌ Tài khoản của bạn đã bị vô hiệu hóa. Vui lòng liên hệ quản trị viên để được hỗ trợ.", true);
                    }
                    else if (errorMessage.Contains("banned") || errorMessage.Contains("suspended"))
                    {
                        ShowMessage("❌ Tài khoản của bạn đã bị tạm khóa. Vui lòng liên hệ quản trị viên để biết thêm chi tiết.", true);
                    }
                    else
                    {
                        ShowMessage(errorMessage, true);
                    }
                    UserSessionManager.IsLoggedIn = false;
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                // Xử lý lỗi cụ thể cho MySQL
                if (ex.Number == 1042) // Unable to connect to MySQL server
                {
                    ShowMessage("❌ Không thể kết nối đến MySQL Server. Vui lòng kiểm tra cấu hình database và đảm bảo MySQL Server đang chạy.", true);
                }
                else if (ex.Number == 1045) // Access denied (wrong username/password)
                {
                    ShowMessage("❌ Không thể đăng nhập vào MySQL Server. Tên đăng nhập/mật khẩu kết nối database không đúng.", true);
                }
                else if (ex.Number == 1049) // Database does not exist
                {
                    ShowMessage("❌ Database EsportsManager không tồn tại. Vui lòng import database từ file SQL trong thư mục database/.", true);
                }
                else
                {
                    ShowMessage($"❌ Lỗi MySQL: {ex.Message}", true);
                }
                UserSessionManager.IsLoggedIn = false;
            }
            catch (Exception ex)
            {
                ShowMessage($"❌ Lỗi đăng nhập: {ex.Message}", true);
                UserSessionManager.IsLoggedIn = false;
            }
        }

        /// <summary>
        /// Hiển thị thông báo
        /// </summary>
        private void ShowMessage(string message, bool isError)
        {
            ConsoleRenderingService.ShowMessageBox(message, isError, 2000);
        }

        #endregion
    }
}
