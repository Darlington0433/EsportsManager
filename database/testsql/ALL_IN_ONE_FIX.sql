-- =====================================================
-- ALL-IN-ONE FIX SCRIPT - TỔNG HỢP TẤT CẢ SỬA LỖI
-- Hợp nhất từ: ADD_SAMPLE_DONATIONS.sql, SYSTEM_STATS_FIX.sql, 
-- TEST_SYSTEM_STATS.sql, update_correct_bcrypt_hashes.sql
-- =====================================================

USE EsportsManager;

-- =====================================================
-- 1. THÊM DỮ LIỆU DONATION MẪU ĐỂ TEST
-- =====================================================

-- Kiểm tra xem có user và team nào chưa, nếu chưa thì tạo
INSERT IGNORE INTO Users (Username, PasswordHash, Email, FullName, Role, Status, IsActive) VALUES
('admin', '$2b$10$defaulthash', 'admin@esports.com', 'Admin System', 'Admin', 'Active', TRUE),
('viewer1', '$2b$10$defaulthash', 'viewer1@test.com', 'Viewer One', 'Viewer', 'Active', TRUE),
('viewer2', '$2b$10$defaulthash', 'viewer2@test.com', 'Viewer Two', 'Viewer', 'Active', TRUE),
('player1', '$2b$10$defaulthash', 'player1@test.com', 'Player One', 'Player', 'Active', TRUE),
('player2', '$2b$10$defaulthash', 'player2@test.com', 'Player Two', 'Player', 'Active', TRUE);

-- Thêm game mẫu
INSERT IGNORE INTO Games (GameName, Description, Genre, IsActive) VALUES
('League of Legends', 'MOBA Game', 'MOBA', TRUE),
('Counter-Strike 2', 'FPS Game', 'FPS', TRUE);

-- Thêm team mẫu
INSERT IGNORE INTO Teams (TeamName, Description, GameID, CreatedBy, IsActive, Status) VALUES
('Team Alpha', 'Professional Esports Team', 1, 4, TRUE, 'Active'),
('Team Beta', 'Semi-Pro Team', 2, 5, TRUE, 'Active');

-- Thêm tournament mẫu
INSERT IGNORE INTO Tournaments (TournamentName, Description, GameID, StartDate, EndDate, CreatedBy, Status) VALUES
('Summer Championship 2024', 'Annual summer tournament', 1, '2024-07-01 10:00:00', '2024-07-15 18:00:00', 1, 'Completed'),
('Winter Cup 2024', 'Winter season tournament', 2, '2024-12-01 10:00:00', '2024-12-15 18:00:00', 1, 'Ongoing');

-- Tạo wallet cho user
INSERT IGNORE INTO Wallets (UserID, Balance, TotalReceived, TotalWithdrawn, IsActive) VALUES
(2, 500.00, 0.00, 0.00, TRUE),  -- viewer1
(3, 300.00, 0.00, 0.00, TRUE),  -- viewer2
(4, 100.00, 150.00, 50.00, TRUE),  -- player1
(5, 200.00, 300.00, 100.00, TRUE); -- player2

-- Thêm donations mẫu
INSERT IGNORE INTO Donations (UserID, Amount, Message, Status, TargetType, TargetID, DonationDate) VALUES
-- Donations from viewer1
(2, 50.00, 'Good luck in the tournament!', 'Completed', 'Player', 4, '2024-06-01 10:00:00'),
(2, 100.00, 'Amazing gameplay!', 'Completed', 'Team', 1, '2024-06-02 14:30:00'),
(2, 25.00, 'Keep it up!', 'Completed', 'Tournament', 1, '2024-06-03 16:00:00'),

-- Donations from viewer2
(3, 75.00, 'Great performance!', 'Completed', 'Player', 5, '2024-06-05 11:00:00'),
(3, 30.00, 'Supporting the team!', 'Completed', 'Team', 2, '2024-06-06 15:00:00'),
(3, 40.00, 'Love this tournament!', 'Completed', 'Tournament', 2, '2024-06-07 13:00:00'),

-- More donations for testing
(2, 20.00, 'Small donation', 'Completed', 'Player', 4, '2024-06-10 09:00:00'),
(3, 60.00, 'Big fan!', 'Completed', 'Player', 4, '2024-06-11 12:00:00'),
(2, 15.00, 'Tournament support', 'Completed', 'Tournament', 1, '2024-06-12 14:00:00'),
(3, 35.00, 'Team support', 'Completed', 'Team', 1, '2024-06-13 16:00:00');

-- Tạo WalletTransactions tương ứng với donations
INSERT IGNORE INTO WalletTransactions (WalletID, UserID, TransactionType, Amount, BalanceAfter, Status, ReferenceCode, Note, RelatedEntityType, RelatedEntityID, CreatedAt) 
SELECT 
    w.WalletID,
    d.UserID,
    'Donation',
    -d.Amount,
    w.Balance,
    'Completed',
    CONCAT('DON-', LPAD(d.DonationID, 6, '0')),
    CONCAT('Donation to ', d.TargetType, ' #', d.TargetID, ' - ', COALESCE(d.Message, 'No message')),
    d.TargetType,
    d.TargetID,
    d.DonationDate
FROM Donations d
JOIN Wallets w ON d.UserID = w.UserID
WHERE d.Status = 'Completed'
AND NOT EXISTS (
    SELECT 1 FROM WalletTransactions wt 
    WHERE wt.UserID = d.UserID 
    AND wt.RelatedEntityType = d.TargetType 
    AND wt.RelatedEntityID = d.TargetID
    AND wt.ReferenceCode = CONCAT('DON-', LPAD(d.DonationID, 6, '0'))
);

-- =====================================================
-- 2. SYSTEM STATS STORED PROCEDURES (từ SYSTEM_STATS_FIX.sql)
-- =====================================================

DELIMITER //

-- Procedure: Get system statistics overview
DROP PROCEDURE IF EXISTS sp_GetSystemStats//
CREATE PROCEDURE sp_GetSystemStats()
BEGIN
    -- Basic counts
    SELECT 
        'Overview' as Section,
        (SELECT COUNT(*) FROM Users) as TotalUsers,
        (SELECT COUNT(*) FROM Users WHERE Status = 'Active') as ActiveUsers,
        (SELECT COUNT(*) FROM Tournaments) as TotalTournaments,
        (SELECT COUNT(*) FROM Tournaments WHERE Status IN ('Ongoing', 'Registration')) as ActiveTournaments,
        (SELECT COUNT(*) FROM Tournaments WHERE Status = 'Completed') as CompletedTournaments,
        (SELECT COUNT(*) FROM Teams) as TotalTeams,
        (SELECT COUNT(*) FROM Teams WHERE Status = 'Active') as ActiveTeams;
        
    -- Financial statistics  
    SELECT 
        'Financial' as Section,
        COALESCE(SUM(PrizePool), 0) as TotalPrizePool,
        COALESCE(SUM(EntryFee * (
            SELECT COUNT(*) 
            FROM TournamentRegistrations tr 
            WHERE tr.TournamentID = t.TournamentID 
            AND tr.Status = 'Approved'
        )), 0) as TotalEntryFees
    FROM Tournaments t;
    
    -- User statistics by role
    SELECT 
        'UsersByRole' as Section,
        Role,
        COUNT(*) as Count
    FROM Users 
    GROUP BY Role;
    
    -- User statistics by status
    SELECT 
        'UsersByStatus' as Section,
        Status,
        COUNT(*) as Count
    FROM Users 
    GROUP BY Status;
    
    -- Tournament statistics by status
    SELECT 
        'TournamentsByStatus' as Section,
        Status,
        COUNT(*) as Count
    FROM Tournaments 
    GROUP BY Status;
    
    -- Recent activity (last 7 days)
    SELECT 
        'RecentActivity' as Section,
        (SELECT COUNT(*) FROM Users WHERE CreatedAt >= DATE_SUB(NOW(), INTERVAL 7 DAY)) as NewUsers,
        (SELECT COUNT(*) FROM Tournaments WHERE CreatedAt >= DATE_SUB(NOW(), INTERVAL 7 DAY)) as NewTournaments,
        (SELECT COUNT(*) FROM Teams WHERE CreatedAt >= DATE_SUB(NOW(), INTERVAL 7 DAY)) as NewTeams;
END//

-- Procedure: Get detailed user statistics
DROP PROCEDURE IF EXISTS sp_GetUserStats//
CREATE PROCEDURE sp_GetUserStats()
BEGIN
    SELECT 
        u.Role,
        u.Status,
        COUNT(*) as Count,
        MIN(u.CreatedAt) as FirstRegistration,
        MAX(u.CreatedAt) as LastRegistration
    FROM Users u
    GROUP BY u.Role, u.Status
    ORDER BY u.Role, u.Status;
    
    -- Top active users by last login
    SELECT 
        'TopActiveUsers' as Section,
        u.Username,
        u.Role,
        u.LastLoginAt
    FROM Users u
    WHERE u.LastLoginAt IS NOT NULL
        AND u.Status = 'Active'
    ORDER BY u.LastLoginAt DESC
    LIMIT 10;
END//

-- Procedure: Get detailed tournament statistics  
DROP PROCEDURE IF EXISTS sp_GetTournamentStats//
CREATE PROCEDURE sp_GetTournamentStats()
BEGIN
    -- Tournament status distribution
    SELECT 
        'StatusDistribution' as Section,
        Status,
        COUNT(*) as Count,
        AVG(PrizePool) as AvgPrizePool,
        SUM(PrizePool) as TotalPrizePool
    FROM Tournaments
    GROUP BY Status;
    
    -- Top tournaments by prize pool
    SELECT 
        'TopByPrizePool' as Section,
        t.TournamentName,
        t.PrizePool,
        t.Status,
        (SELECT COUNT(*) FROM TournamentRegistrations tr WHERE tr.TournamentID = t.TournamentID AND tr.Status = 'Approved') as RegisteredTeams,
        t.StartDate
    FROM Tournaments t
    ORDER BY t.PrizePool DESC
    LIMIT 10;
    
    -- Recent tournaments
    SELECT 
        'Recent' as Section,
        TournamentName,
        Status,
        CreatedAt,
        StartDate
    FROM Tournaments
    WHERE CreatedAt >= DATE_SUB(NOW(), INTERVAL 30 DAY)
    ORDER BY CreatedAt DESC;
END//

-- Procedure: Get detailed team statistics
DROP PROCEDURE IF EXISTS sp_GetTeamStats//
CREATE PROCEDURE sp_GetTeamStats()
BEGIN
    -- Team status distribution
    SELECT 
        'StatusDistribution' as Section,
        Status,
        COUNT(*) as Count
    FROM Teams
    GROUP BY Status;
    
    -- Teams by game
    SELECT 
        'ByGame' as Section,
        g.GameName,
        COUNT(t.TeamID) as TeamCount
    FROM Teams t
    JOIN Games g ON t.GameID = g.GameID
    WHERE t.Status = 'Active'
    GROUP BY g.GameID, g.GameName
    ORDER BY TeamCount DESC;
    
    -- Team sizes (if available)
    SELECT 
        'TeamSizes' as Section,
        COUNT(tm.UserID) as MemberCount,
        COUNT(DISTINCT tm.TeamID) as TeamCount
    FROM Teams t
    LEFT JOIN TeamMembers tm ON t.TeamID = tm.TeamID AND tm.Status = 'Active'
    WHERE t.Status = 'Active'
    GROUP BY t.TeamID
    HAVING COUNT(tm.UserID) > 0;
END//

-- Procedure: Fix missing data and ensure system integrity
DROP PROCEDURE IF EXISTS sp_FixSystemData//
CREATE PROCEDURE sp_FixSystemData()
BEGIN
    -- Create default admin if none exists
    INSERT IGNORE INTO Users (Username, PasswordHash, Email, FullName, Role, Status, IsActive, CreatedAt)
    SELECT 'admin', '$2a$11$AT5YJeJ9yMR60/YFFsWYp.PPMe1ZFHZ.RuR6EBGJ5ZIzmCgtI3zh6', 
           'admin@esportsmanager.com', 'System Administrator', 'Admin', 'Active', TRUE, NOW()
    WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Role = 'Admin');
    
    -- Create default games if none exist
    INSERT IGNORE INTO Games (GameName, Description, Genre, IsActive)
    SELECT * FROM (
        SELECT 'League of Legends' as GameName, 'Popular MOBA game' as Description, 'MOBA' as Genre, TRUE as IsActive
        UNION ALL
        SELECT 'Counter-Strike 2', 'Competitive FPS game', 'FPS', TRUE
        UNION ALL  
        SELECT 'Valorant', 'Tactical FPS game', 'FPS', TRUE
    ) AS DefaultGames
    WHERE NOT EXISTS (SELECT 1 FROM Games);
    
    -- Update user counts
    UPDATE Users SET UpdatedAt = NOW() WHERE UpdatedAt IS NULL;
    
    SELECT 'System data integrity check completed!' as Message;
END//

DELIMITER ;

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_users_role_status ON Users(Role, Status);
CREATE INDEX IF NOT EXISTS idx_users_created ON Users(CreatedAt);
CREATE INDEX IF NOT EXISTS idx_tournaments_status ON Tournaments(Status);
CREATE INDEX IF NOT EXISTS idx_tournaments_created ON Tournaments(CreatedAt);
CREATE INDEX IF NOT EXISTS idx_teams_status ON Teams(Status);
CREATE INDEX IF NOT EXISTS idx_teams_created ON Teams(CreatedAt);

SELECT 'System stats procedures created successfully!' as Message;

-- =====================================================
-- 3. TESTS FOR SYSTEM STATS (từ TEST_SYSTEM_STATS.sql)
-- =====================================================

-- Test basic connection
SELECT 'Database connection successful!' as Status;

-- Check if main tables exist
SELECT 
    'Table Check' as Test,
    (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'EsportsManager' AND table_name = 'Users') as Users_Table,
    (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'EsportsManager' AND table_name = 'Tournaments') as Tournaments_Table,
    (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'EsportsManager' AND table_name = 'Teams') as Teams_Table,
    (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'EsportsManager' AND table_name = 'TournamentRegistrations') as TournamentRegistrations_Table;

-- Test basic counts
SELECT 
    'Current Data' as Test,
    (SELECT COUNT(*) FROM Users) as Total_Users,
    (SELECT COUNT(*) FROM Users WHERE Status = 'Active') as Active_Users,
    (SELECT COUNT(*) FROM Tournaments) as Total_Tournaments,
    (SELECT COUNT(*) FROM Teams) as Total_Teams,
    (SELECT COUNT(*) FROM TournamentRegistrations) as Total_Registrations;

-- Test the RegisteredTeams calculation
SELECT 
    'Tournament Registration Test' as Test,
    t.TournamentID,
    t.TournamentName,
    t.Status,
    (SELECT COUNT(*) FROM TournamentRegistrations tr WHERE tr.TournamentID = t.TournamentID AND tr.Status = 'Approved') as RegisteredTeams
FROM Tournaments t
LIMIT 5;

-- Test if stored procedures exist
SELECT 
    'Stored Procedures Check' as Test,
    (SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema = 'EsportsManager' AND routine_name = 'sp_GetSystemStats') as sp_GetSystemStats,
    (SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema = 'EsportsManager' AND routine_name = 'sp_GetAllTournaments') as sp_GetAllTournaments,
    (SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema = 'EsportsManager' AND routine_name = 'sp_FixSystemData') as sp_FixSystemData;

-- Test the sp_GetAllTournaments procedure (which the service uses)
SELECT 'Testing sp_GetAllTournaments...' as Test;
CALL sp_GetAllTournaments();

-- Test the system stats procedure
SELECT 'Testing sp_GetSystemStats...' as Test;
CALL sp_GetSystemStats();

SELECT 'All tests completed!' as Status;

-- =====================================================
-- 4. UPDATE BCRYPT HASHES (Sửa mật khẩu khớp với BCrypt.Net)
-- =====================================================

-- Cập nhật hash cho admin và superadmin
UPDATE Users SET 
    PasswordHash = '$2a$11$9inYA.zu1eSu2CdJ3XwDMuMl95./WHUIovBSe3VsvXHtgQSCKYcaS',
    Status = 'Active',
    IsActive = TRUE,
    IsEmailVerified = TRUE
WHERE Username IN ('admin', 'superadmin');

-- Cập nhật hash cho tất cả player
UPDATE Users SET 
    PasswordHash = '$2a$11$x752zckXIW/bUITiCFq7FumuhWGe8ssSoY7JNemsH5J7U6yU3KKzC',
    Status = 'Active',
    IsActive = TRUE,
    IsEmailVerified = TRUE
WHERE Username LIKE 'player%';

-- Cập nhật hash cho tất cả viewer
UPDATE Users SET 
    PasswordHash = '$2a$11$u9zcIxF8UCfSjIOuaLhdZ.4lUlXiICWhdSY3uvZ/WTtPm0F0CXouW',
    Status = 'Active',
    IsActive = TRUE,
    IsEmailVerified = TRUE
WHERE Username LIKE 'viewer%';

-- Kiểm tra kết quả
SELECT 
    UserID,
    Username,
    LEFT(PasswordHash, 30) as HashPrefix,
    Status,
    IsActive,
    IsEmailVerified
FROM Users 
WHERE Username IN ('admin', 'superadmin') 
   OR Username LIKE 'player%' 
   OR Username LIKE 'viewer%'
ORDER BY Username;

-- =====================================================
-- 5. FINAL TESTING (bao gồm donation procedures)
-- =====================================================

-- Test the system stats procedure
SELECT 'Testing sp_GetSystemStats...' as Test;
CALL sp_GetSystemStats();

-- Test user stats procedure
SELECT 'Testing sp_GetUserStats...' as Test;
CALL sp_GetUserStats();

-- Test tournament stats procedure
SELECT 'Testing sp_GetTournamentStats...' as Test;
CALL sp_GetTournamentStats();

-- Test team stats procedure
SELECT 'Testing sp_GetTeamStats...' as Test;
CALL sp_GetTeamStats();

-- Test stored procedures from donation system
SELECT 'Testing sp_GetDonationOverview...' as Status;
CALL sp_GetDonationOverview();

SELECT 'Testing sp_GetDonationsByType...' as Status;
CALL sp_GetDonationsByType();

-- Test the donations
SELECT 'Testing donation data...' as Test;
SELECT COUNT(*) as DonationCount FROM Donations;
SELECT COUNT(*) as TransactionCount FROM WalletTransactions WHERE TransactionType = 'Donation';

-- Kiểm tra kết quả cập nhật BCrypt hashes
SELECT 
    'BCrypt Hash Check' as Test,
    UserID,
    Username,
    LEFT(PasswordHash, 30) as HashPrefix,
    Status,
    IsActive,
    IsEmailVerified
FROM Users 
WHERE Username IN ('admin', 'superadmin') 
   OR Username LIKE 'player%' 
   OR Username LIKE 'viewer%'
ORDER BY Username
LIMIT 10;

SELECT 'Tất cả thay đổi đã được áp dụng!' as Message;
