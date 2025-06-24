using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.DAL.Context;
using MySql.Data.MySqlClient;

namespace EsportsManager.UI.Services
{
    /// <summary>
    /// Service kiểm tra tính toàn vẹn của hệ thống
    /// </summary>
    public class SystemIntegrityService
    {
        private readonly DataContext _dataContext;

        public SystemIntegrityService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// Kiểm tra xem các tài khoản mặc định đã được tạo chưa
        /// </summary>
        /// <returns>True nếu tài khoản admin tồn tại</returns>
        public async Task<bool> CheckDefaultAccountsAsync()
        {
            try
            {
                using var connection = _dataContext.CreateConnection() as MySqlConnection;
                if (connection == null) return false;

                await connection.OpenAsync();

                string query = "SELECT COUNT(*) FROM Users WHERE Role = 'Admin' AND Username = 'admin'";
                using var command = new MySqlCommand(query, connection);
                var result = await command.ExecuteScalarAsync();

                return Convert.ToInt32(result) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra xem database có các bảng cần thiết không
        /// </summary>
        /// <returns>Danh sách bảng thiếu (nếu có)</returns>
        public async Task<List<string>> CheckRequiredTablesAsync()
        {
            List<string> missingTables = new List<string>();
            string[] requiredTables = { "Users", "Teams", "Tournaments", "Games", "Matches", "Wallets", "Donations" };

            try
            {
                using var connection = _dataContext.CreateConnection() as MySqlConnection;
                if (connection == null) return requiredTables.ToList();

                await connection.OpenAsync();

                foreach (var table in requiredTables)
                {
                    string query = $"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'EsportsManager' AND table_name = '{table}'";
                    using var command = new MySqlCommand(query, connection);
                    var result = await command.ExecuteScalarAsync();

                    if (Convert.ToInt32(result) == 0)
                    {
                        missingTables.Add(table);
                    }
                }

                return missingTables;
            }
            catch (Exception)
            {
                return requiredTables.ToList(); // Return all tables as missing if we can't check
            }
        }

        /// <summary>
        /// Hiển thị thông báo hướng dẫn nếu database chưa được cài đặt đúng
        /// </summary>
        /// <returns>True nếu database đã sẵn sàng</returns>
        public async Task<bool> ValidateDatabaseSetupAsync()
        {
            try
            {
                // Kiểm tra kết nối database
                bool connectionOk = await _dataContext.TestConnectionAsync();
                if (!connectionOk)
                {
                    DisplayDatabaseSetupInstructions();
                    return false;
                }

                // Kiểm tra các bảng cần thiết
                var missingTables = await CheckRequiredTablesAsync();
                if (missingTables.Count > 0)
                {
                    DisplayMissingTablesMessage(missingTables);
                    return false;
                }

                // Kiểm tra tài khoản mặc định
                bool hasDefaultAccounts = await CheckDefaultAccountsAsync();
                if (!hasDefaultAccounts)
                {
                    DisplayMissingAccountsMessage();
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra database: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Hiển thị hướng dẫn thiết lập database
        /// </summary>
        private void DisplayDatabaseSetupInstructions()
        {
            Console.Clear();
            Console.WriteLine("╔═════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              THIẾT LẬP DATABASE ESPORTSMANAGER                   ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("Hệ thống phát hiện rằng cơ sở dữ liệu chưa được thiết lập đúng cách.");
            Console.WriteLine("Vui lòng thực hiện các bước sau để khắc phục:");
            Console.WriteLine();
            Console.WriteLine("1. Kiểm tra MySQL Server đã được khởi động");
            Console.WriteLine("2. Import file SQL trong thư mục database/esportsmanager.sql");
            Console.WriteLine("   HOẶC chạy tuần tự các file trong database/split_sql/");
            Console.WriteLine("3. Kiểm tra connection string trong appsettings.json");
            Console.WriteLine();
            Console.WriteLine("Chi tiết hướng dẫn được cung cấp trong file database/SETUP_GUIDE.md");
            Console.WriteLine();
            Console.WriteLine("Nhấn phím bất kỳ để thoát...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Hiển thị thông báo về các bảng bị thiếu
        /// </summary>
        private void DisplayMissingTablesMessage(List<string> missingTables)
        {
            Console.Clear();
            Console.WriteLine("╔═════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                   DATABASE KHÔNG HOÀN CHỈNH                      ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("Phát hiện một số bảng dữ liệu cần thiết đang bị thiếu:");
            foreach (var table in missingTables)
            {
                Console.WriteLine($"- {table}");
            }
            Console.WriteLine();
            Console.WriteLine("Vui lòng import lại toàn bộ file SQL trong thư mục database/esportsmanager.sql");
            Console.WriteLine("hoặc chạy tuần tự các file trong database/split_sql/");
            Console.WriteLine();
            Console.WriteLine("Chi tiết hướng dẫn được cung cấp trong file database/SETUP_GUIDE.md");
            Console.WriteLine();
            Console.WriteLine("Nhấn phím bất kỳ để thoát...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Hiển thị thông báo về tài khoản mặc định bị thiếu
        /// </summary>
        private void DisplayMissingAccountsMessage()
        {
            Console.Clear();
            Console.WriteLine("╔═════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                TÀI KHOẢN MẶC ĐỊNH KHÔNG TỒN TẠI                 ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("Không tìm thấy tài khoản admin mặc định trong database.");
            Console.WriteLine("Điều này có thể do file dữ liệu mẫu chưa được import.");
            Console.WriteLine();
            Console.WriteLine("Vui lòng chạy file database/split_sql/07_sample_data.sql");
            Console.WriteLine("hoặc import lại toàn bộ database.");
            Console.WriteLine();
            Console.WriteLine("Chi tiết hướng dẫn được cung cấp trong file database/SETUP_GUIDE.md");
            Console.WriteLine();
            Console.WriteLine("Nhấn phím bất kỳ để thoát...");
            Console.ReadKey(true);
        }
    }
}
