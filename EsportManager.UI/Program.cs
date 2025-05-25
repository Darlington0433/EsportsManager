using System;
using System.Collections.Generic;
using System.Threading;
using EsportManager.UI.Interfaces;
using EsportManager.UI.Screens;
using EsportManager.BLL;
using EsportManager.BLL.Interfaces;
using EsportManager.DAL;
using EsportManager.DAL.Interfaces;
using EsportManager.UI;
using EsportManager.Utils;

namespace EsportManager
{
    class Program
    {
        private const int MIN_CONSOLE_WIDTH = 60;
        private const int MIN_CONSOLE_HEIGHT = 20;

        static void Main(string[] args)
        {
            try
            {
                Console.Title = "ESPORT MANAGER - Ứng dụng quản lý giải đấu Esports";
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                EsportManager.Utils.UIHelper.SafeClear();

                try
                {
                    Console.SetWindowSize(120, 35);
                    Console.SetBufferSize(120, 35);
                }
                catch (Exception)
                {
                    int currentWidth = EsportManager.Utils.UIHelper.SafeGetWindowWidth();
                    int currentHeight = EsportManager.Utils.UIHelper.SafeGetWindowHeight();

                    if (currentWidth < MIN_CONSOLE_WIDTH || currentHeight < MIN_CONSOLE_HEIGHT)
                    {
                        Console.WriteLine($"Kích thước cửa sổ quá nhỏ. Vui lòng mở rộng cửa sổ console (tối thiểu {MIN_CONSOLE_WIDTH}x{MIN_CONSOLE_HEIGHT}).");
                        Console.WriteLine("Nhấn phím bất kỳ để tiếp tục với kích thước hiện tại hoặc mở rộng cửa sổ trước...");
                        Console.ReadKey(true);
                    }
                    else
                    {
                        Console.WriteLine("Không thể đặt kích thước cửa sổ mong muốn. Sử dụng kích thước mặc định.");
                        Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey(true);
                    }
                    EsportManager.Utils.UIHelper.SafeClear();
                }

                ShowSpecialStartupScreen();

                IUserRepository userRepository = null;
                IUserService userService = null;

                try
                {
                    EsportManager.Utils.UIHelper.SafeClear();
                    Console.WriteLine("Đang khởi tạo hệ thống...");
                    System.Threading.Thread.Sleep(500);

                    string connectionString = @"Data Source=(local);Initial Catalog=EsportManager;Integrated Security=True;";
                    userRepository = new UserRepository(connectionString);
                }
                catch (Exception ex)
                {
                    EsportManager.Utils.UIHelper.SafeClear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Lỗi kết nối cơ sở dữ liệu: " + ex.Message);
                    Console.WriteLine("\nĐang chuyển sang chế độ Demo (dữ liệu mẫu)...");
                    Console.ForegroundColor = ConsoleColor.White;
                    System.Threading.Thread.Sleep(2000);

                    userRepository = new MockUserRepository();
                }

                EsportManager.Utils.UIHelper.SafeClear();
                Console.WriteLine("Đang kết nối cơ sở dữ liệu...");
                System.Threading.Thread.Sleep(500);

                userService = new UserService(userRepository);

                EsportManager.Utils.UIHelper.SafeClear();
                Console.WriteLine("Đang khởi tạo dịch vụ người dùng...");
                System.Threading.Thread.Sleep(500);

                IScreen mainScreen = new MainMenuScreen(userService);

                EsportManager.Utils.UIHelper.SafeClear();
                Console.WriteLine("Chuẩn bị hiển thị giao diện...");
                System.Threading.Thread.Sleep(500);

                mainScreen.Show();
            }
            catch (Exception ex)
            {
                EsportManager.Utils.UIHelper.SafeClear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Đã xảy ra lỗi không thể khắc phục: " + ex.Message);
                Console.WriteLine("Chi tiết lỗi: " + ex.StackTrace);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nNhấn phím bất kỳ để thoát...");
                Console.ReadKey();
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private static void ShowSpecialStartupScreen()
        {
            Console.CursorVisible = false;
            EsportManager.Utils.UIHelper.SafeClear();

            try
            {
                int windowWidth = EsportManager.Utils.UIHelper.SafeGetWindowWidth();
                int windowHeight = EsportManager.Utils.UIHelper.SafeGetWindowHeight();
                int startY = windowHeight / 3;

                int borderWidth = Math.Max(10, windowWidth - 4);

                string topBorder = "╔" + new string('═', borderWidth - 2) + "╗";
                string bottomBorder = "╚" + new string('═', borderWidth - 2) + "╝";
                string sideBorder = "║" + new string(' ', borderWidth - 2) + "║";

                int leftPosition = Math.Max(0, (windowWidth - borderWidth) / 2);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                SafeSetCursorPosition(leftPosition, startY - 2);
                Console.Write(topBorder);

                for (int i = 1; i <= 8; i++)
                {
                    SafeSetCursorPosition(leftPosition, startY - 2 + i);
                    Console.Write(sideBorder);
                }

                SafeSetCursorPosition(leftPosition, startY - 2 + 9);
                Console.Write(bottomBorder);

                string[] pixelFont = CreatePixelFont("ESPORT MANAGER");

                Console.ForegroundColor = ConsoleColor.Cyan;
                int titleWidth = pixelFont[0].Length;
                int centerX = Math.Max(0, (windowWidth - titleWidth) / 2);

                for (int i = 0; i < pixelFont.Length; i++)
                {
                    SafeSetCursorPosition(centerX, startY + i);
                    Console.Write(pixelFont[i]);
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                string loadingText = "[ĐANG KHỞI ĐỘNG]";
                SafeSetCursorPosition(Math.Max(0, (windowWidth - loadingText.Length) / 2), startY + 6);
                Console.Write(loadingText);

                for (int i = 0; i < 3; i++)
                {
                    System.Threading.Thread.Sleep(300);
                    SafeSetCursorPosition(Math.Max(0, windowWidth / 2 + 5), startY + 6);
                    Console.Write(new string('.', i + 1));
                }

                Console.ForegroundColor = ConsoleColor.White;
                System.Threading.Thread.Sleep(700);
                EsportManager.Utils.UIHelper.SafeClear();
            }
            catch
            {
                System.Threading.Thread.Sleep(1000);
                EsportManager.Utils.UIHelper.SafeClear();
            }
        }

        private static void SafeSetCursorPosition(int left, int top)
        {
            try
            {
                int windowWidth = EsportManager.Utils.UIHelper.SafeGetWindowWidth();
                int windowHeight = EsportManager.Utils.UIHelper.SafeGetWindowHeight();

                int safeLeft = Math.Max(0, Math.Min(left, windowWidth - 1));
                int safeTop = Math.Max(0, Math.Min(top, windowHeight - 1));

                Console.SetCursorPosition(safeLeft, safeTop);
            }
            catch
            {
            }
        }

        private static string[] CreatePixelFont(string text)
        {
            Dictionary<char, string[]> pixelLetters = new Dictionary<char, string[]>
            {
                {'E', new string[] {"█████", "█    ", "████ ", "█    ", "█████"}},
                {'S', new string[] {"█████", "█    ", "█████", "    █", "█████"}},
                {'P', new string[] {"█████", "█   █", "█████", "█    ", "█    "}},
                {'O', new string[] {"█████", "█   █", "█   █", "█   █", "█████"}},
                {'R', new string[] {"█████", "█   █", "████ ", "█  █ ", "█   █"}},
                {'T', new string[] {"█████", "  █  ", "  █  ", "  █  ", "  █  "}},
                {'M', new string[] {"█   █", "██ ██", "█ █ █", "█   █", "█   █"}},
                {'A', new string[] {"█████", "█   █", "█████", "█   █", "█   █"}},
                {'N', new string[] {"█   █", "██  █", "█ █ █", "█  ██", "█   █"}},
                {'G', new string[] {"█████", "█    ", "█  ██", "█   █", "█████"}},
                {' ', new string[] {"     ", "     ", "     ", "     ", "     "}}
            };

            string[] result = new string[5];
            for (int i = 0; i < 5; i++)
                result[i] = "";

            foreach (char c in text)
            {
                char upperC = char.ToUpper(c);
                if (pixelLetters.ContainsKey(upperC))
                {
                    string[] letterPattern = pixelLetters[upperC];
                    for (int i = 0; i < 5; i++)
                    {
                        result[i] += letterPattern[i] + " ";
                    }
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        result[i] += "      ";
                    }
                }
            }

            return result;
        }

        public class MockUserRepository : IUserRepository
        {
            private List<EsportManager.Models.User> _users;

            public MockUserRepository()
            {
                _users = new List<EsportManager.Models.User>
                {
                    new EsportManager.Models.User
                    {
                        UserID = 1,
                        Username = "@admin",
                        DisplayName = "Quản trị viên",
                        Password = EsportManager.Utils.SecurityHelper.HashPassword("admin123"),
                        Email = "admin@example.com",
                        Role = "Admin",
                        Phone = "0987654321",
                        SecurityQuestion = "Tên trường học đầu tiên?",
                        SecurityAnswer = EsportManager.Utils.SecurityHelper.HashString("truonghoc"),
                        Status = "Approved"
                    },
                    new EsportManager.Models.User
                    {
                        UserID = 2,
                        Username = "@player1",
                        DisplayName = "Player 1",
                        Password = EsportManager.Utils.SecurityHelper.HashPassword("player123"),
                        Email = "player1@example.com",
                        Role = "Player",
                        Phone = "0912345678",
                        SecurityQuestion = "Tên thú cưng đầu tiên?",
                        SecurityAnswer = EsportManager.Utils.SecurityHelper.HashString("meo"),
                        Status = "Approved"
                    },
                    new EsportManager.Models.User
                    {
                        UserID = 3,
                        Username = "@viewer1",
                        DisplayName = "Viewer 1",
                        Password = EsportManager.Utils.SecurityHelper.HashPassword("viewer123"),
                        Email = "viewer1@example.com",
                        Role = "Viewer",
                        Phone = "0923456789",
                        SecurityQuestion = "Tên thành phố sinh ra?",
                        SecurityAnswer = EsportManager.Utils.SecurityHelper.HashString("hanoi"),
                        Status = "Approved"
                    },
                    new EsportManager.Models.User
                    {
                        UserID = 4,
                        Username = "@pending1",
                        DisplayName = "Pending User",
                        Password = EsportManager.Utils.SecurityHelper.HashPassword("pending123"),
                        Email = "pending1@example.com",
                        Role = "Player",
                        Phone = "0934567890",
                        SecurityQuestion = "Tên trường đại học?",
                        SecurityAnswer = EsportManager.Utils.SecurityHelper.HashString("dhbkhn"),
                        Status = "Pending"
                    }
                };
            }

            public bool Add(EsportManager.Models.User user)
            {
                user.UserID = _users.Count + 1;
                _users.Add(user);
                return true;
            }

            public bool ChangePassword(int userID, string newPasswordHash)
            {
                var user = _users.Find(u => u.UserID == userID);
                if (user != null)
                {
                    user.Password = EsportManager.Utils.SecurityHelper.HashPassword(newPasswordHash);
                    return true;
                }
                return false;
            }

            public bool ChangeStatus(int userID, string newStatus)
            {
                var user = _users.Find(u => u.UserID == userID);
                if (user != null)
                {
                    user.Status = newStatus;
                    return true;
                }
                return false;
            }

            public bool Delete(int userID)
            {
                var user = _users.Find(u => u.UserID == userID);
                if (user != null)
                {
                    _users.Remove(user);
                    return true;
                }
                return false;
            }

            public List<EsportManager.Models.User> GetAllUsers()
            {
                return _users;
            }

            public EsportManager.Models.User GetByEmail(string email)
            {
                return _users.Find(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }

            public EsportManager.Models.User GetByID(int userID)
            {
                return _users.Find(u => u.UserID == userID);
            }

            public EsportManager.Models.User GetByUsername(string username)
            {
                return _users.Find(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            }

            public List<EsportManager.Models.User> GetPendingUsers()
            {
                return _users.FindAll(u => u.Status == "Pending");
            }

            public string GetSecurityQuestion(string username)
            {
                var user = _users.Find(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
                return user?.SecurityQuestion;
            }

            public bool IsEmailExists(string email)
            {
                return _users.Exists(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }

            public bool IsUsernameExists(string username)
            {
                return _users.Exists(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            }

            public bool Update(EsportManager.Models.User user)
            {
                var existingUser = _users.Find(u => u.UserID == user.UserID);
                if (existingUser != null)
                {
                    existingUser.DisplayName = user.DisplayName;
                    existingUser.Email = user.Email;
                    existingUser.Phone = user.Phone;
                    existingUser.SecurityQuestion = user.SecurityQuestion;
                    existingUser.SecurityAnswer = user.SecurityAnswer;
                    return true;
                }
                return false;
            }

            public bool VerifySecurityAnswer(string username, string answer)
            {
                var user = _users.Find(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
                if (user != null)
                {
                    return user.SecurityAnswer == EsportManager.Utils.SecurityHelper.HashString(answer);
                }
                return false;
            }

            public bool ChangeRole(int userID, string newRole)
            {
                var user = _users.Find(u => u.UserID == userID);
                if (user != null)
                {
                    user.Role = newRole;
                    return true;
                }
                return false;
            }
        }
    }
}