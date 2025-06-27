-- =====================================================
-- 16E. TOURNAMENTS SAMPLE DATA
-- =====================================================
-- Module: Sample Tournaments and Registrations Data
-- Description: Insert sample tournaments and team registrations
-- Dependencies: 16a_games_sample_data.sql, 16b_users_sample_data.sql, 16d_teams_sample_data.sql
-- =====================================================

USE EsportsManager;

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

-- Add some player rankings
INSERT INTO PlayerRankings (UserID, GameID, CurrentRank, TotalPoints, TotalWins, TotalLosses, TournamentWins) VALUES
(3, 1, 1, 2500, 48, 12, 3),
(4, 3, 2, 2200, 35, 15, 2),
(5, 4, 1, 2700, 52, 8, 4),
(6, 5, 3, 1800, 30, 20, 1),
(7, 6, 2, 2100, 40, 18, 2);

SELECT '16E. Tournaments sample data inserted successfully!' as Message;
SELECT 'Total Tournaments: 5' as TournamentStats;
SELECT 'Total Registrations: 5' as RegistrationStats;
SELECT 'Total Results: 1' as ResultStats;
SELECT 'Total Rankings: 5' as RankingStats;
