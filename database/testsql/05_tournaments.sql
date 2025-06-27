-- =====================================================
-- 05. TOURNAMENTS MODULE
-- =====================================================
-- Module: Tournaments and Tournament Registrations
-- Description: Tournament management by Admin and registration by Players
-- Dependencies: 01_database_setup.sql, 02_games.sql, 03_users.sql, 04_teams.sql
-- =====================================================

USE EsportsManager;

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

SELECT '05. Tournaments module created successfully!' as Message;
