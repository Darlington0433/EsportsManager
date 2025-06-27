-- =====================================================
-- THÊM DỮ LIỆU DONATION MẪU ĐỂ TEST
-- =====================================================

USE EsportsManager;

-- Kiểm tra xem có user và team nào chưa, nếu chưa thì tạo
INSERT IGNORE INTO Users (Username, PasswordHash, Email, FullName, Role, Status, IsActive) VALUES
('admin', '$2b$10$defaulthash', 'admin@esports.com', 'Admin System', 'Admin', 'Active', TRUE),
('viewer1', '$2b$10$defaulthash', 'viewer1@test.com', 'Viewer One', 'Viewer', 'Active', TRUE),
('viewer2', '$2b$10$defaulthash', 'viewer2@test.com', 'Viewer Two', 'Viewer', 'Active', TRUE),
('player1', '$2b$10$defaulthash', 'player1@test.com', 'Player One', 'Player', 'Active', TRUE),
('player2', '$2b$10$defaulthash', 'player2@test.com', 'Player Two', 'Player', 'Active', TRUE);

-- Thêm game mẫu
INSERT IGNORE INTO Games (GameName, Description, Genre, IsActive) VALUES
('League of Legends', 'MOBA Game', 'MOBA', TRUE),
('Counter-Strike 2', 'FPS Game', 'FPS', TRUE);

-- Thêm team mẫu
INSERT IGNORE INTO Teams (TeamName, Description, GameID, CreatedBy, IsActive, Status) VALUES
('Team Alpha', 'Professional Esports Team', 1, 4, TRUE, 'Active'),
('Team Beta', 'Semi-Pro Team', 2, 5, TRUE, 'Active');

-- Thêm tournament mẫu
INSERT IGNORE INTO Tournaments (TournamentName, Description, GameID, StartDate, EndDate, CreatedBy, Status) VALUES
('Summer Championship 2024', 'Annual summer tournament', 1, '2024-07-01 10:00:00', '2024-07-15 18:00:00', 1, 'Completed'),
('Winter Cup 2024', 'Winter season tournament', 2, '2024-12-01 10:00:00', '2024-12-15 18:00:00', 1, 'Ongoing');

-- Tạo wallet cho user
INSERT IGNORE INTO Wallets (UserID, Balance, TotalReceived, TotalWithdrawn, IsActive) VALUES
(2, 500.00, 0.00, 0.00, TRUE),  -- viewer1
(3, 300.00, 0.00, 0.00, TRUE),  -- viewer2
(4, 100.00, 150.00, 50.00, TRUE),  -- player1
(5, 200.00, 300.00, 100.00, TRUE); -- player2

-- Thêm donations mẫu
INSERT IGNORE INTO Donations (UserID, Amount, Message, Status, TargetType, TargetID, DonationDate) VALUES
-- Donations from viewer1
(2, 50.00, 'Good luck in the tournament!', 'Completed', 'Player', 4, '2024-06-01 10:00:00'),
(2, 100.00, 'Amazing gameplay!', 'Completed', 'Team', 1, '2024-06-02 14:30:00'),
(2, 25.00, 'Keep it up!', 'Completed', 'Tournament', 1, '2024-06-03 16:00:00'),

-- Donations from viewer2
(3, 75.00, 'Great performance!', 'Completed', 'Player', 5, '2024-06-05 11:00:00'),
(3, 30.00, 'Supporting the team!', 'Completed', 'Team', 2, '2024-06-06 15:00:00'),
(3, 40.00, 'Love this tournament!', 'Completed', 'Tournament', 2, '2024-06-07 13:00:00'),

-- More donations for testing
(2, 20.00, 'Small donation', 'Completed', 'Player', 4, '2024-06-10 09:00:00'),
(3, 60.00, 'Big fan!', 'Completed', 'Player', 4, '2024-06-11 12:00:00'),
(2, 15.00, 'Tournament support', 'Completed', 'Tournament', 1, '2024-06-12 14:00:00'),
(3, 35.00, 'Team support', 'Completed', 'Team', 1, '2024-06-13 16:00:00');

-- Tạo WalletTransactions tương ứng với donations
INSERT IGNORE INTO WalletTransactions (WalletID, UserID, TransactionType, Amount, BalanceAfter, Status, ReferenceCode, Note, RelatedEntityType, RelatedEntityID, CreatedAt) 
SELECT 
    w.WalletID,
    d.UserID,
    'Donation',
    -d.Amount,
    w.Balance,
    'Completed',
    CONCAT('DON-', LPAD(d.DonationID, 6, '0')),
    CONCAT('Donation to ', d.TargetType, ' #', d.TargetID, ' - ', COALESCE(d.Message, 'No message')),
    d.TargetType,
    d.TargetID,
    d.DonationDate
FROM Donations d
JOIN Wallets w ON d.UserID = w.UserID
WHERE d.Status = 'Completed'
AND NOT EXISTS (
    SELECT 1 FROM WalletTransactions wt 
    WHERE wt.UserID = d.UserID 
    AND wt.RelatedEntityType = d.TargetType 
    AND wt.RelatedEntityID = d.TargetID
    AND wt.ReferenceCode = CONCAT('DON-', LPAD(d.DonationID, 6, '0'))
);

-- Kiểm tra kết quả
SELECT 'Data insertion completed!' as Status;
SELECT COUNT(*) as TotalUsers FROM Users;
SELECT COUNT(*) as TotalDonations FROM Donations;
SELECT COUNT(*) as TotalWalletTransactions FROM WalletTransactions WHERE TransactionType = 'Donation';

-- Test stored procedures
SELECT 'Testing sp_GetDonationOverview...' as Status;
CALL sp_GetDonationOverview();

SELECT 'Testing sp_GetDonationsByType...' as Status;
CALL sp_GetDonationsByType();

SELECT 'Sample data added successfully!' as Message;
