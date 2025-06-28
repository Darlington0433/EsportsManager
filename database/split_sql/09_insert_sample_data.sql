-- =====================================================
-- 09_INSERT_SAMPLE_DATA.sql
-- Chèn dữ liệu mẫu để test hệ thống
-- Run Order: 9
-- Prerequisites: 01-08 (all previous files)
-- =====================================================

USE EsportsManager;

-- Ensure safe update mode is disabled for UPDATE operations
SET SQL_SAFE_UPDATES = 0;

-- =====================================================
-- GAMES DATA
-- =====================================================

-- Insert sample games
INSERT INTO Games (GameName, Description, Genre) VALUES
('League of Legends', 'Most popular MOBA game in the world', 'MOBA'),
('Counter-Strike 2', 'Professional competitive FPS game', 'FPS'),
('Valorant', 'Tactical FPS game', 'FPS'),
('Dota 2', 'Professional MOBA game', 'MOBA'),
('FIFA 24', 'Football/Soccer simulation game', 'Sports'),
('Rocket League', 'Vehicular soccer game', 'Sports'),
('Overwatch 2', 'Team-based first-person shooter', 'FPS');

-- =====================================================
-- USERS DATA (Admins, Players, Viewers)
-- =====================================================

-- Insert admin accounts with correct BCrypt hashes
-- Password: admin123
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive, Status, IsEmailVerified) VALUES
('admin', '$2a$11$9inYA.zu1eSu2CdJ3XwDMuMl95./WHUIovBSe3VsvXHtgQSCKYcaS', 'admin@esportsmanager.com', 'System Administrator', 'Admin', 'Admin', TRUE, 'Active', TRUE),
('superadmin', '$2a$11$9inYA.zu1eSu2CdJ3XwDMuMl95./WHUIovBSe3VsvXHtgQSCKYcaS', 'superadmin@esportsmanager.com', 'Super Administrator', 'SuperAdmin', 'Admin', TRUE, 'Active', TRUE);

-- Insert sample players with correct BCrypt hashes  
-- Password: player123
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive, Status, IsEmailVerified) VALUES
('player1', '$2a$11$x752zckXIW/bUITiCFq7FumuhWGe8ssSoY7JNemsH5J7U6yU3KKzC', 'player1@test.com', 'Nguyen Van A', 'ProGamer1', 'Player', TRUE, 'Active', TRUE),
('player2', '$2a$11$x752zckXIW/bUITiCFq7FumuhWGe8ssSoY7JNemsH5J7U6yU3KKzC', 'player2@test.com', 'Tran Thi B', 'ProGamer2', 'Player', TRUE, 'Active', TRUE),
('player3', '$2a$11$x752zckXIW/bUITiCFq7FumuhWGe8ssSoY7JNemsH5J7U6yU3KKzC', 'player3@test.com', 'Le Van C', 'ProGamer3', 'Player', TRUE, 'Active', TRUE),
('player4', '$2a$11$x752zckXIW/bUITiCFq7FumuhWGe8ssSoY7JNemsH5J7U6yU3KKzC', 'player4@test.com', 'Do Dinh D', 'ProGamer4', 'Player', TRUE, 'Active', TRUE),
('player5', '$2a$11$x752zckXIW/bUITiCFq7FumuhWGe8ssSoY7JNemsH5J7U6yU3KKzC', 'player5@test.com', 'Pham Thi E', 'ProGamer5', 'Player', TRUE, 'Active', TRUE);

-- Insert sample viewers with correct BCrypt hashes
-- Password: viewer123  
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive, Status, IsEmailVerified) VALUES
('viewer1', '$2a$11$u9zcIxF8UCfSjIOuaLhdZ.4lUlXiICWhdSY3uvZ/WTtPm0F0CXouW', 'viewer1@test.com', 'Hoang Van F', 'EsportsFan1', 'Viewer', TRUE, 'Active', TRUE),
('viewer2', '$2a$11$u9zcIxF8UCfSjIOuaLhdZ.4lUlXiICWhdSY3uvZ/WTtPm0F0CXouW', 'viewer2@test.com', 'Ngo Thi G', 'EsportsFan2', 'Viewer', TRUE, 'Active', TRUE),
('viewer3', '$2a$11$u9zcIxF8UCfSjIOuaLhdZ.4lUlXiICWhdSY3uvZ/WTtPm0F0CXouW', 'viewer3@test.com', 'Vuong Van H', 'EsportsFan3', 'Viewer', TRUE, 'Active', TRUE);

-- =====================================================
-- WALLET DATA (Auto-created by triggers, but we update balances)
-- =====================================================

-- Update wallets for players (they're already created by the trigger)
UPDATE Wallets SET 
    Balance = 150000.00, 
    TotalReceived = 200000.00 
WHERE UserID = 3;

UPDATE Wallets SET 
    Balance = 120000.00, 
    TotalReceived = 150000.00 
WHERE UserID = 4;

UPDATE Wallets SET 
    Balance = 80000.00, 
    TotalReceived = 100000.00 
WHERE UserID = 5;

UPDATE Wallets SET 
    Balance = 60000.00, 
    TotalReceived = 75000.00 
WHERE UserID = 6;

UPDATE Wallets SET 
    Balance = 50000.00, 
    TotalReceived = 65000.00 
WHERE UserID = 7;

-- =====================================================
-- TEAMS DATA
-- =====================================================

-- Create teams
INSERT INTO Teams (TeamName, Description, GameID, CreatedBy, LogoURL, MaxMembers, IsActive, Status) VALUES
('Dragons Gaming', 'Premier League of Legends team from Vietnam', 1, 3, '/images/teams/dragons.png', 5, TRUE, 'Active'),
('Phoenix Valorant', 'Professional Valorant squad', 3, 4, '/images/teams/phoenix.png', 5, TRUE, 'Active'),
('Dota Masters', 'Experienced Dota 2 team with multiple tournament wins', 4, 5, '/images/teams/dotamasters.png', 5, TRUE, 'Active'),
('Football Kings', 'FIFA 24 focused team', 5, 6, '/images/teams/footballkings.png', 3, TRUE, 'Active'),
('Rocket Stars', 'Rocket League specialists', 6, 7, '/images/teams/rocketstars.png', 3, TRUE, 'Active');

-- Additional teams
INSERT IGNORE INTO Teams (TeamName, Description, GameID, CreatedBy, IsActive, Status) VALUES
('Team Alpha', 'Professional Esports Team', 1, 4, TRUE, 'Active'),
('Team Beta', 'Semi-Pro Team', 2, 5, TRUE, 'Active');

-- Add team members
INSERT INTO TeamMembers (TeamID, UserID, IsLeader, Position, Status) VALUES
(1, 3, TRUE, 'Mid Lane', 'Active'),
(1, 4, FALSE, 'Top Lane', 'Active'),
(1, 5, FALSE, 'Jungle', 'Active'),
(2, 4, TRUE, 'Duelist', 'Active'),
(2, 6, FALSE, 'Controller', 'Active'),
(3, 5, TRUE, 'Carry', 'Active'),
(3, 7, FALSE, 'Support', 'Active'),
(4, 6, TRUE, 'Forward', 'Active'),
(5, 7, TRUE, 'Striker', 'Active');

-- =====================================================
-- TOURNAMENTS DATA
-- =====================================================

-- Create tournaments - CẬP NHẬT ĐỂ TEST ĐĂNG KÝ
INSERT INTO Tournaments (TournamentName, Description, GameID, StartDate, EndDate, RegistrationDeadline, MaxTeams, EntryFee, PrizePool, Status, CreatedBy) VALUES
('Vietnam LoL Championship 2025', 'Official League of Legends championship for Vietnam', 1, '2025-08-10', '2025-08-15', '2025-08-08', 16, 50.00, 5000.00, 'Registration', 1),
('Valorant Masters Hanoi', 'Professional Valorant tournament in Hanoi', 3, '2025-09-05', '2025-09-10', '2025-09-03', 8, 30.00, 3000.00, 'Registration', 2),
('Dota 2 International Vietnam', 'Dota 2 championship tournament', 4, '2025-10-01', '2025-10-07', '2025-09-28', 12, 75.00, 8000.00, 'Registration', 1),
('FIFA Esports Cup', 'FIFA 24 professional tournament', 5, '2025-07-20', '2025-07-22', '2025-07-18', 6, 25.00, 1500.00, 'Registration', 2);

-- =====================================================
-- SAMPLE ACHIEVEMENTS DATA
-- =====================================================

-- Insert some sample achievements to test the system
INSERT INTO Achievements (UserID, Title, Description, AchievementType, AssignedBy, DateAchieved) VALUES
(3, 'Tournament Winner', 'Won the Summer Championship 2024', 'Tournament Winner', 1, '2024-06-15 18:30:00'),
(4, 'Most Valuable Player', 'MVP in the regional finals', 'Most Valuable Player', 1, '2024-07-20 20:45:00'),
(5, 'Top 3 Finisher', 'Achieved 3rd place in national tournament', 'Top 3 Finisher', 2, '2024-08-10 16:20:00'),
(6, 'Best Team Player', 'Outstanding teamwork in multiplayer matches', 'Best Team Player', 1, '2024-09-05 14:15:00'),
(7, 'Rising Star', 'Promising new player with great potential', 'Rising Star', 2, '2024-10-12 19:30:00');

-- =====================================================
-- SAMPLE DONATIONS DATA
-- =====================================================

-- Insert sample donations
INSERT INTO Donations (UserID, Amount, Message, Status, TargetType, TargetID, DonationDate) VALUES
(8, 50000.00, 'Great gameplay! Keep it up!', 'Completed', 'Player', 3, '2024-06-20 10:30:00'),
(9, 25000.00, 'Supporting my favorite player', 'Completed', 'Player', 4, '2024-06-21 14:15:00'),
(10, 30000.00, 'Amazing tournament performance!', 'Completed', 'Player', 5, '2024-06-22 16:45:00'),
(8, 20000.00, 'Good luck in next tournament!', 'Completed', 'Player', 6, '2024-06-23 20:10:00'),
(9, 15000.00, 'Keep playing well!', 'Completed', 'Player', 7, '2024-06-24 12:30:00');

-- Re-enable safe update mode
SET SQL_SAFE_UPDATES = 1;

SELECT 'Sample data inserted successfully!' as Message;
SELECT CONCAT('Total Users: ', COUNT(*)) as UserCount FROM Users;
SELECT CONCAT('Total Games: ', COUNT(*)) as GameCount FROM Games;  
SELECT CONCAT('Total Teams: ', COUNT(*)) as TeamCount FROM Teams;
SELECT CONCAT('Total Tournaments: ', COUNT(*)) as TournamentCount FROM Tournaments;
SELECT CONCAT('Total Achievements: ', COUNT(*)) as AchievementCount FROM Achievements;
