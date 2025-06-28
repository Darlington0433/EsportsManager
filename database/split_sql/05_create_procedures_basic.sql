-- =====================================================
-- 05_CREATE_PROCEDURES_BASIC.sql
-- Tạo các stored procedures cơ bản cho tournament và team
-- Run Order: 5
-- Prerequisites: 01-04 (all previous files)
-- =====================================================

USE EsportsManager;

-- =====================================================
-- TOURNAMENT PROCEDURES
-- =====================================================

DELIMITER $$

-- Procedure: Get all tournaments
DROP PROCEDURE IF EXISTS sp_GetAllTournaments$$
CREATE PROCEDURE sp_GetAllTournaments()
BEGIN
    SELECT 
        t.TournamentID,
        t.TournamentName,
        t.Description,
        t.GameID,
        g.GameName,
        t.StartDate,
        t.EndDate,
        t.RegistrationDeadline,
        t.MaxTeams,
        t.EntryFee,
        t.PrizePool,
        t.Status,
        (SELECT COUNT(*) FROM TournamentRegistrations WHERE TournamentID = t.TournamentID AND Status = 'Approved') AS RegisteredTeams,
        t.CreatedBy,
        t.CreatedAt
    FROM Tournaments t
    JOIN Games g ON t.GameID = g.GameID
    ORDER BY t.CreatedAt DESC;
END$$

-- Procedure: Get available tournaments (registration open)
DROP PROCEDURE IF EXISTS sp_GetAvailableTournaments$$
CREATE PROCEDURE sp_GetAvailableTournaments()
BEGIN
    SELECT 
        t.TournamentID,
        t.TournamentName,
        t.Description,
        t.GameID,
        g.GameName,
        t.StartDate,
        t.EndDate,
        t.RegistrationDeadline,
        t.MaxTeams,
        t.EntryFee,
        t.PrizePool,
        t.Status,
        (SELECT COUNT(*) FROM TournamentRegistrations WHERE TournamentID = t.TournamentID AND Status = 'Approved') AS RegisteredTeams,
        t.CreatedBy,
        t.CreatedAt
    FROM Tournaments t
    JOIN Games g ON t.GameID = g.GameID
    WHERE t.Status = 'Registration' 
    AND t.RegistrationDeadline >= CURDATE()
    AND (SELECT COUNT(*) FROM TournamentRegistrations WHERE TournamentID = t.TournamentID AND Status = 'Approved') < t.MaxTeams
    ORDER BY t.RegistrationDeadline ASC;
END$$

-- Procedure: Get tournament by ID
DROP PROCEDURE IF EXISTS sp_GetTournamentById$$
CREATE PROCEDURE sp_GetTournamentById(IN p_TournamentID INT)
BEGIN
    SELECT 
        t.TournamentID,
        t.TournamentName,
        t.Description,
        t.GameID,
        g.GameName,
        t.StartDate,
        t.EndDate,
        t.RegistrationDeadline,
        t.MaxTeams,
        t.EntryFee,
        t.PrizePool,
        t.Status,
        (SELECT COUNT(*) FROM TournamentRegistrations WHERE TournamentID = t.TournamentID AND Status = 'Approved') AS RegisteredTeams,
        t.CreatedBy,
        t.CreatedAt
    FROM Tournaments t
    JOIN Games g ON t.GameID = g.GameID
    WHERE t.TournamentID = p_TournamentID;
END$$

-- Procedure: Register team for tournament
DROP PROCEDURE IF EXISTS sp_RegisterTeamForTournament$$
CREATE PROCEDURE sp_RegisterTeamForTournament(
    IN p_TournamentID INT,
    IN p_TeamID INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    -- Check if registration is still open
    SET @registrationOpen = (
        SELECT COUNT(*) 
        FROM Tournaments 
        WHERE TournamentID = p_TournamentID 
        AND Status = 'Registration' 
        AND RegistrationDeadline >= CURDATE()
    );
    
    IF @registrationOpen = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Registration is closed for this tournament';
    END IF;
    
    -- Check if max teams limit is reached
    SET @currentTeams = (
        SELECT COUNT(*) 
        FROM TournamentRegistrations 
        WHERE TournamentID = p_TournamentID 
        AND Status = 'Approved'
    );
    
    SET @maxTeams = (
        SELECT MaxTeams 
        FROM Tournaments 
        WHERE TournamentID = p_TournamentID
    );
    
    IF @currentTeams >= @maxTeams THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Tournament has reached maximum number of teams';
    END IF;
    
    -- Check if team already registered
    SET @alreadyRegistered = (
        SELECT COUNT(*) 
        FROM TournamentRegistrations 
        WHERE TournamentID = p_TournamentID 
        AND TeamID = p_TeamID
    );
    
    IF @alreadyRegistered > 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Team is already registered for this tournament';
    END IF;
    
    START TRANSACTION;
    
    -- Lấy team leader làm RegisteredBy
    SET @teamLeader = (
        SELECT CreatedBy 
        FROM Teams 
        WHERE TeamID = p_TeamID
    );
    
    INSERT INTO TournamentRegistrations (TournamentID, TeamID, RegisteredBy, RegistrationDate, Status)
    VALUES (p_TournamentID, p_TeamID, @teamLeader, NOW(), 'Approved');
    
    COMMIT;
    
    SELECT 'Team registered successfully!' as Result;
END$$

-- Procedure: Get teams registered for a tournament
DROP PROCEDURE IF EXISTS sp_GetTournamentTeams$$
CREATE PROCEDURE sp_GetTournamentTeams(IN p_TournamentID INT)
BEGIN
    SELECT 
        tr.RegistrationID,
        tr.TournamentID,
        tr.TeamID,
        t.TeamName,
        t.Description as TeamDescription,
        g.GameName,
        tr.RegisteredBy,
        u.Username as RegisteredByUsername,
        tr.RegistrationDate,
        tr.Status,
        COUNT(tm.UserID) as MemberCount
    FROM TournamentRegistrations tr
    JOIN Teams t ON tr.TeamID = t.TeamID
    JOIN Games g ON t.GameID = g.GameID
    JOIN Users u ON tr.RegisteredBy = u.UserID
    LEFT JOIN TeamMembers tm ON t.TeamID = tm.TeamID AND tm.Status = 'Active'
    WHERE tr.TournamentID = p_TournamentID
    GROUP BY tr.RegistrationID
    ORDER BY tr.RegistrationDate;
END$$

DELIMITER ;

SELECT 'Basic tournament procedures created successfully!' as Message;
