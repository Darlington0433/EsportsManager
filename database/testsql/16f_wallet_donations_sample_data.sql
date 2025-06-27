-- =====================================================
-- 16F. WALLET AND DONATIONS SAMPLE DATA
-- =====================================================
-- Module: Sample Wallet and Donation Data
-- Description: Update wallet balances and insert donation transactions
-- Dependencies: 16b_users_sample_data.sql
-- =====================================================

USE EsportsManager;

-- Update wallets for players (they're already created by the trigger)
UPDATE Wallets SET 
    Balance = 1000.00, 
    TotalReceived = 1500.00 
WHERE UserID = 3;

UPDATE Wallets SET 
    Balance = 750.00, 
    TotalReceived = 1000.00 
WHERE UserID = 4;

UPDATE Wallets SET 
    Balance = 500.00, 
    TotalReceived = 750.00 
WHERE UserID = 5;

UPDATE Wallets SET 
    Balance = 250.00, 
    TotalReceived = 500.00 
WHERE UserID = 6;

UPDATE Wallets SET 
    Balance = 100.00, 
    TotalReceived = 200.00 
WHERE UserID = 7;

-- Add some donations from viewers to players
INSERT INTO Donations (UserID, Amount, Message, Status, TargetType, TargetID, DonationDate) VALUES
(8, 50.00, 'Great performance in last tournament!', 'Completed', 'Player', 3, '2024-12-01 10:30:00'),
(9, 30.00, 'You are my favorite player!', 'Completed', 'Player', 4, '2024-12-02 14:20:00'),
(10, 25.00, 'Keep up the good work', 'Completed', 'Player', 5, '2024-12-03 16:45:00'),
(8, 15.00, 'Amazing skills!', 'Completed', 'Player', 6, '2024-12-04 09:15:00'),
(9, 20.00, 'Looking forward to your next match', 'Completed', 'Player', 7, '2024-12-05 11:30:00'),
(11, 40.00, 'Supporting your team!', 'Completed', 'Team', 1, '2024-12-06 13:45:00'),
(12, 35.00, 'Great teamwork!', 'Completed', 'Team', 2, '2024-12-07 15:20:00'),
(13, 60.00, 'Amazing tournament organization!', 'Completed', 'Tournament', 1, '2024-12-08 17:30:00'),
(14, 55.00, 'Best tournament ever!', 'Completed', 'Tournament', 2, '2024-12-09 19:45:00'),
(8, 45.00, 'Keep it up!', 'Completed', 'Player', 3, '2024-12-10 12:00:00');

-- Create corresponding wallet transactions for donations
INSERT INTO WalletTransactions (WalletID, UserID, TransactionType, Amount, BalanceAfter, Status, ReferenceCode, Note, RelatedEntityType, RelatedEntityID, CreatedAt) VALUES
(8, 8, 'Donation', -50.00, 450.00, 'Completed', 'DON-001', 'Donation to Player #3', 'Player', 3, '2024-12-01 10:30:00'),
(9, 9, 'Donation', -30.00, 470.00, 'Completed', 'DON-002', 'Donation to Player #4', 'Player', 4, '2024-12-02 14:20:00'),
(10, 10, 'Donation', -25.00, 475.00, 'Completed', 'DON-003', 'Donation to Player #5', 'Player', 5, '2024-12-03 16:45:00'),
(8, 8, 'Donation', -15.00, 435.00, 'Completed', 'DON-004', 'Donation to Player #6', 'Player', 6, '2024-12-04 09:15:00'),
(9, 9, 'Donation', -20.00, 450.00, 'Completed', 'DON-005', 'Donation to Player #7', 'Player', 7, '2024-12-05 11:30:00'),
(11, 11, 'Donation', -40.00, 460.00, 'Completed', 'DON-006', 'Donation to Team #1', 'Team', 1, '2024-12-06 13:45:00'),
(12, 12, 'Donation', -35.00, 465.00, 'Completed', 'DON-007', 'Donation to Team #2', 'Team', 2, '2024-12-07 15:20:00'),
(13, 13, 'Donation', -60.00, 440.00, 'Completed', 'DON-008', 'Donation to Tournament #1', 'Tournament', 1, '2024-12-08 17:30:00'),
(14, 14, 'Donation', -55.00, 445.00, 'Completed', 'DON-009', 'Donation to Tournament #2', 'Tournament', 2, '2024-12-09 19:45:00'),
(8, 8, 'Donation', -45.00, 390.00, 'Completed', 'DON-010', 'Donation to Player #3', 'Player', 3, '2024-12-10 12:00:00');

SELECT '16F. Wallet and donations sample data inserted successfully!' as Message;
SELECT 'Total Player Wallets Updated: 5' as WalletStats;
SELECT 'Total Donations: 10' as DonationStats;
SELECT 'Total Donation Amount: $375.00' as DonationAmount;
