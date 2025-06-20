using System;

namespace EsportsManager.UI.Legacy.Menus
{
    public class PlayerMenu : BaseMenu
    {
        private static readonly string[] PlayerOptions = {
            "1. Xem giải đấu",
            "2. Đăng ký tham gia",
            "3. Quản lý đội của tôi",
            "4. Xem thành tích",
            "5. Quản lý ví",
            "6. Gửi phản hồi",
            "0. Đăng xuất"
        };

        public PlayerMenu() : base("[MENU PLAYER]", PlayerOptions) { }

        protected override void HandleMenuSelection(int selected)
        {
            switch (selected)
            {
                case 0:
                    ViewTournaments();
                    break;
                case 1:
                    RegisterForTournament();
                    break;
                case 2:
                    ManageTeam();
                    break;
                case 3:
                    ViewAchievements();
                    break;
                case 4:
                    ManageWallet();
                    break;
                case 5:
                    SubmitFeedback();
                    break;
            }
        }

        private void ViewTournaments()
        {
            // Xem giải đấu logic
            Console.WriteLine("Chức năng xem giải đấu đang được phát triển...");
            Console.ReadKey();
        }

        private void RegisterForTournament()
        {
            // Đăng ký tham gia logic
            Console.WriteLine("Chức năng đăng ký tham gia đang được phát triển...");
            Console.ReadKey();
        }

        private void ManageTeam()
        {
            // Quản lý đội của tôi logic
            Console.WriteLine("Chức năng quản lý đội đang được phát triển...");
            Console.ReadKey();
        }

        private void ViewAchievements()
        {
            // Xem thành tích logic
            Console.WriteLine("Chức năng xem thành tích đang được phát triển...");
            Console.ReadKey();
        }

        private void ManageWallet()
        {
            Console.WriteLine("Chức năng quản lý ví (nạp/rút) đang được phát triển...");
            Console.ReadKey();
        }

        private void SubmitFeedback()
        {
            Console.WriteLine("Chức năng gửi phản hồi đang được phát triển...");
            Console.ReadKey();
        }
    }
}
