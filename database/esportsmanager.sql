-- =====================================================
-- ESPORTS MANAGER DATABASE - BASED ON USE CASE DIAGRAM
-- =====================================================

-- Create database
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
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    LastLogin DATETIME,
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
    FromUserID INT NOT NULL, -- Viewer donates
    ToUserID INT NOT NULL,   -- Player receives
    Amount DECIMAL(10,2) NOT NULL,
    Message TEXT,
    DonationDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Status ENUM('Completed', 'Failed', 'Refunded') DEFAULT 'Completed',
    
    FOREIGN KEY (FromUserID) REFERENCES Users(UserID) ON DELETE RESTRICT,
    FOREIGN KEY (ToUserID) REFERENCES Users(UserID) ON DELETE RESTRICT
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
CREATE INDEX idx_donations_from ON Donations(FromUserID);
CREATE INDEX idx_donations_to ON Donations(ToUserID);
CREATE INDEX idx_donations_date ON Donations(DonationDate);

-- Indexes for Votes
CREATE INDEX idx_votes_target ON Votes(VoteType, TargetID);
CREATE INDEX idx_votes_voter ON Votes(VoterID);

-- =====================================================
-- VIEWS FOR COMMON QUERIES
-- =====================================================

-- View: Team statistics
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

-- =====================================================
-- SAMPLE DATA
-- =====================================================

-- Insert sample games
INSERT INTO Games (GameName, Description, Genre) VALUES
('League of Legends', 'Most popular MOBA game in the world', 'MOBA'),
('Counter-Strike 2', 'Professional competitive FPS game', 'FPS'),
('Valorant', 'Tactical FPS game', 'FPS'),
('Dota 2', 'Professional MOBA game', 'MOBA');

-- Insert sample admin
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive) VALUES
('admin', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'admin@esportmanager.com', 'System Administrator', 'Admin', 'Admin', TRUE);

-- Insert sample players
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive) VALUES
('player1', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'player1@test.com', 'Nguyen Van A', 'ProGamer1', 'Player', TRUE),
('player2', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'player2@test.com', 'Tran Thi B', 'ProGamer2', 'Player', TRUE);

-- Insert sample viewer
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive) VALUES
('viewer1', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'viewer1@test.com', 'Le Van C', 'EsportsFan1', 'Viewer', TRUE);

-- Create wallets for players
INSERT INTO Wallets (UserID, Balance) VALUES
(2, 0.00),
(3, 0.00);

-- Create profiles for users
INSERT INTO UserProfiles (UserID, Bio) VALUES
(1, 'System Administrator'),
(2, 'Professional Gamer'),
(3, 'Competitive Player'),
(4, 'Esports Fan');

-- =====================================================
-- AUTOMATIC TRIGGERS
-- =====================================================

DELIMITER //

-- Trigger: Auto create wallet when creating player
CREATE TRIGGER tr_create_player_wallet 
AFTER INSERT ON Users
FOR EACH ROW
BEGIN
    IF NEW.Role = 'Player' THEN
        INSERT INTO Wallets (UserID, Balance) VALUES (NEW.UserID, 0.00);
    END IF;
END//

-- Trigger: Auto create profile when creating user
CREATE TRIGGER tr_create_user_profile 
AFTER INSERT ON Users
FOR EACH ROW
BEGIN
    INSERT INTO UserProfiles (UserID, Bio) VALUES (NEW.UserID, 'New user');
END//

-- Trigger: Update wallet when donation is made
CREATE TRIGGER tr_update_wallet_on_donation
AFTER INSERT ON Donations
FOR EACH ROW
BEGIN
    IF NEW.Status = 'Completed' THEN
        UPDATE Wallets 
        SET Balance = Balance + NEW.Amount,
            TotalReceived = TotalReceived + NEW.Amount,
            LastUpdated = CURRENT_TIMESTAMP
        WHERE UserID = NEW.ToUserID;
    END IF;
END//

-- Trigger: Update wallet when withdrawal is completed
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

DELIMITER ;

-- Re-enable foreign key checks
SET FOREIGN_KEY_CHECKS = 1;

-- =====================================================
-- STORED PROCEDURES
-- =====================================================

DELIMITER //

-- Procedure: Get system overview statistics
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
CREATE PROCEDURE sp_GetTournamentLeaderboard(IN p_TournamentID INT)
BEGIN
    SELECT 
        ROW_NUMBER() OVER (ORDER BY tres.Position ASC) as Rank,
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

-- =====================================================
-- SYSTEM PERMISSIONS (OPTIONAL)
-- =====================================================

-- Create users for different roles
-- CREATE USER 'esport_admin'@'localhost' IDENTIFIED BY 'admin_password';
-- CREATE USER 'esport_player'@'localhost' IDENTIFIED BY 'player_password';
-- CREATE USER 'esport_viewer'@'localhost' IDENTIFIED BY 'viewer_password';

-- Grant permissions for each role
-- GRANT ALL PRIVILEGES ON EsportManager.* TO 'esport_admin'@'localhost';
-- GRANT SELECT, INSERT, UPDATE ON EsportManager.* TO 'esport_player'@'localhost';
-- GRANT SELECT ON EsportManager.* TO 'esport_viewer'@'localhost';

-- Apply permission changes
-- FLUSH PRIVILEGES;

-- =====================================================
-- SAMPLE USAGE QUERIES
-- =====================================================

-- Query 1: View system overview statistics
-- CALL sp_GetSystemStats();

-- Query 2: View top 10 players by donations received
-- CALL sp_GetTopPlayersByDonations(10);

-- Query 3: View tournament statistics for League of Legends (ID = 1)
-- CALL sp_GetTournamentStatsByGame(1);

-- Query 4: View all active teams
-- SELECT * FROM v_team_stats WHERE Status = 'Active';

-- Query 5: View player statistics
-- SELECT * FROM v_player_stats ORDER BY TotalDonationsReceived DESC LIMIT 10;

-- Query 6: View tournaments open for registration
-- SELECT * FROM v_tournament_stats WHERE Status = 'Registration';

-- =====================================================
-- END OF SCRIPT
-- =====================================================

-- Display completion message
SELECT 'EsportManager database created successfully with all features!' as Message;

-- =====================================================
-- ADDITIONAL TABLES AND MODIFICATIONS BASED ON ERD ANALYSIS
-- =====================================================

-- Table: WalletTransactions (Based on ERD - detailed transaction history)
CREATE TABLE IF NOT EXISTS WalletTransactions (
    TransactionID INT AUTO_INCREMENT PRIMARY KEY,
    WalletID INT NOT NULL,
    TransactionType ENUM('Deposit', 'Withdrawal', 'Donation_Received', 'Prize_Money', 'Refund') NOT NULL,
    Amount DECIMAL(12,2) NOT NULL,
    Description TEXT,
    ReferenceID INT, -- Reference to Donation/Withdrawal/etc
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (WalletID) REFERENCES Wallets(WalletID) ON DELETE CASCADE,
    INDEX idx_wallet_transactions_type (TransactionType),
    INDEX idx_wallet_transactions_date (CreatedAt)
) ENGINE=InnoDB COMMENT='Detailed wallet transaction history based on ERD';

-- =====================================================
-- ADDITIONAL INDEXES BASED ON ERD RELATIONSHIPS
-- =====================================================

-- Additional indexes for TournamentResults
CREATE INDEX idx_tournament_results_tournament ON TournamentResults(TournamentID);
CREATE INDEX idx_tournament_results_team ON TournamentResults(TeamID);

-- Additional indexes for Teams (based on ERD relationships)
CREATE INDEX idx_teams_created_by ON Teams(CreatedBy);

-- =====================================================
-- ENHANCED TRIGGERS FOR WALLET TRANSACTIONS
-- =====================================================

DELIMITER //

-- Trigger: Log wallet transaction on donation
CREATE TRIGGER tr_log_wallet_transaction_donation
AFTER INSERT ON Donations
FOR EACH ROW
BEGIN
    IF NEW.Status = 'Completed' THEN
        INSERT INTO WalletTransactions (
            WalletID, 
            TransactionType, 
            Amount, 
            Description, 
            ReferenceID
        )
        SELECT 
            w.WalletID,
            'Donation_Received',
            NEW.Amount,
            CONCAT('Donation from user ID: ', NEW.FromUserID, ' - ', COALESCE(NEW.Message, 'No message')),
            NEW.DonationID
        FROM Wallets w 
        WHERE w.UserID = NEW.ToUserID;
    END IF;
END//

-- Trigger: Log wallet transaction on withdrawal
CREATE TRIGGER tr_log_wallet_transaction_withdrawal
AFTER UPDATE ON Withdrawals
FOR EACH ROW
BEGIN
    IF NEW.Status = 'Completed' AND OLD.Status != 'Completed' THEN
        INSERT INTO WalletTransactions (
            WalletID, 
            TransactionType, 
            Amount, 
            Description, 
            ReferenceID
        )
        SELECT 
            w.WalletID,
            'Withdrawal',
            NEW.Amount,
            CONCAT('Withdrawal to ', NEW.BankName, ' - ', NEW.BankAccountNumber),
            NEW.WithdrawalID
        FROM Wallets w 
        WHERE w.UserID = NEW.UserID;
    END IF;
END//

DELIMITER ;

-- =====================================================
-- ADDITIONAL VIEWS BASED ON ERD ANALYSIS
-- =====================================================

-- View: Complete team information with position ranking
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

-- =====================================================
-- ADDITIONAL STORED PROCEDURES BASED ON ERD
-- =====================================================

DELIMITER //

-- Procedure: Get detailed wallet transaction history
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
CREATE PROCEDURE sp_GetTournamentLeaderboard(IN p_TournamentID INT)
BEGIN
    SELECT 
        ROW_NUMBER() OVER (ORDER BY tres.Position ASC) as Rank,
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

-- =====================================================
-- SAMPLE DATA FOR NEW TABLES
-- =====================================================

-- Create some sample wallet transactions for testing
-- (This will be populated automatically via triggers when donations/withdrawals occur)

-- =====================================================
-- ADDITIONAL COMPLETION MESSAGE
-- =====================================================

SELECT 'Database enhanced and synchronized with ERD successfully!' as Message,
       'Added WalletTransactions, enhanced views, procedures, and constraints' as Details;
