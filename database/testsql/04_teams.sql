-- =====================================================
-- 04. TEAMS MODULE
-- =====================================================
-- Module: Teams and Team Members
-- Description: Team management functionality for players
-- Dependencies: 01_database_setup.sql, 02_games.sql, 03_users.sql
-- =====================================================

USE EsportsManager;

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

SELECT '04. Teams module created successfully!' as Message;
