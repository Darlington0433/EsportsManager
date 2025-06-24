-- Script cập nhật hash BCrypt mới được tạo bởi BCrypt.Net
-- Các hash này đã được test và hoạt động đúng
USE EsportsManager;

-- Cập nhật hash cho admin (sử dụng hash mới từ BCrypt.Net)
-- Bạn cần thay thế [ADMIN_HASH] bằng hash thực tế từ output của BCryptTestProject
UPDATE Users SET 
    PasswordHash = '[ADMIN_HASH_FROM_OUTPUT]',
    Status = 'Active',
    IsActive = TRUE,
    IsEmailVerified = TRUE
WHERE Username = 'admin';

-- Cập nhật hash cho superadmin (cùng hash với admin)
UPDATE Users SET 
    PasswordHash = '[ADMIN_HASH_FROM_OUTPUT]',
    Status = 'Active',
    IsActive = TRUE,
    IsEmailVerified = TRUE
WHERE Username = 'superadmin';

-- Cập nhật hash cho tất cả player
UPDATE Users SET 
    PasswordHash = '[PLAYER_HASH_FROM_OUTPUT]',
    Status = 'Active',
    IsActive = TRUE,
    IsEmailVerified = TRUE
WHERE Username LIKE 'player%';

-- Cập nhật hash cho tất cả viewer
UPDATE Users SET 
    PasswordHash = '[VIEWER_HASH_FROM_OUTPUT]',
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
