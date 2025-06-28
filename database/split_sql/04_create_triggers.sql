-- =====================================================
-- 04_CREATE_TRIGGERS.sql
-- Tạo các triggers tự động xử lý business logic
-- Run Order: 4
-- Prerequisites: 01_create_database_and_tables.sql
-- =====================================================

USE EsportsManager;

-- =====================================================
-- AUTOMATIC TRIGGERS
-- =====================================================

DELIMITER //

-- Trigger: Auto create wallet when creating player or viewer
DROP TRIGGER IF EXISTS tr_create_user_wallet//
CREATE TRIGGER tr_create_user_wallet 
AFTER INSERT ON Users
FOR EACH ROW
BEGIN
    IF NEW.Role = 'Player' OR NEW.Role = 'Viewer' THEN
        INSERT INTO Wallets (UserID, Balance) VALUES (NEW.UserID, 0.00);
    END IF;
END//

-- Trigger: Auto create profile when creating user
DROP TRIGGER IF EXISTS tr_create_user_profile//
CREATE TRIGGER tr_create_user_profile 
AFTER INSERT ON Users
FOR EACH ROW
BEGIN
    INSERT INTO UserProfiles (UserID, Bio) VALUES (NEW.UserID, 'New user');
END//

-- Trigger: Update wallet when donation is made
DROP TRIGGER IF EXISTS tr_update_wallet_on_donation//
CREATE TRIGGER tr_update_wallet_on_donation
AFTER INSERT ON Donations
FOR EACH ROW
BEGIN
    IF NEW.Status = 'Completed' AND NEW.TargetType = 'Player' THEN
        UPDATE Wallets 
        SET Balance = Balance + NEW.Amount,
            TotalReceived = TotalReceived + NEW.Amount,
            LastUpdated = CURRENT_TIMESTAMP
        WHERE UserID = NEW.TargetID;
    END IF;
END//

-- Trigger: Update wallet when withdrawal is completed (immediate processing)
DROP TRIGGER IF EXISTS tr_update_wallet_on_withdrawal//
CREATE TRIGGER tr_update_wallet_on_withdrawal
AFTER INSERT ON Withdrawals
FOR EACH ROW
BEGIN
    -- Update wallet balance immediately when withdrawal is created
    UPDATE Wallets 
    SET Balance = Balance - NEW.Amount,
        TotalWithdrawn = TotalWithdrawn + NEW.Amount,
        LastUpdated = CURRENT_TIMESTAMP
    WHERE UserID = NEW.UserID;
END//

-- Trigger: Log wallet transaction on donation
DROP TRIGGER IF EXISTS tr_log_wallet_transaction_donation//
CREATE TRIGGER tr_log_wallet_transaction_donation
AFTER INSERT ON Donations
FOR EACH ROW
BEGIN
    IF NEW.Status = 'Completed' AND NEW.TargetType = 'Player' THEN
        INSERT INTO WalletTransactions (
            WalletID, 
            UserID,
            TransactionType, 
            Amount, 
            BalanceAfter,
            Note, 
            RelatedEntityType,
            RelatedEntityID
        )
        SELECT 
            w.WalletID,
            w.UserID,
            'Donation_Received',
            NEW.Amount,
            w.Balance + NEW.Amount,
            CONCAT('Donation from user ID: ', NEW.UserID, ' to ', NEW.TargetType, ' - ', COALESCE(NEW.Message, 'No message')),
            'Donation',
            NEW.DonationID
        FROM Wallets w 
        WHERE w.UserID = NEW.TargetID;
    END IF;
END//

-- Trigger: Log wallet transaction on withdrawal (immediate processing)
DROP TRIGGER IF EXISTS tr_log_wallet_transaction_withdrawal//
CREATE TRIGGER tr_log_wallet_transaction_withdrawal
AFTER INSERT ON Withdrawals
FOR EACH ROW
BEGIN
    -- Log transaction immediately when withdrawal is created
    INSERT INTO WalletTransactions (
        WalletID, 
        UserID,
        TransactionType, 
        Amount, 
        BalanceAfter,
        Note, 
        RelatedEntityType,
        RelatedEntityID,
        ReferenceCode
    )
    SELECT 
        w.WalletID,
        w.UserID,
        'Withdrawal',
        -NEW.Amount,
        w.Balance - NEW.Amount,
        CONCAT('Withdrawal to ', NEW.BankName, ' - ', NEW.BankAccountNumber),
        'Withdrawal',
        NEW.WithdrawalID,
        NEW.ReferenceCode
    FROM Wallets w 
    WHERE w.UserID = NEW.UserID;
END//

DELIMITER ;

SELECT 'Database triggers created successfully!' as Message;
