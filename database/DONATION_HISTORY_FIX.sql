-- =====================================================
-- FIX DONATION HISTORY PROCEDURES
-- =====================================================

USE EsportsManager;

DELIMITER //

-- Procedure: Get donation history with advanced filtering
DROP PROCEDURE IF EXISTS sp_GetDonationHistory//
CREATE PROCEDURE sp_GetDonationHistory(
    IN p_UserId INT,
    IN p_Username VARCHAR(50),
    IN p_TeamId INT,
    IN p_TournamentId INT,
    IN p_DonationType VARCHAR(50),
    IN p_MinAmount DECIMAL(10,2),
    IN p_MaxAmount DECIMAL(10,2),
    IN p_FromDate DATETIME,
    IN p_ToDate DATETIME,
    IN p_PageNumber INT,
    IN p_PageSize INT
)
BEGIN
    DECLARE v_Offset INT DEFAULT 0;
    
    SET v_Offset = (p_PageNumber - 1) * p_PageSize;
    
    SELECT 
        wt.TransactionID as Id,
        wt.UserID,
        u.Username,
        'Donation' as TransactionType,
        ABS(wt.Amount) as Amount,
        wt.Status,
        wt.CreatedAt,
        wt.ReferenceCode,
        wt.Note,
        wt.RelatedEntityType,
        wt.RelatedEntityID
    FROM WalletTransactions wt
    JOIN Users u ON wt.UserID = u.UserID
    WHERE wt.TransactionType = 'Donation'
        AND (p_UserId IS NULL OR wt.UserID = p_UserId)
        AND (p_Username IS NULL OR u.Username LIKE CONCAT('%', p_Username, '%'))
        AND (p_TeamId IS NULL OR (wt.RelatedEntityType = 'Team' AND wt.RelatedEntityID = p_TeamId))
        AND (p_TournamentId IS NULL OR (wt.RelatedEntityType = 'Tournament' AND wt.RelatedEntityID = p_TournamentId))
        AND (p_DonationType IS NULL OR wt.RelatedEntityType = p_DonationType)
        AND (p_MinAmount IS NULL OR ABS(wt.Amount) >= p_MinAmount)
        AND (p_MaxAmount IS NULL OR ABS(wt.Amount) <= p_MaxAmount)
        AND (p_FromDate IS NULL OR wt.CreatedAt >= p_FromDate)
        AND (p_ToDate IS NULL OR wt.CreatedAt <= p_ToDate)
    ORDER BY wt.CreatedAt DESC
    LIMIT p_PageSize OFFSET v_Offset;
END//

-- Procedure: Get specific donation details by ID
DROP PROCEDURE IF EXISTS sp_GetDonationById//
CREATE PROCEDURE sp_GetDonationById(IN p_DonationId INT)
BEGIN
    SELECT 
        wt.TransactionID as Id,
        wt.UserID,
        u.Username,
        u.FullName,
        u.Email,
        'Donation' as TransactionType,
        ABS(wt.Amount) as Amount,
        wt.BalanceAfter,
        wt.Status,
        wt.CreatedAt,
        wt.ReferenceCode,
        wt.Note,
        wt.RelatedEntityType,
        wt.RelatedEntityID,
        -- Target information
        CASE 
            WHEN wt.RelatedEntityType = 'Team' THEN t.TeamName
            WHEN wt.RelatedEntityType = 'Tournament' THEN tour.TournamentName
            ELSE 'Unknown'
        END as TargetName,
        CASE 
            WHEN wt.RelatedEntityType = 'Team' THEN g1.GameName
            WHEN wt.RelatedEntityType = 'Tournament' THEN g2.GameName
            ELSE 'Unknown'
        END as GameName
    FROM WalletTransactions wt
    JOIN Users u ON wt.UserID = u.UserID
    LEFT JOIN Teams t ON (wt.RelatedEntityType = 'Team' AND wt.RelatedEntityID = t.TeamID)
    LEFT JOIN Tournaments tour ON (wt.RelatedEntityType = 'Tournament' AND wt.RelatedEntityID = tour.TournamentID)
    LEFT JOIN Games g1 ON t.GameID = g1.GameID
    LEFT JOIN Games g2 ON tour.GameID = g2.GameID
    WHERE wt.TransactionID = p_DonationId
        AND wt.TransactionType = 'Donation';
END//

-- Procedure: Get donation statistics for reports
DROP PROCEDURE IF EXISTS sp_GetDonationStats//
CREATE PROCEDURE sp_GetDonationStats()
BEGIN
    SELECT 
        COUNT(DISTINCT wt.TransactionID) as TotalDonations,
        COUNT(DISTINCT wt.UserID) as TotalDonators,
        COUNT(DISTINCT 
            CASE 
                WHEN wt.RelatedEntityType = 'Team' THEN wt.RelatedEntityID
                WHEN wt.RelatedEntityType = 'Tournament' THEN wt.RelatedEntityID
                ELSE NULL
            END
        ) as TotalReceivers,
        COALESCE(SUM(ABS(wt.Amount)), 0) as TotalDonationAmount,
        MAX(wt.CreatedAt) as LastUpdated
    FROM WalletTransactions wt
    WHERE wt.TransactionType = 'Donation'
        AND wt.Status = 'Completed';
        
    -- Get donation by type
    SELECT 
        wt.RelatedEntityType as DonationType,
        COALESCE(SUM(ABS(wt.Amount)), 0) as TotalAmount
    FROM WalletTransactions wt
    WHERE wt.TransactionType = 'Donation'
        AND wt.Status = 'Completed'
        AND wt.RelatedEntityType IS NOT NULL
    GROUP BY wt.RelatedEntityType;
END//

-- Procedure: Fix donation data integrity
DROP PROCEDURE IF EXISTS sp_FixDonationData//
CREATE PROCEDURE sp_FixDonationData()
BEGIN
    -- Create missing wallets for users who don't have them
    INSERT INTO Wallets (UserID, Balance, TotalReceived, TotalWithdrawn, IsActive)
    SELECT u.UserID, 0.00, 0.00, 0.00, TRUE
    FROM Users u
    WHERE u.Role IN ('Player', 'Viewer')
        AND NOT EXISTS (SELECT 1 FROM Wallets w WHERE w.UserID = u.UserID);
    
    -- Update wallet balances based on transactions
    UPDATE Wallets w SET 
        Balance = (
            SELECT COALESCE(SUM(wt.Amount), 0)
            FROM WalletTransactions wt 
            WHERE wt.UserID = w.UserID
                AND wt.Status = 'Completed'
        ),
        TotalReceived = (
            SELECT COALESCE(SUM(wt.Amount), 0)
            FROM WalletTransactions wt 
            WHERE wt.UserID = w.UserID
                AND wt.Amount > 0
                AND wt.Status = 'Completed'
        ),
        TotalWithdrawn = (
            SELECT COALESCE(ABS(SUM(wt.Amount)), 0)
            FROM WalletTransactions wt 
            WHERE wt.UserID = w.UserID
                AND wt.Amount < 0
                AND wt.Status = 'Completed'
        );
        
    SELECT 'Donation data integrity fixed successfully!' as Message;
END//

DELIMITER ;

-- Create some indexes for better performance
CREATE INDEX IF NOT EXISTS idx_wallettransactions_donation ON WalletTransactions(TransactionType, CreatedAt);
CREATE INDEX IF NOT EXISTS idx_wallettransactions_entity ON WalletTransactions(RelatedEntityType, RelatedEntityID);
CREATE INDEX IF NOT EXISTS idx_wallettransactions_user_date ON WalletTransactions(UserID, CreatedAt);

SELECT 'Donation history procedures created successfully!' as Message;
