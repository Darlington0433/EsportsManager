-- =====================================================
-- CREATE DEFAULT ADMIN USER FOR TESTING
-- =====================================================

USE EsportsManager;

-- Create default admin user if not exists
INSERT IGNORE INTO Users (Username, PasswordHash, Email, FullName, Role, Status, IsActive, CreatedAt)
VALUES (
    'admin',
    '$2a$11$AT5YJeJ9yMR60/YFFsWYp.PPMe1ZFHZ.RuR6EBGJ5ZIzmCgtI3zh6', -- Password: admin123
    'admin@esportsmanager.com',
    'System Administrator',
    'Admin',
    'Active',
    TRUE,
    NOW()
);

-- Also create a test player and viewer for statistics
INSERT IGNORE INTO Users (Username, PasswordHash, Email, FullName, Role, Status, IsActive, CreatedAt)
VALUES 
    ('player1', '$2a$11$AT5YJeJ9yMR60/YFFsWYp.PPMe1ZFHZ.RuR6EBGJ5ZIzmCgtI3zh6', 'player1@test.com', 'Test Player 1', 'Player', 'Active', TRUE, NOW()),
    ('viewer1', '$2a$11$AT5YJeJ9yMR60/YFFsWYp.PPMe1ZFHZ.RuR6EBGJ5ZIzmCgtI3zh6', 'viewer1@test.com', 'Test Viewer 1', 'Viewer', 'Active', TRUE, NOW());

-- Create sample game if not exists
INSERT IGNORE INTO Games (GameName, Description, Genre, IsActive)
VALUES ('League of Legends', 'Popular MOBA game', 'MOBA', TRUE);

-- Create sample tournament if not exists
INSERT IGNORE INTO Tournaments (TournamentName, GameID, Description, PrizePool, EntryFee, MaxTeams, StartDate, EndDate, Status, CreatedAt)
SELECT 
    'Sample Tournament 2024',
    g.GameID,
    'Test tournament for system statistics',
    1000000,
    50000,
    16,
    DATE_ADD(NOW(), INTERVAL 7 DAY),
    DATE_ADD(NOW(), INTERVAL 14 DAY),
    'Registration',
    NOW()
FROM Games g 
WHERE g.GameName = 'League of Legends'
LIMIT 1;

-- Create sample team if not exists
INSERT IGNORE INTO Teams (TeamName, GameID, CaptainID, Description, Status, CreatedAt)
SELECT 
    'Sample Team Alpha',
    g.GameID,
    u.UserID,
    'Test team for system statistics',
    'Active',
    NOW()
FROM Games g, Users u
WHERE g.GameName = 'League of Legends' 
    AND u.Username = 'player1'
LIMIT 1;

SELECT 'Admin user and sample data created successfully!' as Result;
SELECT 'Login with: Username=admin, Password=admin123' as LoginInfo;
