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
            // Quản lý người dùng logic
            Console.WriteLine("Chức năng quản lý người dùng đang được phát triển...");
            Console.ReadKey();
        }

        private void HandleTournamentManagement()
        {
            // Quản lý giải đấu logic
            Console.WriteLine("Chức năng quản lý giải đấu đang được phát triển...");
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
    }
}
