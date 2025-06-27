using System;
using System.Threading.Tasks;
using EsportsManager.DAL.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace EsportsManager.Tests.Sync
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

                // Check donations table
                var donationsSql = "SELECT DonationID, UserID, Amount, TargetType, TargetID, Status FROM Donations LIMIT 3";
                using var donationsCmd = connection.CreateCommand();
                donationsCmd.CommandText = donationsSql;
                using var donationsReader = donationsCmd.ExecuteReader();

                Console.WriteLine("\nDonations table data:");
                while (donationsReader.Read())
                {
                    Console.WriteLine($"  DonationID: {donationsReader["DonationID"]}, UserID: {donationsReader["UserID"]}, Amount: {donationsReader["Amount"]}, Target: {donationsReader["TargetType"]} #{donationsReader["TargetID"]}, Status: {donationsReader["Status"]}");
                }
                donationsReader.Close();

                // Check wallets table
                var walletsSql = "SELECT WalletID, UserID, Balance FROM Wallets LIMIT 5";
                using var walletsCmd = connection.CreateCommand();
                walletsCmd.CommandText = walletsSql;
                using var walletsReader = walletsCmd.ExecuteReader();

                Console.WriteLine("\nWallets table data:");
                while (walletsReader.Read())
                {
                    Console.WriteLine($"  WalletID: {walletsReader["WalletID"]}, UserID: {walletsReader["UserID"]}, Balance: {walletsReader["Balance"]}");
                }
                walletsReader.Close(); if (walletTransCount == 0 && donationsCount > 0)
                {
                    Console.WriteLine("\n--- Creating missing WalletTransactions ---");

                    // First create wallets for users who made donations but don't have wallets
                    var createWalletsSql = @"
                        INSERT INTO Wallets (UserID, Balance, TotalReceived, TotalWithdrawn, CreatedAt)
                        SELECT DISTINCT d.UserID, 500.00, 0.00, 0.00, NOW()
                        FROM Donations d
                        LEFT JOIN Wallets w ON d.UserID = w.UserID
                        WHERE w.WalletID IS NULL";

                    using var createWalletsCmd = connection.CreateCommand();
                    createWalletsCmd.CommandText = createWalletsSql;
                    var walletsCreated = createWalletsCmd.ExecuteNonQuery();
                    Console.WriteLine($"Created {walletsCreated} missing wallets");

                    // Now create WalletTransactions
                    var insertSql = @"
                        INSERT INTO WalletTransactions (WalletID, UserID, TransactionType, Amount, BalanceAfter, Status, ReferenceCode, Note, RelatedEntityType, RelatedEntityID, CreatedAt)
                        SELECT 
                            w.WalletID,
                            d.UserID,
                            'Donation',
                            -d.Amount,
                            500.00,
                            'Completed',
                            CONCAT('DON-', d.DonationID),
                            CONCAT('Donation to ', d.TargetType, ' #', d.TargetID),
                            d.TargetType,
                            d.TargetID,
                            d.DonationDate
                        FROM Donations d
                        JOIN Wallets w ON d.UserID = w.UserID
                        WHERE d.Status = 'Completed'";

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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Sync failed with exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}
