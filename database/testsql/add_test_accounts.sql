-- =============================================
-- Add More Test Accounts Script
-- This script adds additional test accounts for development
-- Run this after the main sample data script
-- =============================================

USE `EsportsManager`;

-- =============================================
-- ADDITIONAL TEST USERS
-- =============================================

-- Add more admin accounts
INSERT INTO `Users` (`Username`, `PasswordHash`, `Email`, `FullName`, `Role`, `Status`, `IsActive`, `EmailVerified`, `PhoneVerified`, `Bio`, `Country`, `TotalTournaments`, `TournamentsWon`, `SecurityQuestion`, `SecurityAnswerHash`) VALUES
-- Additional Admin Accounts
('superadmin', '$2a$11$Qp8zPYGrKbmGFSg4j0C9aOhxJ1ZaR8rklKpK8uNklkYJnKRD/IlYe', 'superadmin@esportsmanager.com', 'Super Administrator', 'Admin', 'Active', TRUE, TRUE, FALSE, 'Super administrator with full system access', 'Vietnam', 0, 0, 'What is your favorite esports game?', '$2a$11$Qp8zPYGrKbmGFSg4j0C9aOhxJ1ZaR8rklKpK8uNklkYJnKRD/IlYe'),

('moderator', '$2a$11$Qp8zPYGrKbmGFSg4j0C9aOhxJ1ZaR8rklKpK8uNklkYJnKRD/IlYe', 'moderator@esportsmanager.com', 'System Moderator', 'Admin', 'Active', TRUE, TRUE, FALSE, 'Community moderator and support specialist', 'Vietnam', 0, 0, 'What is your favorite esports game?', '$2a$11$Qp8zPYGrKbmGFSg4j0C9aOhxJ1ZaR8rklKpK8uNklkYJnKRD/IlYe'),

-- Additional Player Accounts (Professional Level)
('progamer1', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa', 'progamer1@esports.vn', 'Nguyen Pro Gamer', 'Player', 'Active', TRUE, TRUE, FALSE, 'Professional League of Legends player, former world champion', 'Vietnam', 5, 3, 'What is your birth year?', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa'),

('fpsmaster', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa', 'fpsmaster@esports.vn', 'Tran FPS Master', 'Player', 'Active', TRUE, TRUE, FALSE, 'Professional CS2 and Valorant player, tournament MVP multiple times', 'Vietnam', 8, 2, 'What is your birth year?', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa'),

('strategist', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa', 'strategist@esports.vn', 'Le Strategy Master', 'Player', 'Active', TRUE, TRUE, FALSE, 'Team captain and strategic analyst, specializes in Dota 2', 'Vietnam', 6, 1, 'What is your birth year?', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa'),

-- Rookie Players (New to competitive scene)
('rookie1', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa', 'rookie1@gmail.com', 'Pham Rookie One', 'Player', 'Active', TRUE, TRUE, FALSE, 'New to competitive gaming, eager to learn and improve', 'Vietnam', 0, 0, 'What is your birth year?', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa'),

('rookie2', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa', 'rookie2@gmail.com', 'Hoang Rookie Two', 'Player', 'Active', TRUE, TRUE, FALSE, 'Aspiring esports player, currently practicing daily', 'Vietnam', 0, 0, 'What is your birth year?', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa'),

('rising_star', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa', 'rising.star@gmail.com', 'Bui Rising Star', 'Player', 'Active', TRUE, TRUE, FALSE, 'Up-and-coming player with high potential', 'Vietnam', 1, 0, 'What is your birth year?', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa'),

-- Content Creators and Streamers
('streamer1', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb', 'streamer1@twitch.com', 'Nguyen Stream King', 'Viewer', 'Active', TRUE, TRUE, FALSE, 'Popular gaming streamer with 100K+ followers', 'Vietnam', 0, 0, 'What is your favorite color?', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb'),

('youtuber1', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb', 'youtuber1@youtube.com', 'Tran Gaming Channel', 'Viewer', 'Active', TRUE, TRUE, FALSE, 'Gaming content creator and tournament highlight maker', 'Vietnam', 0, 0, 'What is your favorite color?', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb'),

('caster1', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb', 'caster1@esports.vn', 'Le Professional Caster', 'Viewer', 'Active', TRUE, TRUE, FALSE, 'Professional esports commentator and analyst', 'Vietnam', 0, 0, 'What is your favorite color?', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb'),

-- Tournament Organizers and Sponsors
('organizer1', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb', 'organizer1@tournaments.vn', 'Pham Event Organizer', 'Viewer', 'Active', TRUE, TRUE, FALSE, 'Professional tournament organizer and event manager', 'Vietnam', 0, 0, 'What is your favorite color?', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb'),

('sponsor1', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb', 'sponsor1@company.vn', 'Hoang Big Sponsor', 'Viewer', 'Active', TRUE, TRUE, FALSE, 'Corporate sponsor supporting esports development', 'Vietnam', 0, 0, 'What is your favorite color?', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb'),

-- Gaming Journalists and Critics
('journalist1', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb', 'journalist1@gamingnews.vn', 'Bui Gaming Journalist', 'Viewer', 'Active', TRUE, TRUE, FALSE, 'Esports journalist covering major tournaments and events', 'Vietnam', 0, 0, 'What is your favorite color?', '$2a$11$GHh4Kl2m8qF9XcDe3sT4rOhVy7Nx5M9B.6zAg/nE2CvKtS7qRk3mb'),

-- Test Accounts for Different Statuses
('pending_user', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa', 'pending@test.com', 'Test Pending User', 'Player', 'Pending', TRUE, FALSE, FALSE, 'Test account with pending status', 'Vietnam', 0, 0, 'What is your birth year?', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa'),

('suspended_user', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa', 'suspended@test.com', 'Test Suspended User', 'Player', 'Suspended', FALSE, TRUE, FALSE, 'Test account with suspended status', 'Vietnam', 0, 0, 'What is your birth year?', '$2a$11$LRc8JWvF2s3aXoFB4yTm5OVk6Zyt8N3d.2wBh/hD9FkJrP8mLk6sa');

-- =============================================
-- CREATE WALLETS FOR NEW USERS
-- =============================================

-- Get the UserIDs of newly created users and create wallets
INSERT INTO `Wallets` (`UserID`, `Balance`, `TotalDeposited`, `TotalWithdrawn`, `TotalDonated`, `TotalReceived`) 
SELECT `UserID`, 0.00, 0.00, 0.00, 0.00, 0.00 
FROM `Users` 
WHERE `Username` IN ('superadmin', 'moderator', 'progamer1', 'fpsmaster', 'strategist', 'rookie1', 'rookie2', 'rising_star', 'streamer1', 'youtuber1', 'caster1', 'organizer1', 'sponsor1', 'journalist1', 'pending_user', 'suspended_user');

-- Add some initial balance for professional players
UPDATE `Wallets` SET 
    `Balance` = 5000000.00,
    `TotalDeposited` = 5000000.00
WHERE `UserID` IN (
    SELECT `UserID` FROM `Users` WHERE `Username` IN ('progamer1', 'fpsmaster', 'strategist')
);

-- Add initial balance for content creators and sponsors
UPDATE `Wallets` SET 
    `Balance` = 2000000.00,
    `TotalDeposited` = 2000000.00
WHERE `UserID` IN (
    SELECT `UserID` FROM `Users` WHERE `Username` IN ('streamer1', 'youtuber1', 'sponsor1', 'organizer1')
);

-- Add small balance for rookies
UPDATE `Wallets` SET 
    `Balance` = 100000.00,
    `TotalDeposited` = 100000.00
WHERE `UserID` IN (
    SELECT `UserID` FROM `Users` WHERE `Username` IN ('rookie1', 'rookie2', 'rising_star')
);

-- =============================================
-- SHOW SUMMARY OF NEW ACCOUNTS
-- =============================================

-- Show success message
SELECT 'NEW TEST ACCOUNTS CREATED SUCCESSFULLY!' as `Message`;

-- Show admin accounts
SELECT 'ADMIN ACCOUNTS' as `AccountType`, `Username`, 'Admin@123' as `Password`, `Role`, `Status`
FROM `Users` 
WHERE `Username` IN ('superadmin', 'moderator');

-- Show professional players
SELECT 'PROFESSIONAL PLAYERS' as `AccountType`, `Username`, 'Player@123' as `Password`, `Role`, `Status`
FROM `Users` 
WHERE `Username` IN ('progamer1', 'fpsmaster', 'strategist');

-- Show rookie players
SELECT 'ROOKIE PLAYERS' as `AccountType`, `Username`, 'Player@123' as `Password`, `Role`, `Status`
FROM `Users` 
WHERE `Username` IN ('rookie1', 'rookie2', 'rising_star');

-- Show content creators
SELECT 'CONTENT CREATORS' as `AccountType`, `Username`, 'Viewer@123' as `Password`, `Role`, `Status`
FROM `Users` 
WHERE `Username` IN ('streamer1', 'youtuber1', 'caster1');

-- Show industry professionals
SELECT 'INDUSTRY PROFESSIONALS' as `AccountType`, `Username`, 'Viewer@123' as `Password`, `Role`, `Status`
FROM `Users` 
WHERE `Username` IN ('organizer1', 'sponsor1', 'journalist1');

-- Show test status accounts
SELECT 'TEST STATUS ACCOUNTS' as `AccountType`, `Username`, 'Player@123' as `Password`, `Role`, `Status`
FROM `Users` 
WHERE `Username` IN ('pending_user', 'suspended_user');

-- Account count summary
SELECT 'ACCOUNT COUNT SUMMARY' as `SummaryType`;
SELECT 'Total Users' as `Metric`, COUNT(*) as `Count` FROM `Users`;
SELECT 'Admin Accounts' as `Metric`, COUNT(*) as `Count` FROM `Users` WHERE `Role` = 'Admin';
SELECT 'Player Accounts' as `Metric`, COUNT(*) as `Count` FROM `Users` WHERE `Role` = 'Player';
SELECT 'Viewer Accounts' as `Metric`, COUNT(*) as `Count` FROM `Users` WHERE `Role` = 'Viewer';
SELECT 'Active Accounts' as `Metric`, COUNT(*) as `Count` FROM `Users` WHERE `Status` = 'Active';
SELECT 'Pending Accounts' as `Metric`, COUNT(*) as `Count` FROM `Users` WHERE `Status` = 'Pending';
SELECT 'Suspended Accounts' as `Metric`, COUNT(*) as `Count` FROM `Users` WHERE `Status` = 'Suspended';

-- Wallet balance summary for professional players
SELECT 'WALLET BALANCES - PROFESSIONAL PLAYERS' as `BalanceType`;
SELECT u.`Username`, w.`Balance` as `Balance_VND`
FROM `Users` u 
JOIN `Wallets` w ON u.`UserID` = w.`UserID` 
WHERE u.`Username` IN ('progamer1', 'fpsmaster', 'strategist');

-- Wallet balance summary for content creators
SELECT 'WALLET BALANCES - CONTENT CREATORS' as `BalanceType`;
SELECT u.`Username`, w.`Balance` as `Balance_VND`
FROM `Users` u 
JOIN `Wallets` w ON u.`UserID` = w.`UserID` 
WHERE u.`Username` IN ('streamer1', 'youtuber1', 'sponsor1', 'organizer1');

-- Final success message
SELECT 'ALL TEST ACCOUNTS CREATED SUCCESSFULLY!' as `FinalMessage`;
SELECT 'You can now use these accounts for comprehensive testing.' as `Note`;
