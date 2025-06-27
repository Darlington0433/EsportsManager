-- =============================================
-- SYSTEM STATS DIAGNOSTIC TEST
-- Run this to check what's causing the error
-- =============================================

USE EsportsManager;

-- Test 1: Check if basic tables exist
SELECT 'Table Existence Check' as Test;
SELECT 
    TABLE_NAME,
    TABLE_ROWS,
    CREATE_TIME
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'EsportsManager' 
    AND TABLE_NAME IN ('Users', 'Tournaments', 'Teams', 'Games');

-- Test 2: Check if we can get basic counts
SELECT 'Basic Counts' as Test;
SELECT 
    'Users' as TableName, COUNT(*) as RecordCount, MAX(CreatedAt) as LastRecord
FROM Users
UNION ALL
SELECT 
    'Tournaments', COUNT(*), MAX(CreatedAt)
FROM Tournaments  
UNION ALL
SELECT 
    'Teams', COUNT(*), MAX(CreatedAt)
FROM Teams
UNION ALL
SELECT 
    'Games', COUNT(*), MAX(CreatedAt)
FROM Games;

-- Test 3: Check Users table structure and sample data
SELECT 'User Data Sample' as Test;
SELECT UserID, Username, Role, Status, CreatedAt, LastLoginAt
FROM Users 
ORDER BY CreatedAt DESC
LIMIT 5;

-- Test 4: Check Tournaments table structure and sample data  
SELECT 'Tournament Data Sample' as Test;
SELECT TournamentID, TournamentName, Status, PrizePool, RegisteredTeams, CreatedAt
FROM Tournaments 
ORDER BY CreatedAt DESC
LIMIT 5;

-- Test 5: Check Teams table structure and sample data
SELECT 'Team Data Sample' as Test;
SELECT TeamID, TeamName, Status, CreatedAt
FROM Teams 
ORDER BY CreatedAt DESC  
LIMIT 5;

-- Test 6: Try the stored procedure
SELECT 'Stored Procedure Test' as Test;
CALL sp_GetSystemStats();

-- Test 7: Test if there are any data type issues
SELECT 'Data Type Check' as Test;
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'EsportsManager' 
    AND TABLE_NAME = 'Users'
    AND COLUMN_NAME IN ('Role', 'Status', 'CreatedAt', 'LastLoginAt');

SELECT 'DIAGNOSTIC COMPLETE' as Result;
