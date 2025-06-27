-- =====================================================
-- 08. ADMIN ACTIONS MODULE
-- =====================================================
-- Module: Admin Activity Logging
-- Description: Admin action logging and audit trail
-- Dependencies: 01_database_setup.sql, 03_users.sql
-- =====================================================

USE EsportsManager;

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

SELECT '08. Admin actions module created successfully!' as Message;
