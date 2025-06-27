-- =====================================================
-- 07. VOTES AND FEEDBACK MODULE
-- =====================================================
-- Module: Voting and Feedback System
-- Description: Voting for players/tournaments and feedback system
-- Dependencies: 01_database_setup.sql, 03_users.sql, 05_tournaments.sql
-- =====================================================

USE EsportsManager;

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

SELECT '07. Votes and feedback module created successfully!' as Message;
