using System;

namespace EsportsManager.UI.Legacy.Menus
{
    /// <summary>
    /// Lớp quản lý menu dành cho người xem (Viewer)
    /// </summary>
    public class ViewerMenu : BaseMenu
    {
        private static readonly string[] ViewerOptions = {
            "1. Xem giải đấu",
            "2. Donate cho đội/giải đấu",
            "3. Gửi phản hồi",
            "0. Đăng xuất"
        };

        /// <summary>
        /// Khởi tạo một menu Viewer mới
        /// </summary>
        public ViewerMenu() : base("[MENU VIEWER]", ViewerOptions) { }

        /// <summary>
        /// Xử lý lựa chọn menu từ người dùng
        /// </summary>
        /// <param name="selected">Chỉ số mục được chọn</param>
        protected override void HandleMenuSelection(int selected)
        {
            try
            {
                switch (selected)
                {
                    case 0:
                        ViewTournaments();
                        break;
                    case 1:
                        MakeDonation();
                        break;
                    case 2:
                        SendFeedback();
                        break;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ!");
                        Console.ReadKey(true);
                        break;
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        /// <summary>
        /// Hiển thị thông tin về các giải đấu
        /// </summary>
        private void ViewTournaments()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== DANH SÁCH GIẢI ĐẤU ===");
                Console.WriteLine("Chức năng đang được phát triển...");
                Console.WriteLine("\nNhấn phím bất kỳ để quay lại");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        /// <summary>
        /// Xử lý chức năng donate cho đội/giải đấu
        /// </summary>
        private void MakeDonation()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== DONATE CHO ĐỘI/GIẢI ĐẤU ===");
                Console.WriteLine("Chức năng đang được phát triển...");
                Console.WriteLine("\nNhấn phím bất kỳ để quay lại");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        /// <summary>
        /// Xử lý chức năng gửi phản hồi
        /// </summary>
        private void SendFeedback()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== GỬI PHẢN HỒI ===");
                Console.WriteLine("Chức năng đang được phát triển...");
                Console.WriteLine("\nNhấn phím bất kỳ để quay lại");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        /// <summary>
        /// Xử lý lỗi trong menu
        /// </summary>
        /// <param name="ex">Exception cần xử lý</param>
        private void HandleError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Đã xảy ra lỗi: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
    }
}
