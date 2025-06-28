-- =====================================================
-- 06_CREATE_PROCEDURES_ACHIEVEMENT.sql
-- Tạo các stored procedures cho achievement system
-- Run Order: 6
-- Prerequisites: 01-05 (all previous files)
-- =====================================================

USE EsportsManager;

-- =====================================================
-- ACHIEVEMENT PROCEDURES
-- =====================================================

DELIMITER $$

-- Procedure: Assign achievement to player
DROP PROCEDURE IF EXISTS sp_AssignAchievement$$
CREATE PROCEDURE sp_AssignAchievement(
    IN p_UserID INT,
    IN p_Title VARCHAR(100),
    IN p_Description TEXT,
    IN p_AchievementType VARCHAR(50),
    IN p_AssignedBy INT,
    IN p_TournamentID INT,
    IN p_TeamID INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    START TRANSACTION;
    
    -- Insert achievement
    INSERT INTO Achievements (
        UserID, Title, Description, AchievementType, 
        AssignedBy, TournamentID, TeamID, DateAchieved
    ) VALUES (
        p_UserID, p_Title, p_Description, p_AchievementType,
        p_AssignedBy, p_TournamentID, p_TeamID, NOW()
    );
    
    COMMIT;
    
    SELECT 'Achievement assigned successfully!' as Result;
END$$

-- Procedure: Get player achievements
DROP PROCEDURE IF EXISTS sp_GetPlayerAchievements$$
CREATE PROCEDURE sp_GetPlayerAchievements(IN p_UserID INT)
BEGIN
    SELECT 
        a.AchievementID,
        a.UserID,
        a.Title,
        a.Description,
        a.AchievementType,
        a.DateAchieved,
        a.AssignedBy,
        assigner.Username as AssignedByUsername,
        a.TournamentID,
        t.TournamentName,
        a.TeamID,
        team.TeamName,
        a.IsActive,
        a.CreatedAt
    FROM Achievements a
    LEFT JOIN Users assigner ON a.AssignedBy = assigner.UserID
    LEFT JOIN Tournaments t ON a.TournamentID = t.TournamentID
    LEFT JOIN Teams team ON a.TeamID = team.TeamID
    WHERE a.UserID = p_UserID AND a.IsActive = TRUE
    ORDER BY a.DateAchieved DESC;
END$$

-- Procedure: Get player statistics
DROP PROCEDURE IF EXISTS sp_GetPlayerStats$$
CREATE PROCEDURE sp_GetPlayerStats(IN p_UserID INT)
BEGIN
    SELECT 
        u.UserID,
        u.Username,
        u.FullName,
        COUNT(DISTINCT tm.TeamID) as TotalTeams,
        COUNT(DISTINCT tr.TournamentID) as TotalTournaments,
        COUNT(DISTINCT a.AchievementID) as TotalAchievements,
        COALESCE(w.Balance, 0.00) as WalletBalance,
        COALESCE(w.TotalReceived, 0.00) as TotalDonationsReceived,
        u.CreatedAt as MemberSince
    FROM Users u
    LEFT JOIN TeamMembers tm ON u.UserID = tm.UserID AND tm.Status = 'Active'
    LEFT JOIN TournamentRegistrations tr ON tm.TeamID = tr.TeamID AND tr.Status = 'Approved'
    LEFT JOIN Achievements a ON u.UserID = a.UserID AND a.IsActive = TRUE
    LEFT JOIN Wallets w ON u.UserID = w.UserID
    WHERE u.UserID = p_UserID
    GROUP BY u.UserID, u.Username, u.FullName;
END$$

-- Procedure: Get player tournament history
DROP PROCEDURE IF EXISTS sp_GetPlayerTournamentHistory$$
CREATE PROCEDURE sp_GetPlayerTournamentHistory(IN p_UserID INT)
BEGIN
    SELECT 
        t.TournamentID,
        t.TournamentName,
        t.StartDate,
        t.EndDate,
        t.Status as TournamentStatus,
        team.TeamName,
        tr.Status as RegistrationStatus,
        COALESCE(tres.Position, 0) as FinalPosition,
        COALESCE(tres.PrizeMoney, 0.00) as PrizeMoney
    FROM Users u
    JOIN TeamMembers tm ON u.UserID = tm.UserID
    JOIN Teams team ON tm.TeamID = team.TeamID
    JOIN TournamentRegistrations tr ON team.TeamID = tr.TeamID
    JOIN Tournaments t ON tr.TournamentID = t.TournamentID
    LEFT JOIN TournamentResults tres ON t.TournamentID = tres.TournamentID AND team.TeamID = tres.TeamID
    WHERE u.UserID = p_UserID
    ORDER BY t.StartDate DESC;
END$$

-- Procedure: Get achievement statistics
DROP PROCEDURE IF EXISTS sp_GetAchievementStats$$
CREATE PROCEDURE sp_GetAchievementStats()
BEGIN
    -- Achievement type distribution
    SELECT 
        'AchievementTypes' as Section,
        AchievementType,
        COUNT(*) as Count
    FROM Achievements 
    WHERE IsActive = TRUE
    GROUP BY AchievementType
    ORDER BY Count DESC;
    
    -- Top players by achievement count
    SELECT 
        'TopAchievers' as Section,
        u.UserID,
        u.Username,
        u.DisplayName,
        COUNT(a.AchievementID) as AchievementCount
    FROM Users u
    JOIN Achievements a ON u.UserID = a.UserID
    WHERE a.IsActive = TRUE AND u.Role = 'Player'
    GROUP BY u.UserID, u.Username, u.DisplayName
    ORDER BY AchievementCount DESC
    LIMIT 10;
END$$

-- Procedure: Remove achievement
DROP PROCEDURE IF EXISTS sp_RemoveAchievement$$
CREATE PROCEDURE sp_RemoveAchievement(IN p_AchievementID INT)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    START TRANSACTION;
    
    -- Soft delete by setting IsActive to FALSE
    UPDATE Achievements 
    SET IsActive = FALSE, UpdatedAt = NOW()
    WHERE AchievementID = p_AchievementID;
    
    COMMIT;
    
    SELECT 'Achievement removed successfully!' as Result;
END$$

DELIMITER ;

SELECT 'Achievement procedures created successfully!' as Message;
