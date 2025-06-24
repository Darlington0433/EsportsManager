// Form quên mật khẩu

using System;
using EsportsManager.UI.Utilities;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.DTOs;

namespace EsportsManager.UI.Forms
{    /// <summary>
     /// Form quên mật khẩu
     /// </summary>
    public class PasswordRecoveryForm
    {
        #region Private Fields

        private readonly IUserService _userService;

        // Các field cần nhập trong form quên mật khẩu
        private readonly string[] _fieldLabels = {
            "Tên đăng nhập",
            "Câu trả lời bảo mật"
        };

        // Giá trị đã nhập cho mỗi field
        private readonly string[] _fieldValues = new string[2];

        // Index của field đang được chọn
        private int _selectedFieldIndex = 0;

        // Câu hỏi bảo mật cố định
        private readonly string _securityQuestion = "Tên trường tiểu học đầu tiên của bạn là gì?";

        #endregion

        #region Constructor

        public PasswordRecoveryForm(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Hiển thị form quên mật khẩu với giao diện giống hệt ảnh mẫu
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
            int formWidth = Math.Min(65, windowWidth - 6);  // Tối đa 65, margin 6
            int formHeight = Math.Min(15, windowHeight - 4); // Tối đa 15, margin 4

            // Căn giữa màn hình với vị trí an toàn
            int left = Math.Max(1, (windowWidth - formWidth) / 2);
            int top = Math.Max(1, (windowHeight - formHeight) / 2);

            // Vẽ border xanh lá
            ConsoleRenderingService.DrawBorder(left, top, formWidth, formHeight, "[QUÊN MẬT KHẨU]", true);

            // Vẽ câu hỏi bảo mật
            Console.SetCursorPosition(left + 2, top + 2);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Câu hỏi bảo mật: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(_securityQuestion);
            Console.ResetColor();

            // Vẽ các input fields
            for (int i = 0; i < _fieldLabels.Length; i++)
            {
                int fieldY = top + 4 + (i * 3);

                // Vẽ label field
                Console.SetCursorPosition(left + 2, fieldY);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{_fieldLabels[i]}:");

                // Vẽ input field với highlight nếu được chọn
                Console.SetCursorPosition(left + 18, fieldY);

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
                // Hiển thị value
                string displayValue = _fieldValues[i] ?? "";

                // Tính chiều rộng field thích ứng với kích thước form
                int fieldWidth = Math.Min(30, formWidth - 25); // Tối đa 30, nhưng không vượt khung

                // Pad để đủ chiều rộng field
                displayValue = displayValue.PadRight(fieldWidth);
                Console.Write(displayValue);
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.Black;
            }
            // Vẽ hướng dẫn phím ở cuối form - căn giữa và cắt nếu quá dài
            string helpText = "↑↓/Tab: Chọn   Enter: Nhập   F1: Khôi phục   Esc: Thoát";
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
            int formWidth = Math.Min(65, windowWidth - 6);
            int formHeight = Math.Min(15, windowHeight - 4);
            int left = Math.Max(1, (windowWidth - formWidth) / 2);
            int top = Math.Max(1, (windowHeight - formHeight) / 2);
            int fieldY = top + 4 + (_selectedFieldIndex * 3);

            // Đặt cursor vào vị trí input
            Console.SetCursorPosition(left + 18, fieldY);
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;

            // Clear field hiện tại
            Console.Write(new string(' ', 30));
            Console.SetCursorPosition(left + 18, fieldY);

            // Đọc input sử dụng UnifiedInputService
            string? input = null;
            if (_selectedFieldIndex == 0) // Username field
            {
                input = UnifiedInputService.ReadUsername(30);
            }
            else // Security answer field
            {
                input = UnifiedInputService.ReadText(30);
            }

            // Xử lý kết quả input
            if (input != null)
            {
                _fieldValues[_selectedFieldIndex] = input;
            }

            Console.ResetColor(); Console.BackgroundColor = ConsoleColor.Black;
        }

        /// <summary>
        /// Validate form trước khi submit
        /// </summary>
        private bool ValidateForm()
        {
            return ValidationService.ValidateRequiredFields(_fieldValues, _fieldLabels, ShowMessage);
        }

        /// <summary>
        /// Xử lý submit form khôi phục mật khẩu
        /// </summary>
        private void HandleSubmit()
        {
            ShowMessage("Mật khẩu mới đã được gửi về email!", false);
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
