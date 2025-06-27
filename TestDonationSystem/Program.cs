using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Services;
using EsportsManager.DAL.Context;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace TestDonationSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("=== QUICK TEST DONATION SYSTEM ===");

            try
            {
                // Create configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                // Create logger
                var loggerFactory = LoggerFactory.Create(builder =>
                    builder.AddConsole().SetMinimumLevel(LogLevel.Information));
                var walletLogger = loggerFactory.CreateLogger<WalletService>();
                var dataContextLogger = loggerFactory.CreateLogger<DataContext>();

                // Create data context
                var dataContext = new DataContext(configuration, dataContextLogger);

                // Create wallet service
                var walletService = new WalletService(walletLogger, dataContext);

                Console.WriteLine("✅ Services initialized successfully");

                // Test 1: Donation Overview
                Console.WriteLine("\n--- Test 1: Donation Overview ---");
                try
                {
                    var overview = await walletService.GetDonationOverviewAsync();
                    Console.WriteLine($"✅ Total Donations: {overview.TotalDonations:N0}");
                    Console.WriteLine($"✅ Total Amount: {overview.TotalDonationAmount:N0}");
                    Console.WriteLine($"✅ Total Receivers: {overview.TotalReceivers}");
                    Console.WriteLine($"✅ Total Donators: {overview.TotalDonators}");

                    Console.WriteLine("\n📊 DONATIONS BY TYPE:");
                    foreach (var item in overview.DonationByType)
                    {
                        Console.WriteLine($"  - {item.Key}: {item.Value:N0}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Donation Overview Error: {ex.Message}");
                    Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                }

                // Test 2: Top Donation Receivers
                Console.WriteLine("\n--- Test 2: Top Donation Receivers ---");
                try
                {
                    var topReceivers = await walletService.GetTopDonationReceiversAsync(5);
                    Console.WriteLine($"✅ Found {topReceivers.Count} top receivers:");
                    foreach (var receiver in topReceivers)
                    {
                        Console.WriteLine($"  - {receiver.Username}: {receiver.TotalAmount:N0}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Top Receivers Error: {ex.Message}");
                    Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                }

                // Test 3: Top Donators
                Console.WriteLine("\n--- Test 3: Top Donators ---");
                try
                {
                    var topDonators = await walletService.GetTopDonatorsAsync(5);
                    Console.WriteLine($"✅ Found {topDonators.Count} top donators:");
                    foreach (var donator in topDonators)
                    {
                        Console.WriteLine($"  - {donator.Username}: {donator.TotalAmount:N0}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Top Donators Error: {ex.Message}");
                    Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                }

                // Test 4: Donation History
                Console.WriteLine("\n--- Test 4: Donation History ---");
                try
                {
                    var filter = new DonationSearchFilterDto
                    {
                        PageNumber = 1,
                        PageSize = 5
                    };

                    var history = await walletService.GetDonationHistoryAsync(filter);
                    Console.WriteLine($"✅ Found {history.Count} donation records:");
                    foreach (var donation in history)
                    {
                        Console.WriteLine($"  - {donation.Username}: {donation.Amount:N0} on {donation.CreatedAt:yyyy-MM-dd HH:mm}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Donation History Error: {ex.Message}");
                    Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                }

                Console.WriteLine("\n=== TEST COMPLETED SUCCESSFULLY ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fatal Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
