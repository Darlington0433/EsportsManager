-- =====================================================
-- 16D. TEAMS AND MEMBERS SAMPLE DATA
-- =====================================================
-- Module: Sample Teams and Team Members Data
-- Description: Insert sample teams and their members
-- Dependencies: 16a_games_sample_data.sql, 16b_users_sample_data.sql
-- =====================================================

USE EsportsManager;

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

SELECT '16D. Teams and members sample data inserted successfully!' as Message;
SELECT 'Total Teams: 5' as TeamStats;
SELECT 'Total Team Members: 9' as MemberStats;
