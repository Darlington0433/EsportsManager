-- =====================================================
-- COMPLETE DATABASE SETUP - THIẾT LẬP CSDL HOÀN CHỈNH
-- Bao gồm: Tạo DB, Tables, Indexes, Views, Triggers, Procedures, Sample Data
-- Hợp nhất từ: esportsmanager.sql + ALL_IN_ONE_FIX.sql
--
-- IMPORTANT: WITHDRAWAL POLICY UPDATE
-- - Withdrawals are processed IMMEDIATELY without admin approval
-- - Status is automatically set to 'Completed' upon creation
-- - No 'Pending' status or admin intervention required
-- - Wallet balance is updated automatically via triggers
-- =====================================================

-- Create database
DROP DATABASE IF EXISTS EsportsManager;
CREATE DATABASE IF NOT EXISTS EsportsManager 
CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE EsportsManager;

-- Disable safe update mode and foreign key checks temporarily
SET SQL_SAFE_UPDATES = 0;
SET FOREIGN_KEY_CHECKS = 0;

-- =====================================================
-- CORE TABLES
-- =====================================================

-- Table: Games (Game management)
CREATE TABLE IF NOT EXISTS Games (
    GameID INT AUTO_INCREMENT PRIMARY KEY,
    GameName VARCHAR(100) NOT NULL UNIQUE,
    Description TEXT,
    Genre VARCHAR(50),
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB COMMENT='Games management table';

-- Table: Users (3 roles: Admin, Player, Viewer)
CREATE TABLE IF NOT EXISTS Users (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    FullName VARCHAR(100),
    PhoneNumber VARCHAR(20),
    DisplayName VARCHAR(100),
    Role ENUM('Admin', 'Player', 'Viewer') NOT NULL DEFAULT 'Viewer',
    IsActive BOOLEAN DEFAULT FALSE,
    Status ENUM('Active', 'Suspended', 'Inactive', 'Pending', 'Deleted') NOT NULL DEFAULT 'Pending',
    IsEmailVerified BOOLEAN DEFAULT FALSE,
    EmailVerificationToken VARCHAR(255),
    PasswordResetToken VARCHAR(255),
    PasswordResetExpiry DATETIME,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP NULL ON UPDATE CURRENT_TIMESTAMP,
    LastLoginAt DATETIME,
    SecurityQuestion VARCHAR(255),
    SecurityAnswer VARCHAR(255)
) ENGINE=InnoDB COMMENT='Users table';

-- =====================================================
-- PLAYER FUNCTIONS
-- =====================================================

-- Table: Teams (Teams created and managed by Players)
CREATE TABLE IF NOT EXISTS Teams (
    TeamID INT AUTO_INCREMENT PRIMARY KEY,
    TeamName VARCHAR(100) NOT NULL UNIQUE,
    Description TEXT,
    GameID INT NOT NULL,
    CreatedBy INT NOT NULL, -- Player who creates team
    LogoURL VARCHAR(255),
    MaxMembers INT DEFAULT 5,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    IsActive BOOLEAN DEFAULT TRUE,
    Status ENUM('Active', 'Disbanded') DEFAULT 'Active',
    
    FOREIGN KEY (GameID) REFERENCES Games(GameID) ON DELETE RESTRICT,
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserID) ON DELETE RESTRICT
) ENGINE=InnoDB COMMENT='Teams table';

-- Table: TeamMembers (Team members)
CREATE TABLE IF NOT EXISTS TeamMembers (
    TeamMemberID INT AUTO_INCREMENT PRIMARY KEY,
    TeamID INT NOT NULL,
    UserID INT NOT NULL,
    JoinDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    IsLeader BOOLEAN DEFAULT FALSE,
    Position VARCHAR(50),
    Status ENUM('Active', 'Left', 'Kicked') DEFAULT 'Active',
    
    FOREIGN KEY (TeamID) REFERENCES Teams(TeamID) ON DELETE CASCADE,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    UNIQUE KEY unique_user_team (TeamID, UserID)
) ENGINE=InnoDB COMMENT='Team members table';

-- =====================================================
-- ADMIN FUNCTIONS
-- =====================================================

-- Table: Tournaments (Tournament management by Admin)
CREATE TABLE IF NOT EXISTS Tournaments (
    TournamentID INT AUTO_INCREMENT PRIMARY KEY,
    TournamentName VARCHAR(200) NOT NULL UNIQUE,
    Description TEXT,
    GameID INT NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    RegistrationDeadline DATETIME,
    MaxTeams INT DEFAULT 16,
    Format VARCHAR(50) DEFAULT 'Single Elimination',
    EntryFee DECIMAL(10,2) DEFAULT 0.00, -- Added based on ERD
    PrizePool DECIMAL(12,2) DEFAULT 0.00,
    Status ENUM('Draft', 'Registration', 'Ongoing', 'Completed', 'Cancelled') DEFAULT 'Draft',
    CreatedBy INT NOT NULL, -- Admin creates
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (GameID) REFERENCES Games(GameID) ON DELETE RESTRICT,
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserID) ON DELETE RESTRICT
) ENGINE=InnoDB COMMENT='Tournaments table';

-- Table: TournamentRegistrations (Tournament registration)
CREATE TABLE IF NOT EXISTS TournamentRegistrations (
    RegistrationID INT AUTO_INCREMENT PRIMARY KEY,
    TournamentID INT NOT NULL,
    TeamID INT NOT NULL,
    RegisteredBy INT NOT NULL, -- Player registers
    RegistrationDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Status ENUM('Registered', 'Approved', 'Rejected') DEFAULT 'Registered',
    
    FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID) ON DELETE CASCADE,
    FOREIGN KEY (TeamID) REFERENCES Teams(TeamID) ON DELETE CASCADE,
    FOREIGN KEY (RegisteredBy) REFERENCES Users(UserID) ON DELETE RESTRICT,
    UNIQUE KEY unique_tournament_team (TournamentID, TeamID)
) ENGINE=InnoDB COMMENT='Tournament registration table';

-- Table: TournamentStandings (Tournament rankings)
CREATE TABLE IF NOT EXISTS TournamentStandings (
    StandingID INT AUTO_INCREMENT PRIMARY KEY,
    TournamentID INT NOT NULL,
    TeamID INT NOT NULL,
    FinalRank INT,
    Points INT DEFAULT 0,
    Wins INT DEFAULT 0,
    Losses INT DEFAULT 0,
    PrizeMoney DECIMAL(10,2) DEFAULT 0.00,
    
    FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID) ON DELETE CASCADE,
    FOREIGN KEY (TeamID) REFERENCES Teams(TeamID) ON DELETE CASCADE,
    UNIQUE KEY unique_tournament_team_standing (TournamentID, TeamID)
) ENGINE=InnoDB COMMENT='Tournament standings table';

-- Table: TournamentResults (Detailed tournament results based on ERD)
CREATE TABLE IF NOT EXISTS TournamentResults (
    ResultID INT AUTO_INCREMENT PRIMARY KEY,
    TournamentID INT NOT NULL,
    TeamID INT NOT NULL,
    Position INT NOT NULL,
    PrizeMoney DECIMAL(12,2) DEFAULT 0.00,
    Notes TEXT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID) ON DELETE CASCADE,
    FOREIGN KEY (TeamID) REFERENCES Teams(TeamID) ON DELETE CASCADE,
    UNIQUE KEY unique_tournament_team_result (TournamentID, TeamID),
    INDEX idx_tournament_position (TournamentID, Position)
) ENGINE=InnoDB COMMENT='Tournament results table based on ERD';

-- =====================================================
-- COMMON FUNCTIONS (VIEWER/PLAYER)
-- =====================================================

-- Table: PlayerRankings (Individual rankings - viewable by Viewers)
CREATE TABLE IF NOT EXISTS PlayerRankings (
    RankingID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    GameID INT NOT NULL,
    CurrentRank INT DEFAULT 0,
    TotalPoints INT DEFAULT 0,
    TotalWins INT DEFAULT 0,
    TotalLosses INT DEFAULT 0,
    TournamentWins INT DEFAULT 0,
    LastUpdated TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (GameID) REFERENCES Games(GameID) ON DELETE CASCADE,
    UNIQUE KEY unique_user_game_ranking (UserID, GameID)
) ENGINE=InnoDB COMMENT='Player rankings table';

-- =====================================================
-- DONATION FUNCTIONS (VIEWER -> PLAYER)
-- =====================================================

-- Table: Wallets (Player wallets for receiving donations)
CREATE TABLE IF NOT EXISTS Wallets (
    WalletID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL UNIQUE,
    Balance DECIMAL(12,2) DEFAULT 0.00,
    TotalReceived DECIMAL(12,2) DEFAULT 0.00,
    TotalWithdrawn DECIMAL(12,2) DEFAULT 0.00,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    LastUpdated TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
) ENGINE=InnoDB COMMENT='Wallets table';

-- Table: Donations (Donations from Viewers to Players)
CREATE TABLE IF NOT EXISTS Donations (
    DonationID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL, -- User who donates (FromUserID)
    Amount DECIMAL(10,2) NOT NULL,
    Message TEXT,
    Status ENUM('Completed', 'Failed', 'Refunded') DEFAULT 'Completed',
    TargetType VARCHAR(50) NOT NULL, -- 'Tournament', 'Team', 'Player', etc.
    TargetID INT, -- ID of the target entity
    TransactionID INT NULL, -- Link to WalletTransactions (nullable to avoid circular dependency)
    DonationDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE RESTRICT
) ENGINE=InnoDB COMMENT='Donations table';

-- Table: Withdrawals (Money withdrawal by Players - NO ADMIN APPROVAL REQUIRED)
CREATE TABLE IF NOT EXISTS Withdrawals (
    WithdrawalID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    BankAccountNumber VARCHAR(50) NOT NULL,
    BankName VARCHAR(100) NOT NULL,
    AccountHolderName VARCHAR(100) NOT NULL,
    Status ENUM('Completed', 'Failed') DEFAULT 'Completed',
    RequestDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CompletedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    Notes TEXT,
    ReferenceCode VARCHAR(50),
    
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE RESTRICT
) ENGINE=InnoDB COMMENT='Withdrawals table - Processed immediately without admin approval';

-- =====================================================
-- VOTING/FEEDBACK FUNCTIONS
-- =====================================================

-- Table: Votes (Voting for Players/Tournaments - Viewers can vote)
CREATE TABLE IF NOT EXISTS Votes (
    VoteID INT AUTO_INCREMENT PRIMARY KEY,
    VoterID INT NOT NULL,
    VoteType ENUM('Player', 'Tournament') NOT NULL,
    TargetID INT NOT NULL,
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment TEXT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (VoterID) REFERENCES Users(UserID) ON DELETE CASCADE,
    UNIQUE KEY unique_vote (VoterID, VoteType, TargetID)
) ENGINE=InnoDB COMMENT='Votes table';

-- =====================================================
-- ADMIN MANAGEMENT FUNCTIONS
-- =====================================================

-- Table: AdminActions (Admin activity log)
CREATE TABLE IF NOT EXISTS AdminActions (
    ActionID INT AUTO_INCREMENT PRIMARY KEY,
    AdminID INT NOT NULL,
    ActionType ENUM('CreateTournament', 'UpdateTournament', 'DeleteUser', 'ProcessWithdrawal', 'ManageGame') NOT NULL,
    TargetType VARCHAR(50),
    TargetID INT,
    Description TEXT,
    ActionDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (AdminID) REFERENCES Users(UserID) ON DELETE CASCADE
) ENGINE=InnoDB COMMENT='Admin actions log table';

-- =====================================================
-- PERSONAL INFORMATION FUNCTIONS
-- =====================================================

-- Table: UserProfiles (Detailed personal information)
CREATE TABLE IF NOT EXISTS UserProfiles (
    ProfileID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL UNIQUE,
    DateOfBirth DATE,
    Gender ENUM('Male', 'Female', 'Other'),
    Address TEXT,
    Country VARCHAR(50),
    City VARCHAR(50),
    Bio TEXT,
    AvatarURL VARCHAR(255),
    SocialLinks JSON,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
) ENGINE=InnoDB COMMENT='User profiles table';

-- Table: Achievements (Player achievements and awards)
CREATE TABLE IF NOT EXISTS Achievements (
    AchievementID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    Title VARCHAR(100) NOT NULL,
    Description TEXT,
    AchievementType ENUM('Tournament Winner', 'Top 3 Finisher', 'Most Valuable Player', 'Best Team Player', 'Rising Star', 'Veteran Player', 'Fair Play Award', 'Community Champion', 'Custom') NOT NULL,
    DateAchieved DATETIME DEFAULT CURRENT_TIMESTAMP,
    AssignedBy INT NOT NULL,
    TournamentID INT NULL,
    TeamID INT NULL,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (AssignedBy) REFERENCES Users(UserID) ON DELETE RESTRICT,
    FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID) ON DELETE SET NULL,
    FOREIGN KEY (TeamID) REFERENCES Teams(TeamID) ON DELETE SET NULL,
    
    INDEX idx_achievements_user (UserID),
    INDEX idx_achievements_type (AchievementType),
    INDEX idx_achievements_date (DateAchieved)
) ENGINE=InnoDB COMMENT='Player achievements and awards table';

-- =====================================================
-- ADDITIONAL TABLES
-- =====================================================

-- Table: WalletTransactions (Based on ERD - detailed transaction history)
CREATE TABLE IF NOT EXISTS WalletTransactions (
    TransactionID INT AUTO_INCREMENT PRIMARY KEY,
    WalletID INT NOT NULL,
    UserID INT NOT NULL,
    TransactionType VARCHAR(50) NOT NULL,
    Amount DECIMAL(12,2) NOT NULL,
    BalanceAfter DECIMAL(12,2) NOT NULL,
    Status VARCHAR(20) DEFAULT 'Completed',
    ReferenceCode VARCHAR(100),
    Note TEXT,
    RelatedEntityType VARCHAR(50),
    RelatedEntityID INT,
    RelatedUserID INT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (WalletID) REFERENCES Wallets(WalletID) ON DELETE CASCADE,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (RelatedUserID) REFERENCES Users(UserID) ON DELETE SET NULL
) ENGINE=InnoDB COMMENT='Detailed wallet transaction history based on ERD';

-- Table: Feedback (Detailed feedback for tournaments)
CREATE TABLE IF NOT EXISTS Feedback (
    FeedbackID INT AUTO_INCREMENT PRIMARY KEY,
    TournamentID INT NOT NULL,
    UserID INT NOT NULL,
    Content TEXT NOT NULL,
    Rating INT NOT NULL CHECK(Rating BETWEEN 1 AND 5),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    Status ENUM('Active', 'Hidden', 'Removed') DEFAULT 'Active',
    
    FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID) ON DELETE CASCADE,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    UNIQUE KEY unique_user_tournament_feedback (UserID, TournamentID)
) ENGINE=InnoDB COMMENT='Tournament feedback table';

-- Re-enable foreign key checks
SET FOREIGN_KEY_CHECKS = 1;

SELECT 'EsportsManager database tables created successfully!' as Message;

-- =====================================================
-- CREATE INDEXES FOR PERFORMANCE
-- =====================================================

-- Indexes for Users
CREATE INDEX idx_users_role ON Users(Role);
CREATE INDEX idx_users_active ON Users(IsActive);
CREATE INDEX idx_users_email ON Users(Email);

-- Indexes for Teams
CREATE INDEX idx_teams_game ON Teams(GameID);
CREATE INDEX idx_teams_creator ON Teams(CreatedBy);
CREATE INDEX idx_teams_status ON Teams(Status);

-- Indexes for Tournaments
CREATE INDEX idx_tournaments_game ON Tournaments(GameID);
CREATE INDEX idx_tournaments_status ON Tournaments(Status);
CREATE INDEX idx_tournaments_dates ON Tournaments(StartDate, EndDate);

-- Indexes for Donations
CREATE INDEX idx_donations_user ON Donations(UserID);
CREATE INDEX idx_donations_target ON Donations(TargetType, TargetID);
CREATE INDEX idx_donations_date ON Donations(DonationDate);

-- Indexes for Votes
CREATE INDEX idx_votes_target ON Votes(VoteType, TargetID);
CREATE INDEX idx_votes_voter ON Votes(VoterID);

-- Additional indexes for TournamentResults
CREATE INDEX idx_tournament_results_tournament ON TournamentResults(TournamentID);
CREATE INDEX idx_tournament_results_team ON TournamentResults(TeamID);

-- Additional indexes for Teams (based on ERD relationships)
CREATE INDEX idx_teams_created_by ON Teams(CreatedBy);

-- Indexes for Wallet Transactions
CREATE INDEX idx_wallet_transactions_type ON WalletTransactions(TransactionType);
CREATE INDEX idx_wallet_transactions_date ON WalletTransactions(CreatedAt);

-- Additional indexes for better performance
CREATE INDEX idx_users_role_status ON Users(Role, Status);
CREATE INDEX idx_users_created ON Users(CreatedAt);
CREATE INDEX idx_tournaments_created ON Tournaments(CreatedAt);
CREATE INDEX idx_teams_created ON Teams(CreatedAt);

SELECT 'Database indexes created successfully!' as Message;

-- =====================================================
-- VIEWS FOR COMMON QUERIES
-- =====================================================

-- View: Team statistics
DROP VIEW IF EXISTS v_team_stats;

CREATE VIEW v_team_stats AS
SELECT 
    t.TeamID,
    t.TeamName,
    g.GameName,
    COUNT(tm.UserID) as MemberCount,
    u.DisplayName as LeaderName,
    t.CreatedAt
FROM Teams t
LEFT JOIN TeamMembers tm ON t.TeamID = tm.TeamID AND tm.Status = 'Active'
LEFT JOIN Games g ON t.GameID = g.GameID
LEFT JOIN Users u ON t.CreatedBy = u.UserID
GROUP BY t.TeamID;

-- View: Tournament statistics with registration count
DROP VIEW IF EXISTS v_tournament_stats;

CREATE VIEW v_tournament_stats AS
SELECT 
    t.TournamentID,
    t.TournamentName,
    g.GameName,
    COUNT(tr.TeamID) as RegisteredTeams,
    t.MaxTeams,
    t.PrizePool,
    t.Status,
    u.DisplayName as CreatedBy
FROM Tournaments t
LEFT JOIN TournamentRegistrations tr ON t.TournamentID = tr.TournamentID AND tr.Status = 'Approved'
LEFT JOIN Games g ON t.GameID = g.GameID
LEFT JOIN Users u ON t.CreatedBy = u.UserID
GROUP BY t.TournamentID;

-- View: Player statistics
DROP VIEW IF EXISTS v_player_stats;

CREATE VIEW v_player_stats AS
SELECT 
    u.UserID,
    u.Username,
    u.DisplayName,
    COUNT(DISTINCT tm.TeamID) as TeamsJoined,
    COUNT(DISTINCT tr.TournamentID) as TournamentsPlayed,
    COALESCE(w.Balance, 0.00) as WalletBalance,
    COALESCE(w.TotalReceived, 0.00) as TotalDonationsReceived
FROM Users u
LEFT JOIN TeamMembers tm ON u.UserID = tm.UserID AND tm.Status = 'Active'
LEFT JOIN TournamentRegistrations tr ON tm.TeamID = tr.TeamID AND tr.Status = 'Approved'
LEFT JOIN Wallets w ON u.UserID = w.UserID
WHERE u.Role = 'Player'
GROUP BY u.UserID;

-- View: User wallet summary for detailed wallet operations
DROP VIEW IF EXISTS v_user_wallet_summary;

CREATE VIEW v_user_wallet_summary AS
SELECT 
    u.UserID,
    u.Username,
    u.DisplayName,
    w.WalletID,
    COALESCE(w.Balance, 0.00) as Balance,
    COALESCE(w.TotalReceived, 0.00) as TotalReceived,
    COALESCE(w.TotalWithdrawn, 0.00) as TotalWithdrawn,
    COUNT(DISTINCT wt_in.TransactionID) as TotalIncomingTransactions,
    COUNT(DISTINCT wt_out.TransactionID) as TotalOutgoingTransactions,
    w.LastUpdated
FROM Users u
LEFT JOIN Wallets w ON u.UserID = w.UserID
LEFT JOIN WalletTransactions wt_in ON w.WalletID = wt_in.WalletID 
    AND wt_in.TransactionType IN ('Donation_Received', 'Prize_Money', 'Deposit')
LEFT JOIN WalletTransactions wt_out ON w.WalletID = wt_out.WalletID 
    AND wt_out.TransactionType IN ('Withdrawal')
WHERE u.Role = 'Player'
GROUP BY u.UserID, w.WalletID;

SELECT 'Database views created successfully!' as Message;

-- =====================================================
-- AUTOMATIC TRIGGERS
-- =====================================================

DELIMITER //

-- Trigger: Auto create wallet when creating player
DROP TRIGGER IF EXISTS tr_create_player_wallet//
CREATE TRIGGER tr_create_player_wallet 
AFTER INSERT ON Users
FOR EACH ROW
BEGIN
    IF NEW.Role = 'Player' THEN
        INSERT INTO Wallets (UserID, Balance) VALUES (NEW.UserID, 0.00);
    END IF;
END//

-- Trigger: Auto create profile when creating user
DROP TRIGGER IF EXISTS tr_create_user_profile//
CREATE TRIGGER tr_create_user_profile 
AFTER INSERT ON Users
FOR EACH ROW
BEGIN
    INSERT INTO UserProfiles (UserID, Bio) VALUES (NEW.UserID, 'New user');
END//

-- Trigger: Update wallet when donation is made
DROP TRIGGER IF EXISTS tr_update_wallet_on_donation//
CREATE TRIGGER tr_update_wallet_on_donation
AFTER INSERT ON Donations
FOR EACH ROW
BEGIN
    IF NEW.Status = 'Completed' AND NEW.TargetType = 'Player' THEN
        UPDATE Wallets 
        SET Balance = Balance + NEW.Amount,
            TotalReceived = TotalReceived + NEW.Amount,
            LastUpdated = CURRENT_TIMESTAMP
        WHERE UserID = NEW.TargetID;
    END IF;
END//

-- Trigger: Update wallet when withdrawal is completed (immediate processing)
DROP TRIGGER IF EXISTS tr_update_wallet_on_withdrawal//
CREATE TRIGGER tr_update_wallet_on_withdrawal
AFTER INSERT ON Withdrawals
FOR EACH ROW
BEGIN
    -- Update wallet balance immediately when withdrawal is created
    UPDATE Wallets 
    SET Balance = Balance - NEW.Amount,
        TotalWithdrawn = TotalWithdrawn + NEW.Amount,
        LastUpdated = CURRENT_TIMESTAMP
    WHERE UserID = NEW.UserID;
END//

-- Trigger: Log wallet transaction on donation
DROP TRIGGER IF EXISTS tr_log_wallet_transaction_donation//
CREATE TRIGGER tr_log_wallet_transaction_donation
AFTER INSERT ON Donations
FOR EACH ROW
BEGIN
    IF NEW.Status = 'Completed' AND NEW.TargetType = 'Player' THEN
        INSERT INTO WalletTransactions (
            WalletID, 
            UserID,
            TransactionType, 
            Amount, 
            BalanceAfter,
            Note, 
            RelatedEntityType,
            RelatedEntityID
        )
        SELECT 
            w.WalletID,
            w.UserID,
            'Donation_Received',
            NEW.Amount,
            w.Balance + NEW.Amount,
            CONCAT('Donation from user ID: ', NEW.UserID, ' to ', NEW.TargetType, ' - ', COALESCE(NEW.Message, 'No message')),
            'Donation',
            NEW.DonationID
        FROM Wallets w 
        WHERE w.UserID = NEW.TargetID;
    END IF;
END//

-- Trigger: Log wallet transaction on withdrawal (immediate processing)
DROP TRIGGER IF EXISTS tr_log_wallet_transaction_withdrawal//
CREATE TRIGGER tr_log_wallet_transaction_withdrawal
AFTER INSERT ON Withdrawals
FOR EACH ROW
BEGIN
    -- Log transaction immediately when withdrawal is created
    INSERT INTO WalletTransactions (
        WalletID, 
        UserID,
        TransactionType, 
        Amount, 
        BalanceAfter,
        Note, 
        RelatedEntityType,
        RelatedEntityID,
        ReferenceCode
    )
    SELECT 
        w.WalletID,
        w.UserID,
        'Withdrawal',
        -NEW.Amount,
        w.Balance - NEW.Amount,
        CONCAT('Withdrawal to ', NEW.BankName, ' - ', NEW.BankAccountNumber),
        'Withdrawal',
        NEW.WithdrawalID,
        NEW.ReferenceCode
    FROM Wallets w 
    WHERE w.UserID = NEW.UserID;
END//

DELIMITER ;

SELECT 'Database triggers created successfully!' as Message;

-- =====================================================
-- STORED PROCEDURES
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

-- Procedure: Get available tournaments(registration open)
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

-- Procedure: Get tournaments registered by a team
DROP PROCEDURE IF EXISTS sp_GetTeamTournaments$$
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
END$$

-- Procedure: Unregister team from tournament
DROP PROCEDURE IF EXISTS sp_UnregisterTeamFromTournament$$
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
END$$

-- Procedure: Create a new tournament
DROP PROCEDURE IF EXISTS sp_CreateTournament$$
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
END$$

-- Procedure: Update tournament information
DROP PROCEDURE IF EXISTS sp_UpdateTournament$$
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
END$$

-- Procedure: Delete tournament (soft delete)
DROP PROCEDURE IF EXISTS sp_DeleteTournament$$
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
END$$

-- Procedure: Get teams registered for a tournament
DROP PROCEDURE IF EXISTS sp_GetTournamentTeams$$
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
END$$

-- Procedure: Submit feedback for a tournament
DROP PROCEDURE IF EXISTS sp_SubmitFeedback$$
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
END$$

-- Procedure: Get feedback for a tournament
DROP PROCEDURE IF EXISTS sp_GetTournamentFeedback$$
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
END$$

-- Procedure: Get system statistics overview
DROP PROCEDURE IF EXISTS sp_GetSystemStats$$
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
END$$

-- Procedure: Get detailed user statistics
DROP PROCEDURE IF EXISTS sp_GetUserStats$$
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
END$$

-- Procedure: Get detailed tournament statistics  
DROP PROCEDURE IF EXISTS sp_GetTournamentStats$$
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
END$$

-- Procedure: Get detailed team statistics
DROP PROCEDURE IF EXISTS sp_GetTeamStats$$
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
END$$

-- Procedure: Fix missing data and ensure system integrity
DROP PROCEDURE IF EXISTS sp_FixSystemData$$
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
END$$

-- Procedure: Get donation overview
DROP PROCEDURE IF EXISTS sp_GetDonationOverview$$
CREATE PROCEDURE sp_GetDonationOverview()
BEGIN
    SELECT 
        COUNT(*) as TotalDonations,
        COUNT(DISTINCT UserID) as TotalDonators,
        COUNT(DISTINCT CASE WHEN TargetType = 'Player' THEN TargetID END) as TotalReceivers,
        SUM(Amount) as TotalAmount
    FROM Donations
    WHERE Status = 'Completed';
END$$

-- Procedure: Get donations by type
DROP PROCEDURE IF EXISTS sp_GetDonationsByType$$
CREATE PROCEDURE sp_GetDonationsByType()
BEGIN
    -- Group donations by TargetType
    SELECT 
        TargetType as DonationType,
        SUM(Amount) as Amount
    FROM Donations
    WHERE Status = 'Completed'
    GROUP BY TargetType;
END$$

-- Procedure: Get top donation receivers
DROP PROCEDURE IF EXISTS sp_GetTopDonationReceivers$$
CREATE PROCEDURE sp_GetTopDonationReceivers(
    IN p_Limit INT
)
BEGIN
    -- For Player donations, we can get user info
    SELECT 
        d.TargetID as EntityId,
        d.TargetType as EntityType,
        COALESCE(u.Username, CONCAT(d.TargetType, ' #', d.TargetID)) as EntityName,
        COUNT(*) as DonationCount,
        SUM(d.Amount) as TotalAmount,
        MIN(d.DonationDate) as FirstDonation,
        MAX(d.DonationDate) as LastDonation
    FROM Donations d
    LEFT JOIN Users u ON (d.TargetType = 'Player' AND d.TargetID = u.UserID)
    WHERE d.Status = 'Completed'
    GROUP BY d.TargetID, d.TargetType
    ORDER BY TotalAmount DESC
    LIMIT p_Limit;
END$$

-- Procedure: Get top donators
DROP PROCEDURE IF EXISTS sp_GetTopDonators$$
CREATE PROCEDURE sp_GetTopDonators(
    IN p_Limit INT
)
BEGIN
    SELECT 
        d.UserID,
        u.Username,
        COUNT(*) as DonationCount,
        SUM(d.Amount) as TotalAmount,
        MIN(d.DonationDate) as FirstDonation,
        MAX(d.DonationDate) as LastDonation
    FROM Donations d
    JOIN Users u ON d.UserID = u.UserID
    WHERE d.Status = 'Completed'
    GROUP BY d.UserID, u.Username
    ORDER BY TotalAmount DESC
    LIMIT p_Limit;
END$$

-- Procedure: Get user transaction history
DROP PROCEDURE IF EXISTS sp_GetUserTransactionHistory$$
CREATE PROCEDURE sp_GetUserTransactionHistory(
    IN p_UserID INT,
    IN p_PageNumber INT,
    IN p_PageSize INT
)
BEGIN
    DECLARE v_Offset INT DEFAULT 0;
    
    -- Calculate offset for pagination
    SET v_Offset = (p_PageNumber - 1) * p_PageSize;
    
    -- Get wallet transactions for the user
    SELECT 
        wt.TransactionID as Id,
        wt.UserID,
        u.Username,
        wt.TransactionType,
        wt.Amount,
        wt.Status,
        wt.CreatedAt,
        wt.ReferenceCode,
        wt.Note,
        wt.RelatedEntityType,
        wt.RelatedEntityID
    FROM WalletTransactions wt
    JOIN Users u ON wt.UserID = u.UserID
    WHERE wt.UserID = p_UserID
    ORDER BY wt.CreatedAt DESC
    LIMIT p_PageSize OFFSET v_Offset;
END$$

-- Procedure: Get top players by donations received
DROP PROCEDURE IF EXISTS sp_GetTopPlayersByDonations$$
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
END$$

-- Procedure: Get tournament statistics by game
DROP PROCEDURE IF EXISTS sp_GetTournamentStatsByGame$$
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
END$$

-- Procedure: Get tournament results with rankings
DROP PROCEDURE IF EXISTS sp_GetTournamentResults$$
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
END$$

-- Procedure: Add tournament result
DROP PROCEDURE IF EXISTS sp_AddTournamentResult$$
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
END$$

-- Procedure: Get detailed wallet transaction history


-- Procedure: Get team performance statistics
DROP PROCEDURE IF EXISTS sp_GetTeamPerformanceStats$$
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
END$$

-- Procedure: Get tournament leaderboard
DROP PROCEDURE IF EXISTS sp_GetTournamentLeaderboard$$
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
END$$

-- Procedure: Create withdrawal (immediate processing - no admin approval)
DROP PROCEDURE IF EXISTS sp_CreateWithdrawal$$
CREATE PROCEDURE sp_CreateWithdrawal(
    IN p_UserID INT,
    IN p_Amount DECIMAL(10,2),
    IN p_ReferenceCode VARCHAR(50),
    IN p_Note TEXT,
    IN p_BankName VARCHAR(100),
    IN p_AccountName VARCHAR(100),
    IN p_BankAccount VARCHAR(50)
)
BEGIN
    DECLARE v_WithdrawalID INT;
    DECLARE v_WalletBalance DECIMAL(10,2);
    DECLARE v_WalletID INT;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    START TRANSACTION;
    
    -- Get wallet information
    SELECT WalletID, Balance INTO v_WalletID, v_WalletBalance
    FROM Wallets 
    WHERE UserID = p_UserID;
    
    -- Check if wallet exists and has sufficient balance
    IF v_WalletID IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Wallet not found for user';
    END IF;
    
    IF v_WalletBalance < p_Amount THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Insufficient balance';
    END IF;
    
    -- Create withdrawal record (triggers will handle wallet update automatically)
    INSERT INTO Withdrawals (
        UserID,
        Amount,
        BankAccountNumber,
        BankName,
        AccountHolderName,
        Status,
        RequestDate,
        CompletedDate,
        Notes,
        ReferenceCode
    ) VALUES (
        p_UserID,
        p_Amount,
        p_BankAccount,
        p_BankName,
        p_AccountName,
        'Completed',
        NOW(),
        NOW(),
        p_Note,
        p_ReferenceCode
    );
    
    SET v_WithdrawalID = LAST_INSERT_ID();
    
    COMMIT;
    
    -- Return withdrawal details
    SELECT 
        v_WithdrawalID as TransactionID,
        p_UserID as UserID,
        p_Amount as Amount,
        'Withdrawal' as TransactionType,
        'Completed' as Status,
        p_Note as Note,
        p_ReferenceCode as ReferenceCode,
        NOW() as CreatedAt;
END$$

-- Procedure: Create deposit transaction
DROP PROCEDURE IF EXISTS sp_CreateDeposit$$
CREATE PROCEDURE sp_CreateDeposit(
    IN p_UserID INT,
    IN p_Amount DECIMAL(10,2),
    IN p_ReferenceCode VARCHAR(50),
    IN p_Note TEXT
)
BEGIN
    DECLARE v_TransactionID INT;
    DECLARE v_WalletID INT;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    START TRANSACTION;
    
    -- Get wallet ID
    SELECT WalletID INTO v_WalletID
    FROM Wallets 
    WHERE UserID = p_UserID;
    
    IF v_WalletID IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Wallet not found for user';
    END IF;
    
    -- Update wallet balance directly (since we don't have a Deposits table)
    UPDATE Wallets 
    SET Balance = Balance + p_Amount,
        TotalReceived = TotalReceived + p_Amount,
        LastUpdated = NOW()
    WHERE UserID = p_UserID;
    
    -- Create transaction record
    INSERT INTO WalletTransactions (
        WalletID,
        UserID,
        TransactionType,
        Amount,
        BalanceAfter,
        Status,
        ReferenceCode,
        Note,
        RelatedEntityType,
        CreatedAt
    ) SELECT 
        w.WalletID,
        p_UserID,
        'Deposit',
        p_Amount,
        w.Balance,
        'Completed',
        p_ReferenceCode,
        p_Note,
        'Deposit',
        NOW()
    FROM Wallets w
    WHERE w.UserID = p_UserID;
    
    SET v_TransactionID = LAST_INSERT_ID();
    
    COMMIT;
    
    -- Return transaction details
    SELECT 
        v_TransactionID as TransactionID,
        p_UserID as UserID,
        p_Amount as Amount,
        'Deposit' as TransactionType,
        'Completed' as Status,
        p_Note as Note,
        p_ReferenceCode as ReferenceCode,
        NOW() as CreatedAt;
END$$

-- Procedure: Get wallet transaction history
DROP PROCEDURE IF EXISTS sp_GetWalletTransactionHistory$$
CREATE PROCEDURE sp_GetWalletTransactionHistory(
    IN p_UserID INT,
    IN p_Limit INT
)
BEGIN
    SELECT 
        wt.TransactionID,
        wt.UserID,
        wt.TransactionType,
        wt.Amount,
        wt.BalanceAfter,
        wt.Status,
        wt.ReferenceCode,
        wt.Note,
        wt.CreatedAt
    FROM WalletTransactions wt
    JOIN Wallets w ON wt.WalletID = w.WalletID
    WHERE w.UserID = p_UserID
    ORDER BY wt.CreatedAt DESC
    LIMIT p_Limit;
END$$

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
        a.Title,
        a.Description,
        a.AchievementType,
        a.DateAchieved,
        u_admin.Username as AssignedByUsername,
        t.TournamentName,
        tm.TeamName
    FROM Achievements a
    LEFT JOIN Users u_admin ON a.AssignedBy = u_admin.UserID
    LEFT JOIN Tournaments t ON a.TournamentID = t.TournamentID
    LEFT JOIN Teams tm ON a.TeamID = tm.TeamID
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
        COUNT(DISTINCT tr.TournamentID) as TotalTournaments,
        COUNT(DISTINCT CASE WHEN tres.Position = 1 THEN tres.TournamentID END) as TournamentsWon,
        COUNT(DISTINCT CASE WHEN tres.Position <= 2 THEN tres.TournamentID END) as FinalsAppearances,
        COUNT(DISTINCT CASE WHEN tres.Position <= 4 THEN tres.TournamentID END) as SemiFinalsAppearances,
        COALESCE(SUM(tres.PrizeMoney), 0) as TotalPrizeMoney,
        COALESCE(AVG(f.Rating), 0) as AverageRating,
        COUNT(DISTINCT a.AchievementID) as TotalAchievements
    FROM Users u
    LEFT JOIN TeamMembers tm ON u.UserID = tm.UserID AND tm.Status = 'Active'
    LEFT JOIN TournamentRegistrations tr ON tm.TeamID = tr.TeamID AND tr.Status = 'Approved'
    LEFT JOIN TournamentResults tres ON tr.TeamID = tres.TeamID
    LEFT JOIN Feedback f ON u.UserID = f.UserID
    LEFT JOIN Achievements a ON u.UserID = a.UserID AND a.IsActive = TRUE
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
        tres.Position,
        CASE 
            WHEN tres.Position = 1 THEN 'Champion'
            WHEN tres.Position = 2 THEN 'Runner-up'
            WHEN tres.Position <= 4 THEN 'Semi-Final'
            WHEN tres.Position <= 8 THEN 'Quarter-Final'
            ELSE 'Participant'
        END as Result,
        COALESCE(tres.PrizeMoney, 0) as PrizeMoney
    FROM Users u
    JOIN TeamMembers tm ON u.UserID = tm.UserID
    JOIN TournamentRegistrations tr ON tm.TeamID = tr.TeamID
    JOIN Tournaments t ON tr.TournamentID = t.TournamentID
    LEFT JOIN TournamentResults tres ON tr.TeamID = tres.TeamID AND tr.TournamentID = tres.TournamentID
    WHERE u.UserID = p_UserID 
    AND tm.Status = 'Active'
    AND tr.Status = 'Approved'
    ORDER BY t.StartDate DESC;
END$$

DELIMITER ;

-- =====================================================
-- ADDITIONAL SYSTEM STATISTICS PROCEDURES
-- =====================================================

DELIMITER //

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

SELECT 'Database procedures created successfully!' as Message;

-- =====================================================
-- DATA INTEGRITY ENHANCEMENTS
-- =====================================================

-- Add constraints to ensure data integrity based on ERD
ALTER TABLE Teams ADD CONSTRAINT chk_max_members CHECK (MaxMembers > 0 AND MaxMembers <= 10);
ALTER TABLE Tournaments ADD CONSTRAINT chk_tournament_dates CHECK (StartDate < EndDate);
ALTER TABLE Tournaments ADD CONSTRAINT chk_registration_deadline CHECK (RegistrationDeadline <= StartDate);
ALTER TABLE TournamentResults ADD CONSTRAINT chk_position_positive CHECK (Position > 0);
ALTER TABLE Donations ADD CONSTRAINT chk_donation_amount CHECK (Amount > 0);
ALTER TABLE Withdrawals ADD CONSTRAINT chk_withdrawal_amount CHECK (Amount > 0);

-- Add indexes for better withdrawal performance (ignore errors if they already exist)
CREATE INDEX idx_withdrawals_user_date ON Withdrawals(UserID, RequestDate);
CREATE INDEX idx_withdrawals_status ON Withdrawals(Status);
CREATE INDEX idx_withdrawals_reference ON Withdrawals(ReferenceCode);

SELECT 'Database constraints added successfully!' as Message;

-- =====================================================
-- SAMPLE DATA INSERTION
-- =====================================================

-- Ensure safe update mode is disabled for UPDATE operations
SET SQL_SAFE_UPDATES = 0;

-- Insert sample games
INSERT INTO Games (GameName, Description, Genre) VALUES
('League of Legends', 'Most popular MOBA game in the world', 'MOBA'),
('Counter-Strike 2', 'Professional competitive FPS game', 'FPS'),
('Valorant', 'Tactical FPS game', 'FPS'),
('Dota 2', 'Professional MOBA game', 'MOBA'),
('FIFA 24', 'Football/Soccer simulation game', 'Sports'),
('Rocket League', 'Vehicular soccer game', 'Sports'),
('Overwatch 2', 'Team-based first-person shooter', 'FPS');

-- Insert admin accounts with correct BCrypt hashes
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive, Status, IsEmailVerified) VALUES
('admin', '$2a$11$9inYA.zu1eSu2CdJ3XwDMuMl95./WHUIovBSe3VsvXHtgQSCKYcaS', 'admin@esportsmanager.com', 'System Administrator', 'Admin', 'Admin', TRUE, 'Active', TRUE),
('superadmin', '$2a$11$9inYA.zu1eSu2CdJ3XwDMuMl95./WHUIovBSe3VsvXHtgQSCKYcaS', 'superadmin@esportsmanager.com', 'Super Administrator', 'SuperAdmin', 'Admin', TRUE, 'Active', TRUE);

-- Insert sample players with correct BCrypt hashes
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive, Status, IsEmailVerified) VALUES
('player1', '$2a$11$x752zckXIW/bUITiCFq7FumuhWGe8ssSoY7JNemsH5J7U6yU3KKzC', 'player1@test.com', 'Nguyen Van A', 'ProGamer1', 'Player', TRUE, 'Active', TRUE),
('player2', '$2a$11$x752zckXIW/bUITiCFq7FumuhWGe8ssSoY7JNemsH5J7U6yU3KKzC', 'player2@test.com', 'Tran Thi B', 'ProGamer2', 'Player', TRUE, 'Active', TRUE),
('player3', '$2a$11$x752zckXIW/bUITiCFq7FumuhWGe8ssSoY7JNemsH5J7U6yU3KKzC', 'player3@test.com', 'Le Van C', 'ProGamer3', 'Player', TRUE, 'Active', TRUE),
('player4', '$2a$11$x752zckXIW/bUITiCFq7FumuhWGe8ssSoY7JNemsH5J7U6yU3KKzC', 'player4@test.com', 'Do Dinh D', 'ProGamer4', 'Player', TRUE, 'Active', TRUE),
('player5', '$2a$11$x752zckXIW/bUITiCFq7FumuhWGe8ssSoY7JNemsH5J7U6yU3KKzC', 'player5@test.com', 'Pham Thi E', 'ProGamer5', 'Player', TRUE, 'Active', TRUE);

-- Insert sample viewers with correct BCrypt hashes
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive, Status, IsEmailVerified) VALUES
('viewer1', '$2a$11$u9zcIxF8UCfSjIOuaLhdZ.4lUlXiICWhdSY3uvZ/WTtPm0F0CXouW', 'viewer1@test.com', 'Hoang Van F', 'EsportsFan1', 'Viewer', TRUE, 'Active', TRUE),
('viewer2', '$2a$11$u9zcIxF8UCfSjIOuaLhdZ.4lUlXiICWhdSY3uvZ/WTtPm0F0CXouW', 'viewer2@test.com', 'Ngo Thi G', 'EsportsFan2', 'Viewer', TRUE, 'Active', TRUE),
('viewer3', '$2a$11$u9zcIxF8UCfSjIOuaLhdZ.4lUlXiICWhdSY3uvZ/WTtPm0F0CXouW', 'viewer3@test.com', 'Vuong Van H', 'EsportsFan3', 'Viewer', TRUE, 'Active', TRUE);

-- Update wallets for players (they're already created by the trigger)
UPDATE Wallets SET 
    Balance = 150000.00, 
    TotalReceived = 200000.00 
WHERE UserID = 3;

UPDATE Wallets SET 
    Balance = 120000.00, 
    TotalReceived = 150000.00 
WHERE UserID = 4;

UPDATE Wallets SET 
    Balance = 80000.00, 
    TotalReceived = 100000.00 
WHERE UserID = 5;

UPDATE Wallets SET 
    Balance = 60000.00, 
    TotalReceived = 75000.00 
WHERE UserID = 6;

UPDATE Wallets SET 
    Balance = 50000.00, 
    TotalReceived = 65000.00 
WHERE UserID = 7;

-- Create teams (from esportsmanager.sql)
INSERT INTO Teams (TeamName, Description, GameID, CreatedBy, LogoURL, MaxMembers, IsActive, Status) VALUES
('Dragons Gaming', 'Premier League of Legends team from Vietnam', 1, 3, '/images/teams/dragons.png', 5, TRUE, 'Active'),
('Phoenix Valorant', 'Professional Valorant squad', 3, 4, '/images/teams/phoenix.png', 5, TRUE, 'Active'),
('Dota Masters', 'Experienced Dota 2 team with multiple tournament wins', 4, 5, '/images/teams/dotamasters.png', 5, TRUE, 'Active'),
('Football Kings', 'FIFA 24 focused team', 5, 6, '/images/teams/footballkings.png', 3, TRUE, 'Active'),
('Rocket Stars', 'Rocket League specialists', 6, 7, '/images/teams/rocketstars.png', 3, TRUE, 'Active');

-- Additional teams from ADD_SAMPLE_DONATION.sql
INSERT IGNORE INTO Teams (TeamName, Description, GameID, CreatedBy, IsActive, Status) VALUES
('Team Alpha', 'Professional Esports Team', 1, 4, TRUE, 'Active'),
('Team Beta', 'Semi-Pro Team', 2, 5, TRUE, 'Active');

-- Add team members
INSERT INTO TeamMembers (TeamID, UserID, IsLeader, Position, Status) VALUES
(1, 3, TRUE, 'Mid Lane', 'Active'),
(1, 4, FALSE, 'Top Lane', 'Active'),
(1, 5, FALSE, 'Jungle', 'Active'),
(2, 4, TRUE, 'Duelist', 'Active'),
(2, 6, FALSE, 'Controller', 'Active'),
(3, 5, TRUE, 'Carry', 'Active'),
(3, 7, FALSE, 'Support', 'Active'),
(4, 6, TRUE, 'Forward', 'Active'),
(5, 7, TRUE, 'Striker', 'Active');

-- Create tournaments (from esportsmanager.sql) - CẬP NHẬT ĐỂ TEST ĐĂNG KÝ
INSERT INTO Tournaments (TournamentName, Description, GameID, StartDate, EndDate, RegistrationDeadline, MaxTeams, EntryFee, PrizePool, Status, CreatedBy) VALUES
('Vietnam LoL Championship 2025', 'Official League of Legends championship for Vietnam', 1, '2025-08-10', '2025-08-15', '2025-08-08', 16, 50.00, 5000.00, 'Registration', 1),
('Valorant Masters Hanoi', 'Professional Valorant tournament in Hanoi', 3, '2025-09-05', '2025-09-10', '2025-09-03', 8, 30.00, 3000.00, 'Registration', 2),
('Dota 2 Vietnam Cup', 'Premier Dota 2 event in Southeast Asia', 4, '2025-10-15', '2025-10-20', '2025-10-13', 12, 40.00, 4000.00, 'Registration', 1),
('FIFA National Tournament', 'Annual FIFA competition', 5, '2025-06-30', '2025-07-02', '2025-06-15', 32, 20.00, 2000.00, 'Ongoing', 2),
('Rocket League Showdown', 'Fast-paced Rocket League tournament', 6, '2025-07-25', '2025-07-27', '2025-07-23', 16, 25.00, 1500.00, 'Registration', 1);

-- Additional tournaments from ADD_SAMPLE_DONATION.sql
INSERT IGNORE INTO Tournaments (TournamentName, Description, GameID, StartDate, EndDate, CreatedBy, Status) VALUES
('Summer Championship 2024', 'Annual summer tournament', 1, '2024-07-01 10:00:00', '2024-07-15 18:00:00', 1, 'Completed'),
('Winter Cup 2024', 'Winter season tournament', 2, '2024-12-01 10:00:00', '2024-12-15 18:00:00', 1, 'Ongoing');

-- Register teams for tournaments (XÓA TẤT CẢ ĐỂ TEST ĐĂNG KÝ THỰC TẾ)
-- INSERT INTO TournamentRegistrations (TournamentID, TeamID, RegisteredBy, Status) VALUES
-- (3, 3, 5, 'Registered'),
-- (4, 4, 6, 'Approved'),
-- (5, 5, 7, 'Registered');

-- Để trống tất cả tournaments để test đăng ký từ đầu

-- Add some completed tournament results
INSERT INTO TournamentResults (TournamentID, TeamID, Position, PrizeMoney, Notes) VALUES
(4, 4, 1, 1000.00, 'Champion with perfect record');

-- Add some donations from viewers to players
INSERT INTO Donations (UserID, Amount, Message, Status, TargetType, TargetID) VALUES
(8, 50.00, 'Great performance in last tournament!', 'Completed', 'Player', 3),
(9, 30.00, 'You are my favorite player!', 'Completed', 'Player', 4),
(10, 25.00, 'Keep up the good work', 'Completed', 'Player', 5),
(8, 15.00, 'Amazing skills!', 'Completed', 'Player', 6),
(9, 20.00, 'Looking forward to your next match', 'Completed', 'Player', 7),
(8, 100.00, 'Amazing gameplay!', 'Completed', 'Team', 1),
(9, 30.00, 'Supporting the team!', 'Completed', 'Team', 2),
(10, 40.00, 'Love this tournament!', 'Completed', 'Tournament', 1),
(8, 15.00, 'Tournament support', 'Completed', 'Tournament', 2);

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

-- Add some admin actions for audit trail
INSERT INTO AdminActions (AdminID, ActionType, TargetType, TargetID, Description) VALUES
(1, 'CreateTournament', 'Tournament', 1, 'Created Vietnam LoL Championship 2025'),
(2, 'CreateTournament', 'Tournament', 2, 'Created Valorant Masters Hanoi'),
(1, 'CreateTournament', 'Tournament', 3, 'Created Dota 2 Vietnam Cup'),
(2, 'CreateTournament', 'Tournament', 4, 'Created FIFA National Tournament'),
(1, 'CreateTournament', 'Tournament', 5, 'Created Rocket League Showdown'),
(1, 'ManageGame', 'Game', 1, 'Added League of Legends to platform'),
(2, 'ProcessWithdrawal', 'Withdrawal', 1, 'Processed withdrawal request');

-- Add some votes/ratings
INSERT INTO Votes (VoterID, VoteType, TargetID, Rating, Comment) VALUES
(8, 'Player', 3, 5, 'Excellent player, very skilled'),
(9, 'Player', 4, 4, 'Great positioning and strategy'),
(10, 'Player', 5, 5, 'One of the best Dota 2 players I\'ve seen'),
(8, 'Tournament', 1, 5, 'Well organized tournament'),
(9, 'Tournament', 2, 4, 'Great matches but streaming had some issues');

-- Add some player rankings
INSERT INTO PlayerRankings (UserID, GameID, CurrentRank, TotalPoints, TotalWins, TotalLosses, TournamentWins) VALUES
(3, 1, 1, 2500, 48, 12, 3),
(4, 3, 2, 2200, 35, 15, 2),
(5, 4, 1, 2700, 52, 8, 4),
(6, 5, 3, 1800, 30, 20, 1),
(7, 6, 2, 2100, 40, 18, 2);

-- Add sample wallet transactions for testing
INSERT INTO WalletTransactions (WalletID, UserID, TransactionType, Amount, BalanceAfter, Status, Note, RelatedEntityType, RelatedEntityID, CreatedAt) VALUES
-- Player 3 transactions
(1, 3, 'Donation_Received', 50000.00, 50000.00, 'Completed', 'Quyên góp từ viewer', 'Donation', 1, '2025-06-01 10:00:00'),
(1, 3, 'Donation_Received', 100000.00, 150000.00, 'Completed', 'Donation từ fan', 'Donation', 2, '2025-06-15 14:30:00'),
-- Player 4 transactions  
(2, 4, 'Donation_Received', 30000.00, 30000.00, 'Completed', 'Support từ viewer', 'Donation', 3, '2025-06-10 09:15:00'),
(2, 4, 'Donation_Received', 90000.00, 120000.00, 'Completed', 'Thưởng tournament', 'Prize', 1, '2025-06-20 16:45:00'),
-- Player 5 transactions
(3, 5, 'Donation_Received', 25000.00, 25000.00, 'Completed', 'Chúc mừng chiến thắng', 'Donation', 4, '2025-06-05 11:20:00'),
(3, 5, 'Donation_Received', 55000.00, 80000.00, 'Completed', 'Keep up the good work!', 'Donation', 5, '2025-06-18 13:10:00'),
-- Player 6 transactions
(4, 6, 'Donation_Received', 15000.00, 15000.00, 'Completed', 'Amazing skills!', 'Donation', 6, '2025-06-08 15:30:00'),
(4, 6, 'Donation_Received', 45000.00, 60000.00, 'Completed', 'Tournament support', 'Donation', 7, '2025-06-22 17:20:00'),
-- Player 7 transactions
(5, 7, 'Donation_Received', 20000.00, 20000.00, 'Completed', 'Good game!', 'Donation', 8, '2025-06-12 12:40:00'),
(5, 7, 'Donation_Received', 30000.00, 50000.00, 'Completed', 'Looking forward to next match', 'Donation', 9, '2025-06-25 19:15:00');

-- Insert sample achievements for players
INSERT INTO Achievements (UserID, Title, Description, AchievementType, AssignedBy, TournamentID, DateAchieved) VALUES
(3, 'First Tournament Win', 'Won your first tournament', 'Tournament Winner', 1, 4, '2025-06-01 10:00:00'),
(3, 'Team Leader', 'Excellent leadership in Dragons Gaming', 'Best Team Player', 1, NULL, '2025-06-05 14:30:00'),
(4, 'Rising Star', 'Outstanding performance as a new player', 'Rising Star', 2, NULL, '2025-06-10 11:15:00'),
(5, 'MVP Award', 'Most Valuable Player in Dota 2 tournament', 'Most Valuable Player', 1, 3, '2025-06-15 16:20:00'),
(6, 'Fair Play Award', 'Exemplary sportsmanship and fair play', 'Fair Play Award', 2, NULL, '2025-06-20 09:45:00'),
(7, 'Community Champion', 'Active contributor to esports community', 'Community Champion', 1, NULL, '2025-06-25 13:30:00');

SELECT 'Sample data inserted successfully!' as Message;

-- =====================================================
-- FINAL TESTING AND VERIFICATION
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

-- Test if stored procedures exist
SELECT 
    'Stored Procedures Check' as Test,
    (SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema = 'EsportsManager' AND routine_name = 'sp_GetSystemStats') as sp_GetSystemStats,
    (SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema = 'EsportsManager' AND routine_name = 'sp_GetAllTournaments') as sp_GetAllTournaments,
    (SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema = 'EsportsManager' AND routine_name = 'sp_FixSystemData') as sp_FixSystemData;

-- Test the system stats procedure
SELECT 'Testing sp_GetSystemStats...' as Test;
CALL sp_GetSystemStats();

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

-- =====================================================
-- COMPREHENSIVE SYSTEM TESTING SECTION
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

-- Test additional procedures
SELECT 'Testing sp_GetUserStats...' as Test;
CALL sp_GetUserStats();

SELECT 'Testing sp_GetTournamentStats...' as Test;
CALL sp_GetTournamentStats();

SELECT 'Testing sp_GetTeamStats...' as Test;
CALL sp_GetTeamStats();

-- Test data integrity fix
SELECT 'Testing sp_FixSystemData...' as Test;
CALL sp_FixSystemData();

SELECT 'All tests completed!' as Status;

-- =====================================================
-- DEBUG QUERIES - Kiểm tra dữ liệu sau khi setup
-- =====================================================

SELECT 'DEBUG: Checking tournament registration data...' as Message;

-- Kiểm tra available tournaments
SELECT 'Available Tournaments:' as Section;
SELECT 
    TournamentID,
    TournamentName,
    Status,
    RegistrationDeadline,
    MaxTeams,
    (SELECT COUNT(*) FROM TournamentRegistrations WHERE TournamentID = t.TournamentID AND Status = 'Approved') as CurrentRegistrations
FROM Tournaments t
WHERE Status = 'Registration' AND RegistrationDeadline >= CURDATE();

-- Kiểm tra team Dragons Gaming và player1
SELECT 'Player1 Team Info:' as Section;
SELECT 
    u.UserID,
    u.Username,
    t.TeamID,
    t.TeamName,
    tm.IsLeader,
    tm.Position
FROM Users u
JOIN TeamMembers tm ON u.UserID = tm.UserID
JOIN Teams t ON tm.TeamID = t.TeamID
WHERE u.Username = 'player1' AND tm.Status = 'Active';

-- Kiểm tra current registrations
SELECT 'Current Tournament Registrations:' as Section;
SELECT 
    tr.TournamentID,
    t.TournamentName,
    tr.TeamID,
    team.TeamName,
    tr.Status
FROM TournamentRegistrations tr
JOIN Tournaments t ON tr.TournamentID = t.TournamentID
JOIN Teams team ON tr.TeamID = team.TeamID;

-- =====================================================
-- DEBUG: KIỂM TRA DỮ LIỆU SAU KHI SETUP
-- =====================================================

-- Kiểm tra tournaments có thể đăng ký
SELECT '=== TOURNAMENTS CÓ THỂ ĐĂNG KÝ ===' as Debug;
SELECT 
    TournamentID,
    TournamentName,
    Status,
    RegistrationDeadline,
    MaxTeams,
    (SELECT COUNT(*) FROM TournamentRegistrations WHERE TournamentID = t.TournamentID AND Status = 'Approved') as CurrentRegistrations
FROM Tournaments t
WHERE Status = 'Registration' 
AND RegistrationDeadline >= CURDATE()
ORDER BY TournamentID;

-- Kiểm tra team Dragons Gaming
SELECT '=== TEAM DRAGONS GAMING ===' as Debug;
SELECT 
    t.TeamID,
    t.TeamName,
    t.CreatedBy,
    COUNT(tm.UserID) as MemberCount
FROM Teams t
LEFT JOIN TeamMembers tm ON t.TeamID = tm.TeamID AND tm.Status = 'Active'
WHERE t.TeamName = 'Dragons Gaming'
GROUP BY t.TeamID, t.TeamName, t.CreatedBy;

-- Kiểm tra user player1
SELECT '=== USER PLAYER1 TEAM ===' as Debug;
SELECT 
    u.UserID,
    u.Username,
    t.TeamID,
    t.TeamName,
    tm.IsLeader
FROM Users u
JOIN TeamMembers tm ON u.UserID = tm.UserID
JOIN Teams t ON tm.TeamID = t.TeamID
WHERE u.Username = 'player1' AND tm.Status = 'Active';

-- Kiểm tra registrations hiện tại (nên trống)
SELECT '=== TOURNAMENT REGISTRATIONS (NÊN TRỐNG) ===' as Debug;
SELECT COUNT(*) as TotalRegistrations FROM TournamentRegistrations;

-- =====================================================
-- TEST FEEDBACK SYSTEM
-- =====================================================

-- Kiểm tra bảng Feedback có tồn tại không
SELECT '=== KIỂM TRA BẢNG FEEDBACK ===' as Debug;
SELECT COUNT(*) FROM information_schema.tables 
WHERE table_schema = 'EsportsManager' AND table_name = 'Feedback';

-- Kiểm tra stored procedure sp_SubmitFeedback
SELECT '=== KIỂM TRA STORED PROCEDURE ===' as Debug;
SELECT COUNT(*) FROM information_schema.routines 
WHERE routine_schema = 'EsportsManager' AND routine_name = 'sp_SubmitFeedback';

-- Test insert feedback thử nghiệm (sẽ được xóa sau)
SELECT '=== TEST FEEDBACK SUBMISSION ===' as Debug;
-- Thử gọi stored procedure với dữ liệu test
SET @test_tournament = (SELECT MIN(TournamentID) FROM Tournaments WHERE Status = 'Registration');
SET @test_player = (SELECT MIN(UserID) FROM Users WHERE Role = 'Player');

-- Hiển thị thông tin test
SELECT @test_tournament as TestTournamentID, @test_player as TestPlayerID;

-- Test stored procedure nếu có dữ liệu
-- Xóa feedback test cũ nếu có
DELETE FROM Feedback WHERE TournamentID = IFNULL(@test_tournament, 0) AND UserID = IFNULL(@test_player, 0) AND Content = 'Test feedback from database setup';

-- Test stored procedure (chỉ chạy nếu có tournament và player)
-- Sử dụng tournament ID = 1 và player ID = 3 để test
CALL sp_SubmitFeedback(1, 3, 'Test feedback from database setup', 4);

-- Kiểm tra kết quả
SELECT COUNT(*) as TestFeedbackCount FROM Feedback 
WHERE TournamentID = 1 AND UserID = 3 AND Content = 'Test feedback from database setup';

-- Xóa feedback test để giữ database sạch
DELETE FROM Feedback WHERE TournamentID = 1 AND UserID = 3 AND Content = 'Test feedback from database setup';

SELECT 'Feedback system test completed successfully!' as TestResult;

-- =====================================================
-- TEST ACHIEVEMENT SYSTEM
-- =====================================================

SELECT '=== TEST ACHIEVEMENT SYSTEM ===' as Debug;

-- Kiểm tra bảng Achievements có tồn tại không
SELECT COUNT(*) as AchievementTableExists FROM information_schema.tables 
WHERE table_schema = 'EsportsManager' AND table_name = 'Achievements';

-- Kiểm tra stored procedures
SELECT COUNT(*) as SP_AssignAchievement FROM information_schema.routines 
WHERE routine_schema = 'EsportsManager' AND routine_name = 'sp_AssignAchievement';

SELECT COUNT(*) as SP_GetPlayerAchievements FROM information_schema.routines 
WHERE routine_schema = 'EsportsManager' AND routine_name = 'sp_GetPlayerAchievements';

-- Test assign achievement
SET @test_player = (SELECT MIN(UserID) FROM Users WHERE Role = 'Player');
SET @test_admin = (SELECT MIN(UserID) FROM Users WHERE Role = 'Admin');

-- Test stored procedure để gán achievement
CALL sp_AssignAchievement(@test_player, 'Test Achievement', 'This is a test achievement', 'Custom', @test_admin, NULL, NULL);

-- Kiểm tra achievement đã được gán
SELECT COUNT(*) as TestAchievementCount FROM Achievements 
WHERE UserID = @test_player AND Title = 'Test Achievement';

-- Test lấy achievements của player
CALL sp_GetPlayerAchievements(@test_player);

-- Test player stats
CALL sp_GetPlayerStats(@test_player);

-- Xóa test achievement để giữ database sạch
DELETE FROM Achievements WHERE UserID = @test_player AND Title = 'Test Achievement';

SELECT 'Achievement system test completed successfully!' as TestResult;

-- Re-enable safe update mode
SET SQL_SAFE_UPDATES = 1;

SELECT 'COMPLETE DATABASE SETUP FINISHED SUCCESSFULLY!' as FinalMessage;
SELECT 'You can now use the EsportsManager database with all features enabled.' as Instructions;
