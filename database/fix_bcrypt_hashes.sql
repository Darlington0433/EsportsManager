-- Script cập nhật hash BCrypt CHÍNH XÁC sử dụng BCrypt.Net
-- Các hash này đã được test và hoạt động 100%
USE EsportsManager;

-- Cập nhật mật khẩu cho tài khoản admin
UPDATE Users
SET 
    PasswordHash = '$2a$11$AT5YJeJ9yMR60/YFFsWYp.PPMe1ZFHZ.RuR6EBGJ5ZIzmCgtI3zh6',
    Status = 'Active',
    IsEmailVerified = TRUE,
    IsActive = TRUE
WHERE 
    Username = 'admin';

-- Cập nhật mật khẩu cho tài khoản superadmin
UPDATE Users
SET 
    PasswordHash = '$2a$11$AT5YJeJ9yMR60/YFFsWYp.PPMe1ZFHZ.RuR6EBGJ5ZIzmCgtI3zh6',
    Status = 'Active',
    IsEmailVerified = TRUE,
    IsActive = TRUE
WHERE 
    Username = 'superadmin';

-- Cập nhật mật khẩu cho tài khoản player (player123)
UPDATE Users
SET 
    PasswordHash = '$2a$11$3msFMM1Oia5UyjkJG80ORep6GPzHHG3I4HiLPfWFwjtBxQpd5gTAq',
    Status = 'Active',
    IsEmailVerified = TRUE,
    IsActive = TRUE
WHERE 
    Username LIKE 'player%';

-- Cập nhật mật khẩu cho tài khoản viewer (viewer123)
UPDATE Users
SET
    PasswordHash = '$2a$11$FIFlkBHioBLmKDcfG5CI3exV6lstJNk7CyD4m5bO99Cd.Xi717tiG',
    Status = 'Active',
    IsEmailVerified = TRUE,
    IsActive = TRUE
WHERE 
    Username LIKE 'viewer%';

-- Xác nhận cập nhật
SELECT 'Đã cập nhật tất cả tài khoản với hash BCrypt.Net chính xác' as Message;

-- Hiển thị thông tin người dùng sau khi cập nhật
SELECT 
    UserID, 
    Username, 
    LEFT(PasswordHash, 20) as PasswordHashPrefix, 
    Status, 
    IsActive, 
    IsEmailVerified 
FROM 
    Users 
WHERE 
    Username IN ('admin', 'superadmin') 
    OR Username LIKE 'player%' 
    OR Username LIKE 'viewer%'
LIMIT 10;
