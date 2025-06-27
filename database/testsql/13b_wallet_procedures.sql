-- =====================================================
-- 13B. WALLET AND DONATION PROCEDURES
-- =====================================================
-- Module: Wallet and Donation Stored Procedures
-- Description: All stored procedures related to wallet operations and donations
-- Dependencies: 06_wallet_donations.sql, 03_users.sql
-- =====================================================

USE EsportsManager;

DELIMITER //

-- ------------------------------------------------------
-- Wallet Transaction Procedures
-- ------------------------------------------------------

-- Procedure: Create a deposit transaction
DROP PROCEDURE IF EXISTS sp_CreateDeposit//
CREATE PROCEDURE sp_CreateDeposit(
    IN p_UserId INT,
    IN p_Amount DECIMAL(15, 2),
    IN p_ReferenceCode VARCHAR(100),
    IN p_Note VARCHAR(255)
)
BEGIN
    DECLARE v_WalletId INT;
    DECLARE v_BalanceAfter DECIMAL(15, 2);
    DECLARE v_Username VARCHAR(50);
    DECLARE v_TransactionId INT;
    
    -- Get wallet id and username
    SELECT WalletID, Username INTO v_WalletId, v_Username 
    FROM Wallets w
    JOIN Users u ON w.UserID = u.UserID
    WHERE w.UserID = p_UserId;
    
    -- If wallet doesn't exist, create one
    IF v_WalletId IS NULL THEN
        INSERT INTO Wallets (UserID, Balance, IsActive)
        VALUES (p_UserId, 0, TRUE);
        
        SET v_WalletId = LAST_INSERT_ID();
        SELECT Username INTO v_Username FROM Users WHERE UserID = p_UserId;
    END IF;
    
    -- Update wallet balance
    UPDATE Wallets 
    SET Balance = Balance + p_Amount,
        TotalReceived = TotalReceived + p_Amount,
        LastUpdated = NOW()
    WHERE WalletID = v_WalletId;
    
    -- Get new balance
    SELECT Balance INTO v_BalanceAfter
    FROM Wallets
    WHERE WalletID = v_WalletId;
    
    -- Create transaction record
    INSERT INTO WalletTransactions (
        WalletID, UserID, TransactionType, Amount, 
        BalanceAfter, Status, ReferenceCode, Note
    )
    VALUES (
        v_WalletId, p_UserId, 'Deposit', p_Amount,
        v_BalanceAfter, 'Completed', p_ReferenceCode, p_Note
    );
    
    SET v_TransactionId = LAST_INSERT_ID();
    
    -- Return the transaction details
    SELECT 
        wt.TransactionID,
        wt.UserID,
        u.Username,
        wt.TransactionType,
        wt.Amount,
        wt.BalanceAfter,
        wt.Status,
        wt.CreatedAt,
        wt.ReferenceCode,
        wt.Note,
        wt.RelatedUserID,
        IFNULL(ru.Username, '') as RelatedUsername,
        wt.RelatedEntityID,
        wt.RelatedEntityType
    FROM WalletTransactions wt
    JOIN Users u ON wt.UserID = u.UserID
    LEFT JOIN Users ru ON wt.RelatedUserID = ru.UserID
    WHERE wt.TransactionID = v_TransactionId;
END//

-- Procedure: Create donation
DROP PROCEDURE IF EXISTS sp_CreateDonation//
CREATE PROCEDURE sp_CreateDonation(
    IN p_UserId INT,
    IN p_Amount DECIMAL(15, 2),
    IN p_ReferenceCode VARCHAR(100),
    IN p_Note VARCHAR(255),
    IN p_Message VARCHAR(255),
    IN p_EntityType VARCHAR(50),
    IN p_EntityId INT
)
BEGIN
    DECLARE v_WalletId INT;
    DECLARE v_Balance DECIMAL(15, 2);
    DECLARE v_Username VARCHAR(50);
    DECLARE v_BalanceAfter DECIMAL(15, 2);
    DECLARE v_TransactionId INT;
    DECLARE v_DonationId INT;
    
    -- Get wallet info
    SELECT w.WalletID, w.Balance, u.Username INTO v_WalletId, v_Balance, v_Username
    FROM Wallets w
    JOIN Users u ON w.UserID = u.UserID
    WHERE w.UserID = p_UserId;
    
    -- Check if wallet exists and has sufficient balance
    IF v_WalletId IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Wallet not found';
    ELSEIF v_Balance < p_Amount THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Insufficient balance';
    END IF;
    
    -- Start transaction
    START TRANSACTION;
    
    -- Update wallet balance
    UPDATE Wallets 
    SET Balance = Balance - p_Amount,
        LastUpdated = NOW()
    WHERE WalletID = v_WalletId;
    
    -- Get new balance
    SELECT Balance INTO v_BalanceAfter
    FROM Wallets
    WHERE WalletID = v_WalletId;
    
    -- Create transaction record
    INSERT INTO WalletTransactions (
        WalletID, UserID, TransactionType, Amount, 
        BalanceAfter, Status, ReferenceCode, Note,
        RelatedEntityType, RelatedEntityID
    )
    VALUES (
        v_WalletId, p_UserId, 'Donation', -p_Amount,
        v_BalanceAfter, 'Completed', p_ReferenceCode, p_Note,
        p_EntityType, p_EntityId
    );
    
    SET v_TransactionId = LAST_INSERT_ID();
    
    -- Create donation record
    INSERT INTO Donations (
        UserID, Amount, Message, Status,
        TargetType, TargetID, TransactionID
    )
    VALUES (
        p_UserId, p_Amount, p_Message, 'Completed',
        p_EntityType, p_EntityId, v_TransactionId
    );
    
    SET v_DonationId = LAST_INSERT_ID();
    
    COMMIT;
    
    -- Return the transaction details
    SELECT 
        wt.TransactionID,
        wt.UserID,
        u.Username,
        wt.TransactionType,
        wt.Amount,
        wt.BalanceAfter,
        wt.Status,
        wt.CreatedAt,
        wt.ReferenceCode,
        wt.Note,
        wt.RelatedUserID,
        IFNULL(ru.Username, '') as RelatedUsername,
        wt.RelatedEntityID,
        wt.RelatedEntityType
    FROM WalletTransactions wt
    JOIN Users u ON wt.UserID = u.UserID
    LEFT JOIN Users ru ON wt.RelatedUserID = ru.UserID
    WHERE wt.TransactionID = v_TransactionId;
END//

-- ------------------------------------------------------
-- Donation Overview Procedures
-- ------------------------------------------------------

-- Procedure: Get donation overview
DROP PROCEDURE IF EXISTS sp_GetDonationOverview//
CREATE PROCEDURE sp_GetDonationOverview()
BEGIN
    SELECT 
        COUNT(*) as TotalDonations,
        COUNT(DISTINCT UserID) as TotalDonators,
        COUNT(DISTINCT CASE WHEN TargetType = 'Player' THEN TargetID END) as TotalReceivers,
        SUM(Amount) as TotalAmount
    FROM Donations
    WHERE Status = 'Completed';
END//

-- Procedure: Get donations by type
DROP PROCEDURE IF EXISTS sp_GetDonationsByType//
CREATE PROCEDURE sp_GetDonationsByType()
BEGIN
    -- Group donations by TargetType
    SELECT 
        TargetType as DonationType,
        SUM(Amount) as Amount
    FROM Donations
    WHERE Status = 'Completed'
    GROUP BY TargetType;
END//

-- Procedure: Get top donation receivers
DROP PROCEDURE IF EXISTS sp_GetTopDonationReceivers//
CREATE PROCEDURE sp_GetTopDonationReceivers(
    IN p_Limit INT
)
BEGIN
    -- For Player donations, we can get user info
    SELECT 
        d.TargetID as EntityId,
        d.TargetType as EntityType,
        COALESCE(u.Username, CONCAT(d.TargetType, ' #', d.TargetID)) as EntityName,
        COUNT(*) as DonationCount,
        SUM(d.Amount) as TotalAmount,
        MIN(d.DonationDate) as FirstDonation,
        MAX(d.DonationDate) as LastDonation
    FROM Donations d
    LEFT JOIN Users u ON (d.TargetType = 'Player' AND d.TargetID = u.UserID)
    WHERE d.Status = 'Completed'
    GROUP BY d.TargetID, d.TargetType
    ORDER BY TotalAmount DESC
    LIMIT p_Limit;
END//

-- Procedure: Get top donators
DROP PROCEDURE IF EXISTS sp_GetTopDonators//
CREATE PROCEDURE sp_GetTopDonators(
    IN p_Limit INT
)
BEGIN
    SELECT 
        d.UserID,
        u.Username,
        COUNT(*) as DonationCount,
        SUM(d.Amount) as TotalAmount,
        MIN(d.DonationDate) as FirstDonation,
        MAX(d.DonationDate) as LastDonation
    FROM Donations d
    JOIN Users u ON d.UserID = u.UserID
    WHERE d.Status = 'Completed'
    GROUP BY d.UserID, u.Username
    ORDER BY TotalAmount DESC
    LIMIT p_Limit;
END//

-- Procedure: Get user transaction history
DROP PROCEDURE IF EXISTS sp_GetUserTransactionHistory//
CREATE PROCEDURE sp_GetUserTransactionHistory(
    IN p_UserId INT,
    IN p_PageNumber INT,
    IN p_PageSize INT,
    IN p_FromDate DATETIME,
    IN p_ToDate DATETIME,
    IN p_TransactionType VARCHAR(50)
)
BEGIN
    DECLARE v_Offset INT;
    SET v_Offset = (p_PageNumber - 1) * p_PageSize;
    
    SELECT 
        wt.TransactionID,
        wt.UserID,
        u.Username,
        wt.TransactionType,
        wt.Amount,
        wt.BalanceAfter,
        wt.Status,
        wt.CreatedAt,
        wt.ReferenceCode,
        wt.Note,
        wt.RelatedEntityType,
        wt.RelatedEntityID
    FROM WalletTransactions wt
    JOIN Users u ON wt.UserID = u.UserID
    WHERE wt.UserID = p_UserId
    AND (p_FromDate IS NULL OR wt.CreatedAt >= p_FromDate)
    AND (p_ToDate IS NULL OR wt.CreatedAt <= p_ToDate)
    AND (p_TransactionType IS NULL OR wt.TransactionType = p_TransactionType)
    ORDER BY wt.CreatedAt DESC
    LIMIT p_PageSize OFFSET v_Offset;
END//

-- Procedure: Get donation history with pagination and filtering
DROP PROCEDURE IF EXISTS sp_GetDonationHistory//
CREATE PROCEDURE sp_GetDonationHistory(
    IN p_PageNumber INT,
    IN p_PageSize INT,
    IN p_FromDate DATETIME,
    IN p_ToDate DATETIME,
    IN p_MinAmount DECIMAL(15,2),
    IN p_MaxAmount DECIMAL(15,2)
)
BEGIN
    DECLARE v_Offset INT;
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
        AND (p_FromDate IS NULL OR wt.CreatedAt >= p_FromDate)
        AND (p_ToDate IS NULL OR wt.CreatedAt <= p_ToDate)
        AND (p_MinAmount IS NULL OR ABS(wt.Amount) >= p_MinAmount)
        AND (p_MaxAmount IS NULL OR ABS(wt.Amount) <= p_MaxAmount)
    ORDER BY wt.CreatedAt DESC
    LIMIT p_PageSize OFFSET v_Offset;
END//

-- Procedure: Search donations with advanced filtering
DROP PROCEDURE IF EXISTS sp_SearchDonations//
CREATE PROCEDURE sp_SearchDonations(
    IN p_Username VARCHAR(50),
    IN p_TeamId INT,
    IN p_TournamentId INT,
    IN p_DonationType VARCHAR(20),
    IN p_MinAmount DECIMAL(15,2),
    IN p_MaxAmount DECIMAL(15,2),
    IN p_FromDate DATETIME,
    IN p_ToDate DATETIME,
    IN p_PageNumber INT,
    IN p_PageSize INT
)
BEGIN
    DECLARE v_Offset INT;
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
        AND (p_Username IS NULL OR u.Username LIKE CONCAT('%', p_Username, '%'))
        AND (p_TeamId IS NULL OR (wt.RelatedEntityType = 'Team' AND wt.RelatedEntityID = p_TeamId))
        AND (p_TournamentId IS NULL OR (wt.RelatedEntityType = 'Tournament' AND wt.RelatedEntityID = p_TournamentId))
        AND (p_DonationType IS NULL OR wt.RelatedEntityType = p_DonationType)
        AND (p_FromDate IS NULL OR wt.CreatedAt >= p_FromDate)
        AND (p_ToDate IS NULL OR wt.CreatedAt <= p_ToDate)
        AND (p_MinAmount IS NULL OR ABS(wt.Amount) >= p_MinAmount)
        AND (p_MaxAmount IS NULL OR ABS(wt.Amount) <= p_MaxAmount)
    ORDER BY wt.CreatedAt DESC
    LIMIT p_PageSize OFFSET v_Offset;
END//

DELIMITER ;

SELECT '13B. Wallet procedures created successfully!' as Message;
