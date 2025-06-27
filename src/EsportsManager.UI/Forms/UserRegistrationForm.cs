// Form đăng ký

using System;
using EsportsManager.UI.Utilities;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Models;
using System.Linq;

namespace EsportsManager.UI.Forms
{    /// <summary>
     /// Form đăng ký
     /// </summary>
    public class UserRegistrationForm
    {
        #region Private Fields

        private readonly IUserService _userService;

        // Các field cần nhập trong form đăng ký
        private readonly string[] _fieldLabels = {
            "Tên đăng nhập",
            "Email",
            "Mật khẩu",
            "Xác nhận mật khẩu",
            "Họ tên đầy đủ",
            "Vai trò",
            "Câu hỏi bảo mật",
            "Câu trả lời bảo mật"
        };

        // Giá trị đã nhập cho mỗi field
        private readonly string[] _fieldValues = new string[8];
        // Index của field đang được chọn
        private int _selectedFieldIndex = 0;

        // Danh sách vai trò cho phép đăng ký
        private readonly string[] _availableRoles = { "Player", "Viewer" };

        #endregion

        #region Constructor

        public UserRegistrationForm(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Hiển thị form đăng ký với giao diện giống hệt ảnh mẫu
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
                        _selectedFieldIndex = (_selectedFieldIndex < _fieldLabels.Length - 1) ? _selectedFieldIndex + 1 : 0;
                        break;

                    case ConsoleKey.Tab:
                        _selectedFieldIndex = (_selectedFieldIndex < _fieldLabels.Length - 1) ? _selectedFieldIndex + 1 : 0;
                        break;

                    case ConsoleKey.Enter:
                        HandleFieldInput();
                        break;
                    case ConsoleKey.Escape:
                        return false; // User hủy bỏ

                    case ConsoleKey.F1:
                        // Xử lý submit form (giả lập)
                        if (ValidateForm())
                        {
                            HandleSubmit();
                            return true; // User hoàn thành thành công
                        }
                        break;
                }
            }
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
            int formWidth = Math.Min(70, windowWidth - 6);  // Tối đa 70, margin 6
            int formHeight = Math.Min(22, windowHeight - 4); // Tăng lên 22 để chứa 8 fields

            // Căn giữa màn hình với vị trí an toàn
            int left = Math.Max(1, (windowWidth - formWidth) / 2);
            int top = Math.Max(1, (windowHeight - formHeight) / 2);

            // Vẽ border xanh lá
            ConsoleRenderingService.DrawBorder(left, top, formWidth, formHeight, "[ĐĂNG KÝ]", true);

            // Vẽ hướng dẫn đăng ký
            Console.SetCursorPosition(left + 2, top + 2);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Vui lòng nhập thông tin đăng ký tài khoản:");
            Console.ResetColor();

            // Vẽ các input fields
            for (int i = 0; i < _fieldLabels.Length; i++)
            {
                int fieldY = top + 4 + (i * 2);

                // Vẽ label field
                Console.SetCursorPosition(left + 2, fieldY);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{_fieldLabels[i]}:");

                // Vẽ input field với highlight nếu được chọn
                Console.SetCursorPosition(left + 22, fieldY);

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

                // Hiển thị value (password và confirm password thì hiển thị *)
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
                int fieldWidth = Math.Min(25, formWidth - 28); // Tối đa 25, nhưng không vượt khung

                // Pad để đủ chiều rộng field
                displayValue = displayValue.PadRight(fieldWidth);
                Console.Write(displayValue);
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.Black;
            }
            // Vẽ hướng dẫn phím ở cuối form - căn giữa và cắt nếu quá dài
            string helpText = "↑↓/Tab: Chọn   Enter: Nhập   F1: Đăng ký   Esc: Thoát";
            int maxHelpWidth = formWidth - 4; // Để trong khung border

            // Cắt text nếu quá dài
            if (helpText.Length > maxHelpWidth)
            {
                helpText = "↑↓: Chọn   Enter: Nhập   F1: OK   Esc: Thoát";
            }

            // Tính vị trí căn giữa
            int helpX = left + ((formWidth - helpText.Length) / 2);
            Console.SetCursorPosition(helpX, top + formHeight - 2);
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
            int formWidth = 70;
            int formHeight = 22; // Tăng lên cho 8 fields
            int left = Math.Max(0, (windowWidth - formWidth) / 2);
            int top = Math.Max(0, (windowHeight - formHeight) / 2);
            int fieldY = top + 4 + (_selectedFieldIndex * 2);

            // Xử lý đặc biệt cho field "Vai trò"
            if (_fieldLabels[_selectedFieldIndex] == "Vai trò")
            {
                HandleRoleSelection();
                return;
            }

            // Đặt cursor vào vị trí input
            Console.SetCursorPosition(left + 22, fieldY);
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            // Clear field hiện tại
            int fieldWidth = Math.Min(25, formWidth - 28);
            Console.Write(new string(' ', fieldWidth));
            Console.SetCursorPosition(left + 22, fieldY);
            // Đọc input dựa trên loại field
            if (_fieldLabels[_selectedFieldIndex].Contains("Mật khẩu"))
            {
                _fieldValues[_selectedFieldIndex] = UnifiedInputService.ReadPassword() ?? "";
            }
            else if (_fieldLabels[_selectedFieldIndex].Contains("Email"))
            {
                _fieldValues[_selectedFieldIndex] = UnifiedInputService.ReadEmail() ?? "";
            }
            else
            {
                _fieldValues[_selectedFieldIndex] = UnifiedInputService.ReadText() ?? "";
            }

            Console.ResetColor();
            Console.BackgroundColor = ConsoleColor.Black;
        }

        /// <summary>
        /// Xử lý chọn vai trò (Player/Viewer)
        /// </summary>
        private void HandleRoleSelection()
        {
            int currentSelection = Array.IndexOf(_availableRoles, _fieldValues[_selectedFieldIndex]);
            if (currentSelection == -1) currentSelection = 0;

            int roleSelection = InteractiveMenuService.DisplayInteractiveMenu("CHỌN VAI TRÒ", _availableRoles);

            if (roleSelection >= 0 && roleSelection < _availableRoles.Length)
            {
                _fieldValues[_selectedFieldIndex] = _availableRoles[roleSelection];
            }
        }          /// <summary>
                   /// Validate form trước khi submit
                   /// </summary>
        private bool ValidateForm()
        {
            // Validate các field bắt buộc
            for (int i = 0; i < _fieldValues.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(_fieldValues[i]))
                {
                    ShowMessage($"{_fieldLabels[i]} không được để trống!", true);
                    return false;
                }
            }

            // Validate password match
            if (_fieldValues[2] != _fieldValues[3])
            {
                ShowMessage("Mật khẩu và xác nhận mật khẩu không khớp!", true);
                return false;
            }

            // Validate email format
            if (!_fieldValues[1].Contains("@"))
            {
                ShowMessage("Email không hợp lệ!", true);
                return false;
            }

            // Validate role
            if (!_availableRoles.Contains(_fieldValues[5]))
            {
                ShowMessage("Vai trò không hợp lệ!", true);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Xử lý submit form đăng ký
        /// </summary>
        private async void HandleSubmit()
        {
            try
            {
                var registerDto = new RegisterDto
                {
                    Username = _fieldValues[0],
                    Email = _fieldValues[1],
                    Password = _fieldValues[2],
                    ConfirmPassword = _fieldValues[3],
                    FullName = _fieldValues[4],
                    Role = _fieldValues[5],
                    SecurityQuestion = _fieldValues[6],
                    SecurityAnswer = _fieldValues[7]
                };

                var result = await _userService.RegisterAsync(registerDto);

                if (result.IsSuccess)
                {
                    ShowMessage("Đăng ký thành công! Tài khoản đang chờ Admin phê duyệt.", false);
                }
                else
                {
                    ShowMessage($"Đăng ký thất bại: {result.ErrorMessage}", true);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Lỗi: {ex.Message}", true);
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
