-- =====================================================
-- 16B. USERS SAMPLE DATA
-- =====================================================
-- Module: Sample Users Data (Admin, Player, Viewer)
-- Description: Insert sample user accounts with BCrypt hashed passwords
-- Dependencies: 03_users.sql
-- =====================================================

USE EsportsManager;

-- Insert admin accounts - Hash BCrypt đã test và hoạt động 100%
-- admin/admin123
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive, Status, IsEmailVerified) VALUES
('admin', '$2a$11$AT5YJeJ9yMR60/YFFsWYp.PPMe1ZFHZ.RuR6EBGJ5ZIzmCgtI3zh6', 'admin@esportsmanager.com', 'System Administrator', 'Admin', 'Admin', TRUE, 'Active', TRUE),
('superadmin', '$2a$11$AT5YJeJ9yMR60/YFFsWYp.PPMe1ZFHZ.RuR6EBGJ5ZIzmCgtI3zh6', 'superadmin@esportsmanager.com', 'Super Administrator', 'SuperAdmin', 'Admin', TRUE, 'Active', TRUE);

-- Insert sample players - Hash BCrypt đã test và hoạt động 100%
-- player1/player123, player2/player123, etc.
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive, Status, IsEmailVerified) VALUES
('player1', '$2a$11$vQx1EAmFP67.XJ1bCQgVpe7VQHxL1CgY3o9dF5I4HJy6TGHHyOmKS', 'player1@test.com', 'Nguyen Van A', 'ProGamer1', 'Player', TRUE, 'Active', TRUE),
('player2', '$2a$11$vQx1EAmFP67.XJ1bCQgVpe7VQHxL1CgY3o9dF5I4HJy6TGHHyOmKS', 'player2@test.com', 'Tran Thi B', 'ProGamer2', 'Player', TRUE, 'Active', TRUE),
('player3', '$2a$11$vQx1EAmFP67.XJ1bCQgVpe7VQHxL1CgY3o9dF5I4HJy6TGHHyOmKS', 'player3@test.com', 'Le Van C', 'ProGamer3', 'Player', TRUE, 'Active', TRUE),
('player4', '$2a$11$vQx1EAmFP67.XJ1bCQgVpe7VQHxL1CgY3o9dF5I4HJy6TGHHyOmKS', 'player4@test.com', 'Do Dinh D', 'ProGamer4', 'Player', TRUE, 'Active', TRUE),
('player5', '$2a$11$vQx1EAmFP67.XJ1bCQgVpe7VQHxL1CgY3o9dF5I4HJy6TGHHyOmKS', 'player5@test.com', 'Pham Thi E', 'ProGamer5', 'Player', TRUE, 'Active', TRUE);

-- Insert sample viewers - Hash BCrypt đã test và hoạt động 100%
-- viewer1/viewer123, viewer2/viewer123, etc.
INSERT INTO Users (Username, PasswordHash, Email, FullName, DisplayName, Role, IsActive, Status, IsEmailVerified) VALUES
('viewer1', '$2a$11$mOBCKR5/l5EG5EYh5sPhb.sYgtbdO3eXGJQ5k8I8k8SnVGLzJmq2e', 'viewer1@test.com', 'Hoang Van F', 'EsportsFan1', 'Viewer', TRUE, 'Active', TRUE),
('viewer2', '$2a$11$mOBCKR5/l5EG5EYh5sPhb.sYgtbdO3eXGJQ5k8I8k8SnVGLzJmq2e', 'viewer2@test.com', 'Ngo Thi G', 'EsportsFan2', 'Viewer', TRUE, 'Active', TRUE),
('viewer3', '$2a$11$mOBCKR5/l5EG5EYh5sPhb.sYgtbdO3eXGJQ5k8I8k8SnVGLzJmq2e', 'viewer3@test.com', 'Vuong Van H', 'EsportsFan3', 'Viewer', TRUE, 'Active', TRUE);

SELECT '16B. Users sample data inserted successfully!' as Message;
SELECT 'Total Users: 10 (2 Admin, 5 Player, 3 Viewer)' as UserStats;
