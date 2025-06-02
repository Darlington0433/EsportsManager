-- Tạo database
CREATE DATABASE IF NOT EXISTS EsportManager;
USE EsportManager;

-- Bảng Games (Thêm mới cho chức năng quản lý game)
CREATE TABLE IF NOT EXISTS Games (
    GameID INT AUTO_INCREMENT PRIMARY KEY,
    GameName VARCHAR(100) NOT NULL,
    Description TEXT,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- Bảng Users (Cập nhật)
CREATE TABLE IF NOT EXISTS Users (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    FullName VARCHAR(100),
    PhoneNumber VARCHAR(20),
    DisplayName VARCHAR(100),
    Role ENUM('Admin', 'Player', 'Viewer') NOT NULL DEFAULT 'Viewer',
    IsActive BOOLEAN DEFAULT FALSE, -- Mới tạo mặc định chưa kích hoạt
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    LastLogin DATETIME,
    SecurityQuestion VARCHAR(255),
    SecurityAnswer VARCHAR(255)
) ENGINE=InnoDB;

-- Bảng Teams
CREATE TABLE IF NOT EXISTS Teams (
    TeamID INT AUTO_INCREMENT PRIMARY KEY,
    TeamName VARCHAR(100) NOT NULL,
    Description TEXT,
    GameID INT, -- Thêm liên kết với game
    CreatedBy INT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    IsActive BOOLEAN DEFAULT TRUE,
    Status ENUM('Pending', 'Approved', 'Rejected') DEFAULT 'Pending',
    FOREIGN KEY (GameID) REFERENCES Games(GameID),
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserID)
) ENGINE=InnoDB;

-- Bảng TeamMembers
CREATE TABLE IF NOT EXISTS TeamMembers (
    TeamMemberID INT AUTO_INCREMENT PRIMARY KEY,
    TeamID INT,
    UserID INT,
    JoinDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    IsLeader BOOLEAN DEFAULT FALSE,
    Status ENUM('Pending', 'Active', 'Rejected', 'Left') DEFAULT 'Pending',
    FOREIGN KEY (TeamID) REFERENCES Teams(TeamID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
) ENGINE=InnoDB;

-- Bảng Tournaments (Cập nhật thêm GameID)
CREATE TABLE IF NOT EXISTS Tournaments (
    TournamentID INT AUTO_INCREMENT PRIMARY KEY,
    TournamentName VARCHAR(200) NOT NULL,
    Description TEXT,
    GameID INT, -- Thêm liên kết với game
    StartDate DATETIME,
    EndDate DATETIME,
    EntryFee DECIMAL(18,2) DEFAULT 0,
    PrizePool DECIMAL(18,2) DEFAULT 0,
    MaxTeams INT,
    MinTeamSize INT,
    MaxTeamSize INT,
    Status ENUM('Upcoming', 'Ongoing', 'Completed', 'Cancelled') DEFAULT 'Upcoming',
    CreatedBy INT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (GameID) REFERENCES Games(GameID),
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserID)
) ENGINE=InnoDB;

-- Bảng Registrations
CREATE TABLE IF NOT EXISTS Registrations (
    RegistrationID INT AUTO_INCREMENT PRIMARY KEY,
    TournamentID INT,
    TeamID INT,
    RegistrationDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Status ENUM('Pending', 'Approved', 'Rejected') DEFAULT 'Pending',
    ApprovedBy INT,
    ApprovedAt DATETIME,
    FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID),
    FOREIGN KEY (TeamID) REFERENCES Teams(TeamID),
    FOREIGN KEY (ApprovedBy) REFERENCES Users(UserID)
) ENGINE=InnoDB;

-- Bảng TournamentResults
CREATE TABLE IF NOT EXISTS TournamentResults (
    ResultID INT AUTO_INCREMENT PRIMARY KEY,
    TournamentID INT,
    TeamID INT,
    PrizeMoney DECIMAL(18,2),
    Notes TEXT,
    FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID),
    FOREIGN KEY (TeamID) REFERENCES Teams(TeamID)
) ENGINE=InnoDB;

-- Bảng Achievements
CREATE TABLE IF NOT EXISTS Achievements (
    AchievementID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT,
    Title VARCHAR(200) NOT NULL,
    Description TEXT,
    AchievementDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    AwardedBy INT,
    Status ENUM('Pending', 'Approved', 'Rejected') DEFAULT 'Approved', -- Thêm trạng thái
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (AwardedBy) REFERENCES Users(UserID)
) ENGINE=InnoDB;

-- Bảng Feedback
CREATE TABLE IF NOT EXISTS Feedback (
    FeedbackID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT,
    TournamentID INT,
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment TEXT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID)
) ENGINE=InnoDB;

-- Bảng Wallets
CREATE TABLE IF NOT EXISTS Wallets (
    WalletID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT UNIQUE,
    Balance DECIMAL(18,2) DEFAULT 0,
    LastUpdated TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
) ENGINE=InnoDB;

-- Bảng WalletHistory
CREATE TABLE IF NOT EXISTS WalletHistory (
    HistoryID INT AUTO_INCREMENT PRIMARY KEY,
    WalletID INT,
    Amount DECIMAL(18,2) NOT NULL,
    Type ENUM('TopUp', 'Donation', 'Withdrawal', 'Prize', 'Refund') NOT NULL,
    Reference INT, -- ID của đơn hàng/transaction liên quan
    Note TEXT,
    State ENUM('Pending', 'Completed', 'Failed', 'Refunded') DEFAULT 'Pending',
    CreatedTime TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ProcessedTime DATETIME,
    FOREIGN KEY (WalletID) REFERENCES Wallets(WalletID)
) ENGINE=InnoDB;

-- Bảng Donations
CREATE TABLE IF NOT EXISTS Donations (
    DonationID INT AUTO_INCREMENT PRIMARY KEY,
    FromUserID INT,
    ToUserID INT,
    Amount DECIMAL(18,2) NOT NULL,
    Message TEXT,
    DonationDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Status ENUM('Pending', 'Completed', 'Failed', 'Refunded') DEFAULT 'Completed',
    HistoryID INT,
    FOREIGN KEY (FromUserID) REFERENCES Users(UserID),
    FOREIGN KEY (ToUserID) REFERENCES Users(UserID),
    FOREIGN KEY (HistoryID) REFERENCES WalletHistory(HistoryID)
) ENGINE=InnoDB;

-- Bảng Withdrawals
CREATE TABLE IF NOT EXISTS Withdrawals (
    WithdrawalID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT,
    Amount DECIMAL(18,2) NOT NULL,
    BankAccountNumber VARCHAR(50),
    BankName VARCHAR(100),
    AccountHolderName VARCHAR(100),
    Status ENUM('Pending', 'Approved', 'Rejected', 'Processing') DEFAULT 'Pending',
    RequestDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ProcessedBy INT,
    ProcessedAt DATETIME,
    Notes TEXT,
    HistoryID INT,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ProcessedBy) REFERENCES Users(UserID),
    FOREIGN KEY (HistoryID) REFERENCES WalletHistory(HistoryID)
) ENGINE=InnoDB;

-- Bảng RoleChangeRequests
CREATE TABLE IF NOT EXISTS RoleChangeRequests (
    RequestID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT,
    RequestedRole ENUM('Admin', 'Player', 'Viewer') NOT NULL,
    Reason TEXT,
    Status ENUM('Pending', 'Approved', 'Rejected') DEFAULT 'Pending',
    RequestDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ProcessedBy INT,
    ProcessedAt DATETIME,
    Notes TEXT,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ProcessedBy) REFERENCES Users(UserID)
) ENGINE=InnoDB;

-- Bảng Votes (Cập nhật để hỗ trợ vote cho nhiều đối tượng)
CREATE TABLE IF NOT EXISTS Votes (
    VoteID INT AUTO_INCREMENT PRIMARY KEY,
    VoterID INT,
    VoteType ENUM('Player', 'Tournament', 'Game', 'Team') NOT NULL,
    TargetID INT NOT NULL, -- ID của đối tượng được vote
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (VoterID) REFERENCES Users(UserID),
    UNIQUE KEY unique_vote (VoterID, VoteType, TargetID)
) ENGINE=InnoDB;

-- Tạo indexes cho hiệu suất
CREATE INDEX IF NOT EXISTS idx_users_email ON Users(Email);
CREATE INDEX IF NOT EXISTS idx_users_username ON Users(Username);
CREATE INDEX IF NOT EXISTS idx_teams_gamename ON Teams(TeamName);
CREATE INDEX IF NOT EXISTS idx_tournaments_gamedate ON Tournaments(GameID, StartDate);
CREATE INDEX IF NOT EXISTS idx_registrations_status ON Registrations(Status);
CREATE INDEX IF NOT EXISTS idx_wallet_history ON WalletHistory(WalletID, Type, State);