-- =====================================================
-- SYSTEM STATS STORED PROCEDURES
-- =====================================================

USE EsportsManager;

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
