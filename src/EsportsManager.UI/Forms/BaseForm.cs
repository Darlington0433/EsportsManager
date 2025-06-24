// Lớp cơ sở cho các form

using System;

namespace EsportsManager.UI.Forms
{    /// <summary>
     /// Lớp cơ sở cho form
     /// </summary>
     /// 
     /// LIFECYCLE:
     /// 1. Show() - Entry point, calls DrawForm() then ProcessForm()
     /// 2. DrawForm() - Abstract method, renders form UI
     /// 3. ProcessForm() - Abstract method, handles form logic
     /// 4. Return result to caller
     /// 
     /// SUBCLASSES:
     /// - UserAuthenticationForm: Login form
     /// - UserRegistrationForm: Registration form  
     /// - PasswordRecoveryForm: Forgot password form
     /// - AdminForms: Admin-specific forms
     /// - PlayerForms: Player-specific forms
     /// </summary>
    public abstract class BaseForm
    {
        #region Fields - Các trường dữ liệu

        /// <summary>
        /// Tiêu đề của form
        /// </summary>
        protected readonly string FormTitle;

        /// <summary>
        /// Chiều rộng của form
        /// </summary>
        protected readonly int Width;

        /// <summary>
        /// Chiều cao của form
        /// </summary>
        protected readonly int Height;

        #endregion

        #region Constructor - Hàm khởi tạo

        /// <summary>
        /// Khởi tạo BaseForm với các tham số cần thiết
        /// </summary>
        /// <param name="formTitle">Tiêu đề form</param>
        /// <param name="width">Chiều rộng form (mặc định 60)</param>
        /// <param name="height">Chiều cao form (mặc định 20)</param>
        protected BaseForm(string formTitle, int width = 60, int height = 20)
        {
            // Gán giá trị cho các trường
            FormTitle = formTitle;
            Width = width;
            Height = height;
        }

        #endregion

        #region Public Methods - Các phương thức công khai

        /// <summary>
        /// Hiển thị form và xử lý logic
        /// Phương thức chính để chạy form
        /// </summary>
        /// <returns>Kết quả từ form hoặc null nếu có lỗi</returns>
        public virtual object? Show()
        {
            try
            {
                // Vẽ form trên console
                DrawForm();
                // Xử lý logic của form
                return ProcessForm();
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi nếu có exception
                ShowMessage($"Error: {ex.Message}", true);
                return null;
            }
        }

        #endregion

        #region Protected Abstract Methods - Các phương thức trừu tượng được bảo vệ

        /// <summary>
        /// Vẽ giao diện form - phải được implement bởi class con
        /// </summary>
        protected abstract void DrawForm();

        /// <summary>
        /// Xử lý logic của form - phải được implement bởi class con
        /// </summary>
        /// <returns>Kết quả xử lý</returns>
        protected abstract object? ProcessForm();

        #endregion

        #region Protected Virtual Methods - Các phương thức ảo được bảo vệ

        /// <summary>
        /// Hiển thị thông báo cho người dùng
        /// </summary>
        /// <param name="message">Nội dung thông báo</param>
        /// <param name="isError">True nếu là thông báo lỗi (màu đỏ), False nếu là thông báo thành công (màu xanh)</param>
        protected virtual void ShowMessage(string message, bool isError = false)
        {
            // Tính toán vị trí hiển thị thông báo (dưới form)
            int messageTop = Height + 1;

            // Đặt cursor về vị trí đầu dòng thông báo
            Console.SetCursorPosition(0, messageTop);

            // Xóa dòng cũ bằng cách ghi đè bằng khoảng trắng
            Console.Write(new string(' ', Console.WindowWidth));

            // Tính toán vị trí để căn giữa thông báo
            Console.SetCursorPosition((Console.WindowWidth - message.Length) / 2, messageTop);

            // Đặt màu cho thông báo (đỏ cho lỗi, xanh cho thành công)
            Console.ForegroundColor = isError ? ConsoleColor.Red : ConsoleColor.Green;

            // Hiển thị thông báo            Console.Write(message);

            // Reset màu về mặc định
            Console.ResetColor();
        }

        #endregion
    }
}
