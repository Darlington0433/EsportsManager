using System;
using EsportsManager.UI.Legacy.Utilities;

namespace EsportsManager.UI.Legacy.Forms
{
    /// <summary>
    /// Lớp cơ sở cho tất cả các form trong ứng dụng
    /// </summary>
    public abstract class BaseForm
    {
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
        
        /// <summary>
        /// Khởi tạo một form mới
        /// </summary>
        /// <param name="formTitle">Tiêu đề của form</param>
        /// <param name="width">Chiều rộng của form</param>
        /// <param name="height">Chiều cao của form</param>
        protected BaseForm(string formTitle, int width = 60, int height = 20)
        {
            FormTitle = formTitle;
            Width = width;
            Height = height;
        }
        
        /// <summary>
        /// Hiển thị form
        /// </summary>
        /// <returns>Kết quả từ form (có thể là đối tượng bất kỳ tùy thuộc vào loại form)</returns>
        public virtual object? Show()
        {
            try
            {
                // Vẽ khung form
                DrawForm();
                
                // Xử lý logic riêng của từng form
                return ProcessForm();
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return null;
            }
        }
        
        /// <summary>
        /// Vẽ khung form
        /// </summary>
        protected virtual void DrawForm()
        {
            Console.Clear();
            
            // Tính toán vị trí để căn giữa form
            int left = (Console.WindowWidth - Width) / 2;
            int top = (Console.WindowHeight - Height) / 2;
            
            // Vẽ khung với tiêu đề
            ConsoleDrawing.DrawBoxWithTitle(Width, Height, left, top, FormTitle);
        }
        
        /// <summary>
        /// Xử lý logic riêng của từng form
        /// </summary>
        /// <returns>Kết quả từ form</returns>
        protected abstract object? ProcessForm();
        
        /// <summary>
        /// Xử lý lỗi
        /// </summary>
        /// <param name="ex">Exception cần xử lý</param>
        protected virtual void HandleError(Exception ex)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Lỗi: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        
        /// <summary>
        /// Hiển thị thông báo trong form
        /// </summary>
        /// <param name="message">Thông báo cần hiển thị</param>
        /// <param name="isError">Có phải là thông báo lỗi không</param>
        protected void ShowMessage(string message, bool isError = false)
        {
            Console.ForegroundColor = isError ? ConsoleColor.Red : ConsoleColor.Green;
            int left = (Console.WindowWidth - message.Length) / 2;
            int messageTop = (Console.WindowHeight + Height) / 2 - 2;
            Console.SetCursorPosition(left, messageTop);
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
