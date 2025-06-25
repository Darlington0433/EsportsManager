-- Cập nhật hash BCrypt mới được tạo bằng BCrypt.Net
-- Đây là hash chính xác và hoạt động với PasswordHasherAdapter
USE EsportsManager;

-- Cập nhật hash cho admin và superadmin
UPDATE Users SET 
    PasswordHash = '$2a$11$9inYA.zu1eSu2CdJ3XwDMuMl95./WHUIovBSe3VsvXHtgQSCKYcaS',
    Status = 'Active',
    IsActive = TRUE,
    IsEmailVerified = TRUE
WHERE Username IN ('admin', 'superadmin');

-- Cập nhật hash cho tất cả player
UPDATE Users SET 
    PasswordHash = '$2a$11$x752zckXIW/bUITiCFq7FumuhWGe8ssSoY7JNemsH5J7U6yU3KKzC',
    Status = 'Active',
    IsActive = TRUE,
    IsEmailVerified = TRUE
WHERE Username LIKE 'player%';

-- Cập nhật hash cho tất cả viewer
UPDATE Users SET 
    PasswordHash = '$2a$11$u9zcIxF8UCfSjIOuaLhdZ.4lUlXiICWhdSY3uvZ/WTtPm0F0CXouW',
    Status = 'Active',
    IsActive = TRUE,
    IsEmailVerified = TRUE
WHERE Username LIKE 'viewer%';

-- Kiểm tra kết quả
SELECT 
    UserID,
    Username,
    LEFT(PasswordHash, 30) as HashPrefix,
    Status,
    IsActive,
    IsEmailVerified
FROM Users 
WHERE Username IN ('admin', 'superadmin') 
   OR Username LIKE 'player%' 
   OR Username LIKE 'viewer%'
ORDER BY Username;

SELECT 'Hash BCrypt mới đã được cập nhật successfully!' as Result;
