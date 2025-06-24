-- =====================================================
-- FEEDBACK TABLE (Missing from original schema)
-- =====================================================

USE EsportManager;

SET FOREIGN_KEY_CHECKS = 0;

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

SET FOREIGN_KEY_CHECKS = 1;

SELECT 'Feedback table created successfully!' as Message;
