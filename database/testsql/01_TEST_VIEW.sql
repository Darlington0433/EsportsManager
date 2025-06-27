-- =====================================================
-- TEST VIEW v_user_wallet_summary
-- =====================================================
-- Test if the view used by WalletService works correctly
-- =====================================================

USE EsportsManager;

-- Test if view exists
SHOW CREATE VIEW v_user_wallet_summary;

-- Test view data
SELECT 'Testing v_user_wallet_summary view...' as Status;
SELECT * FROM v_user_wallet_summary LIMIT 5;

-- Test specific query used by WalletService
SELECT 'Testing WalletService query...' as Status;
SELECT * FROM v_user_wallet_summary WHERE UserID = 3;

-- Test if all required columns exist
SELECT 'Testing required columns...' as Status;
SELECT 
    UserID,
    Username,
    Balance,
    TotalReceived,
    TotalWithdrawn,
    LastUpdated
FROM v_user_wallet_summary 
WHERE UserID = 3;

SELECT '✅ VIEW TEST COMPLETED' as FinalResult;
