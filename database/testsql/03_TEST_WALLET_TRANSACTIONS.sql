-- =====================================================
-- TEST WALLET TRANSACTIONS DATA
-- =====================================================

USE EsportsManager;

-- Check if WalletTransactions table exists and has data
SELECT 'Testing WalletTransactions table...' as Status;
SELECT COUNT(*) as WalletTransactionCount FROM WalletTransactions;

-- Show some sample data
SELECT 'Sample WalletTransactions data...' as Status;
SELECT * FROM WalletTransactions LIMIT 10;

-- Check what types of transactions exist
SELECT 'Transaction types...' as Status;
SELECT DISTINCT TransactionType, COUNT(*) as Count 
FROM WalletTransactions 
GROUP BY TransactionType;

-- Check Donations table vs WalletTransactions
SELECT 'Donations vs WalletTransactions...' as Status;
SELECT 
    (SELECT COUNT(*) FROM Donations) as DonationsCount,
    (SELECT COUNT(*) FROM WalletTransactions WHERE TransactionType = 'Donation') as WalletDonationsCount;

SELECT '✅ WALLET TRANSACTIONS TEST COMPLETED' as FinalResult;
