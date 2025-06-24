-- =====================================================
-- SAMPLE DATA
-- =====================================================

USE EsportManager;

-- Insert sample games
INSERT INTO Games (GameName, Description, Genre) VALUES
('League of Legends', 'Most popular MOBA game in the world', 'MOBA'),
('Counter-Strike 2', 'Professional competitive FPS game', 'FPS'),
('Valorant', 'Tactical FPS game', 'FPS'),
('Dota 2', 'Professional MOBA game', 'MOBA'),
('FIFA 24', 'Football/Soccer simulation game', 'Sports'),
('Rocket League', 'Vehicular soccer game', 'Sports'),
('Overwatch 2', 'Team-based first-person shooter', 'FPS');

-- Insert admin accounts
-- Password hash corresponds to 'admin123' using bcrypt
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive) VALUES
('admin', '$2a$10$yGTZMMjfWyunReqDn.sZ1uMazm8Q.z7xYJYUkj50TBFKlJcX4X5F2', 'admin@esportmanager.com', 'System Administrator', 'Admin', 'Admin', TRUE),
('superadmin', '$2a$10$yGTZMMjfWyunReqDn.sZ1uMazm8Q.z7xYJYUkj50TBFKlJcX4X5F2', 'superadmin@esportmanager.com', 'Super Administrator', 'SuperAdmin', 'Admin', TRUE);

-- Insert sample players
-- Password hash corresponds to 'player123' using bcrypt
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive) VALUES
('player1', '$2a$10$biekkN19VAMsTLml2ihbfOVAdGf0nRtOD3h92cN6J1lKvEGRcXjzW', 'player1@test.com', 'Nguyen Van A', 'ProGamer1', 'Player', TRUE),
('player2', '$2a$10$biekkN19VAMsTLml2ihbfOVAdGf0nRtOD3h92cN6J1lKvEGRcXjzW', 'player2@test.com', 'Tran Thi B', 'ProGamer2', 'Player', TRUE),
('player3', '$2a$10$biekkN19VAMsTLml2ihbfOVAdGf0nRtOD3h92cN6J1lKvEGRcXjzW', 'player3@test.com', 'Le Van C', 'ProGamer3', 'Player', TRUE),
('player4', '$2a$10$biekkN19VAMsTLml2ihbfOVAdGf0nRtOD3h92cN6J1lKvEGRcXjzW', 'player4@test.com', 'Do Dinh D', 'ProGamer4', 'Player', TRUE),
('player5', '$2a$10$biekkN19VAMsTLml2ihbfOVAdGf0nRtOD3h92cN6J1lKvEGRcXjzW', 'player5@test.com', 'Pham Thi E', 'ProGamer5', 'Player', TRUE);

-- Insert sample viewers
-- Password hash corresponds to 'viewer123' using bcrypt
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive) VALUES
('viewer1', '$2a$10$msdkzPt9.U6vCFrNXTVbEeO1Vt0OB/BZZ7jyR9b8KY0oJz1aSpONW', 'viewer1@test.com', 'Hoang Van F', 'EsportsFan1', 'Viewer', TRUE),
('viewer2', '$2a$10$msdkzPt9.U6vCFrNXTVbEeO1Vt0OB/BZZ7jyR9b8KY0oJz1aSpONW', 'viewer2@test.com', 'Ngo Thi G', 'EsportsFan2', 'Viewer', TRUE),
('viewer3', '$2a$10$msdkzPt9.U6vCFrNXTVbEeO1Vt0OB/BZZ7jyR9b8KY0oJz1aSpONW', 'viewer3@test.com', 'Vuong Van H', 'EsportsFan3', 'Viewer', TRUE);

-- Update wallets for players (they're already created by the trigger)
UPDATE Wallets SET 
    Balance = 1000.00, 
    TotalReceived = 1500.00 
WHERE UserID = 3;

UPDATE Wallets SET 
    Balance = 750.00, 
    TotalReceived = 1000.00 
WHERE UserID = 4;

UPDATE Wallets SET 
    Balance = 500.00, 
    TotalReceived = 750.00 
WHERE UserID = 5;

UPDATE Wallets SET 
    Balance = 250.00, 
    TotalReceived = 500.00 
WHERE UserID = 6;

UPDATE Wallets SET 
    Balance = 100.00, 
    TotalReceived = 200.00 
WHERE UserID = 7;

-- Update profiles for users with more detailed information (they're already created by the trigger)
UPDATE UserProfiles SET
    DateOfBirth = '1990-01-15',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Ho Chi Minh City',
    Bio = 'System Administrator',
    AvatarURL = '/images/avatars/admin.png'
WHERE UserID = 1;

UPDATE UserProfiles SET
    DateOfBirth = '1988-05-20',
    Gender = 'Female',
    Country = 'Vietnam',
    City = 'Hanoi',
    Bio = 'Super Administrator',
    AvatarURL = '/images/avatars/superadmin.png'
WHERE UserID = 2;

UPDATE UserProfiles SET
    DateOfBirth = '1995-03-24',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Hanoi',
    Bio = 'Professional League of Legends player with 5 years experience',
    AvatarURL = '/images/avatars/player1.png'
WHERE UserID = 3;

UPDATE UserProfiles SET
    DateOfBirth = '1997-07-12',
    Gender = 'Female',
    Country = 'Vietnam',
    City = 'Ho Chi Minh City',
    Bio = 'Former CS:GO champion, now playing Valorant professionally',
    AvatarURL = '/images/avatars/player2.png'
WHERE UserID = 4;

UPDATE UserProfiles SET
    DateOfBirth = '1998-11-03',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Da Nang',
    Bio = 'Dota 2 specialist, twice finalist in national tournaments',
    AvatarURL = '/images/avatars/player3.png'
WHERE UserID = 5;

UPDATE UserProfiles SET
    DateOfBirth = '1996-08-18',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Can Tho',
    Bio = 'FIFA professional player, represented Vietnam in Asia Cup',
    AvatarURL = '/images/avatars/player4.png'
WHERE UserID = 6;

UPDATE UserProfiles SET
    DateOfBirth = '1999-02-27',
    Gender = 'Female',
    Country = 'Vietnam',
    City = 'Hai Phong',
    Bio = 'Rocket League champion, streaming on Twitch',
    AvatarURL = '/images/avatars/player5.png'
WHERE UserID = 7;

UPDATE UserProfiles SET
    DateOfBirth = '1994-04-15',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Ho Chi Minh City',
    Bio = 'Esports enthusiast and supporter',
    AvatarURL = '/images/avatars/viewer1.png'
WHERE UserID = 8;

UPDATE UserProfiles SET
    DateOfBirth = '1992-09-22',
    Gender = 'Female',
    Country = 'Vietnam',
    City = 'Hanoi',
    Bio = 'Regular tournament viewer and commentator',
    AvatarURL = '/images/avatars/viewer2.png'
WHERE UserID = 9;

UPDATE UserProfiles SET
    DateOfBirth = '1997-12-05',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Da Nang',
    Bio = 'Esports blogger and fan',
    AvatarURL = '/images/avatars/viewer3.png'
WHERE UserID = 10;

-- Create teams
INSERT INTO Teams (TeamName, Description, GameID, CreatedBy, LogoURL, MaxMembers, IsActive, Status) VALUES
('Dragons Gaming', 'Premier League of Legends team from Vietnam', 1, 3, '/images/teams/dragons.png', 5, TRUE, 'Active'),
('Phoenix Valorant', 'Professional Valorant squad', 3, 4, '/images/teams/phoenix.png', 5, TRUE, 'Active'),
('Dota Masters', 'Experienced Dota 2 team with multiple tournament wins', 4, 5, '/images/teams/dotamasters.png', 5, TRUE, 'Active'),
('Football Kings', 'FIFA 24 focused team', 5, 6, '/images/teams/footballkings.png', 3, TRUE, 'Active'),
('Rocket Stars', 'Rocket League specialists', 6, 7, '/images/teams/rocketstars.png', 3, TRUE, 'Active');

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

-- Create tournaments
INSERT INTO Tournaments (TournamentName, Description, GameID, StartDate, EndDate, RegistrationDeadline, MaxTeams, EntryFee, PrizePool, Status, CreatedBy) VALUES
('Vietnam LoL Championship 2025', 'Official League of Legends championship for Vietnam', 1, '2025-07-10', '2025-07-15', '2025-07-01', 16, 50.00, 5000.00, 'Registration', 1),
('Valorant Masters Hanoi', 'Professional Valorant tournament in Hanoi', 3, '2025-08-05', '2025-08-10', '2025-07-25', 8, 30.00, 3000.00, 'Registration', 2),
('Dota 2 Vietnam Cup', 'Premier Dota 2 event in Southeast Asia', 4, '2025-09-15', '2025-09-20', '2025-09-01', 12, 40.00, 4000.00, 'Draft', 1),
('FIFA National Tournament', 'Annual FIFA competition', 5, '2025-06-30', '2025-07-02', '2025-06-15', 32, 20.00, 2000.00, 'Ongoing', 2),
('Rocket League Showdown', 'Fast-paced Rocket League tournament', 6, '2025-07-25', '2025-07-27', '2025-07-10', 16, 25.00, 1500.00, 'Registration', 1);

-- Register teams for tournaments
INSERT INTO TournamentRegistrations (TournamentID, TeamID, RegisteredBy, Status) VALUES
(1, 1, 3, 'Approved'),
(2, 2, 4, 'Approved'),
(3, 3, 5, 'Registered'),
(4, 4, 6, 'Approved'),
(5, 5, 7, 'Registered');

-- Add some completed tournament results
INSERT INTO TournamentResults (TournamentID, TeamID, Position, PrizeMoney, Notes) VALUES
(4, 4, 1, 1000.00, 'Champion with perfect record');

-- Add some donations from viewers to players
INSERT INTO Donations (FromUserID, ToUserID, Amount, Message, Status) VALUES
(8, 3, 50.00, 'Great performance in last tournament!', 'Completed'),
(9, 4, 30.00, 'You\'re my favorite player!', 'Completed'),
(10, 5, 25.00, 'Keep up the good work', 'Completed'),
(8, 6, 15.00, 'Amazing skills!', 'Completed'),
(9, 7, 20.00, 'Looking forward to your next match', 'Completed');

-- Add some admin actions for audit trail
INSERT INTO AdminActions (AdminID, ActionType, TargetType, TargetID, Description) VALUES
(1, 'CreateTournament', 'Tournament', 1, 'Created Vietnam LoL Championship 2025'),
(2, 'CreateTournament', 'Tournament', 2, 'Created Valorant Masters Hanoi'),
(1, 'CreateTournament', 'Tournament', 3, 'Created Dota 2 Vietnam Cup'),
(2, 'CreateTournament', 'Tournament', 4, 'Created FIFA National Tournament'),
(1, 'CreateTournament', 'Tournament', 5, 'Created Rocket League Showdown'),
(1, 'ManageGame', 'Game', 1, 'Added League of Legends to platform'),
(2, 'ProcessWithdrawal', 'Withdrawal', 1, 'Processed withdrawal request');

-- Add some votes/ratings
INSERT INTO Votes (VoterID, VoteType, TargetID, Rating, Comment) VALUES
(8, 'Player', 3, 5, 'Excellent player, very skilled'),
(9, 'Player', 4, 4, 'Great positioning and strategy'),
(10, 'Player', 5, 5, 'One of the best Dota 2 players I\'ve seen'),
(8, 'Tournament', 1, 5, 'Well organized tournament'),
(9, 'Tournament', 2, 4, 'Great matches but streaming had some issues');

-- Add some player rankings
INSERT INTO PlayerRankings (UserID, GameID, CurrentRank, TotalPoints, TotalWins, TotalLosses, TournamentWins) VALUES
(3, 1, 1, 2500, 48, 12, 3),
(4, 3, 2, 2200, 35, 15, 2),
(5, 4, 1, 2700, 52, 8, 4),
(6, 5, 3, 1800, 30, 20, 1),
(7, 6, 2, 2100, 40, 18, 2);

SELECT 'Sample data inserted successfully!' as Message;
