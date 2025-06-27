-- =====================================================
-- 09. RANKINGS AND RESULTS MODULE
-- =====================================================
-- Module: Player Rankings and Tournament Results
-- Description: Individual player rankings and tournament results/standings
-- Dependencies: 01_database_setup.sql, 02_games.sql, 03_users.sql, 04_teams.sql, 05_tournaments.sql
-- =====================================================

USE EsportsManager;

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

SELECT '09. Rankings and results module created successfully!' as Message;
