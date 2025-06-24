-- =====================================================
-- CẬP NHẬT HASH BCRYPT MỚI - ĐƯỢC TẠO BỞI BCRYPT.NET
-- =====================================================

USE EsportsManager;

-- Cập nhật hash cho admin (admin123)
UPDATE Users 
SET PasswordHash = '$2a$11$9inYA.zu1eSu2CdJ3XwDMuMl95./WHUIovBSe3VsvXHtgQSCKYcaS',
    Status = 'Active',
    IsActive = TRUE,
    IsEmailVerified = TRUE
WHERE Username = 'admin';

-- Cập nhật hash cho superadmin (admin123)
UPDATE Users 
SET PasswordHash = '$2a$11$9inYA.zu1eSu2CdJ3XwDMuMl95./WHUIovBSe3VsvXHtgQSCKYcaS',
    Status = 'Active',
    IsActive = TRUE,
    IsEmailVerified = TRUE
WHERE Username = 'superadmin';

-- Cập nhật hash cho tất cả player (player123)
UPDATE Users 
SET PasswordHash = '$2a$11$x752zckXIW/bUITiCFq7FumuhWGe8ssSoY7JNemsH5J7U6yU3KKzC',
    Status = 'Active',
    IsActive = TRUE,
    IsEmailVerified = TRUE
WHERE Username LIKE 'player%';

-- Cập nhật hash cho tất cả viewer (viewer123)
UPDATE Users 
SET PasswordHash = '$2a$11$u9zcIxF8UCfSjIOuaLhdZ.4lUlXiICWhdSY3uvZ/WTtPm0F0CXouW',
    Status = 'Active',
    IsActive = TRUE,
    IsEmailVerified = TRUE
WHERE Username LIKE 'viewer%';

-- Xác nhận cập nhật
SELECT 'Hash BCrypt mới đã được cập nhật thành công!' as Message;

-- Hiển thị thông tin sau cập nhật
SELECT 
    UserID,
    Username,
    LEFT(PasswordHash, 30) as PasswordHashPrefix,
    Status,
    IsActive,
    IsEmailVerified,
    Role
FROM Users 
WHERE Username IN ('admin', 'superadmin') 
   OR Username LIKE 'player%' 
   OR Username LIKE 'viewer%'
ORDER BY Role, Username;
