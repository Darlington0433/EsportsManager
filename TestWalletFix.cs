using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EsportsManager.DAL.Context;
using EsportsManager.BL.Services;

namespace TestWalletFix
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Testing WalletService GetWalletByUserIdAsync fix...");

            try
            {
                // Tạo logger mock
                var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                var walletLogger = loggerFactory.CreateLogger<WalletService>();

                // Tạo DataContext (giả sử có connection string)
                var dataContext = new DataContext("Server=localhost;Database=esportsmanager;Uid=root;Pwd=;");

                // Tạo WalletService
                var walletService = new WalletService(walletLogger, dataContext);

                // Test phương thức GetWalletByUserIdAsync với userId = 1
                Console.WriteLine("Calling GetWalletByUserIdAsync(1)...");
                var wallet = await walletService.GetWalletByUserIdAsync(1);

                if (wallet != null)
                {
                    Console.WriteLine($"Success! Retrieved wallet info:");
                    Console.WriteLine($"  Wallet ID: {wallet.Id}");
                    Console.WriteLine($"  User ID: {wallet.UserId}");
                    Console.WriteLine($"  Username: {wallet.Username}");
                    Console.WriteLine($"  Balance: {wallet.Balance:N0} VND");
                    Console.WriteLine($"  Total Received: {wallet.TotalReceived:N0} VND");
                    Console.WriteLine($"  Total Withdrawn: {wallet.TotalWithdrawn:N0} VND");
                    Console.WriteLine($"  Status: {wallet.Status}");
                    Console.WriteLine($"  Last Updated: {wallet.LastUpdatedAt}");
                }
                else
                {
                    Console.WriteLine("Wallet not found for user ID 1, but no error occurred (this is expected behavior)");
                }

                // Test GetWalletStatsAsync
                Console.WriteLine("\nCalling GetWalletStatsAsync(1)...");
                var stats = await walletService.GetWalletStatsAsync(1);

                if (stats != null)
                {
                    Console.WriteLine($"Success! Retrieved wallet stats:");
                    Console.WriteLine($"  Total Transactions: {stats.TotalTransactions}");
                    Console.WriteLine($"  Total Income: {stats.TotalIncome:N0} VND");
                    Console.WriteLine($"  Total Expense: {stats.TotalExpense:N0} VND");
                    Console.WriteLine($"  Current Balance: {stats.CurrentBalance:N0} VND");
                    Console.WriteLine($"  Monthly Stats Count: {stats.MonthlyStats.Count}");
                }
                else
                {
                    Console.WriteLine("Wallet stats not found, but no error occurred");
                }

                Console.WriteLine("\nAll wallet service methods executed successfully without DBNull exceptions!");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                }
            }

            Console.WriteLine("\nTest completed. Press any key to exit...");
            Console.ReadKey();
        }
    }
}
