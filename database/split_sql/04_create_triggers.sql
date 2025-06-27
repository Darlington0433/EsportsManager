-- =====================================================
-- AUTOMATIC TRIGGERS
-- =====================================================

USE EsportsManager;

DELIMITER //

-- Trigger: Auto create wallet when creating player
DROP TRIGGER IF EXISTS tr_create_player_wallet//
CREATE TRIGGER tr_create_player_wallet 
AFTER INSERT ON Users
FOR EACH ROW
BEGIN
    IF NEW.Role = 'Player' THEN
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

-- Trigger: Update wallet when withdrawal is completed
DROP TRIGGER IF EXISTS tr_update_wallet_on_withdrawal//
CREATE TRIGGER tr_update_wallet_on_withdrawal
AFTER UPDATE ON Withdrawals
FOR EACH ROW
BEGIN
    IF NEW.Status = 'Completed' AND OLD.Status != 'Completed' THEN
        UPDATE Wallets 
        SET Balance = Balance - NEW.Amount,
            TotalWithdrawn = TotalWithdrawn + NEW.Amount,
            LastUpdated = CURRENT_TIMESTAMP
        WHERE UserID = NEW.UserID;
    END IF;
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
            TransactionType, 
            Amount, 
            Description, 
            ReferenceID
        )
        SELECT 
            w.WalletID,
            'Donation_Received',
            NEW.Amount,
            CONCAT('Donation from user ID: ', NEW.UserID, ' to ', NEW.TargetType, ' - ', COALESCE(NEW.Message, 'No message')),
            NEW.DonationID
        FROM Wallets w 
        WHERE w.UserID = NEW.TargetID;
    END IF;
END//

-- Trigger: Log wallet transaction on withdrawal
DROP TRIGGER IF EXISTS tr_log_wallet_transaction_withdrawal//
CREATE TRIGGER tr_log_wallet_transaction_withdrawal
AFTER UPDATE ON Withdrawals
FOR EACH ROW
BEGIN
    IF NEW.Status = 'Completed' AND OLD.Status != 'Completed' THEN
        INSERT INTO WalletTransactions (
            WalletID, 
            TransactionType, 
            Amount, 
            Description, 
            ReferenceID
        )
        SELECT 
            w.WalletID,
            'Withdrawal',
            NEW.Amount,
            CONCAT('Withdrawal to ', NEW.BankName, ' - ', NEW.BankAccountNumber),
            NEW.WithdrawalID
        FROM Wallets w 
        WHERE w.UserID = NEW.UserID;
    END IF;
END//

DELIMITER ;

SELECT 'Database triggers created successfully!' as Message;
