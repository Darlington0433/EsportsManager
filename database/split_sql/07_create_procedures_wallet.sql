-- =====================================================
-- 07_CREATE_PROCEDURES_WALLET.sql
-- Tạo các stored procedures cho wallet system
-- Run Order: 7
-- Prerequisites: 01-06 (all previous files)
-- =====================================================

USE EsportsManager;

-- =====================================================
-- WALLET & TRANSACTION PROCEDURES
-- =====================================================

DELIMITER $$

-- Procedure: Create withdrawal (immediate processing - no admin approval)
DROP PROCEDURE IF EXISTS sp_CreateWithdrawal$$
CREATE PROCEDURE sp_CreateWithdrawal(
    IN p_UserID INT,
    IN p_Amount DECIMAL(10,2),
    IN p_BankAccountNumber VARCHAR(50),
    IN p_BankName VARCHAR(100),
    IN p_AccountHolderName VARCHAR(100),
    IN p_Notes TEXT,
    IN p_ReferenceCode VARCHAR(50)
)
BEGIN
    DECLARE v_WithdrawalID INT;
    DECLARE v_CurrentBalance DECIMAL(12,2);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    START TRANSACTION;
    
    -- Check current balance
    SELECT Balance INTO v_CurrentBalance 
    FROM Wallets 
    WHERE UserID = p_UserID;
    
    IF v_CurrentBalance < p_Amount THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Insufficient balance for withdrawal';
    END IF;
    
    -- Create withdrawal record (automatically processed)
    INSERT INTO Withdrawals (
        UserID, Amount, BankAccountNumber, BankName, 
        AccountHolderName, Status, RequestDate, CompletedDate, Notes, ReferenceCode
    ) VALUES (
        p_UserID, p_Amount, p_BankAccountNumber, p_BankName,
        p_AccountHolderName, 'Completed', NOW(), NOW(), p_Notes, p_ReferenceCode
    );
    
    SET v_WithdrawalID = LAST_INSERT_ID();
    
    COMMIT;
    
    -- Return withdrawal details
    SELECT 
        v_WithdrawalID as WithdrawalID,
        p_UserID as UserID,
        p_Amount as Amount,
        'Completed' as Status,
        p_BankName as BankName,
        p_ReferenceCode as ReferenceCode,
        NOW() as CompletedDate;
END$$

-- Procedure: Create deposit transaction
DROP PROCEDURE IF EXISTS sp_CreateDeposit$$
CREATE PROCEDURE sp_CreateDeposit(
    IN p_UserID INT,
    IN p_Amount DECIMAL(10,2),
    IN p_ReferenceCode VARCHAR(100),
    IN p_Note TEXT
)
BEGIN
    DECLARE v_TransactionID INT;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    START TRANSACTION;
    
    -- Update wallet balance
    UPDATE Wallets 
    SET Balance = Balance + p_Amount,
        TotalReceived = TotalReceived + p_Amount,
        LastUpdated = NOW()
    WHERE UserID = p_UserID;
    
    -- Create transaction record
    INSERT INTO WalletTransactions (
        WalletID,
        UserID,
        TransactionType,
        Amount,
        BalanceAfter,
        Status,
        ReferenceCode,
        Note,
        RelatedEntityType,
        CreatedAt
    ) SELECT 
        w.WalletID,
        p_UserID,
        'Deposit',
        p_Amount,
        w.Balance,
        'Completed',
        p_ReferenceCode,
        p_Note,
        'Deposit',
        NOW()
    FROM Wallets w
    WHERE w.UserID = p_UserID;
    
    SET v_TransactionID = LAST_INSERT_ID();
    
    COMMIT;
    
    -- Return transaction details
    SELECT 
        v_TransactionID as TransactionID,
        p_UserID as UserID,
        p_Amount as Amount,
        'Deposit' as TransactionType,
        'Completed' as Status,
        p_Note as Note,
        p_ReferenceCode as ReferenceCode,
        NOW() as CreatedAt;
END$$

-- Procedure: Get wallet transaction history
DROP PROCEDURE IF EXISTS sp_GetWalletTransactionHistory$$
CREATE PROCEDURE sp_GetWalletTransactionHistory(
    IN p_UserID INT,
    IN p_Limit INT
)
BEGIN
    SELECT 
        wt.TransactionID,
        wt.UserID,
        wt.TransactionType,
        wt.Amount,
        wt.BalanceAfter,
        wt.Status,
        wt.ReferenceCode,
        wt.Note,
        wt.CreatedAt
    FROM WalletTransactions wt
    JOIN Wallets w ON wt.WalletID = w.WalletID
    WHERE w.UserID = p_UserID
    ORDER BY wt.CreatedAt DESC
    LIMIT p_Limit;
END$$

-- Procedure: Get user transaction history with pagination
DROP PROCEDURE IF EXISTS sp_GetUserTransactionHistory$$
CREATE PROCEDURE sp_GetUserTransactionHistory(
    IN p_UserID INT,
    IN p_PageNumber INT,
    IN p_PageSize INT
)
BEGIN
    DECLARE v_Offset INT DEFAULT 0;
    
    -- Calculate offset for pagination
    SET v_Offset = (p_PageNumber - 1) * p_PageSize;
    
    -- Get wallet transactions for the user
    SELECT 
        wt.TransactionID as Id,
        wt.UserID,
        u.Username,
        wt.TransactionType,
        wt.Amount,
        wt.Status,
        wt.CreatedAt,
        wt.ReferenceCode,
        wt.Note,
        wt.RelatedEntityType,
        wt.RelatedEntityID
    FROM WalletTransactions wt
    JOIN Users u ON wt.UserID = u.UserID
    WHERE wt.UserID = p_UserID
    ORDER BY wt.CreatedAt DESC
    LIMIT p_PageSize OFFSET v_Offset;
END$$

-- Procedure: Get donation overview
DROP PROCEDURE IF EXISTS sp_GetDonationOverview$$
CREATE PROCEDURE sp_GetDonationOverview()
BEGIN
    SELECT 
        COUNT(*) as TotalDonations,
        COUNT(DISTINCT UserID) as TotalDonators,
        COUNT(DISTINCT CASE WHEN TargetType = 'Player' THEN TargetID END) as TotalReceivers,
        SUM(Amount) as TotalAmount
    FROM Donations
    WHERE Status = 'Completed';
END$$

-- Procedure: Get top donation receivers
DROP PROCEDURE IF EXISTS sp_GetTopDonationReceivers$$
CREATE PROCEDURE sp_GetTopDonationReceivers(IN p_Limit INT)
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
END$$

-- Procedure: Get top donators
DROP PROCEDURE IF EXISTS sp_GetTopDonators$$
CREATE PROCEDURE sp_GetTopDonators(IN p_Limit INT)
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
END$$

DELIMITER ;

SELECT 'Wallet procedures created successfully!' as Message;
