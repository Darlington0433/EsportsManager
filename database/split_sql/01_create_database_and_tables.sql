-- =====================================================
-- 01_CREATE_DATABASE_AND_TABLES.sql
-- Tạo database và tất cả các tables cơ bản
-- Run Order: 1
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

SELECT 'Database tables created successfully!' as Message;
