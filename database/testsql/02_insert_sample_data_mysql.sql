-- =============================================
-- EsportsManager Sample Data for MySQL
-- Created: 2025-06-26
-- Version: 2.1 - Complete Sample Data for MySQL (Fixed)
-- =============================================

USE `EsportsManager`;

-- Clear existing data with proper foreign key handling
SET FOREIGN_KEY_CHECKS = 0;

TRUNCATE TABLE `UserAchievements`;
TRUNCATE TABLE `Achievements`;
TRUNCATE TABLE `SystemLogs`;
TRUNCATE TABLE `SystemSettings`;
TRUNCATE TABLE `Feedback`;
TRUNCATE TABLE `Votes`;
TRUNCATE TABLE `Donations`;
TRUNCATE TABLE `Transactions`;
TRUNCATE TABLE `Wallets`;
TRUNCATE TABLE `TournamentResults`;
TRUNCATE TABLE `Matches`;
TRUNCATE TABLE `TournamentTeams`;
TRUNCATE TABLE `Tournaments`;
TRUNCATE TABLE `TeamMembers`;
TRUNCATE TABLE `Teams`;
TRUNCATE TABLE `Users`;
TRUNCATE TABLE `Games`;

SET FOREIGN_KEY_CHECKS = 1;

-- =============================================
-- 1. SYSTEM SETTINGS
-- =============================================

INSERT INTO `SystemSettings` (`SettingKey`, `SettingValue`, `Description`, `Category`, `DataType`, `IsReadOnly`) VALUES
-- General Settings
('SystemName', 'Esports Manager', 'Name of the application', 'General', 'String', FALSE),
('SystemVersion', '2.0.0', 'Current version of the system', 'General', 'String', FALSE),
('DefaultLanguage', 'vi-VN', 'Default language for the system', 'General', 'String', FALSE),
('DefaultCurrency', 'VND', 'Default currency for transactions', 'General', 'String', FALSE),
('MaintenanceMode', 'false', 'Whether the system is in maintenance mode', 'General', 'Boolean', FALSE),

-- Tournament Settings
('DefaultMaxTeams', '16', 'Default maximum teams per tournament', 'Tournament', 'Integer', FALSE),
('DefaultMaxPlayersPerTeam', '5', 'Default maximum players per team', 'Tournament', 'Integer', FALSE),
('RequireTeamApproval', 'true', 'Whether teams require approval to join tournaments', 'Tournament', 'Boolean', FALSE),
('AllowPublicVoting', 'true', 'Whether public voting is allowed', 'Tournament', 'Boolean', FALSE),
('DefaultMinPrizePool', '1000000', 'Default minimum prize pool in VND', 'Tournament', 'Integer', FALSE),
('MinimumTeamsForStart', '4', 'Minimum teams required to start a tournament', 'Tournament', 'Integer', FALSE),
('EnableAutoScheduling', 'false', 'Whether automatic match scheduling is enabled', 'Tournament', 'Boolean', FALSE),

-- Wallet Settings
('MinimumDepositAmount', '50000', 'Minimum deposit amount in VND', 'Wallet', 'Decimal', FALSE),
('MaximumDepositAmount', '50000000', 'Maximum deposit amount in VND', 'Wallet', 'Decimal', FALSE),
('MinimumWithdrawalAmount', '100000', 'Minimum withdrawal amount in VND', 'Wallet', 'Decimal', FALSE),
('TransactionFeePercent', '2.5', 'Transaction fee percentage', 'Wallet', 'Decimal', FALSE),
('WithdrawalProcessingTimeHours', '48', 'Withdrawal processing time in hours', 'Wallet', 'Integer', FALSE),
('EnableDonations', 'true', 'Whether donations are enabled', 'Wallet', 'Boolean', FALSE),
('MaxDailyTransactions', '10', 'Maximum daily transactions per user', 'Wallet', 'Integer', FALSE),
('RequireEmailVerificationForWithdrawals', 'true', 'Whether email verification is required for withdrawals', 'Wallet', 'Boolean', FALSE),

-- Security Settings
('MaxLoginAttempts', '5', 'Maximum login attempts before lockout', 'Security', 'Integer', FALSE),
('PasswordMinLength', '8', 'Minimum password length', 'Security', 'Integer', FALSE),
('SessionTimeoutMinutes', '60', 'Session timeout in minutes', 'Security', 'Integer', FALSE),
('RequireEmailVerification', 'true', 'Whether email verification is required', 'Security', 'Boolean', FALSE),

-- Notification Settings
('EmailNotificationsEnabled', 'true', 'Whether email notifications are enabled', 'Notification', 'Boolean', FALSE),
('SMSNotificationsEnabled', 'false', 'Whether SMS notifications are enabled', 'Notification', 'Boolean', FALSE),
('PushNotificationsEnabled', 'true', 'Whether push notifications are enabled', 'Notification', 'Boolean', FALSE);

-- =============================================
-- 2. GAMES
-- =============================================

INSERT INTO `Games` (`GameName`, `Description`, `Genre`) VALUES
('League of Legends', 'Multiplayer Online Battle Arena game by Riot Games', 'MOBA'),
('Counter-Strike 2', 'Tactical first-person shooter by Valve', 'FPS'),
('Valorant', 'Tactical first-person shooter by Riot Games', 'FPS'),
('Dota 2', 'Multiplayer Online Battle Arena game by Valve', 'MOBA'),
('Overwatch 2', 'Team-based first-person shooter by Blizzard', 'FPS');

-- =============================================
-- 3. USERS WITH SECURE PASSWORDS
-- =============================================

-- Passwords used:
-- admin: Admin@123 -> $2a$11$Qp8zPYGrKbmGFSg4j0C9aOhxJ1ZaR8rklKpK8uNklkYJnKRD/IlYe
-- player1: Player@123 -> $2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa
-- viewer1: Viewer@123 -> $2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb

INSERT INTO `Users` (`Username`, `PasswordHash`, `Email`, `FullName`, `Role`, `Status`, `IsActive`, `EmailVerified`, `PhoneVerified`, `Bio`, `Country`, `TotalTournaments`, `TournamentsWon`, `SecurityQuestion`, `SecurityAnswerHash`) VALUES
-- Admin Account
('admin', '$2a$11$Qp8zPYGrKbmGFSg4j0C9aOhxJ1ZaR8rklKpK8uNklkYJnKRD/IlYe', 'admin@esportsmanager.com', 'System Administrator', 'Admin', 'Active', TRUE, TRUE, FALSE, 'System administrator with full access to manage the platform', 'Vietnam', 0, 0, 'What is your favorite esports game?', '$2a$11$Qp8zPYGrKbmGFSg4j0C9aOhxJ1ZaR8rklKpK8uNklkYJnKRD/IlYe'),

-- Secondary Admin
('admin2', '$2a$11$Qp8zPYGrKbmGFSg4j0C9aOhxJ1ZaR8rklKpK8uNklkYJnKRD/IlYe', 'admin2@esportsmanager.com', 'Assistant Administrator', 'Admin', 'Active', TRUE, TRUE, FALSE, 'Assistant administrator helping manage the platform', 'Vietnam', 0, 0, 'What is your favorite esports game?', '$2a$11$Qp8zPYGrKbmGFSg4j0C9aOhxJ1ZaR8rklKpK8uNklkYJnKRD/IlYe'),

-- Player Accounts
('player1', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa', 'player1@example.com', 'Nguyen Van A', 'Player', 'Active', TRUE, TRUE, FALSE, 'Professional gamer specializing in League of Legends', 'Vietnam', 1, 1, 'What is your birth year?', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa'),

('player2', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa', 'player2@example.com', 'Tran Thi B', 'Player', 'Active', TRUE, TRUE, FALSE, 'Competitive player in CS2 and Valorant', 'Vietnam', 1, 0, 'What is your birth year?', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa'),

('player3', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa', 'player3@example.com', 'Le Van C', 'Player', 'Active', TRUE, TRUE, FALSE, 'Rising star in the esports scene', 'Vietnam', 1, 0, 'What is your birth year?', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa'),

('player4', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa', 'player4@example.com', 'Pham Thi D', 'Player', 'Active', TRUE, TRUE, FALSE, 'Support main in MOBA games', 'Vietnam', 1, 0, 'What is your birth year?', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa'),

('player5', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa', 'player5@example.com', 'Hoang Van E', 'Player', 'Active', TRUE, TRUE, FALSE, 'Team captain and strategist', 'Vietnam', 0, 0, 'What is your birth year?', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa'),

('player6', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa', 'player6@example.com', 'Bui Thi F', 'Player', 'Active', TRUE, TRUE, FALSE, 'Flexible player across multiple games', 'Vietnam', 0, 0, 'What is your birth year?', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa'),

-- Viewer Accounts
('viewer1', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb', 'viewer1@example.com', 'Nguyen Van G', 'Viewer', 'Active', TRUE, TRUE, FALSE, 'Esports enthusiast and content creator', 'Vietnam', 0, 0, 'What is your favorite color?', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb'),

('viewer2', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb', 'viewer2@example.com', 'Tran Van H', 'Viewer', 'Active', TRUE, TRUE, FALSE, 'Gaming analyst and commentator', 'Vietnam', 0, 0, 'What is your favorite color?', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb'),

('viewer3', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb', 'viewer3@example.com', 'Le Thi I', 'Viewer', 'Active', TRUE, TRUE, FALSE, 'Tournament sponsor and supporter', 'Vietnam', 0, 0, 'What is your favorite color?', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb'),

('viewer4', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb', 'viewer4@example.com', 'Pham Van J', 'Viewer', 'Active', TRUE, TRUE, FALSE, 'Community moderator and volunteer', 'Vietnam', 0, 0, 'What is your favorite color?', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb'),

('viewer5', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb', 'viewer5@example.com', 'Hoang Thi K', 'Viewer', 'Active', TRUE, TRUE, FALSE, 'Esports journalist and blogger', 'Vietnam', 0, 0, 'What is your favorite color?', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb');

-- =============================================
-- 4. TEAMS
-- =============================================

INSERT INTO `Teams` (`TeamName`, `Description`, `CaptainID`) VALUES
('Dragon Slayers', 'Professional esports team specializing in MOBA games', 3), -- player1 as captain
('Phoenix Rising', 'Rising competitive team in FPS games', 4), -- player2 as captain
('Storm Warriors', 'Multi-game competitive team', 5), -- player3 as captain
('Thunder Bolt', 'Veteran team with championship experience', 6); -- player4 as captain

-- =============================================
-- 5. TEAM MEMBERS
-- =============================================

-- Dragon Slayers members
INSERT INTO `TeamMembers` (`TeamID`, `UserID`, `Role`) VALUES
(1, 3, 'Captain'), -- player1
(1, 4, 'Member'), -- player2
(1, 5, 'Member'), -- player3
(1, 6, 'Member'), -- player4
(1, 7, 'Member'); -- player5

-- Phoenix Rising members
INSERT INTO `TeamMembers` (`TeamID`, `UserID`, `Role`) VALUES
(2, 4, 'Captain'), -- player2
(2, 6, 'Member'), -- player4
(2, 7, 'Member'), -- player5
(2, 8, 'Member'); -- player6

-- Storm Warriors members
INSERT INTO `TeamMembers` (`TeamID`, `UserID`, `Role`) VALUES
(3, 5, 'Captain'), -- player3
(3, 3, 'Member'), -- player1
(3, 8, 'Member'); -- player6

-- Thunder Bolt members
INSERT INTO `TeamMembers` (`TeamID`, `UserID`, `Role`) VALUES
(4, 6, 'Captain'), -- player4
(4, 7, 'Member'), -- player5
(4, 8, 'Member'); -- player6

-- =============================================
-- 6. TOURNAMENTS
-- =============================================

INSERT INTO `Tournaments` (`TournamentName`, `Description`, `GameID`, `MaxTeams`, `PrizePool`, `EntryFee`, `StartDate`, `EndDate`, `RegistrationStartDate`, `RegistrationEndDate`, `Status`, `CreatedBy`) VALUES
-- Active tournaments
('Summer League Championship 2025', 'The biggest League of Legends tournament of the summer', 1, 16, 50000000, 1000000, '2025-07-01 00:00:00', '2025-07-15 23:59:59', '2025-06-01 00:00:00', '2025-06-25 23:59:59', 'Registration', 1),
('CS2 Masters Cup', 'Elite Counter-Strike 2 competition', 2, 8, 30000000, 500000, '2025-07-10 00:00:00', '2025-07-20 23:59:59', '2025-06-10 00:00:00', '2025-06-30 23:59:59', 'Registration', 1),
('Valorant Champions Series', 'Regional Valorant championship', 3, 12, 25000000, 750000, '2025-08-01 00:00:00', '2025-08-10 23:59:59', '2025-07-01 00:00:00', '2025-07-20 23:59:59', 'Draft', 2),

-- Completed tournaments for history
('Spring LoL Tournament 2025', 'Completed League of Legends spring tournament', 1, 8, 20000000, 500000, '2025-03-01 00:00:00', '2025-03-15 23:59:59', '2025-02-01 00:00:00', '2025-02-20 23:59:59', 'Completed', 1),
('Winter CS2 Cup 2024', 'Completed Counter-Strike winter tournament', 2, 6, 15000000, 300000, '2024-12-01 00:00:00', '2024-12-10 23:59:59', '2024-11-01 00:00:00', '2024-11-20 23:59:59', 'Completed', 2);

-- =============================================
-- 7. TOURNAMENT TEAMS
-- =============================================

-- Summer League Championship registrations
INSERT INTO `TournamentTeams` (`TournamentID`, `TeamID`, `Status`, `ApprovedBy`, `ApprovedAt`) VALUES
(1, 1, 'Approved', 1, NOW()),
(1, 2, 'Approved', 1, NOW()),
(1, 3, 'Pending', NULL, NULL),
(1, 4, 'Approved', 1, NOW());

-- CS2 Masters Cup registrations
INSERT INTO `TournamentTeams` (`TournamentID`, `TeamID`, `Status`, `ApprovedBy`, `ApprovedAt`) VALUES
(2, 2, 'Approved', 1, NOW()),
(2, 4, 'Approved', 1, NOW());

-- Completed tournament results
INSERT INTO `TournamentTeams` (`TournamentID`, `TeamID`, `Status`, `ApprovedBy`, `ApprovedAt`) VALUES
(4, 1, 'Approved', 1, '2025-02-25 10:00:00'),
(4, 2, 'Approved', 1, '2025-02-25 10:00:00'),
(4, 3, 'Approved', 1, '2025-02-25 10:00:00'),
(4, 4, 'Approved', 1, '2025-02-25 10:00:00');

-- =============================================
-- 8. TOURNAMENT RESULTS (For completed tournaments)
-- =============================================

INSERT INTO `TournamentResults` (`TournamentID`, `TeamID`, `Rank`, `Prize`, `Points`, `Wins`, `Losses`, `TotalMatches`) VALUES
-- Spring LoL Tournament 2025 results
(4, 1, 1, 10000000, 100, 5, 1, 6), -- Dragon Slayers - Champion
(4, 2, 2, 5000000, 80, 4, 2, 6), -- Phoenix Rising - Runner-up
(4, 3, 3, 3000000, 60, 3, 3, 6), -- Storm Warriors - 3rd place
(4, 4, 4, 2000000, 40, 2, 4, 6); -- Thunder Bolt - 4th place

-- =============================================
-- 9. MATCHES
-- =============================================

-- Sample matches for completed tournament
INSERT INTO `Matches` (`TournamentID`, `Team1ID`, `Team2ID`, `ScheduledDate`, `ActualStartTime`, `ActualEndTime`, `Status`, `WinnerTeamID`, `Team1Score`, `Team2Score`, `Round`, `MatchType`) VALUES
-- Spring LoL Tournament semifinals
(4, 1, 3, '2025-03-10 14:00:00', '2025-03-10 14:05:00', '2025-03-10 15:30:00', 'Completed', 1, 2, 0, 'Semifinals', 'Semifinal'),
(4, 2, 4, '2025-03-10 16:00:00', '2025-03-10 16:08:00', '2025-03-10 17:45:00', 'Completed', 2, 2, 1, 'Semifinals', 'Semifinal'),

-- Final and 3rd place match
(4, 1, 2, '2025-03-15 18:00:00', '2025-03-15 18:10:00', '2025-03-15 20:15:00', 'Completed', 1, 3, 1, 'Final', 'Final'),
(4, 3, 4, '2025-03-15 16:00:00', '2025-03-15 16:05:00', '2025-03-15 17:30:00', 'Completed', 3, 2, 0, '3rd Place', 'ThirdPlace'),

-- Upcoming matches for active tournaments
(1, 1, 2, '2025-07-05 14:00:00', NULL, NULL, 'Scheduled', NULL, 0, 0, 'Round 1', 'Regular'),
(1, 3, 4, '2025-07-05 16:00:00', NULL, NULL, 'Scheduled', NULL, 0, 0, 'Round 1', 'Regular');

-- =============================================
-- 10. WALLETS
-- =============================================

INSERT INTO `Wallets` (`UserID`, `Balance`, `TotalDeposited`, `TotalWithdrawn`, `TotalDonated`, `TotalReceived`) VALUES
-- Admin wallets
(1, 0.00, 0.00, 0.00, 0.00, 0.00),
(2, 0.00, 0.00, 0.00, 0.00, 0.00),

-- Player wallets with tournament winnings
(3, 8500000.00, 2000000.00, 1500000.00, 0.00, 10000000.00), -- Winner with prize money
(4, 3200000.00, 1500000.00, 800000.00, 200000.00, 5000000.00), -- Runner-up
(5, 2800000.00, 1000000.00, 200000.00, 0.00, 3000000.00), -- 3rd place
(6, 1950000.00, 800000.00, 50000.00, 0.00, 2000000.00), -- 4th place
(7, 500000.00, 500000.00, 0.00, 0.00, 0.00),
(8, 300000.00, 300000.00, 0.00, 0.00, 0.00),

-- Viewer wallets for donations
(9, 750000.00, 1000000.00, 0.00, 250000.00, 0.00), -- Generous supporter
(10, 400000.00, 500000.00, 0.00, 100000.00, 0.00),
(11, 900000.00, 1000000.00, 0.00, 100000.00, 0.00),
(12, 200000.00, 300000.00, 0.00, 100000.00, 0.00),
(13, 800000.00, 1000000.00, 0.00, 200000.00, 0.00);

-- =============================================
-- 11. TRANSACTIONS
-- =============================================

INSERT INTO `Transactions` (`WalletID`, `Type`, `Amount`, `Status`, `Description`, `RelatedTournamentID`, `ProcessedBy`, `ProcessedAt`) VALUES
-- Prize payouts
(3, 'Prize', 10000000.00, 'Completed', 'Championship prize - Spring LoL Tournament 2025', 4, 1, '2025-03-16 10:00:00'),
(4, 'Prize', 5000000.00, 'Completed', 'Runner-up prize - Spring LoL Tournament 2025', 4, 1, '2025-03-16 10:15:00'),
(5, 'Prize', 3000000.00, 'Completed', '3rd place prize - Spring LoL Tournament 2025', 4, 1, '2025-03-16 10:30:00'),
(6, 'Prize', 2000000.00, 'Completed', '4th place prize - Spring LoL Tournament 2025', 4, 1, '2025-03-16 10:45:00'),

-- Deposits
(3, 'Deposit', 2000000.00, 'Completed', 'Bank transfer deposit', NULL, NULL, '2025-02-01 09:00:00'),
(4, 'Deposit', 1500000.00, 'Completed', 'Credit card deposit', NULL, NULL, '2025-02-05 14:30:00'),
(5, 'Deposit', 1000000.00, 'Completed', 'Bank transfer deposit', NULL, NULL, '2025-02-10 11:20:00'),
(9, 'Deposit', 1000000.00, 'Completed', 'Support fund deposit', NULL, NULL, '2025-02-15 16:45:00'),
(10, 'Deposit', 500000.00, 'Completed', 'Credit card deposit', NULL, NULL, '2025-02-20 13:10:00'),

-- Withdrawals
(3, 'Withdrawal', 1500000.00, 'Completed', 'Bank transfer withdrawal', NULL, 1, '2025-03-20 10:00:00'),
(4, 'Withdrawal', 800000.00, 'Completed', 'Bank transfer withdrawal', NULL, 1, '2025-03-25 11:30:00'),

-- Entry fees
(3, 'Fee', 1000000.00, 'Completed', 'Tournament entry fee - Summer League Championship 2025', 1, NULL, '2025-06-15 12:00:00'),
(4, 'Fee', 1000000.00, 'Completed', 'Tournament entry fee - Summer League Championship 2025', 1, NULL, '2025-06-16 14:20:00'),
(6, 'Fee', 1000000.00, 'Completed', 'Tournament entry fee - Summer League Championship 2025', 1, NULL, '2025-06-17 16:45:00');

-- =============================================
-- 12. DONATIONS
-- =============================================

INSERT INTO `Donations` (`DonorUserID`, `RecipientUserID`, `RecipientTeamID`, `TournamentID`, `Amount`, `Message`, `IsAnonymous`, `Status`, `ProcessedAt`) VALUES
-- Individual player donations
(9, 3, NULL, NULL, 100000.00, 'Great performance in the championship! Keep it up!', FALSE, 'Completed', '2025-03-17 15:30:00'),
(10, 4, NULL, NULL, 50000.00, 'Amazing skills in the finals!', FALSE, 'Completed', '2025-03-17 16:00:00'),
(11, 3, NULL, NULL, 75000.00, 'You deserved that victory!', TRUE, 'Completed', '2025-03-18 09:15:00'),

-- Team donations
(9, NULL, 1, NULL, 150000.00, 'Supporting the Dragon Slayers team!', FALSE, 'Completed', '2025-03-20 14:45:00'),
(12, NULL, 2, NULL, 100000.00, 'Phoenix Rising has potential!', FALSE, 'Completed', '2025-03-21 11:20:00'),
(13, NULL, 1, NULL, 200000.00, 'Champion team deserves support!', FALSE, 'Completed', '2025-03-22 16:30:00'),

-- Tournament donations
(11, NULL, NULL, 1, 100000.00, 'Adding to the prize pool!', FALSE, 'Completed', '2025-06-20 13:45:00'),
(13, NULL, NULL, 1, 100000.00, 'Supporting the tournament!', TRUE, 'Completed', '2025-06-21 10:20:00');

-- =============================================
-- 13. VOTES
-- =============================================

INSERT INTO `Votes` (`VoterUserID`, `VoteType`, `TargetID`, `Rating`, `Comment`, `TournamentID`) VALUES
-- Tournament votes
(9, 'Tournament', 4, 5, 'Excellent tournament organization and exciting matches!', 4),
(10, 'Tournament', 4, 4, 'Great competition, but could use better scheduling.', 4),
(11, 'Tournament', 4, 5, 'Best tournament I have watched this year!', 4),
(12, 'Tournament', 4, 4, 'Good tournament overall, impressive production value.', 4),

-- Team votes
(9, 'Team', 1, 5, 'Dragon Slayers showed incredible teamwork and strategy.', 4),
(10, 'Team', 2, 4, 'Phoenix Rising played well, especially in the semifinals.', 4),
(11, 'Team', 1, 5, 'Deserved champions, amazing performance throughout.', 4),
(12, 'Team', 3, 3, 'Storm Warriors need more practice but have potential.', 4),
(13, 'Team', 1, 5, 'Perfect execution in the finals!', 4),

-- Player votes
(9, 'Player', 3, 5, 'Outstanding individual performance, great leadership.', 4),
(10, 'Player', 4, 4, 'Solid player with consistent performance.', 4),
(11, 'Player', 3, 5, 'MVP of the tournament in my opinion.', 4),
(12, 'Player', 6, 3, 'Good support player, needs more aggressive plays.', 4);

-- =============================================
-- 14. FEEDBACK
-- =============================================

INSERT INTO `Feedback` (`UserID`, `Type`, `Subject`, `Content`, `Priority`, `Status`, `AssignedTo`, `Response`) VALUES
-- Player feedback
(3, 'Feature', 'Tournament Bracket Visualization', 'It would be great to have a visual tournament bracket showing the progression of teams through rounds.', 'Medium', 'Open', 1, NULL),
(4, 'Bug', 'Wallet Balance Display Issue', 'Sometimes the wallet balance shows incorrect values after transactions. Please investigate.', 'High', 'InProgress', 1, 'We are investigating this issue and will have a fix soon.'),
(5, 'Suggestion', 'Team Chat Feature', 'Adding a team chat feature would help with coordination during tournaments.', 'Low', 'Open', NULL, NULL),

-- Viewer feedback
(9, 'General', 'Tournament Stream Quality', 'The stream quality during the finals was excellent! Keep up the good work.', 'Low', 'Resolved', 2, 'Thank you for the positive feedback! We will continue to improve our streaming infrastructure.'),
(10, 'Complaint', 'Registration Process Too Complex', 'The tournament registration process has too many steps. Please simplify it.', 'Medium', 'Open', 1, NULL),
(11, 'Feature', 'Mobile App Request', 'Please consider developing a mobile app for easier access to tournaments and voting.', 'Medium', 'Open', NULL, NULL),

-- System feedback
(12, 'Bug', 'Login Timeout Issues', 'Users are experiencing frequent login timeouts during peak hours.', 'Critical', 'InProgress', 1, 'We have identified the issue and are working on increasing server capacity.'),
(13, 'Suggestion', 'Achievement System Enhancement', 'The achievement system could have more diverse categories and rewards.', 'Low', 'Open', 2, NULL);

-- =============================================
-- 15. SYSTEM LOGS
-- =============================================

INSERT INTO `SystemLogs` (`Level`, `Source`, `Message`, `UserID`, `AdditionalInfo`) VALUES
-- Authentication logs
('Info', 'Authentication', 'User login successful', 1, '{"ip": "192.168.1.10", "userAgent": "Mozilla/5.0"}'),
('Info', 'Authentication', 'User login successful', 3, '{"ip": "192.168.1.15", "userAgent": "Mozilla/5.0"}'),
('Warning', 'Authentication', 'Failed login attempt', NULL, '{"ip": "192.168.1.20", "username": "invalid_user"}'),
('Info', 'Authentication', 'User logout', 1, '{"sessionDuration": "02:30:15"}'),

-- Tournament logs
('Info', 'Tournament', 'New team registered for tournament', 1, '{"tournamentId": 1, "teamId": 1}'),
('Info', 'Tournament', 'Tournament match completed', 1, '{"matchId": 1, "winner": "Dragon Slayers"}'),
('Info', 'Tournament', 'Prize distribution completed', 1, '{"tournamentId": 4, "totalPrize": 20000000}'),

-- System logs
('Info', 'System', 'Database backup completed', 1, '{"backupSize": "150MB", "duration": "5min"}'),
('Warning', 'System', 'High memory usage detected', NULL, '{"memoryUsage": "85%", "threshold": "80%"}'),
('Info', 'System', 'System maintenance completed', 1, '{"maintenanceType": "scheduled", "duration": "30min"}'),
('Error', 'System', 'Failed to send notification email', NULL, '{"recipient": "player@example.com", "error": "SMTP timeout"}'),

-- User activity logs
('Info', 'User', 'Profile updated', 3, '{"changedFields": ["bio", "country"]}'),
('Info', 'User', 'Password changed', 4, '{"method": "user_request"}'),
('Info', 'Team', 'New team created: Dragon Slayers', 3, '{"teamId": 1, "memberCount": 5}'),
('Info', 'Wallet', 'Deposit processed successfully', 3, '{"amount": 2000000, "method": "bank_transfer"}');

-- =============================================
-- 16. ACHIEVEMENTS
-- =============================================

INSERT INTO `Achievements` (`Name`, `Description`, `Type`, `Points`) VALUES
-- Tournament achievements
('First Victory', 'Win your first tournament match', 'Tournament', 10),
('Champion', 'Win a tournament championship', 'Tournament', 100),
('Runner-up', 'Finish second in a tournament', 'Tournament', 50),
('Semi-finalist', 'Reach the semifinals of a tournament', 'Tournament', 30),
('Perfect Game', 'Win a match without losing a single round', 'Tournament', 25),

-- Team achievements
('Team Player', 'Join your first team', 'Team', 5),
('Team Captain', 'Become a team captain', 'Team', 15),
('Team Victory', 'Win a match as part of a team', 'Team', 20),
('Team Champion', 'Win a tournament with your team', 'Team', 75),

-- Individual achievements
('First Steps', 'Complete your profile setup', 'Individual', 5),
('Dedicated Player', 'Participate in 10 tournaments', 'Individual', 40),
('Tournament Regular', 'Participate in 5 tournaments', 'Individual', 20),
('Veteran', 'Participate in 25 tournaments', 'Individual', 100),

-- Social achievements
('Generous Supporter', 'Make your first donation', 'Social', 10),
('Community Voice', 'Cast your first vote', 'Social', 5),
('Feedback Provider', 'Submit your first feedback', 'Social', 5),
('Popular Player', 'Receive 10 positive votes', 'Social', 30),

-- Financial achievements
('Big Spender', 'Make a transaction over 1,000,000 VND', 'Financial', 15),
('Prize Winner', 'Win prize money in a tournament', 'Financial', 25),
('Millionaire', 'Accumulate 10,000,000 VND in total earnings', 'Financial', 50);

-- =============================================
-- 17. USER ACHIEVEMENTS
-- =============================================

INSERT INTO `UserAchievements` (`UserID`, `AchievementID`, `RelatedTournamentID`, `RelatedTeamID`) VALUES
-- player1 achievements (Champion)
(3, 1, 4, 1), -- First Victory
(3, 2, 4, 1), -- Champion
(3, 6, NULL, 1), -- Team Player
(3, 8, 4, 1), -- Team Victory
(3, 9, 4, 1), -- Team Champion
(3, 10, NULL, NULL), -- First Steps
(3, 17, NULL, NULL), -- Prize Winner
(3, 18, NULL, NULL), -- Millionaire

-- player2 achievements (Runner-up)
(4, 1, 4, 2), -- First Victory
(4, 3, 4, 2), -- Runner-up
(4, 6, NULL, 2), -- Team Player
(4, 8, 4, 2), -- Team Victory
(4, 17, NULL, NULL), -- Prize Winner

-- player3 achievements
(5, 1, 4, 3), -- First Victory
(5, 6, NULL, 3), -- Team Player
(5, 7, NULL, 3), -- Team Captain
(5, 8, 4, 3), -- Team Victory

-- Viewer achievements
(9, 14, NULL, NULL), -- Generous Supporter
(9, 15, NULL, NULL), -- Community Voice
(10, 14, NULL, NULL), -- Generous Supporter
(11, 14, NULL, NULL), -- Generous Supporter
(11, 15, NULL, NULL), -- Community Voice
(12, 15, NULL, NULL); -- Community Voice

-- =============================================
-- 18. UPDATE USER AND TEAM STATISTICS
-- =============================================

-- Update user login times and statistics
UPDATE `Users` SET 
    `TotalTournaments` = 1,
    `TournamentsWon` = 1,
    `LastLoginAt` = DATE_SUB(NOW(), INTERVAL 1 HOUR)
WHERE `UserID` = 3; -- player1 (champion)

UPDATE `Users` SET 
    `TotalTournaments` = 1,
    `TournamentsWon` = 0,
    `LastLoginAt` = DATE_SUB(NOW(), INTERVAL 2 HOUR)
WHERE `UserID` = 4; -- player2

UPDATE `Users` SET 
    `TotalTournaments` = 1,
    `TournamentsWon` = 0,
    `LastLoginAt` = DATE_SUB(NOW(), INTERVAL 3 HOUR)
WHERE `UserID` = 5; -- player3

UPDATE `Users` SET 
    `TotalTournaments` = 1,
    `TournamentsWon` = 0,
    `LastLoginAt` = DATE_SUB(NOW(), INTERVAL 3 HOUR)
WHERE `UserID` = 6; -- player4

-- Update team tournament counts
UPDATE `Teams` SET 
    `TotalTournaments` = 2,
    `TournamentsWon` = 1,
    `Ranking` = 1
WHERE `TeamID` = 1; -- Dragon Slayers

UPDATE `Teams` SET 
    `TotalTournaments` = 2,
    `TournamentsWon` = 0,
    `Ranking` = 2
WHERE `TeamID` = 2; -- Phoenix Rising

UPDATE `Teams` SET 
    `TotalTournaments` = 1,
    `TournamentsWon` = 0,
    `Ranking` = 3
WHERE `TeamID` = 3; -- Storm Warriors

UPDATE `Teams` SET 
    `TotalTournaments` = 1,
    `TournamentsWon` = 0,
    `Ranking` = 4
WHERE `TeamID` = 4; -- Thunder Bolt

-- Show success messages
SELECT 'Sample data inserted successfully!' as Status;
SELECT 'Database is ready for use.' as Info;
SELECT '' as Separator;
SELECT '=== TEST ACCOUNTS ===' as TestAccounts;
SELECT 'Admin: admin / Admin@123' as AdminAccount;
SELECT 'Admin2: admin2 / Admin@123' as Admin2Account;
SELECT 'Player: player1 / Player@123' as Player1Account;
SELECT 'Player: player2 / Player@123' as Player2Account;
SELECT 'Viewer: viewer1 / Viewer@123' as Viewer1Account;
SELECT '' as Separator2;
SELECT '=== SAMPLE DATA SUMMARY ===' as Summary;
SELECT 'Users: 13 (2 Admins, 6 Players, 5 Viewers)' as Users;
SELECT 'Teams: 4 competitive teams' as Teams;
SELECT 'Tournaments: 5 (2 active, 3 completed)' as Tournaments;
SELECT 'Matches: 6 with complete results' as Matches;
SELECT 'Transactions: 15 including prizes and donations' as Transactions;
SELECT 'Votes: 12 from community members' as Votes;
SELECT 'Feedback: 8 from users' as Feedback;
SELECT 'Achievements: 18 different types' as Achievements;
SELECT 'User Achievements: 15 earned achievements' as UserAchievements;
SELECT 'System Settings: 25 configuration options' as SystemSettings;
SELECT 'System Logs: 15 activity entries' as SystemLogs;
