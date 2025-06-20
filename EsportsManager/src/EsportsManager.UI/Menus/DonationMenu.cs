using EsportsManager.BL.Interfaces;
using EsportsManager.UI.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace EsportsManager.UI.Menus
{
    /// <summary>
    /// Donation Menu - handles donation operations
    /// </summary>
    public class DonationMenu
    {
        private readonly IDonationService _donationService;
        private readonly IUserService _userService;
        private readonly IWalletService _walletService;
        private readonly ILogger<DonationMenu> _logger;
        private readonly int _currentUserId;

        public DonationMenu(
            IDonationService donationService,
            IUserService userService,
            IWalletService walletService,
            ILogger<DonationMenu> logger,
            int currentUserId)
        {
            _donationService = donationService ?? throw new ArgumentNullException(nameof(donationService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUserId = currentUserId;
        }

        /// <summary>
        /// Show donation menu
        /// </summary>
        public async Task ShowAsync()
        {
            while (true)
            {
                try
                {
                    ConsoleHelper.ShowHeader("Donations");

                    Console.WriteLine("1. Make a Donation");
                    Console.WriteLine("2. View Donations Made");
                    Console.WriteLine("3. View Donations Received");
                    Console.WriteLine("4. View Donation Statistics");
                    Console.WriteLine("0. Back");
                    Console.WriteLine();

                    var choice = ConsoleInput.GetChoice("Enter your choice", 0, 4);

                    switch (choice)
                    {
                        case 1:
                            await MakeDonationAsync();
                            break;
                        case 2:
                            await ViewDonationsMadeAsync();
                            break;
                        case 3:
                            await ViewDonationsReceivedAsync();
                            break;
                        case 4:
                            await ViewDonationStatisticsAsync();
                            break;
                        case 0:
                            return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in donation menu");
                    ConsoleHelper.ShowError($"Error: {ex.Message}");
                    ConsoleHelper.PressAnyKeyToContinue();
                }
            }
        }

        /// <summary>
        /// Make a donation
        /// </summary>
        private async Task MakeDonationAsync()
        {
            ConsoleHelper.ShowHeader("Make a Donation");

            try
            {
                // Get current balance
                var balanceResult = await _walletService.GetBalanceAsync(_currentUserId);

                if (!balanceResult.IsSuccess)
                {
                    ConsoleHelper.ShowError($"Could not retrieve balance: {balanceResult.ErrorMessage}");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                Console.WriteLine($"Current Balance: {balanceResult.Data:C2}");
                Console.WriteLine();

                // Get recipient user ID
                var recipientId = ConsoleInput.GetInt("Enter recipient user ID", 1);

                if (recipientId == _currentUserId)
                {
                    ConsoleHelper.ShowError("You cannot donate to yourself");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                // Check if recipient exists
                var recipientResult = await _userService.GetUserByIdAsync(recipientId);

                if (!recipientResult.IsSuccess)
                {
                    ConsoleHelper.ShowError($"Recipient not found: {recipientResult.ErrorMessage}");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                Console.WriteLine($"Recipient: {recipientResult.Data!.Username}");

                // Get donation amount
                var amount = ConsoleInput.GetDecimal("Enter donation amount", 1, (decimal)balanceResult.Data);

                // Get donation message
                var message = ConsoleInput.GetString("Enter donation message (optional)");

                Console.WriteLine($"Donating {amount:C2} to {recipientResult.Data!.Username}...");

                // Make donation
                var result = await _donationService.MakeDonationAsync(_currentUserId, recipientId, amount, message);

                if (result.IsSuccess)
                {
                    ConsoleHelper.ShowSuccess($"Successfully donated {amount:C2} to {recipientResult.Data!.Username}");
                }
                else
                {
                    ConsoleHelper.ShowError($"Failed to make donation: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making donation");
                ConsoleHelper.ShowError($"Error: {ex.Message}");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }

        /// <summary>
        /// View donations made
        /// </summary>
        private async Task ViewDonationsMadeAsync()
        {
            ConsoleHelper.ShowHeader("Donations Made");

            try
            {
                var result = await _donationService.GetDonationsByUserIdAsync(_currentUserId);

                if (!result.IsSuccess)
                {
                    ConsoleHelper.ShowError($"Failed to get donations: {result.ErrorMessage}");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                var donations = result.Data!;

                if (!donations.Any())
                {
                    Console.WriteLine("You haven't made any donations yet");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                Console.WriteLine($"{"Date",-20} {"Recipient",-20} {"Amount",-15} {"Message",-40}");
                Console.WriteLine(new string('-', 95));

                foreach (var donation in donations.OrderByDescending(d => d.DonationDate))
                {
                    // Get recipient username
                    var recipientResult = await _userService.GetUserByIdAsync(donation.ToUserId);
                    var recipientName = recipientResult.IsSuccess ? recipientResult.Data!.Username : $"User {donation.ToUserId}";

                    Console.WriteLine($"{donation.DonationDate,-20} {recipientName,-20} {donation.Amount,-15:C2} {donation.Message,-40}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error viewing donations made");
                ConsoleHelper.ShowError($"Error: {ex.Message}");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }

        /// <summary>
        /// View donations received
        /// </summary>
        private async Task ViewDonationsReceivedAsync()
        {
            ConsoleHelper.ShowHeader("Donations Received");

            try
            {
                var result = await _donationService.GetDonationsByUserIdAsync(_currentUserId, true);

                if (!result.IsSuccess)
                {
                    ConsoleHelper.ShowError($"Failed to get donations: {result.ErrorMessage}");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                var donations = result.Data!;

                if (!donations.Any())
                {
                    Console.WriteLine("You haven't received any donations yet");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                Console.WriteLine($"{"Date",-20} {"Donor",-20} {"Amount",-15} {"Message",-40}");
                Console.WriteLine(new string('-', 95));

                foreach (var donation in donations.OrderByDescending(d => d.DonationDate))
                {
                    // Get donor username
                    var donorResult = await _userService.GetUserByIdAsync(donation.FromUserId);
                    var donorName = donorResult.IsSuccess ? donorResult.Data!.Username : $"User {donation.FromUserId}";

                    Console.WriteLine($"{donation.DonationDate,-20} {donorName,-20} {donation.Amount,-15:C2} {donation.Message,-40}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error viewing donations received");
                ConsoleHelper.ShowError($"Error: {ex.Message}");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }

        /// <summary>
        /// View donation statistics
        /// </summary>
        private async Task ViewDonationStatisticsAsync()
        {
            ConsoleHelper.ShowHeader("Donation Statistics");

            try
            {
                var result = await _donationService.GetDonationStatisticsAsync(_currentUserId);

                if (!result.IsSuccess)
                {
                    ConsoleHelper.ShowError($"Failed to get donation statistics: {result.ErrorMessage}");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                var stats = result.Data!;

                Console.WriteLine("Summary:");
                Console.WriteLine($"Total donations made: {stats.TotalDonationsMade}");
                Console.WriteLine($"Total amount donated: {stats.TotalAmountDonated:C2}");
                Console.WriteLine($"Total donations received: {stats.TotalDonationsReceived}");
                Console.WriteLine($"Total amount received: {stats.TotalAmountReceived:C2}");

                Console.WriteLine();
                Console.WriteLine("Top Recipients:");
                if (stats.TopRecipients.Any())
                {
                    Console.WriteLine($"{"User",-20} {"Amount",-15} {"Count",-10}");
                    Console.WriteLine(new string('-', 45));

                    foreach (var recipient in stats.TopRecipients)
                    {
                        Console.WriteLine($"{recipient.Username,-20} {recipient.TotalAmount,-15:C2} {recipient.Count,-10}");
                    }
                }
                else
                {
                    Console.WriteLine("No recipients");
                }

                Console.WriteLine();
                Console.WriteLine("Top Donors:");
                if (stats.TopDonors.Any())
                {
                    Console.WriteLine($"{"User",-20} {"Amount",-15} {"Count",-10}");
                    Console.WriteLine(new string('-', 45));

                    foreach (var donor in stats.TopDonors)
                    {
                        Console.WriteLine($"{donor.Username,-20} {donor.TotalAmount,-15:C2} {donor.Count,-10}");
                    }
                }
                else
                {
                    Console.WriteLine("No donors");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error viewing donation statistics");
                ConsoleHelper.ShowError($"Error: {ex.Message}");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }
    }
}
