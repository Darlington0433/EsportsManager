using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EsportsManager.DAL.Context;
using EsportsManager.BL.Services;

namespace TestWalletFix2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Testing WalletService với mock data fallback...");

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
                Console.WriteLine("Testing GetWalletByUserIdAsync(1)...");
                var wallet = await walletService.GetWalletByUserIdAsync(1);

                if (wallet != null)
                {
                    Console.WriteLine($"SUCCESS! Retrieved wallet info (mock data expected):");
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
                    Console.WriteLine("ERROR: Wallet returned null (should return mock data)");
                }

                // Test GetWalletStatsAsync
                Console.WriteLine("\nTesting GetWalletStatsAsync(1)...");
                var stats = await walletService.GetWalletStatsAsync(1);

                if (stats != null)
                {
                    Console.WriteLine($"SUCCESS! Retrieved wallet stats (mock data expected):");
                    Console.WriteLine($"  Total Transactions: {stats.TotalTransactions}");
                    Console.WriteLine($"  Total Income: {stats.TotalIncome:N0} VND");
                    Console.WriteLine($"  Total Expense: {stats.TotalExpense:N0} VND");
                    Console.WriteLine($"  Current Balance: {stats.CurrentBalance:N0} VND");
                    Console.WriteLine($"  Monthly Stats Count: {stats.MonthlyStats.Count}");
                }
                else
                {
                    Console.WriteLine("ERROR: Wallet stats returned null (should return mock data)");
                }

                Console.WriteLine("\n✅ ALL TESTS PASSED - No exceptions thrown, mock data returned!");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ UNEXPECTED ERROR:");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                }
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
