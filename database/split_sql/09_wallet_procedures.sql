-- =====================================================
-- WALLET AND DONATION PROCEDURES
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
        INSERT INTO Wallets (UserID, Balance, IsLocked, Status)
        VALUES (p_UserId, 0, FALSE, 'Active');
        
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

-- Procedure: Create a withdrawal transaction
DROP PROCEDURE IF EXISTS sp_CreateWithdrawal//
CREATE PROCEDURE sp_CreateWithdrawal(
    IN p_UserId INT,
    IN p_Amount DECIMAL(15, 2),
    IN p_ReferenceCode VARCHAR(100),
    IN p_Note VARCHAR(255),
    IN p_BankName VARCHAR(100),
    IN p_AccountName VARCHAR(100),
    IN p_BankAccount VARCHAR(100)
)
BEGIN
    DECLARE v_WalletId INT;
    DECLARE v_Balance DECIMAL(15, 2);
    DECLARE v_BalanceAfter DECIMAL(15, 2);
    DECLARE v_Username VARCHAR(50);
    DECLARE v_TransactionId INT;
    
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
    
    -- Update wallet balance
    UPDATE Wallets 
    SET Balance = Balance - p_Amount,
        TotalWithdrawn = TotalWithdrawn + p_Amount,
        LastUpdated = NOW()
    WHERE WalletID = v_WalletId;
    
    -- Get new balance
    SELECT Balance INTO v_BalanceAfter
    FROM Wallets
    WHERE WalletID = v_WalletId;
    
    -- Create withdrawal transaction record
    INSERT INTO WalletTransactions (
        WalletID, UserID, TransactionType, Amount, 
        BalanceAfter, Status, ReferenceCode, Note,
        BankName, BankAccountNumber, BankAccountName
    )
    VALUES (
        v_WalletId, p_UserId, 'Withdrawal', -p_Amount,
        v_BalanceAfter, 'Completed', p_ReferenceCode, p_Note,
        p_BankName, p_BankAccount, p_AccountName
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

-- Procedure: Create a transfer transaction
DROP PROCEDURE IF EXISTS sp_CreateTransfer//
CREATE PROCEDURE sp_CreateTransfer(
    IN p_FromUserId INT,
    IN p_ToUserId INT,
    IN p_Amount DECIMAL(15, 2),
    IN p_ReferenceCode VARCHAR(100),
    IN p_Note VARCHAR(255)
)
BEGIN
    DECLARE v_FromWalletId INT;
    DECLARE v_ToWalletId INT;
    DECLARE v_FromBalance DECIMAL(15, 2);
    DECLARE v_FromUsername VARCHAR(50);
    DECLARE v_ToUsername VARCHAR(50);
    DECLARE v_FromBalanceAfter DECIMAL(15, 2);
    DECLARE v_ToBalanceAfter DECIMAL(15, 2);
    DECLARE v_SenderTransactionId INT;
    
    -- Start transaction
    START TRANSACTION;
    
    -- Get sender wallet info
    SELECT w.WalletID, w.Balance, u.Username INTO v_FromWalletId, v_FromBalance, v_FromUsername
    FROM Wallets w
    JOIN Users u ON w.UserID = u.UserID
    WHERE w.UserID = p_FromUserId;
    
    -- Get receiver username
    SELECT Username INTO v_ToUsername
    FROM Users
    WHERE UserID = p_ToUserId;
    
    -- Check if wallets exist and sender has sufficient balance
    IF v_FromWalletId IS NULL THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Sender wallet not found';
    ELSEIF v_ToUsername IS NULL THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Receiver not found';
    ELSEIF v_FromBalance < p_Amount THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Insufficient balance';
    END IF;
    
    -- Get or create receiver wallet
    SELECT WalletID INTO v_ToWalletId
    FROM Wallets
    WHERE UserID = p_ToUserId;
    
    IF v_ToWalletId IS NULL THEN
        INSERT INTO Wallets (UserID, Balance, IsLocked, Status)
        VALUES (p_ToUserId, 0, FALSE, 'Active');
        
        SET v_ToWalletId = LAST_INSERT_ID();
    END IF;
    
    -- Update sender wallet
    UPDATE Wallets 
    SET Balance = Balance - p_Amount,
        LastUpdated = NOW()
    WHERE WalletID = v_FromWalletId;
    
    -- Update receiver wallet
    UPDATE Wallets 
    SET Balance = Balance + p_Amount,
        TotalReceived = TotalReceived + p_Amount,
        LastUpdated = NOW()
    WHERE WalletID = v_ToWalletId;
    
    -- Get new balances
    SELECT Balance INTO v_FromBalanceAfter
    FROM Wallets
    WHERE WalletID = v_FromWalletId;
    
    SELECT Balance INTO v_ToBalanceAfter
    FROM Wallets
    WHERE WalletID = v_ToWalletId;
    
    -- Create sender transaction record
    INSERT INTO WalletTransactions (
        WalletID, UserID, TransactionType, Amount, 
        BalanceAfter, Status, ReferenceCode, Note,
        RelatedUserID, RelatedUsername
    )
    VALUES (
        v_FromWalletId, p_FromUserId, 'TransferOut', -p_Amount,
        v_FromBalanceAfter, 'Completed', p_ReferenceCode, 
        CONCAT('Chuyển tiền đến ', v_ToUsername, IF(p_Note <> '', CONCAT(' - ', p_Note), '')),
        p_ToUserId, v_ToUsername
    );
    
    SET v_SenderTransactionId = LAST_INSERT_ID();
    
    -- Create receiver transaction record
    INSERT INTO WalletTransactions (
        WalletID, UserID, TransactionType, Amount, 
        BalanceAfter, Status, ReferenceCode, Note,
        RelatedUserID, RelatedUsername
    )
    VALUES (
        v_ToWalletId, p_ToUserId, 'TransferIn', p_Amount,
        v_ToBalanceAfter, 'Completed', p_ReferenceCode, 
        CONCAT('Nhận tiền từ ', v_FromUsername, IF(p_Note <> '', CONCAT(' - ', p_Note), '')),
        p_FromUserId, v_FromUsername
    );
    
    COMMIT;
    
    -- Return the sender transaction details
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
        wt.RelatedUsername,
        wt.RelatedEntityID,
        wt.RelatedEntityType
    FROM WalletTransactions wt
    JOIN Users u ON wt.UserID = u.UserID
    WHERE wt.TransactionID = v_SenderTransactionId;
END//

-- Procedure: Create a donation transaction
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
-- Transaction History and Statistics Procedures
-- ------------------------------------------------------

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
    
    -- Không cần gán giá trị mặc định, sử dụng kiểm tra NULL trong câu truy vấn
    
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
    WHERE wt.UserID = p_UserId
        AND (p_FromDate IS NULL OR wt.CreatedAt >= p_FromDate)
        AND (p_ToDate IS NULL OR wt.CreatedAt <= p_ToDate)
        AND (p_TransactionType IS NULL OR wt.TransactionType = p_TransactionType)
    ORDER BY wt.CreatedAt DESC
    LIMIT p_PageSize OFFSET v_Offset;
END//

-- Procedure: Get user wallet statistics
DROP PROCEDURE IF EXISTS sp_GetUserWalletStats//
CREATE PROCEDURE sp_GetUserWalletStats(
    IN p_UserId INT
)
BEGIN
    -- Basic wallet stats
    SELECT 
        w.WalletID,
        u.UserID,
        u.Username,
        w.Balance as CurrentBalance,
        COUNT(wt.TransactionID) as TotalTransactions,
        SUM(CASE WHEN wt.Amount > 0 THEN wt.Amount ELSE 0 END) as TotalIncome,
        ABS(SUM(CASE WHEN wt.Amount < 0 THEN wt.Amount ELSE 0 END)) as TotalExpense
    FROM Wallets w
    JOIN Users u ON w.UserID = u.UserID
    LEFT JOIN WalletTransactions wt ON w.WalletID = wt.WalletID
    WHERE w.UserID = p_UserId;
END//

-- Procedure: Get income by source for a user
DROP PROCEDURE IF EXISTS sp_GetUserIncomeBySource//
CREATE PROCEDURE sp_GetUserIncomeBySource(
    IN p_UserId INT
)
BEGIN
    SELECT 
        wt.TransactionType,
        SUM(wt.Amount) as Amount
    FROM WalletTransactions wt
    JOIN Wallets w ON wt.WalletID = w.WalletID
    WHERE w.UserID = p_UserId
        AND wt.Amount > 0
    GROUP BY wt.TransactionType;
END//

-- Procedure: Get expense by category for a user
DROP PROCEDURE IF EXISTS sp_GetUserExpenseByCategory//
CREATE PROCEDURE sp_GetUserExpenseByCategory(
    IN p_UserId INT
)
BEGIN
    SELECT 
        wt.TransactionType,
        ABS(SUM(wt.Amount)) as Amount
    FROM WalletTransactions wt
    JOIN Wallets w ON wt.WalletID = w.WalletID
    WHERE w.UserID = p_UserId
        AND wt.Amount < 0
    GROUP BY wt.TransactionType;
END//

-- Procedure: Get monthly stats for a user
DROP PROCEDURE IF EXISTS sp_GetUserMonthlyStats//
CREATE PROCEDURE sp_GetUserMonthlyStats(
    IN p_UserId INT
)
BEGIN
    SELECT 
        YEAR(wt.CreatedAt) as Year,
        MONTH(wt.CreatedAt) as Month,
        SUM(CASE WHEN wt.Amount > 0 THEN wt.Amount ELSE 0 END) as TotalIncome,
        ABS(SUM(CASE WHEN wt.Amount < 0 THEN wt.Amount ELSE 0 END)) as TotalExpense,
        COUNT(wt.TransactionID) as TransactionCount
    FROM WalletTransactions wt
    JOIN Wallets w ON wt.WalletID = w.WalletID
    WHERE w.UserID = p_UserId
    GROUP BY YEAR(wt.CreatedAt), MONTH(wt.CreatedAt)
    ORDER BY YEAR(wt.CreatedAt) DESC, MONTH(wt.CreatedAt) DESC
    LIMIT 6;
END//

-- Procedure: Get recent transactions for a user
DROP PROCEDURE IF EXISTS sp_GetUserRecentTransactions//
CREATE PROCEDURE sp_GetUserRecentTransactions(
    IN p_UserId INT
)
BEGIN
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
    JOIN Wallets w ON wt.WalletID = w.WalletID
    WHERE w.UserID = p_UserId
    ORDER BY wt.CreatedAt DESC
    LIMIT 10;
END//

-- ------------------------------------------------------
-- Donation Report Procedures
-- ------------------------------------------------------

-- Procedure: Get donation overview
DROP PROCEDURE IF EXISTS sp_GetDonationOverview//
CREATE PROCEDURE sp_GetDonationOverview()
BEGIN
    SELECT 
        COUNT(*) as TotalDonations,
        COUNT(DISTINCT FromUserID) as TotalDonators,
        COUNT(DISTINCT ToUserID) as TotalReceivers,
        SUM(Amount) as TotalAmount
    FROM Donations
    WHERE Status = 'Completed';
END//

-- Procedure: Get donations by type
DROP PROCEDURE IF EXISTS sp_GetDonationsByType//
CREATE PROCEDURE sp_GetDonationsByType()
BEGIN
    -- Sử dụng UserRoles để phân loại donation theo loại người dùng nhận
    SELECT 
        CASE 
            WHEN ur.RoleID = 2 THEN 'Player'
            WHEN ur.RoleID = 3 THEN 'Team'
            WHEN ur.RoleID = 4 THEN 'Tournament'
            ELSE 'Other'
        END as DonationType,
        SUM(d.Amount) as Amount
    FROM Donations d
    JOIN Users u ON d.ToUserID = u.UserID
    JOIN UserRoles ur ON u.UserID = ur.UserID
    WHERE d.Status = 'Completed'
    GROUP BY ur.RoleID;
END//

-- Procedure: Get top donation receivers
DROP PROCEDURE IF EXISTS sp_GetTopDonationReceivers//
CREATE PROCEDURE sp_GetTopDonationReceivers(
    IN p_Limit INT
)
BEGIN
    SELECT 
        d.ToUserID as EntityId,
        CASE 
            WHEN ur.RoleID = 2 THEN 'Player'
            WHEN ur.RoleID = 3 THEN 'Team'
            WHEN ur.RoleID = 4 THEN 'Tournament'
            ELSE 'Other'
        END as EntityType,
        u.Username as EntityName,
        COUNT(*) as DonationCount,
        SUM(d.Amount) as TotalAmount,
        MIN(d.DonationDate) as FirstDonation,
        MAX(d.DonationDate) as LastDonation
    FROM Donations d
    JOIN Users u ON d.ToUserID = u.UserID
    JOIN UserRoles ur ON u.UserID = ur.UserID
    WHERE d.Status = 'Completed'
    GROUP BY d.ToUserID, ur.RoleID, u.Username
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
        d.FromUserID as UserID,
        u.Username,
        COUNT(*) as DonationCount,
        SUM(d.Amount) as TotalAmount,
        MIN(d.DonationDate) as FirstDonation,
        MAX(d.DonationDate) as LastDonation
    FROM Donations d
    JOIN Users u ON d.FromUserID = u.UserID
    WHERE d.Status = 'Completed'
    GROUP BY d.FromUserID, u.Username
    ORDER BY TotalAmount DESC
    LIMIT p_Limit;
END//

-- Procedure: Get donation history
DROP PROCEDURE IF EXISTS sp_GetDonationHistory//
CREATE PROCEDURE sp_GetDonationHistory(
    IN p_PageNumber INT,
    IN p_PageSize INT,
    IN p_FromDate DATETIME,
    IN p_ToDate DATETIME,
    IN p_MinAmount DECIMAL(15, 2),
    IN p_MaxAmount DECIMAL(15, 2),
    IN p_UserId INT,
    IN p_Username VARCHAR(50),
    IN p_TeamId INT,
    IN p_TournamentId INT,
    IN p_DonationType VARCHAR(50)
)
BEGIN
    DECLARE v_Offset INT;
    SET v_Offset = (p_PageNumber - 1) * p_PageSize;
    
    -- Không cần gán giá trị mặc định, sử dụng kiểm tra NULL trong câu truy vấn
    
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
    JOIN Wallets w ON wt.WalletID = w.WalletID
    WHERE wt.TransactionType = 'Donation'
        AND (p_FromDate IS NULL OR wt.CreatedAt >= p_FromDate)
        AND (p_ToDate IS NULL OR wt.CreatedAt <= p_ToDate)
        AND (p_MinAmount IS NULL OR ABS(wt.Amount) >= p_MinAmount)
        AND (p_MaxAmount IS NULL OR ABS(wt.Amount) <= p_MaxAmount)
        AND (p_UserId IS NULL OR wt.UserID = p_UserId)
        AND (p_Username IS NULL OR u.Username LIKE CONCAT('%', p_Username, '%'))
        AND (p_TeamId IS NULL OR (wt.RelatedEntityType = 'Team' AND wt.RelatedEntityID = p_TeamId))
        AND (p_TournamentId IS NULL OR (wt.RelatedEntityType = 'Tournament' AND wt.RelatedEntityID = p_TournamentId))
        AND (p_DonationType IS NULL OR wt.RelatedEntityType = p_DonationType)
    ORDER BY wt.CreatedAt DESC
    LIMIT p_PageSize OFFSET v_Offset;
END//

DELIMITER ;

SELECT 'Wallet and donation procedures created successfully!' as Message;
