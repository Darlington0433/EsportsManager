-- =====================================================
-- 02. GAMES MODULE
-- =====================================================
-- Module: Games Management
-- Description: Games table and related functionality
-- Dependencies: 01_database_setup.sql
-- =====================================================

USE EsportsManager;

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

SELECT '02. Games module created successfully!' as Message;
