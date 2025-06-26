-- =============================================
-- Quick Database Test and Verification Script
-- This script tests the database structure and verifies user data
-- =============================================

USE `EsportsManager`;

-- Test 1: Check if Users table exists and has correct structure
DESCRIBE `Users`;

-- Test 2: Count total users
SELECT COUNT(*) as TotalUsers FROM `Users`;

-- Test 3: Check admin accounts
SELECT 
    `UserID`,
    `Username`,
    `Email`,
    `FullName`,
    `Role`,
    `Status`,
    `IsActive`,
    `EmailVerified`,
    `CreatedAt`
FROM `Users` 
WHERE `Role` = 'Admin';

-- Test 4: Check player accounts
SELECT 
    `UserID`,
    `Username`,
    `Email`,
    `FullName`,
    `Role`,
    `Status`,
    `IsActive`,
    `EmailVerified`,
    `TotalTournaments`,
    `TournamentsWon`
FROM `Users` 
WHERE `Role` = 'Player';

-- Test 5: Check viewer accounts
SELECT 
    `UserID`,
    `Username`,
    `Email`,
    `FullName`,
    `Role`,
    `Status`,
    `IsActive`,
    `EmailVerified`
FROM `Users` 
WHERE `Role` = 'Viewer';

-- Test 6: Test authentication data integrity
SELECT 
    `Username`,
    LENGTH(`PasswordHash`) as PasswordHashLength,
    LENGTH(`SecurityAnswerHash`) as SecurityAnswerHashLength,
    `EmailVerified`,
    `IsActive`
FROM `Users`
WHERE `Username` IN ('admin', 'player1', 'viewer1');

-- Test 7: Check for any NULL values in required fields
SELECT 
    `Username`,
    CASE WHEN `PasswordHash` IS NULL THEN 'NULL' ELSE 'OK' END as PasswordHash_Status,
    CASE WHEN `Email` IS NULL THEN 'NULL' ELSE 'OK' END as Email_Status,
    CASE WHEN `Role` IS NULL THEN 'NULL' ELSE 'OK' END as Role_Status,
    CASE WHEN `Status` IS NULL THEN 'NULL' ELSE 'OK' END as Status_Status,
    CASE WHEN `IsActive` IS NULL THEN 'NULL' ELSE 'OK' END as IsActive_Status,
    CASE WHEN `TotalTournaments` IS NULL THEN 'NULL' ELSE 'OK' END as TotalTournaments_Status,
    CASE WHEN `TournamentsWon` IS NULL THEN 'NULL' ELSE 'OK' END as TournamentsWon_Status
FROM `Users`;

-- Test 8: Verify password hash format (should start with $2a$ for BCrypt)
SELECT 
    `Username`,
    `Role`,
    CASE 
        WHEN `PasswordHash` LIKE '$2a$%' THEN 'Valid BCrypt Hash'
        ELSE 'Invalid Hash Format'
    END as PasswordHashFormat
FROM `Users`;

-- Test 9: Check for duplicate usernames or emails
SELECT 'Username Duplicates' as CheckType, `Username`, COUNT(*) as Count
FROM `Users`
GROUP BY `Username`
HAVING COUNT(*) > 1
UNION ALL
SELECT 'Email Duplicates' as CheckType, `Email`, COUNT(*) as Count
FROM `Users`
GROUP BY `Email`
HAVING COUNT(*) > 1;

-- Test 10: Final verification summary
SELECT 
    'Database Status' as CheckType,
    CASE 
        WHEN COUNT(*) = 13 THEN 'PASSED - All 13 users created successfully'
        ELSE CONCAT('FAILED - Expected 13 users, found ', COUNT(*))
    END as Result
FROM `Users`
UNION ALL
SELECT 
    'Admin Accounts' as CheckType,
    CASE 
        WHEN COUNT(*) = 2 THEN 'PASSED - 2 admin accounts found'
        ELSE CONCAT('FAILED - Expected 2 admin accounts, found ', COUNT(*))
    END as Result
FROM `Users` WHERE `Role` = 'Admin'
UNION ALL
SELECT 
    'Player Accounts' as CheckType,
    CASE 
        WHEN COUNT(*) = 6 THEN 'PASSED - 6 player accounts found'
        ELSE CONCAT('FAILED - Expected 6 player accounts, found ', COUNT(*))
    END as Result
FROM `Users` WHERE `Role` = 'Player'
UNION ALL
SELECT 
    'Viewer Accounts' as CheckType,
    CASE 
        WHEN COUNT(*) = 5 THEN 'PASSED - 5 viewer accounts found'
        ELSE CONCAT('FAILED - Expected 5 viewer accounts, found ', COUNT(*))
    END as Result
FROM `Users` WHERE `Role` = 'Viewer';
