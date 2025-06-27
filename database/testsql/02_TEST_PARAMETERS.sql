-- =====================================================
-- TEST SPECIFIC STORED PROCEDURES WITH PARAMETERS
-- =====================================================

USE EsportsManager;

-- Test stored procedure với parameter
SELECT 'Testing sp_GetTopDonationReceivers with parameter...' as Status;
CALL sp_GetTopDonationReceivers(5);

SELECT 'Testing sp_GetTopDonators with parameter...' as Status;
CALL sp_GetTopDonators(5);

-- Check the actual parameter names in the procedures
SELECT 'Checking procedure definitions...' as Status;
SHOW CREATE PROCEDURE sp_GetTopDonationReceivers;
SHOW CREATE PROCEDURE sp_GetTopDonators;

SELECT '✅ PARAMETER TEST COMPLETED' as FinalResult;
