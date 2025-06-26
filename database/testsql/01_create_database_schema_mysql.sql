-- =============================================
-- EsportsManager Database Schema for MySQL
-- Created: 2025-06-26
-- Version: 2.1 - Complete and Optimized for MySQL (Fixed)
-- =============================================

-- Drop database if exists (with warning suppression)
DROP DATABASE IF EXISTS `EsportsManager`;

-- Create database with proper charset
CREATE DATABASE `EsportsManager` 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE `EsportsManager`;

-- =============================================
-- 1. CORE TABLES
-- =============================================

-- Games Table
CREATE TABLE `Games` (
    `GameID` INT AUTO_INCREMENT PRIMARY KEY,
    `GameName` VARCHAR(100) NOT NULL UNIQUE,
    `Description` TEXT,
    `Genre` VARCHAR(50),
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `CreatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Users Table (Enhanced)
CREATE TABLE `Users` (
    `UserID` INT AUTO_INCREMENT PRIMARY KEY,
    `Username` VARCHAR(50) NOT NULL UNIQUE,
    `PasswordHash` VARCHAR(255) NOT NULL,
    `Email` VARCHAR(100) NOT NULL UNIQUE,
    `FullName` VARCHAR(100),
    `PhoneNumber` VARCHAR(20),
    `Role` ENUM('Admin', 'Player', 'Viewer') NOT NULL DEFAULT 'Player',
    `Status` ENUM('Active', 'Pending', 'Suspended', 'Deleted') NOT NULL DEFAULT 'Active',
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `EmailVerified` BOOLEAN NOT NULL DEFAULT FALSE,
    `PhoneVerified` BOOLEAN NOT NULL DEFAULT FALSE,
    `ProfileImageUrl` VARCHAR(500),
    `Bio` TEXT,
    `Country` VARCHAR(50),
    `DateOfBirth` DATE,
    `PreferredLanguage` VARCHAR(10) DEFAULT 'vi-VN',
    `TotalTournaments` INT NOT NULL DEFAULT 0,
    `TournamentsWon` INT NOT NULL DEFAULT 0,
    `SecurityQuestion` TEXT,
    `SecurityAnswerHash` VARCHAR(255),
    `CreatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `LastLoginAt` TIMESTAMP NULL,
    `IsDeleted` BOOLEAN NOT NULL DEFAULT FALSE
);

-- Teams Table (Enhanced)
CREATE TABLE `Teams` (
    `TeamID` INT AUTO_INCREMENT PRIMARY KEY,
    `TeamName` VARCHAR(100) NOT NULL UNIQUE,
    `Description` TEXT,
    `LogoUrl` VARCHAR(500),
    `CaptainID` INT NOT NULL,
    `MaxMembers` INT NOT NULL DEFAULT 5,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `CreatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `TotalTournaments` INT NOT NULL DEFAULT 0,
    `TournamentsWon` INT NOT NULL DEFAULT 0,
    `Ranking` INT,
    `IsDeleted` BOOLEAN NOT NULL DEFAULT FALSE,
    FOREIGN KEY (`CaptainID`) REFERENCES `Users`(`UserID`)
);

-- TeamMembers Table
CREATE TABLE `TeamMembers` (
    `TeamMemberID` INT AUTO_INCREMENT PRIMARY KEY,
    `TeamID` INT NOT NULL,
    `UserID` INT NOT NULL,
    `Role` VARCHAR(50) NOT NULL DEFAULT 'Member',
    `JoinedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    FOREIGN KEY (`TeamID`) REFERENCES `Teams`(`TeamID`),
    FOREIGN KEY (`UserID`) REFERENCES `Users`(`UserID`),
    UNIQUE KEY `UQ_TeamMembers_TeamUser` (`TeamID`, `UserID`)
);

-- Tournaments Table (Enhanced)
CREATE TABLE `Tournaments` (
    `TournamentID` INT AUTO_INCREMENT PRIMARY KEY,
    `TournamentName` VARCHAR(100) NOT NULL,
    `Description` TEXT,
    `GameID` INT NOT NULL,
    `MaxTeams` INT NOT NULL DEFAULT 16,
    `MaxPlayersPerTeam` INT NOT NULL DEFAULT 5,
    `PrizePool` DECIMAL(18,2) NOT NULL DEFAULT 0,
    `EntryFee` DECIMAL(18,2) NOT NULL DEFAULT 0,
    `StartDate` TIMESTAMP NOT NULL,
    `EndDate` TIMESTAMP NOT NULL,
    `RegistrationStartDate` TIMESTAMP NOT NULL,
    `RegistrationEndDate` TIMESTAMP NOT NULL,
    `Status` ENUM('Draft', 'Registration', 'InProgress', 'Completed', 'Cancelled') NOT NULL DEFAULT 'Draft',
    `TournamentType` ENUM('Elimination', 'RoundRobin', 'Swiss') NOT NULL DEFAULT 'Elimination',
    `IsPublic` BOOLEAN NOT NULL DEFAULT TRUE,
    `RequireApproval` BOOLEAN NOT NULL DEFAULT FALSE,
    `AllowVoting` BOOLEAN NOT NULL DEFAULT TRUE,
    `Rules` TEXT,
    `BannerImageUrl` VARCHAR(500),
    `CreatedBy` INT NOT NULL,
    `CreatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `IsDeleted` BOOLEAN NOT NULL DEFAULT FALSE,
    FOREIGN KEY (`GameID`) REFERENCES `Games`(`GameID`),
    FOREIGN KEY (`CreatedBy`) REFERENCES `Users`(`UserID`)
);

-- =============================================
-- 2. TOURNAMENT MANAGEMENT TABLES
-- =============================================

-- TournamentTeams Table
CREATE TABLE `TournamentTeams` (
    `TournamentTeamID` INT AUTO_INCREMENT PRIMARY KEY,
    `TournamentID` INT NOT NULL,
    `TeamID` INT NOT NULL,
    `RegistrationDate` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `Status` ENUM('Pending', 'Approved', 'Rejected', 'Withdrawn') NOT NULL DEFAULT 'Pending',
    `SeedNumber` INT,
    `GroupNumber` INT,
    `ApprovedBy` INT,
    `ApprovedAt` TIMESTAMP NULL,
    `Notes` TEXT,
    FOREIGN KEY (`TournamentID`) REFERENCES `Tournaments`(`TournamentID`),
    FOREIGN KEY (`TeamID`) REFERENCES `Teams`(`TeamID`),
    FOREIGN KEY (`ApprovedBy`) REFERENCES `Users`(`UserID`),
    UNIQUE KEY `UQ_TournamentTeams` (`TournamentID`, `TeamID`)
);

-- Matches Table (Enhanced)
CREATE TABLE `Matches` (
    `MatchID` INT AUTO_INCREMENT PRIMARY KEY,
    `TournamentID` INT NOT NULL,
    `Team1ID` INT NOT NULL,
    `Team2ID` INT NOT NULL,
    `ScheduledDate` TIMESTAMP NOT NULL,
    `ActualStartTime` TIMESTAMP NULL,
    `ActualEndTime` TIMESTAMP NULL,
    `Status` ENUM('Scheduled', 'InProgress', 'Completed', 'Cancelled', 'Postponed') NOT NULL DEFAULT 'Scheduled',
    `WinnerTeamID` INT,
    `Team1Score` INT DEFAULT 0,
    `Team2Score` INT DEFAULT 0,
    `Round` VARCHAR(50),
    `MatchType` ENUM('Regular', 'Semifinal', 'Final', 'ThirdPlace') DEFAULT 'Regular',
    `VenueLocation` VARCHAR(200),
    `StreamUrl` VARCHAR(500),
    `Notes` TEXT,
    `CreatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (`TournamentID`) REFERENCES `Tournaments`(`TournamentID`),
    FOREIGN KEY (`Team1ID`) REFERENCES `Teams`(`TeamID`),
    FOREIGN KEY (`Team2ID`) REFERENCES `Teams`(`TeamID`),
    FOREIGN KEY (`WinnerTeamID`) REFERENCES `Teams`(`TeamID`)
);

-- TournamentResults Table
CREATE TABLE `TournamentResults` (
    `ResultID` INT AUTO_INCREMENT PRIMARY KEY,
    `TournamentID` INT NOT NULL,
    `TeamID` INT NOT NULL,
    `Rank` INT NOT NULL,
    `Prize` DECIMAL(18,2) DEFAULT 0,
    `Points` INT DEFAULT 0,
    `Wins` INT DEFAULT 0,
    `Losses` INT DEFAULT 0,
    `TotalMatches` INT DEFAULT 0,
    `Notes` TEXT,
    `CreatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (`TournamentID`) REFERENCES `Tournaments`(`TournamentID`),
    FOREIGN KEY (`TeamID`) REFERENCES `Teams`(`TeamID`),
    UNIQUE KEY `UQ_TournamentResults` (`TournamentID`, `TeamID`)
);

-- =============================================
-- 3. FINANCIAL SYSTEM TABLES
-- =============================================

-- Wallets Table (Enhanced)
CREATE TABLE `Wallets` (
    `WalletID` INT AUTO_INCREMENT PRIMARY KEY,
    `UserID` INT NOT NULL,
    `Balance` DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    `PendingBalance` DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    `TotalDeposited` DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    `TotalWithdrawn` DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    `TotalDonated` DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    `TotalReceived` DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    `Currency` VARCHAR(3) NOT NULL DEFAULT 'VND',
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `IsFrozen` BOOLEAN NOT NULL DEFAULT FALSE,
    `CreatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (`UserID`) REFERENCES `Users`(`UserID`),
    UNIQUE KEY `UQ_Wallets_User` (`UserID`)
);

-- Transactions Table (Enhanced)
CREATE TABLE `Transactions` (
    `TransactionID` INT AUTO_INCREMENT PRIMARY KEY,
    `WalletID` INT NOT NULL,
    `Type` ENUM('Deposit', 'Withdrawal', 'Prize', 'Fee', 'Donation', 'Refund', 'Transfer') NOT NULL,
    `Amount` DECIMAL(18,2) NOT NULL,
    `Currency` VARCHAR(3) NOT NULL DEFAULT 'VND',
    `Status` ENUM('Pending', 'Completed', 'Failed', 'Cancelled', 'Processing') NOT NULL DEFAULT 'Pending',
    `Description` TEXT,
    `ReferenceID` VARCHAR(100),
    `RelatedTournamentID` INT,
    `RelatedUserID` INT,
    `ProcessedBy` INT,
    `ProcessedAt` TIMESTAMP NULL,
    `CreatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `ExternalTransactionID` VARCHAR(100),
    `PaymentMethod` VARCHAR(50),
    `PaymentDetails` JSON,
    FOREIGN KEY (`WalletID`) REFERENCES `Wallets`(`WalletID`),
    FOREIGN KEY (`RelatedTournamentID`) REFERENCES `Tournaments`(`TournamentID`),
    FOREIGN KEY (`RelatedUserID`) REFERENCES `Users`(`UserID`),
    FOREIGN KEY (`ProcessedBy`) REFERENCES `Users`(`UserID`)
);

-- Donations Table (Enhanced)
CREATE TABLE `Donations` (
    `DonationID` INT AUTO_INCREMENT PRIMARY KEY,
    `DonorUserID` INT NOT NULL,
    `RecipientUserID` INT,
    `RecipientTeamID` INT,
    `TournamentID` INT,
    `Amount` DECIMAL(18,2) NOT NULL,
    `Currency` VARCHAR(3) NOT NULL DEFAULT 'VND',
    `Message` TEXT,
    `IsAnonymous` BOOLEAN NOT NULL DEFAULT FALSE,
    `Status` ENUM('Pending', 'Completed', 'Failed', 'Refunded') NOT NULL DEFAULT 'Pending',
    `TransactionID` INT,
    `CreatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `ProcessedAt` TIMESTAMP NULL,
    FOREIGN KEY (`DonorUserID`) REFERENCES `Users`(`UserID`),
    FOREIGN KEY (`RecipientUserID`) REFERENCES `Users`(`UserID`),
    FOREIGN KEY (`RecipientTeamID`) REFERENCES `Teams`(`TeamID`),
    FOREIGN KEY (`TournamentID`) REFERENCES `Tournaments`(`TournamentID`),
    FOREIGN KEY (`TransactionID`) REFERENCES `Transactions`(`TransactionID`)
);

-- =============================================
-- 4. VOTING SYSTEM TABLES
-- =============================================

-- Votes Table (Enhanced)
CREATE TABLE `Votes` (
    `VoteID` INT AUTO_INCREMENT PRIMARY KEY,
    `VoterUserID` INT NOT NULL,
    `VoteType` ENUM('Tournament', 'Team', 'Player', 'Match') NOT NULL,
    `TargetID` INT NOT NULL,
    `Rating` INT CHECK (`Rating` >= 1 AND `Rating` <= 5),
    `Comment` TEXT,
    `IsAnonymous` BOOLEAN NOT NULL DEFAULT FALSE,
    `TournamentID` INT,
    `CreatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `IsDeleted` BOOLEAN NOT NULL DEFAULT FALSE,
    FOREIGN KEY (`VoterUserID`) REFERENCES `Users`(`UserID`),
    FOREIGN KEY (`TournamentID`) REFERENCES `Tournaments`(`TournamentID`),
    UNIQUE KEY `UQ_Votes_UserTypeTarget` (`VoterUserID`, `VoteType`, `TargetID`)
);

-- =============================================
-- 5. FEEDBACK SYSTEM TABLES
-- =============================================

-- Feedback Table (Enhanced)
CREATE TABLE `Feedback` (
    `FeedbackID` INT AUTO_INCREMENT PRIMARY KEY,
    `UserID` INT NOT NULL,
    `Type` ENUM('Bug', 'Feature', 'General', 'Complaint', 'Suggestion') NOT NULL,
    `Subject` VARCHAR(200) NOT NULL,
    `Content` TEXT NOT NULL,
    `Priority` ENUM('Low', 'Medium', 'High', 'Critical') NOT NULL DEFAULT 'Medium',
    `Status` ENUM('Open', 'InProgress', 'Resolved', 'Closed', 'Rejected') NOT NULL DEFAULT 'Open',
    `Category` VARCHAR(50),
    `AssignedTo` INT,
    `Response` TEXT,
    `ResolutionDate` TIMESTAMP NULL,
    `CreatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `IsDeleted` BOOLEAN NOT NULL DEFAULT FALSE,
    FOREIGN KEY (`UserID`) REFERENCES `Users`(`UserID`),
    FOREIGN KEY (`AssignedTo`) REFERENCES `Users`(`UserID`)
);

-- =============================================
-- 6. SYSTEM CONFIGURATION TABLES
-- =============================================

-- SystemSettings Table (Enhanced)
CREATE TABLE `SystemSettings` (
    `SettingID` INT AUTO_INCREMENT PRIMARY KEY,
    `SettingKey` VARCHAR(100) NOT NULL UNIQUE,
    `SettingValue` TEXT NOT NULL,
    `Description` TEXT,
    `Category` VARCHAR(50) NOT NULL DEFAULT 'General',
    `DataType` ENUM('String', 'Integer', 'Decimal', 'Boolean', 'DateTime', 'JSON') NOT NULL DEFAULT 'String',
    `IsReadOnly` BOOLEAN NOT NULL DEFAULT FALSE,
    `CreatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `UpdatedBy` INT,
    FOREIGN KEY (`UpdatedBy`) REFERENCES `Users`(`UserID`)
);

-- SystemLogs Table (Enhanced)
CREATE TABLE `SystemLogs` (
    `LogID` INT AUTO_INCREMENT PRIMARY KEY,
    `Level` ENUM('Debug', 'Info', 'Warning', 'Error', 'Critical') NOT NULL,
    `Source` VARCHAR(100) NOT NULL,
    `Message` TEXT NOT NULL,
    `Exception` TEXT,
    `UserID` INT,
    `IPAddress` VARCHAR(45),
    `UserAgent` TEXT,
    `RequestPath` VARCHAR(500),
    `AdditionalData` JSON,
    `Timestamp` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (`UserID`) REFERENCES `Users`(`UserID`)
);

-- =============================================
-- 7. ACHIEVEMENT SYSTEM TABLES
-- =============================================

-- Achievements Table
CREATE TABLE `Achievements` (
    `AchievementID` INT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(100) NOT NULL,
    `Description` TEXT NOT NULL,
    `Type` ENUM('Tournament', 'Team', 'Individual', 'Social', 'Financial') NOT NULL,
    `Criteria` TEXT,
    `BadgeImageUrl` VARCHAR(500),
    `Points` INT NOT NULL DEFAULT 0,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `CreatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- UserAchievements Table
CREATE TABLE `UserAchievements` (
    `UserAchievementID` INT AUTO_INCREMENT PRIMARY KEY,
    `UserID` INT NOT NULL,
    `AchievementID` INT NOT NULL,
    `EarnedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `RelatedTournamentID` INT,
    `RelatedTeamID` INT,
    `Notes` TEXT,
    FOREIGN KEY (`UserID`) REFERENCES `Users`(`UserID`),
    FOREIGN KEY (`AchievementID`) REFERENCES `Achievements`(`AchievementID`),
    FOREIGN KEY (`RelatedTournamentID`) REFERENCES `Tournaments`(`TournamentID`),
    FOREIGN KEY (`RelatedTeamID`) REFERENCES `Teams`(`TeamID`),
    UNIQUE KEY `UQ_UserAchievements` (`UserID`, `AchievementID`)
);

-- =============================================
-- 8. INDEXES FOR PERFORMANCE
-- =============================================

-- Users indexes
CREATE INDEX `IX_Users_Email` ON `Users`(`Email`);
CREATE INDEX `IX_Users_Username` ON `Users`(`Username`);
CREATE INDEX `IX_Users_Role` ON `Users`(`Role`);
CREATE INDEX `IX_Users_Status` ON `Users`(`Status`);
CREATE INDEX `IX_Users_IsActive` ON `Users`(`IsActive`);
CREATE INDEX `IX_Users_LastLoginAt` ON `Users`(`LastLoginAt`);

-- Teams indexes
CREATE INDEX `IX_Teams_CaptainID` ON `Teams`(`CaptainID`);
CREATE INDEX `IX_Teams_IsActive` ON `Teams`(`IsActive`);
CREATE INDEX `IX_Teams_Ranking` ON `Teams`(`Ranking`);

-- Tournaments indexes
CREATE INDEX `IX_Tournaments_GameID` ON `Tournaments`(`GameID`);
CREATE INDEX `IX_Tournaments_Status` ON `Tournaments`(`Status`);
CREATE INDEX `IX_Tournaments_StartDate` ON `Tournaments`(`StartDate`);
CREATE INDEX `IX_Tournaments_CreatedBy` ON `Tournaments`(`CreatedBy`);

-- Matches indexes
CREATE INDEX `IX_Matches_TournamentID` ON `Matches`(`TournamentID`);
CREATE INDEX `IX_Matches_ScheduledDate` ON `Matches`(`ScheduledDate`);
CREATE INDEX `IX_Matches_Status` ON `Matches`(`Status`);

-- Wallets indexes
CREATE INDEX `IX_Wallets_UserID` ON `Wallets`(`UserID`);
CREATE INDEX `IX_Wallets_IsActive` ON `Wallets`(`IsActive`);

-- Transactions indexes
CREATE INDEX `IX_Transactions_WalletID` ON `Transactions`(`WalletID`);
CREATE INDEX `IX_Transactions_Type` ON `Transactions`(`Type`);
CREATE INDEX `IX_Transactions_Status` ON `Transactions`(`Status`);
CREATE INDEX `IX_Transactions_CreatedAt` ON `Transactions`(`CreatedAt`);

-- Votes indexes
CREATE INDEX `IX_Votes_VoterUserID` ON `Votes`(`VoterUserID`);
CREATE INDEX `IX_Votes_VoteType` ON `Votes`(`VoteType`);
CREATE INDEX `IX_Votes_TargetID` ON `Votes`(`TargetID`);
CREATE INDEX `IX_Votes_TournamentID` ON `Votes`(`TournamentID`);

-- Feedback indexes
CREATE INDEX `IX_Feedback_UserID` ON `Feedback`(`UserID`);
CREATE INDEX `IX_Feedback_Type` ON `Feedback`(`Type`);
CREATE INDEX `IX_Feedback_Status` ON `Feedback`(`Status`);
CREATE INDEX `IX_Feedback_CreatedAt` ON `Feedback`(`CreatedAt`);

-- System logs indexes
CREATE INDEX `IX_SystemLogs_Level` ON `SystemLogs`(`Level`);
CREATE INDEX `IX_SystemLogs_Source` ON `SystemLogs`(`Source`);
CREATE INDEX `IX_SystemLogs_Timestamp` ON `SystemLogs`(`Timestamp`);
CREATE INDEX `IX_SystemLogs_UserID` ON `SystemLogs`(`UserID`);

-- Achievements indexes
CREATE INDEX `IX_Achievements_Type` ON `Achievements`(`Type`);
CREATE INDEX `IX_Achievements_IsActive` ON `Achievements`(`IsActive`);
CREATE INDEX `IX_UserAchievements_UserID` ON `UserAchievements`(`UserID`);
CREATE INDEX `IX_UserAchievements_AchievementID` ON `UserAchievements`(`AchievementID`);

-- Show success message
SELECT 'Database schema created successfully!' as `Status`;
SELECT 'Tables created: Games, Users, Teams, TeamMembers, Tournaments, TournamentTeams, Matches, TournamentResults' as `Info`;
SELECT 'Financial: Wallets, Transactions, Donations' as `Financial`;
SELECT 'Social: Votes, Feedback' as `Social`;
SELECT 'System: SystemSettings, SystemLogs' as `SystemInfo`;
SELECT 'Achievements: Achievements, UserAchievements' as `Achievements`;
SELECT 'All indexes and constraints are in place.' as `Final`;
