// Form đăng ký

using System;
using EsportsManager.UI.Utilities;
using EsportsManager.UI.ConsoleUI.Utilities;

namespace EsportsManager.UI.Forms
{    /// <summary>
    /// Form đăng ký
    /// </summary>
    public class UserRegistrationForm
    {
        #region Private Fields
        
        // Các field cần nhập trong form đăng ký
        private readonly string[] _fieldLabels = {
            "Tên đăng nhập",
            "Email", 
            "Mật khẩu",
            "Xác nhận mật khẩu",
            "Câu trả lời bảo mật"
        };
        
        // Giá trị đã nhập cho mỗi field
        private readonly string[] _fieldValues = new string[5];
        
        // Index của field đang được chọn
        private int _selectedFieldIndex = 0;
        
        // Câu hỏi bảo mật cố định
        private readonly string _securityQuestion = "Tên trường tiểu học đầu tiên của bạn là gì?";
        
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
            int formHeight = Math.Min(18, windowHeight - 4); // Tối đa 18, margin 4
            
            // Căn giữa màn hình với vị trí an toàn
            int left = Math.Max(1, (windowWidth - formWidth) / 2);
            int top = Math.Max(1, (windowHeight - formHeight) / 2);
            
            // Vẽ border xanh lá
            ConsoleRenderingService.DrawBorder(left, top, formWidth, formHeight, "[ĐĂNG KÝ]", true);
            
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
                
                // Hiển thị value (password thì hiển thị *)
                string displayValue = "";
                if (!string.IsNullOrEmpty(_fieldValues[i]))
                {
                    if (_fieldLabels[i].Contains("Mật khẩu"))
                    {
                        displayValue = new string('*', _fieldValues[i].Length);
                    }
                    else                    {
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
            int formHeight = 18;
            int left = Math.Max(0, (windowWidth - formWidth) / 2);
            int top = Math.Max(0, (windowHeight - formHeight) / 2);
            int fieldY = top + 4 + (_selectedFieldIndex * 2);
            
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
        }          /// <summary>
        /// Validate form trước khi submit
        /// </summary>
        private bool ValidateForm()
        {
            // Validate các field bắt buộc
            if (!ValidationService.ValidateRequiredFields(_fieldValues, _fieldLabels, ShowMessage))
            {
                return false;
            }
            
            // Validate password match
            if (!ValidationService.ValidatePasswordMatch(_fieldValues[2], _fieldValues[3], ShowMessage))
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>        /// Xử lý submit form đăng ký
        /// </summary>
        private void HandleSubmit()
        {
            ShowMessage("Đăng ký thành công!", false);
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
