using System;
using System.Threading.Tasks;
using EsportsManager.DAL.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace EsportsManager.Tests
{
    /// <summary>
    /// Test program để sync donation data
    /// </summary>
    class SyncDonationData
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("=== SYNC DONATION DATA ===");

                // Setup configuration
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                // Setup logging
                using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                var logger = loggerFactory.CreateLogger<DataContext>();

                // Create DataContext
                var dataContext = new DataContext(configuration, logger);

                Console.WriteLine("✅ DataContext initialized successfully");

                // Check current data
                Console.WriteLine("\n--- Checking current data ---");

                var donationsCountSql = "SELECT COUNT(*) FROM Donations";
                var walletTransCountSql = "SELECT COUNT(*) FROM WalletTransactions WHERE TransactionType = 'Donation'";

                using var connection = dataContext.CreateConnection();
                connection.Open();

                using var cmd1 = connection.CreateCommand();
                cmd1.CommandText = donationsCountSql;
                var donationsCount = Convert.ToInt32(cmd1.ExecuteScalar());

                using var cmd2 = connection.CreateCommand();
                cmd2.CommandText = walletTransCountSql;
                var walletTransCount = Convert.ToInt32(cmd2.ExecuteScalar());

                Console.WriteLine($"Donations count: {donationsCount}");
                Console.WriteLine($"WalletTransactions (Donation type) count: {walletTransCount}");

                if (walletTransCount == 0 && donationsCount > 0)
                {
                    Console.WriteLine("\n--- Creating missing WalletTransactions ---");

                    // Insert WalletTransactions for existing donations
                    var insertSql = @"
                        INSERT INTO WalletTransactions (WalletID, UserID, TransactionType, Amount, BalanceAfter, Status, ReferenceCode, Note, RelatedEntityType, RelatedEntityID, CreatedAt)
                        SELECT 
                            w.WalletID,
                            d.UserID,
                            'Donation' as TransactionType,
                            -d.Amount as Amount,
                            w.Balance as BalanceAfter,
                            'Completed' as Status,
                            CONCAT('DON-', LPAD(d.DonationID, 3, '0')) as ReferenceCode,
                            CONCAT('Donation to ', d.TargetType, ' #', d.TargetID) as Note,
                            d.TargetType as RelatedEntityType,
                            d.TargetID as RelatedEntityID,
                            d.DonationDate as CreatedAt
                        FROM Donations d
                        JOIN Wallets w ON d.UserID = w.UserID
                        WHERE d.Status = 'Completed'
                        AND NOT EXISTS (
                            SELECT 1 FROM WalletTransactions wt 
                            WHERE wt.UserID = d.UserID 
                            AND wt.RelatedEntityType = d.TargetType 
                            AND wt.RelatedEntityID = d.TargetID
                            AND wt.TransactionType = 'Donation'
                        )";

                    using var cmd3 = connection.CreateCommand();
                    cmd3.CommandText = insertSql;
                    var inserted = cmd3.ExecuteNonQuery();

                    Console.WriteLine($"✅ Created {inserted} WalletTransactions");
                }
                else if (walletTransCount > 0)
                {
                    Console.WriteLine("✅ WalletTransactions already exist");
                }
                else
                {
                    Console.WriteLine("⚠️ No donations found to sync");
                }

                // Check final counts
                using var cmd4 = connection.CreateCommand();
                cmd4.CommandText = walletTransCountSql;
                var finalCount = Convert.ToInt32(cmd4.ExecuteScalar());
                Console.WriteLine($"\nFinal WalletTransactions count: {finalCount}");

                Console.WriteLine("\n=== SYNC COMPLETED ===");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Sync failed with exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
