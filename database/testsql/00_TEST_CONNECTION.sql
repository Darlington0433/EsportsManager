-- =====================================================
-- TEST CONNECTION AND BASIC SETUP
-- =====================================================
-- Run this to test basic MySQL connection and check database state
-- =====================================================

-- Test if we can connect and show databases
SHOW DATABASES;

-- Try to use EsportsManager database
USE EsportsManager;

-- Show all tables in the database
SHOW TABLES;

-- Check if essential tables exist with data
SELECT 'Testing Users table...' as Status;
SELECT COUNT(*) as UserCount FROM Users;

SELECT 'Testing Wallets table...' as Status;
SELECT COUNT(*) as WalletCount FROM Wallets;

SELECT 'Testing Donations table...' as Status;
SELECT COUNT(*) as DonationCount FROM Donations;

-- Check if stored procedures exist
SELECT 'Testing stored procedures...' as Status;
SHOW PROCEDURE STATUS WHERE Name LIKE 'sp_GetDonation%';

-- Test a simple stored procedure call
SELECT 'Testing sp_GetDonationOverview...' as Status;
CALL sp_GetDonationOverview();

SELECT '✅ CONNECTION TEST COMPLETED' as FinalResult;
