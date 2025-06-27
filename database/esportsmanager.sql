-- =====================================================
-- ESPORTS MANAGER DATABASE - BASED ON USE CASE DIAGRAM
-- =====================================================

-- Create database
DROP DATABASE IF EXISTS EsportsManager;
CREATE DATABASE IF NOT EXISTS EsportsManager 
CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE EsportsManager;

-- Disable foreign key checks temporarily
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

-- Table: Withdrawals (Money withdrawal by Players)
CREATE TABLE IF NOT EXISTS Withdrawals (
    WithdrawalID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    BankAccountNumber VARCHAR(50) NOT NULL,
    BankName VARCHAR(100) NOT NULL,
    AccountHolderName VARCHAR(100) NOT NULL,
    Status ENUM('Pending', 'Approved', 'Processing', 'Completed', 'Rejected') DEFAULT 'Pending',
    RequestDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ProcessedDate DATETIME,
    ProcessedBy INT, -- Admin processes
    Notes TEXT,
    
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE RESTRICT,
    FOREIGN KEY (ProcessedBy) REFERENCES Users(UserID) ON DELETE SET NULL
) ENGINE=InnoDB COMMENT='Withdrawals table';

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

USE EsportsManager;

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

SELECT 'Database indexes created successfully!' as Message;
-- =====================================================
-- VIEWS FOR COMMON QUERIES
-- =====================================================

USE EsportsManager;

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

-- View: Tournament results with team details
DROP VIEW IF EXISTS v_tournament_results;

CREATE VIEW v_tournament_results AS
SELECT 
    tr.ResultID,
    t.TournamentName,
    g.GameName,
    team.TeamName,
    tr.Position,
    tr.PrizeMoney,
    tr.Notes,
    t.StartDate,
    t.EndDate
FROM TournamentResults tr
JOIN Tournaments t ON tr.TournamentID = t.TournamentID
JOIN Teams team ON tr.TeamID = team.TeamID
JOIN Games g ON t.GameID = g.GameID
ORDER BY t.TournamentID, tr.Position;

-- View: Complete team information with position ranking
DROP VIEW IF EXISTS v_team_rankings;

CREATE VIEW v_team_rankings AS
SELECT 
    t.TeamID,
    t.TeamName,
    g.GameName,
    COUNT(DISTINCT tm.UserID) as ActiveMembers,
    COUNT(DISTINCT tr.TournamentID) as TournamentsParticipated,
    AVG(tres.Position) as AveragePosition,
    SUM(tres.PrizeMoney) as TotalPrizeMoneyWon,
    RANK() OVER (PARTITION BY t.GameID ORDER BY SUM(tres.PrizeMoney) DESC) as GameRanking
FROM Teams t
JOIN Games g ON t.GameID = g.GameID
LEFT JOIN TeamMembers tm ON t.TeamID = tm.TeamID AND tm.Status = 'Active'
LEFT JOIN TournamentRegistrations tr ON t.TeamID = tr.TeamID AND tr.Status = 'Approved'
LEFT JOIN TournamentResults tres ON t.TeamID = tres.TeamID
WHERE t.Status = 'Active'
GROUP BY t.TeamID, t.TeamName, g.GameName;

-- View: Enhanced user wallet summary
DROP VIEW IF EXISTS v_user_wallet_summary;

CREATE VIEW v_user_wallet_summary AS
SELECT 
    u.UserID,
    u.Username,
    u.DisplayName,
    w.Balance,
    w.TotalReceived,
    w.TotalWithdrawn,
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
GROUP BY u.UserID;

SELECT 'Database views created successfully!' as Message;

-- =====================================================
-- AUTOMATIC TRIGGERS
-- =====================================================

USE EsportsManager;

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

-- Trigger: Update wallet when withdrawal is completed
DROP TRIGGER IF EXISTS tr_update_wallet_on_withdrawal//
CREATE TRIGGER tr_update_wallet_on_withdrawal
AFTER UPDATE ON Withdrawals
FOR EACH ROW
BEGIN
    IF NEW.Status = 'Completed' AND OLD.Status != 'Completed' THEN
        UPDATE Wallets 
        SET Balance = Balance - NEW.Amount,
            TotalWithdrawn = TotalWithdrawn + NEW.Amount,
            LastUpdated = CURRENT_TIMESTAMP
        WHERE UserID = NEW.UserID;
    END IF;
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

-- Trigger: Log wallet transaction on withdrawal
DROP TRIGGER IF EXISTS tr_log_wallet_transaction_withdrawal//
CREATE TRIGGER tr_log_wallet_transaction_withdrawal
AFTER UPDATE ON Withdrawals
FOR EACH ROW
BEGIN
    IF NEW.Status = 'Completed' AND OLD.Status != 'Completed' THEN
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
            'Withdrawal',
            -NEW.Amount,
            w.Balance - NEW.Amount,
            CONCAT('Withdrawal to ', NEW.BankName, ' - ', NEW.BankAccountNumber),
            'Withdrawal',
            NEW.WithdrawalID
        FROM Wallets w 
        WHERE w.UserID = NEW.UserID;
    END IF;
END//

DELIMITER ;

SELECT 'Database triggers created successfully!' as Message;
-- =====================================================
-- STORED PROCEDURES
-- =====================================================

USE EsportsManager;

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
        wt.Note,
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
-- =====================================================
-- DATA INTEGRITY ENHANCEMENTS
-- =====================================================

USE EsportsManager;

-- Add constraints to ensure data integrity based on ERD
ALTER TABLE Teams ADD CONSTRAINT chk_max_members CHECK (MaxMembers > 0 AND MaxMembers <= 10);
ALTER TABLE Tournaments ADD CONSTRAINT chk_tournament_dates CHECK (StartDate < EndDate);
ALTER TABLE Tournaments ADD CONSTRAINT chk_registration_deadline CHECK (RegistrationDeadline <= StartDate);
ALTER TABLE TournamentResults ADD CONSTRAINT chk_position_positive CHECK (Position > 0);
ALTER TABLE Donations ADD CONSTRAINT chk_donation_amount CHECK (Amount > 0);
ALTER TABLE Withdrawals ADD CONSTRAINT chk_withdrawal_amount CHECK (Amount > 0);

SELECT 'Database constraints added successfully!' as Message;
-- =====================================================
-- SAMPLE DATA
-- =====================================================

USE EsportsManager;

-- Insert sample games
INSERT INTO Games (GameName, Description, Genre) VALUES
('League of Legends', 'Most popular MOBA game in the world', 'MOBA'),
('Counter-Strike 2', 'Professional competitive FPS game', 'FPS'),
('Valorant', 'Tactical FPS game', 'FPS'),
('Dota 2', 'Professional MOBA game', 'MOBA'),
('FIFA 24', 'Football/Soccer simulation game', 'Sports'),
('Rocket League', 'Vehicular soccer game', 'Sports'),
('Overwatch 2', 'Team-based first-person shooter', 'FPS');

-- Insert admin accounts
-- Insert admin accounts - Hash BCrypt đã test và hoạt động 100%
-- admin/admin123
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive, Status, IsEmailVerified) VALUES
('admin', '$2a$11$AT5YJeJ9yMR60/YFFsWYp.PPMe1ZFHZ.RuR6EBGJ5ZIzmCgtI3zh6', 'admin@esportsmanager.com', 'System Administrator', 'Admin', 'Admin', TRUE, 'Active', TRUE),
('superadmin', '$2a$11$AT5YJeJ9yMR60/YFFsWYp.PPMe1ZFHZ.RuR6EBGJ5ZIzmCgtI3zh6', 'superadmin@esportsmanager.com', 'Super Administrator', 'SuperAdmin', 'Admin', TRUE, 'Active', TRUE);

-- Insert sample players - Hash BCrypt đã test và hoạt động 100%
-- player1/player123, player2/player123, etc.
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive, Status, IsEmailVerified) VALUES
('player1', '$2a$11$vQx1EAmFP67.XJ1bCQgVpe7VQHxL1CgY3o9dF5I4HJy6TGHHyOmKS', 'player1@test.com', 'Nguyen Van A', 'ProGamer1', 'Player', TRUE, 'Active', TRUE),
('player2', '$2a$11$vQx1EAmFP67.XJ1bCQgVpe7VQHxL1CgY3o9dF5I4HJy6TGHHyOmKS', 'player2@test.com', 'Tran Thi B', 'ProGamer2', 'Player', TRUE, 'Active', TRUE),
('player3', '$2a$11$vQx1EAmFP67.XJ1bCQgVpe7VQHxL1CgY3o9dF5I4HJy6TGHHyOmKS', 'player3@test.com', 'Le Van C', 'ProGamer3', 'Player', TRUE, 'Active', TRUE),
('player4', '$2a$11$vQx1EAmFP67.XJ1bCQgVpe7VQHxL1CgY3o9dF5I4HJy6TGHHyOmKS', 'player4@test.com', 'Do Dinh D', 'ProGamer4', 'Player', TRUE, 'Active', TRUE),
('player5', '$2a$11$vQx1EAmFP67.XJ1bCQgVpe7VQHxL1CgY3o9dF5I4HJy6TGHHyOmKS', 'player5@test.com', 'Pham Thi E', 'ProGamer5', 'Player', TRUE, 'Active', TRUE);

-- Insert sample viewers - Hash BCrypt đã test và hoạt động 100%
-- viewer1/viewer123, viewer2/viewer123, etc.
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive, Status, IsEmailVerified) VALUES
('viewer1', '$2a$11$mOBCKR5/l5EG5EYh5sPhb.sYgtbdO3eXGJQ5k8I8k8SnVGLzJmq2e', 'viewer1@test.com', 'Hoang Van F', 'EsportsFan1', 'Viewer', TRUE, 'Active', TRUE),
('viewer2', '$2a$11$mOBCKR5/l5EG5EYh5sPhb.sYgtbdO3eXGJQ5k8I8k8SnVGLzJmq2e', 'viewer2@test.com', 'Ngo Thi G', 'EsportsFan2', 'Viewer', TRUE, 'Active', TRUE),
('viewer3', '$2a$11$mOBCKR5/l5EG5EYh5sPhb.sYgtbdO3eXGJQ5k8I8k8SnVGLzJmq2e', 'viewer3@test.com', 'Vuong Van H', 'EsportsFan3', 'Viewer', TRUE, 'Active', TRUE);

-- Update wallets for players (they're already created by the trigger)
UPDATE Wallets SET 
    Balance = 1000.00, 
    TotalReceived = 1500.00 
WHERE UserID = 3;

UPDATE Wallets SET 
    Balance = 750.00, 
    TotalReceived = 1000.00 
WHERE UserID = 4;

UPDATE Wallets SET 
    Balance = 500.00, 
    TotalReceived = 750.00 
WHERE UserID = 5;

UPDATE Wallets SET 
    Balance = 250.00, 
    TotalReceived = 500.00 
WHERE UserID = 6;

UPDATE Wallets SET 
    Balance = 100.00, 
    TotalReceived = 200.00 
WHERE UserID = 7;

-- Update profiles for users with more detailed information (they're already created by the trigger)
UPDATE UserProfiles SET
    DateOfBirth = '1990-01-15',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Ho Chi Minh City',
    Bio = 'System Administrator',
    AvatarURL = '/images/avatars/admin.png'
WHERE UserID = 1;

UPDATE UserProfiles SET
    DateOfBirth = '1988-05-20',
    Gender = 'Female',
    Country = 'Vietnam',
    City = 'Hanoi',
    Bio = 'Super Administrator',
    AvatarURL = '/images/avatars/superadmin.png'
WHERE UserID = 2;

UPDATE UserProfiles SET
    DateOfBirth = '1995-03-24',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Hanoi',
    Bio = 'Professional League of Legends player with 5 years experience',
    AvatarURL = '/images/avatars/player1.png'
WHERE UserID = 3;

UPDATE UserProfiles SET
    DateOfBirth = '1997-07-12',
    Gender = 'Female',
    Country = 'Vietnam',
    City = 'Ho Chi Minh City',
    Bio = 'Former CS:GO champion, now playing Valorant professionally',
    AvatarURL = '/images/avatars/player2.png'
WHERE UserID = 4;

UPDATE UserProfiles SET
    DateOfBirth = '1998-11-03',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Da Nang',
    Bio = 'Dota 2 specialist, twice finalist in national tournaments',
    AvatarURL = '/images/avatars/player3.png'
WHERE UserID = 5;

UPDATE UserProfiles SET
    DateOfBirth = '1996-08-18',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Can Tho',
    Bio = 'FIFA professional player, represented Vietnam in Asia Cup',
    AvatarURL = '/images/avatars/player4.png'
WHERE UserID = 6;

UPDATE UserProfiles SET
    DateOfBirth = '1999-02-27',
    Gender = 'Female',
    Country = 'Vietnam',
    City = 'Hai Phong',
    Bio = 'Rocket League champion, streaming on Twitch',
    AvatarURL = '/images/avatars/player5.png'
WHERE UserID = 7;

UPDATE UserProfiles SET
    DateOfBirth = '1994-04-15',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Ho Chi Minh City',
    Bio = 'Esports enthusiast and supporter',
    AvatarURL = '/images/avatars/viewer1.png'
WHERE UserID = 8;

UPDATE UserProfiles SET
    DateOfBirth = '1992-09-22',
    Gender = 'Female',
    Country = 'Vietnam',
    City = 'Hanoi',
    Bio = 'Regular tournament viewer and commentator',
    AvatarURL = '/images/avatars/viewer2.png'
WHERE UserID = 9;

UPDATE UserProfiles SET
    DateOfBirth = '1997-12-05',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Da Nang',
    Bio = 'Esports blogger and fan',
    AvatarURL = '/images/avatars/viewer3.png'
WHERE UserID = 10;

-- Create teams
INSERT INTO Teams (TeamName, Description, GameID, CreatedBy, LogoURL, MaxMembers, IsActive, Status) VALUES
('Dragons Gaming', 'Premier League of Legends team from Vietnam', 1, 3, '/images/teams/dragons.png', 5, TRUE, 'Active'),
('Phoenix Valorant', 'Professional Valorant squad', 3, 4, '/images/teams/phoenix.png', 5, TRUE, 'Active'),
('Dota Masters', 'Experienced Dota 2 team with multiple tournament wins', 4, 5, '/images/teams/dotamasters.png', 5, TRUE, 'Active'),
('Football Kings', 'FIFA 24 focused team', 5, 6, '/images/teams/footballkings.png', 3, TRUE, 'Active'),
('Rocket Stars', 'Rocket League specialists', 6, 7, '/images/teams/rocketstars.png', 3, TRUE, 'Active');

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

-- Create tournaments
INSERT INTO Tournaments (TournamentName, Description, GameID, StartDate, EndDate, RegistrationDeadline, MaxTeams, EntryFee, PrizePool, Status, CreatedBy) VALUES
('Vietnam LoL Championship 2025', 'Official League of Legends championship for Vietnam', 1, '2025-07-10', '2025-07-15', '2025-07-01', 16, 50.00, 5000.00, 'Registration', 1),
('Valorant Masters Hanoi', 'Professional Valorant tournament in Hanoi', 3, '2025-08-05', '2025-08-10', '2025-07-25', 8, 30.00, 3000.00, 'Registration', 2),
('Dota 2 Vietnam Cup', 'Premier Dota 2 event in Southeast Asia', 4, '2025-09-15', '2025-09-20', '2025-09-01', 12, 40.00, 4000.00, 'Draft', 1),
('FIFA National Tournament', 'Annual FIFA competition', 5, '2025-06-30', '2025-07-02', '2025-06-15', 32, 20.00, 2000.00, 'Ongoing', 2),
('Rocket League Showdown', 'Fast-paced Rocket League tournament', 6, '2025-07-25', '2025-07-27', '2025-07-10', 16, 25.00, 1500.00, 'Registration', 1);

-- Register teams for tournaments
INSERT INTO TournamentRegistrations (TournamentID, TeamID, RegisteredBy, Status) VALUES
(1, 1, 3, 'Approved'),
(2, 2, 4, 'Approved'),
(3, 3, 5, 'Registered'),
(4, 4, 6, 'Approved'),
(5, 5, 7, 'Registered');

-- Add some completed tournament results
INSERT INTO TournamentResults (TournamentID, TeamID, Position, PrizeMoney, Notes) VALUES
(4, 4, 1, 1000.00, 'Champion with perfect record');

-- Add some donations from viewers to players
INSERT INTO Donations (UserID, Amount, Message, Status, TargetType, TargetID) VALUES
(8, 50.00, 'Great performance in last tournament!', 'Completed', 'Player', 3),
(9, 30.00, 'You are my favorite player!', 'Completed', 'Player', 4),
(10, 25.00, 'Keep up the good work', 'Completed', 'Player', 5),
(8, 15.00, 'Amazing skills!', 'Completed', 'Player', 6),
(9, 20.00, 'Looking forward to your next match', 'Completed', 'Player', 7);

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

SELECT 'Sample data inserted successfully!' as Message;
-- =====================================================
-- TOURNAMENT-RELATED STORED PROCEDURES
-- =====================================================

USE EsportsManager;

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
    
    INSERT INTO TournamentRegistrations (TournamentID, TeamID, RegistrationDate, Status)
    VALUES (p_TournamentID, p_TeamID, NOW(), 'Approved');
    
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

DELIMITER ;

SELECT 'Tournament procedures created successfully!' as Message;
-- =====================================================
-- DONATION PROCEDURES
-- =====================================================

DELIMITER $$

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

DELIMITER ;

SELECT 'Donation procedures created successfully!' as Message;
