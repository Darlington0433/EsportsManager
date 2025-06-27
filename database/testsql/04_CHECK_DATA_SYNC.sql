-- =====================================================
-- CHECK AND SYNC DONATIONS AND WALLET TRANSACTIONS
-- =====================================================

USE EsportsManager;

-- Check current data counts
SELECT 'Current data counts...' as Status;
SELECT 
    (SELECT COUNT(*) FROM Donations) as DonationsCount,
    (SELECT COUNT(*) FROM WalletTransactions WHERE TransactionType = 'Donation') as DonationTransactionsCount,
    (SELECT COUNT(*) FROM WalletTransactions) as TotalTransactionsCount;

-- Show some donation data
SELECT 'Sample Donations...' as Status;
SELECT * FROM Donations LIMIT 5;

-- Show some wallet transaction data
SELECT 'Sample WalletTransactions...' as Status; 
SELECT * FROM WalletTransactions WHERE TransactionType = 'Donation' LIMIT 5;

-- If WalletTransactions are missing, let's create them based on existing Donations
SET @missing_transactions = (
    SELECT COUNT(*) FROM Donations d 
    LEFT JOIN WalletTransactions wt ON (wt.RelatedEntityType = d.TargetType AND wt.RelatedEntityID = d.TargetID AND wt.UserID = d.UserID)
    WHERE wt.TransactionID IS NULL
);

SELECT CONCAT('Missing WalletTransactions: ', @missing_transactions) as MissingCheck;

SELECT '✅ DATA SYNC CHECK COMPLETED' as FinalResult;
