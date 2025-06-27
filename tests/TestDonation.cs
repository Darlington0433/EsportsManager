using System;
using System.Threading.Tasks;
using EsportsManager.BL.Services;
using EsportsManager.DAL.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EsportsManager.Tests
{
    /// <summary>
    /// Test program để kiểm tra tính năng donation
    /// </summary>
    class TestDonation
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("=== TEST DONATION FEATURE ===");

                // Setup configuration
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                // Setup logging
                using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                var logger = loggerFactory.CreateLogger<WalletService>();

                // Create DataContext
                var dataContext = new DataContext(configuration, loggerFactory.CreateLogger<DataContext>());

                // Create WalletService
                var walletService = new WalletService(logger, dataContext);

                Console.WriteLine("✅ Services initialized successfully");

                // Test connection
                Console.WriteLine("\n--- Testing Database Connection ---");
                try
                {
                    var testWallet = await walletService.GetWalletByUserIdAsync(1);
                    Console.WriteLine("✅ Database connection test passed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Database connection test failed: {ex.Message}");
                    return;
                }

                // Test GetDonationOverviewAsync
                Console.WriteLine("\n--- Testing GetDonationOverviewAsync ---");
                try
                {
                    var overview = await walletService.GetDonationOverviewAsync();
                    Console.WriteLine($"✅ GetDonationOverviewAsync successful!");
                    Console.WriteLine($"   Total Donations: {overview.TotalDonations}");
                    Console.WriteLine($"   Total Donators: {overview.TotalDonators}");
                    Console.WriteLine($"   Total Receivers: {overview.TotalReceivers}");
                    Console.WriteLine($"   Total Amount: {overview.TotalDonationAmount:C}");
                    Console.WriteLine($"   Last Updated: {overview.LastUpdated}");

                    if (overview.DonationByType.Count > 0)
                    {
                        Console.WriteLine("   Donation by Type:");
                        foreach (var kvp in overview.DonationByType)
                        {
                            Console.WriteLine($"     {kvp.Key}: {kvp.Value:C}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ GetDonationOverviewAsync failed: {ex.Message}");
                    Console.WriteLine($"   Stack Trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"   Inner Exception: {ex.InnerException.Message}");
                    }
                }

                // Test GetTopDonationReceiversAsync
                Console.WriteLine("\n--- Testing GetTopDonationReceiversAsync ---");
                try
                {
                    var topReceivers = await walletService.GetTopDonationReceiversAsync(5);
                    Console.WriteLine($"✅ GetTopDonationReceiversAsync successful! Found {topReceivers.Count} receivers");
                    foreach (var receiver in topReceivers)
                    {
                        Console.WriteLine($"   {receiver.Username}: {receiver.TotalAmount:C} ({receiver.DonationCount} donations)");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ GetTopDonationReceiversAsync failed: {ex.Message}");
                }

                // Test GetTopDonatorsAsync
                Console.WriteLine("\n--- Testing GetTopDonatorsAsync ---");
                try
                {
                    var topDonators = await walletService.GetTopDonatorsAsync(5);
                    Console.WriteLine($"✅ GetTopDonatorsAsync successful! Found {topDonators.Count} donators");
                    foreach (var donator in topDonators)
                    {
                        Console.WriteLine($"   {donator.Username}: {donator.TotalAmount:C} ({donator.DonationCount} donations)");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ GetTopDonatorsAsync failed: {ex.Message}");
                }

                // Test GetDonationHistoryAsync
                Console.WriteLine("\n--- Testing GetDonationHistoryAsync ---");
                try
                {
                    var filter = new EsportsManager.BL.DTOs.DonationSearchFilterDto
                    {
                        PageNumber = 1,
                        PageSize = 5
                    };
                    var history = await walletService.GetDonationHistoryAsync(filter);
                    Console.WriteLine($"✅ GetDonationHistoryAsync successful! Found {history.Count} transactions");
                    foreach (var transaction in history)
                    {
                        Console.WriteLine($"   ID:{transaction.Id} {transaction.Username}: {transaction.Amount:C} - {transaction.CreatedAt:dd/MM/yyyy}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ GetDonationHistoryAsync failed: {ex.Message}");
                }

                Console.WriteLine("\n=== TEST COMPLETED ===");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test failed with exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
