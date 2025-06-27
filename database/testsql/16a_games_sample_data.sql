-- =====================================================
-- 16A. GAMES SAMPLE DATA
-- =====================================================
-- Module: Sample Games Data
-- Description: Insert sample games data for testing
-- Dependencies: 02_games.sql
-- =====================================================

USE EsportsManager;

-- Insert sample games
INSERT INTO Games (GameName, Description, Genre) VALUES
('League of Legends', 'Most popular MOBA game in the world', 'MOBA'),
('Counter-Strike 2', 'Professional competitive FPS game', 'FPS'),
('Valorant', 'Tactical FPS game', 'FPS'),
('Dota 2', 'Professional MOBA game', 'MOBA'),
('FIFA 24', 'Football/Soccer simulation game', 'Sports'),
('Rocket League', 'Vehicular soccer game', 'Sports'),
('Overwatch 2', 'Team-based first-person shooter', 'FPS');

SELECT '16A. Games sample data inserted successfully!' as Message;
