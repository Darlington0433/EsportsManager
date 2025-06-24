-- =====================================================
-- STORED PROCEDURES
-- =====================================================

USE EsportManager;

DELIMITER //

-- Procedure: Get system overview statistics
DROP PROCEDURE IF EXISTS sp_GetSystemStats//
CREATE PROCEDURE sp_GetSystemStats()
BEGIN
    SELECT 
        (SELECT COUNT(*) FROM Users WHERE Role = 'Player') as TotalPlayers,
        (SELECT COUNT(*) FROM Users WHERE Role = 'Viewer') as TotalViewers,
        (SELECT COUNT(*) FROM Teams WHERE Status = 'Active') as ActiveTeams,
        (SELECT COUNT(*) FROM Tournaments WHERE Status IN ('Registration', 'Ongoing')) as ActiveTournaments,
        (SELECT COALESCE(SUM(Amount), 0) FROM Donations WHERE Status = 'Completed') as TotalDonations,
        (SELECT COUNT(*) FROM Games WHERE IsActive = TRUE) as ActiveGames;
END//

-- Procedure: Get top players by donations received
DROP PROCEDURE IF EXISTS sp_GetTopPlayersByDonations//
CREATE PROCEDURE sp_GetTopPlayersByDonations(IN p_Limit INT)
BEGIN
    SELECT 
        u.UserID,
        u.Username,
        u.DisplayName,
        COALESCE(w.TotalReceived, 0) as TotalDonationsReceived,
        COALESCE(w.Balance, 0) as CurrentBalance
    FROM Users u
    LEFT JOIN Wallets w ON u.UserID = w.UserID
    WHERE u.Role = 'Player' AND u.IsActive = TRUE
    ORDER BY w.TotalReceived DESC
    LIMIT p_Limit;
END//

-- Procedure: Get tournament statistics by game
DROP PROCEDURE IF EXISTS sp_GetTournamentStatsByGame//
CREATE PROCEDURE sp_GetTournamentStatsByGame(IN p_GameID INT)
BEGIN
    SELECT 
        t.TournamentID,
        t.TournamentName,
        t.Status,
        COUNT(tr.TeamID) as TeamsParticipating,
        t.PrizePool,
        t.StartDate,
        t.EndDate
    FROM Tournaments t
    LEFT JOIN TournamentRegistrations tr ON t.TournamentID = tr.TournamentID AND tr.Status = 'Approved'
    WHERE t.GameID = p_GameID
    GROUP BY t.TournamentID
    ORDER BY t.CreatedAt DESC;
END//

-- Procedure: Get tournament results with rankings
DROP PROCEDURE IF EXISTS sp_GetTournamentResults//
CREATE PROCEDURE sp_GetTournamentResults(IN p_TournamentID INT)
BEGIN
    SELECT 
        tr.Position,
        team.TeamName,
        tr.PrizeMoney,
        tr.Notes,
        u.DisplayName as TeamLeader,
        team.TeamID
    FROM TournamentResults tr
    JOIN Teams team ON tr.TeamID = team.TeamID
    JOIN Users u ON team.CreatedBy = u.UserID
    WHERE tr.TournamentID = p_TournamentID
    ORDER BY tr.Position ASC;
END//

-- Procedure: Add tournament result
DROP PROCEDURE IF EXISTS sp_AddTournamentResult//
CREATE PROCEDURE sp_AddTournamentResult(
    IN p_TournamentID INT,
    IN p_TeamID INT, 
    IN p_Position INT,
    IN p_PrizeMoney DECIMAL(12,2),
    IN p_Notes TEXT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    START TRANSACTION;
    
    -- Insert tournament result
    INSERT INTO TournamentResults (TournamentID, TeamID, Position, PrizeMoney, Notes)
    VALUES (p_TournamentID, p_TeamID, p_Position, p_PrizeMoney, p_Notes);
    
    -- Update wallet if prize money > 0
    IF p_PrizeMoney > 0 THEN
        UPDATE Wallets w
        JOIN TeamMembers tm ON w.UserID = tm.UserID
        SET w.Balance = w.Balance + (p_PrizeMoney / (SELECT COUNT(*) FROM TeamMembers WHERE TeamID = p_TeamID AND Status = 'Active')),
            w.TotalReceived = w.TotalReceived + (p_PrizeMoney / (SELECT COUNT(*) FROM TeamMembers WHERE TeamID = p_TeamID AND Status = 'Active'))
        WHERE tm.TeamID = p_TeamID AND tm.Status = 'Active';
    END IF;
    
    COMMIT;
END//

-- Procedure: Get detailed wallet transaction history
DROP PROCEDURE IF EXISTS sp_GetWalletTransactionHistory//
CREATE PROCEDURE sp_GetWalletTransactionHistory(
    IN p_UserID INT,
    IN p_Limit INT
)
BEGIN
    SELECT 
        wt.TransactionID,
        wt.TransactionType,
        wt.Amount,
        wt.Description,
        wt.CreatedAt,
        w.Balance as CurrentBalance
    FROM WalletTransactions wt
    JOIN Wallets w ON wt.WalletID = w.WalletID
    WHERE w.UserID = p_UserID
    ORDER BY wt.CreatedAt DESC
    LIMIT p_Limit;
END//

-- Procedure: Get team performance statistics
DROP PROCEDURE IF EXISTS sp_GetTeamPerformanceStats//
CREATE PROCEDURE sp_GetTeamPerformanceStats(IN p_TeamID INT)
BEGIN
    SELECT 
        t.TeamName,
        g.GameName,
        COUNT(DISTINCT tres.TournamentID) as TournamentsCompleted,
        AVG(tres.Position) as AveragePosition,
        MIN(tres.Position) as BestPosition,
        SUM(tres.PrizeMoney) as TotalPrizeWon,
        COUNT(DISTINCT tm.UserID) as CurrentMembers
    FROM Teams t
    JOIN Games g ON t.GameID = g.GameID
    LEFT JOIN TournamentResults tres ON t.TeamID = tres.TeamID
    LEFT JOIN TeamMembers tm ON t.TeamID = tm.TeamID AND tm.Status = 'Active'
    WHERE t.TeamID = p_TeamID
    GROUP BY t.TeamID;
END//

-- Procedure: Get tournament leaderboard
DROP PROCEDURE IF EXISTS sp_GetTournamentLeaderboard//
CREATE PROCEDURE sp_GetTournamentLeaderboard(IN p_TournamentID INT)
BEGIN
    SELECT 
        ROW_NUMBER() OVER (ORDER BY tres.Position ASC) as `RankPosition`,
        team.TeamName,
        tres.Position,
        tres.PrizeMoney,
        COUNT(DISTINCT tm.UserID) as TeamSize,
        GROUP_CONCAT(u.DisplayName SEPARATOR ', ') as TeamMembers
    FROM TournamentResults tres
    JOIN Teams team ON tres.TeamID = team.TeamID
    LEFT JOIN TeamMembers tm ON team.TeamID = tm.TeamID AND tm.Status = 'Active'
    LEFT JOIN Users u ON tm.UserID = u.UserID
    WHERE tres.TournamentID = p_TournamentID
    GROUP BY tres.ResultID, team.TeamName, tres.Position, tres.PrizeMoney
    ORDER BY tres.Position ASC;
END//

DELIMITER ;

SELECT 'Database procedures created successfully!' as Message;
