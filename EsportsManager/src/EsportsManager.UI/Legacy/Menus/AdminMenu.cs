using System;

namespace EsportsManager.UI.Legacy.Menus
{
    public class AdminMenu : BaseMenu
    {
        private static readonly string[] AdminOptions = {
            "1. Quản lý người dùng",
            "2. Quản lý giải đấu",
            "3. Quản lý đội",
            "4. Thống kê",
            "0. Đăng xuất"
        };

        public AdminMenu() : base("[MENU ADMIN]", AdminOptions) { }

        protected override void HandleMenuSelection(int selected)
        {
            switch (selected)
            {
                case 0:
                    HandleUserManagement();
                    break;
                case 1:
                    HandleTournamentManagement();
                    break;
                case 2:
                    HandleTeamManagement();
                    break;
                case 3:
                    HandleStatistics();
                    break;
            }
        }

        private void HandleUserManagement()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== QUẢN LÝ NGƯỜI DÙNG ===");
                Console.WriteLine("1. Xem danh sách người dùng");
                Console.WriteLine("2. Thêm người dùng mới");
                Console.WriteLine("3. Sửa thông tin người dùng");
                Console.WriteLine("4. Xóa người dùng");
                Console.WriteLine("0. Quay lại menu Admin");
                Console.Write("Chọn chức năng: ");
                var key = Console.ReadKey(true);
                switch (key.KeyChar)
                {
                    case '1':
                        ShowUserList();
                        break;
                    case '2':
                        AddUser();
                        break;
                    case '3':
                        EditUser();
                        break;
                    case '4':
                        DeleteUser();
                        break;
                    case '0':
                        return;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowUserList()
        {
            Console.Clear();
            Console.WriteLine("=== DANH SÁCH NGƯỜI DÙNG ===");
            Console.WriteLine("Chức năng đang được phát triển...");
            Console.WriteLine("\nNhấn phím bất kỳ để quay lại");
            Console.ReadKey();
        }

        private void AddUser()
        {
            Console.Clear();
            Console.WriteLine("=== THÊM NGƯỜI DÙNG MỚI ===");
            Console.WriteLine("Chức năng đang được phát triển...");
            Console.WriteLine("\nNhấn phím bất kỳ để quay lại");
            Console.ReadKey();
        }

        private void EditUser()
        {
            Console.Clear();
            Console.WriteLine("=== SỬA THÔNG TIN NGƯỜI DÙNG ===");
            Console.WriteLine("Chức năng đang được phát triển...");
            Console.WriteLine("\nNhấn phím bất kỳ để quay lại");
            Console.ReadKey();
        }

        private void DeleteUser()
        {
            Console.Clear();
            Console.WriteLine("=== XÓA NGƯỜI DÙNG ===");
            Console.WriteLine("Chức năng đang được phát triển...");
            Console.WriteLine("\nNhấn phím bất kỳ để quay lại");
            Console.ReadKey();
        }

        private void HandleTournamentManagement()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== QUẢN LÝ GIẢI ĐẤU ===");
                Console.WriteLine("1. Xem danh sách giải đấu");
                Console.WriteLine("2. Thêm giải đấu mới");
                Console.WriteLine("3. Sửa thông tin giải đấu");
                Console.WriteLine("4. Xóa giải đấu");
                Console.WriteLine("0. Quay lại menu Admin");
                Console.Write("Chọn chức năng: ");
                var key = Console.ReadKey(true);
                switch (key.KeyChar)
                {
                    case '1':
                        ShowTournamentList();
                        break;
                    case '2':
                        AddTournament();
                        break;
                    case '3':
                        EditTournament();
                        break;
                    case '4':
                        DeleteTournament();
                        break;
                    case '0':
                        return;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowTournamentList()
        {
            Console.Clear();
            Console.WriteLine("=== DANH SÁCH GIẢI ĐẤU ===");
            Console.WriteLine("Chức năng đang được phát triển...");
            Console.WriteLine("\nNhấn phím bất kỳ để quay lại");
            Console.ReadKey();
        }

        private void AddTournament()
        {
            Console.Clear();
            Console.WriteLine("=== THÊM GIẢI ĐẤU MỚI ===");
            Console.WriteLine("Chức năng đang được phát triển...");
            Console.WriteLine("\nNhấn phím bất kỳ để quay lại");
            Console.ReadKey();
        }

        private void EditTournament()
        {
            Console.Clear();
            Console.WriteLine("=== SỬA THÔNG TIN GIẢI ĐẤU ===");
            Console.WriteLine("Chức năng đang được phát triển...");
            Console.WriteLine("\nNhấn phím bất kỳ để quay lại");
            Console.ReadKey();
        }

        private void DeleteTournament()
        {
            Console.Clear();
            Console.WriteLine("=== XÓA GIẢI ĐẤU ===");
            Console.WriteLine("Chức năng đang được phát triển...");
            Console.WriteLine("\nNhấn phím bất kỳ để quay lại");
            Console.ReadKey();
        }

        private void HandleTeamManagement()
        {
            // Quản lý đội logic
            Console.WriteLine("Chức năng quản lý đội đang được phát triển...");
            Console.ReadKey();
        }

        private void HandleStatistics()
        {
            // Thống kê logic
            Console.WriteLine("Chức năng thống kê đang được phát triển...");
            Console.ReadKey();
        }

        private void HandleUserDeletion()
        {
            // Xóa user logic
            Console.WriteLine("Chức năng xóa user đang được phát triển...");
            Console.ReadKey();
        }

        private void HandleAchievementGrant()
        {
            // Trao thành tích logic
            Console.WriteLine("Chức năng trao thành tích đang được phát triển...");
            Console.ReadKey();
        }

        private void HandleFeedbackView()
        {
            // Xem feedback logic
            Console.WriteLine("Chức năng xem phản hồi đang được phát triển...");
            Console.ReadKey();
        }
    }
}
