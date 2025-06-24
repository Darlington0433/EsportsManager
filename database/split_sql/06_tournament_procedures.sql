-- =====================================================
-- TOURNAMENT-RELATED STORED PROCEDURES
-- =====================================================

USE EsportManager;

-- Procedure: Get all tournaments
DROP PROCEDURE IF EXISTS sp_GetAllTournaments//
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
END//

-- Procedure: Get available tournaments (registration open)
DROP PROCEDURE IF EXISTS sp_GetAvailableTournaments//
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
END//

-- Procedure: Get tournament by ID
DROP PROCEDURE IF EXISTS sp_GetTournamentById//
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
END//

-- Procedure: Get tournaments registered by a team
DROP PROCEDURE IF EXISTS sp_GetTeamTournaments//
CREATE PROCEDURE sp_GetTeamTournaments(IN p_TeamID INT)
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
    JOIN TournamentRegistrations tr ON t.TournamentID = tr.TournamentID
    WHERE tr.TeamID = p_TeamID
    ORDER BY t.StartDate;
END//

-- Procedure: Register team for tournament
DROP PROCEDURE IF EXISTS sp_RegisterTeamForTournament//
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
    
    INSERT INTO TournamentRegistrations (TournamentID, TeamID, RegistrationDate, Status)
    VALUES (p_TournamentID, p_TeamID, NOW(), 'Approved');
    
    COMMIT;
END//

-- Procedure: Unregister team from tournament
DROP PROCEDURE IF EXISTS sp_UnregisterTeamFromTournament//
CREATE PROCEDURE sp_UnregisterTeamFromTournament(
    IN p_TournamentID INT,
    IN p_TeamID INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    -- Check if tournament hasn't started yet
    SET @canUnregister = (
        SELECT COUNT(*) 
        FROM Tournaments 
        WHERE TournamentID = p_TournamentID 
        AND StartDate > CURDATE()
    );
    
    IF @canUnregister = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Cannot unregister from a tournament that has already started';
    END IF;
    
    START TRANSACTION;
    
    -- Delete registration
    DELETE FROM TournamentRegistrations 
    WHERE TournamentID = p_TournamentID 
    AND TeamID = p_TeamID;
    
    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Team is not registered for this tournament';
    END IF;
    
    COMMIT;
END//

-- Procedure: Create a new tournament
DROP PROCEDURE IF EXISTS sp_CreateTournament//
CREATE PROCEDURE sp_CreateTournament(
    IN p_TournamentName VARCHAR(100),
    IN p_Description TEXT,
    IN p_GameID INT,
    IN p_StartDate DATETIME,
    IN p_EndDate DATETIME,
    IN p_RegistrationDeadline DATETIME,
    IN p_MaxTeams INT,
    IN p_EntryFee DECIMAL(12,2),
    IN p_PrizePool DECIMAL(12,2),
    IN p_CreatedBy INT
)
BEGIN
    DECLARE v_TournamentID INT;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    START TRANSACTION;
    
    INSERT INTO Tournaments (
        TournamentName, 
        Description, 
        GameID, 
        StartDate, 
        EndDate, 
        RegistrationDeadline, 
        MaxTeams, 
        EntryFee, 
        PrizePool, 
        Status, 
        CreatedBy, 
        CreatedAt
    )
    VALUES (
        p_TournamentName,
        p_Description,
        p_GameID,
        p_StartDate,
        p_EndDate,
        p_RegistrationDeadline,
        p_MaxTeams,
        p_EntryFee,
        p_PrizePool,
        'Draft',
        p_CreatedBy,
        NOW()
    );
    
    SET v_TournamentID = LAST_INSERT_ID();
    
    -- Return the created tournament
    SELECT 
        v_TournamentID AS TournamentID,
        g.GameName
    FROM Games g
    WHERE g.GameID = p_GameID;
    
    COMMIT;
END//

-- Procedure: Update tournament information
DROP PROCEDURE IF EXISTS sp_UpdateTournament//
CREATE PROCEDURE sp_UpdateTournament(
    IN p_TournamentID INT,
    IN p_TournamentName VARCHAR(100),
    IN p_Description TEXT,
    IN p_StartDate DATETIME,
    IN p_EndDate DATETIME,
    IN p_RegistrationDeadline DATETIME,
    IN p_MaxTeams INT,
    IN p_EntryFee DECIMAL(12,2),
    IN p_PrizePool DECIMAL(12,2),
    IN p_Status VARCHAR(20)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    START TRANSACTION;
    
    -- Check if tournament exists
    IF NOT EXISTS (SELECT 1 FROM Tournaments WHERE TournamentID = p_TournamentID) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Tournament not found';
    END IF;
    
    UPDATE Tournaments
    SET 
        TournamentName = p_TournamentName,
        Description = p_Description,
        StartDate = p_StartDate,
        EndDate = p_EndDate,
        RegistrationDeadline = p_RegistrationDeadline,
        MaxTeams = p_MaxTeams,
        EntryFee = p_EntryFee,
        PrizePool = p_PrizePool,
        Status = p_Status,
        UpdatedAt = NOW()
    WHERE TournamentID = p_TournamentID;
    
    COMMIT;
END//

-- Procedure: Delete tournament (soft delete)
DROP PROCEDURE IF EXISTS sp_DeleteTournament//
CREATE PROCEDURE sp_DeleteTournament(
    IN p_TournamentID INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    -- Check if tournament exists
    IF NOT EXISTS (SELECT 1 FROM Tournaments WHERE TournamentID = p_TournamentID) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Tournament not found';
    END IF;
    
    -- Check if tournament has any registrations
    IF EXISTS (SELECT 1 FROM TournamentRegistrations WHERE TournamentID = p_TournamentID) THEN
        -- Soft delete - mark as cancelled
        UPDATE Tournaments
        SET Status = 'Cancelled', UpdatedAt = NOW()
        WHERE TournamentID = p_TournamentID;
    ELSE
        -- Hard delete if no registrations
        DELETE FROM Tournaments WHERE TournamentID = p_TournamentID;
    END IF;
END//

-- Procedure: Get teams registered for a tournament
DROP PROCEDURE IF EXISTS sp_GetTournamentTeams//
CREATE PROCEDURE sp_GetTournamentTeams(
    IN p_TournamentID INT
)
BEGIN
    SELECT 
        t.TeamID,
        t.TeamName,
        t.Description,
        t.LogoURL,
        t.CreatedBy AS TeamLeaderID,
        u.DisplayName AS TeamLeaderName,
        (SELECT COUNT(*) FROM TeamMembers WHERE TeamID = t.TeamID AND Status = 'Active') AS MemberCount,
        tr.Status AS RegistrationStatus
    FROM Teams t
    JOIN TournamentRegistrations tr ON t.TeamID = tr.TeamID
    JOIN Users u ON t.CreatedBy = u.UserID
    WHERE tr.TournamentID = p_TournamentID
    ORDER BY tr.RegistrationDate;
END//

-- Procedure: Submit feedback for a tournament
DROP PROCEDURE IF EXISTS sp_SubmitFeedback//
CREATE PROCEDURE sp_SubmitFeedback(
    IN p_TournamentID INT,
    IN p_UserID INT,
    IN p_Content TEXT,
    IN p_Rating INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    -- Check if tournament exists
    IF NOT EXISTS (SELECT 1 FROM Tournaments WHERE TournamentID = p_TournamentID) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Tournament not found';
    END IF;
    
    START TRANSACTION;
    
    -- Check if user has already submitted feedback
    IF EXISTS (SELECT 1 FROM Feedback WHERE TournamentID = p_TournamentID AND UserID = p_UserID) THEN
        -- Update existing feedback
        UPDATE Feedback
        SET Content = p_Content, Rating = p_Rating, UpdatedAt = NOW()
        WHERE TournamentID = p_TournamentID AND UserID = p_UserID;
    ELSE
        -- Insert new feedback
        INSERT INTO Feedback (TournamentID, UserID, Content, Rating, CreatedAt, Status)
        VALUES (p_TournamentID, p_UserID, p_Content, p_Rating, NOW(), 'Active');
    END IF;
    
    COMMIT;
END//

-- Procedure: Get feedback for a tournament
DROP PROCEDURE IF EXISTS sp_GetTournamentFeedback//
CREATE PROCEDURE sp_GetTournamentFeedback(
    IN p_TournamentID INT
)
BEGIN
    SELECT 
        f.FeedbackID,
        f.TournamentID,
        f.UserID,
        u.Username AS UserName,
        f.Content,
        f.Rating,
        f.CreatedAt,
        f.Status
    FROM Feedback f
    JOIN Users u ON f.UserID = u.UserID
    WHERE f.TournamentID = p_TournamentID AND f.Status = 'Active'
    ORDER BY f.CreatedAt DESC;
END//

DELIMITER ;

SELECT 'Tournament procedures created successfully!' as Message;
