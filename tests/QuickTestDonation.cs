using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Services;
using EsportsManager.DAL.Context;
using Microsoft.Extensions.Logging;

class QuickTestDonation
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== QUICK TEST DONATION SYSTEM ===");

        try
        {
            // Create logger
            var loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Information));
            var logger = loggerFactory.CreateLogger<WalletService>();

            // Create data context
            var dataContext = new DataContext();

            // Create wallet service
            var walletService = new WalletService(logger, dataContext);

            Console.WriteLine("✅ Services initialized successfully");

            // Test 1: Donation Overview
            Console.WriteLine("\n--- Test 1: Donation Overview ---");
            try
            {
                var overview = await walletService.GetDonationOverviewAsync();
                Console.WriteLine($"✅ Total Donations: {overview.TotalDonations:N0}");
                Console.WriteLine($"✅ Total Amount: {overview.TotalAmount:N0}");
                Console.WriteLine($"✅ Total Receivers: {overview.TotalReceivers}");
                Console.WriteLine($"✅ Total Donators: {overview.TotalDonators}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Donation Overview Error: {ex.Message}");
            }

            // Test 2: Top Donation Receivers
            Console.WriteLine("\n--- Test 2: Top Donation Receivers ---");
            try
            {
                var topReceivers = await walletService.GetTopDonationReceiversAsync(5);
                Console.WriteLine($"✅ Found {topReceivers.Count} top receivers:");
                foreach (var receiver in topReceivers)
                {
                    Console.WriteLine($"  - {receiver.Username}: {receiver.TotalReceived:N0}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Top Receivers Error: {ex.Message}");
            }

            // Test 3: Top Donators
            Console.WriteLine("\n--- Test 3: Top Donators ---");
            try
            {
                var topDonators = await walletService.GetTopDonatorsAsync(5);
                Console.WriteLine($"✅ Found {topDonators.Count} top donators:");
                foreach (var donator in topDonators)
                {
                    Console.WriteLine($"  - {donator.Username}: {donator.TotalDonated:N0}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Top Donators Error: {ex.Message}");
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
            }

            Console.WriteLine("\n=== TEST COMPLETED SUCCESSFULLY ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fatal Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }
}
