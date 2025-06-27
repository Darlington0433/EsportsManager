-- =====================================================
-- RUN ALL DONATION FIXES - COMPLETE SOLUTION
-- =====================================================

-- 1. Use the database
USE EsportsManager;

-- 2. Execute the donation history fix first
-- Run this manually: mysql -u root -p EsportsManager < database/DONATION_HISTORY_FIX.sql

-- 3. Execute the sample data script  
-- Run this manually: mysql -u root -p EsportsManager < database/ADD_SAMPLE_DONATIONS.sql

-- 4. Fix data integrity
DELIMITER //
CREATE PROCEDURE IF NOT EXISTS sp_RunAllFixes()
BEGIN
    -- Create missing wallets for users who don't have them
    INSERT INTO Wallets (UserID, Balance, TotalReceived, TotalWithdrawn, IsActive)
    SELECT u.UserID, 0.00, 0.00, 0.00, TRUE
    FROM Users u
    WHERE u.Role IN ('Player', 'Viewer')
        AND NOT EXISTS (SELECT 1 FROM Wallets w WHERE w.UserID = u.UserID);
    
    -- Update wallet balances based on transactions
    UPDATE Wallets w SET 
        Balance = (
            SELECT COALESCE(SUM(wt.Amount), 0)
            FROM WalletTransactions wt 
            WHERE wt.UserID = w.UserID
                AND wt.Status = 'Completed'
        ),
        TotalReceived = (
            SELECT COALESCE(SUM(wt.Amount), 0)
            FROM WalletTransactions wt 
            WHERE wt.UserID = w.UserID
                AND wt.Amount > 0
                AND wt.Status = 'Completed'
        ),
        TotalWithdrawn = (
            SELECT COALESCE(ABS(SUM(wt.Amount)), 0)
            FROM WalletTransactions wt 
            WHERE wt.UserID = w.UserID
                AND wt.Amount < 0
                AND wt.Status = 'Completed'
        );
        
    SELECT 'Data integrity fixes completed!' as Message;
END//
DELIMITER ;

-- Run the fix procedure
CALL sp_RunAllFixes();

-- 5. Verify data
SELECT 'Verification Results:' as Status;
SELECT COUNT(*) as TotalUsers FROM Users;
SELECT COUNT(*) as TotalDonations FROM Donations;
SELECT COUNT(*) as TotalWalletTransactions FROM WalletTransactions WHERE TransactionType = 'Donation';
SELECT COUNT(*) as TotalWallets FROM Wallets;

-- 6. Test a simple query to ensure donation data exists
SELECT 'Sample Donation Data:' as Status;
SELECT 
    wt.TransactionID,
    u.Username,
    ABS(wt.Amount) as Amount,
    wt.RelatedEntityType,
    wt.CreatedAt
FROM WalletTransactions wt
JOIN Users u ON wt.UserID = u.UserID
WHERE wt.TransactionType = 'Donation'
    AND wt.Status = 'Completed'
ORDER BY wt.CreatedAt DESC
LIMIT 5;

-- 7. Display success message
SELECT '✅ DONATION FIXES PREPARATION COMPLETED!' as Status;
SELECT 'Next steps:' as Message;
SELECT '1. Run: mysql -u root -p EsportsManager < database/DONATION_HISTORY_FIX.sql' as Step1;
SELECT '2. Run: mysql -u root -p EsportsManager < database/ADD_SAMPLE_DONATIONS.sql' as Step2;
SELECT '3. Test the donation history feature in the application' as Step3;
