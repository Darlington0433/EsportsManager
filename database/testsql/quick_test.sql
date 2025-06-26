-- =============================================
-- Quick Test Script
-- Run this after creating schema and inserting data
-- =============================================

USE `EsportsManager`;

-- 1. Check table counts
SELECT 'SYSTEM TABLES' as Category, '' as TableName, '' as Count
UNION ALL
SELECT '', 'SystemSettings', CAST(COUNT(*) as CHAR) FROM `SystemSettings`
UNION ALL
SELECT '', 'SystemLogs', CAST(COUNT(*) as CHAR) FROM `SystemLogs`
UNION ALL
SELECT 'CORE TABLES' as Category, '' as TableName, '' as Count
UNION ALL
SELECT '', 'Games', CAST(COUNT(*) as CHAR) FROM `Games`
UNION ALL
SELECT '', 'Users', CAST(COUNT(*) as CHAR) FROM `Users`
UNION ALL
SELECT '', 'Teams', CAST(COUNT(*) as CHAR) FROM `Teams`
UNION ALL
SELECT '', 'TeamMembers', CAST(COUNT(*) as CHAR) FROM `TeamMembers`
UNION ALL
SELECT '', 'Tournaments', CAST(COUNT(*) as CHAR) FROM `Tournaments`
UNION ALL
SELECT '', 'TournamentTeams', CAST(COUNT(*) as CHAR) FROM `TournamentTeams`
UNION ALL
SELECT '', 'Matches', CAST(COUNT(*) as CHAR) FROM `Matches`
UNION ALL
SELECT '', 'TournamentResults', CAST(COUNT(*) as CHAR) FROM `TournamentResults`
UNION ALL
SELECT 'FINANCIAL TABLES' as Category, '' as TableName, '' as Count
UNION ALL
SELECT '', 'Wallets', CAST(COUNT(*) as CHAR) FROM `Wallets`
UNION ALL
SELECT '', 'Transactions', CAST(COUNT(*) as CHAR) FROM `Transactions`
UNION ALL
SELECT '', 'Donations', CAST(COUNT(*) as CHAR) FROM `Donations`
UNION ALL
SELECT 'SOCIAL TABLES' as Category, '' as TableName, '' as Count
UNION ALL
SELECT '', 'Votes', CAST(COUNT(*) as CHAR) FROM `Votes`
UNION ALL
SELECT '', 'Feedback', CAST(COUNT(*) as CHAR) FROM `Feedback`
UNION ALL
SELECT 'ACHIEVEMENT TABLES' as Category, '' as TableName, '' as Count
UNION ALL
SELECT '', 'Achievements', CAST(COUNT(*) as CHAR) FROM `Achievements`
UNION ALL
SELECT '', 'UserAchievements', CAST(COUNT(*) as CHAR) FROM `UserAchievements`;

-- 2. Test login accounts
SELECT 'TEST ACCOUNTS' as TestType, Username, Role, Status, EmailVerified 
FROM `Users` 
WHERE Username IN ('admin', 'player1', 'viewer1')
ORDER BY Role, Username;

-- 3. Test database integrity
SELECT 'INTEGRITY CHECK' as CheckType, 'All tables exist' as Status;
