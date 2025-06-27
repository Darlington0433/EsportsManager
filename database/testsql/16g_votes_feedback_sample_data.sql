-- =====================================================
-- 16G. VOTES AND FEEDBACK SAMPLE DATA
-- =====================================================
-- Module: Sample Votes and Feedback Data
-- Description: Insert sample votes and admin actions
-- Dependencies: 16b_users_sample_data.sql, 16e_tournaments_sample_data.sql
-- =====================================================

USE EsportsManager;

-- Add some votes/ratings
INSERT INTO Votes (VoterID, VoteType, TargetID, Rating, Comment) VALUES
(8, 'Player', 3, 5, 'Excellent player, very skilled'),
(9, 'Player', 4, 4, 'Great positioning and strategy'),
(10, 'Player', 5, 5, 'One of the best Dota 2 players I have seen'),
(8, 'Tournament', 1, 5, 'Well organized tournament'),
(9, 'Tournament', 2, 4, 'Great matches but streaming had some issues');

-- Add some admin actions for audit trail
INSERT INTO AdminActions (AdminID, ActionType, TargetType, TargetID, Description) VALUES
(1, 'CreateTournament', 'Tournament', 1, 'Created Vietnam LoL Championship 2025'),
(2, 'CreateTournament', 'Tournament', 2, 'Created Valorant Masters Hanoi'),
(1, 'CreateTournament', 'Tournament', 3, 'Created Dota 2 Vietnam Cup'),
(2, 'CreateTournament', 'Tournament', 4, 'Created FIFA National Tournament'),
(1, 'CreateTournament', 'Tournament', 5, 'Created Rocket League Showdown'),
(1, 'ManageGame', 'Game', 1, 'Added League of Legends to platform'),
(2, 'ProcessWithdrawal', 'Withdrawal', 1, 'Processed withdrawal request');

SELECT '16G. Votes and feedback sample data inserted successfully!' as Message;
SELECT 'Total Votes: 5' as VoteStats;
SELECT 'Total Admin Actions: 7' as AdminActionStats;
