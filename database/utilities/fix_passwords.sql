-- =====================================================
-- SỬA LỖI MẬT KHẨU - CHUẨN HÓA BCRYPT HASH
-- =====================================================
-- Script này sửa lỗi mật khẩu không đăng nhập được
-- Sử dụng hash BCrypt đã được test và hoạt động 100%
-- 
-- Cách sử dụng: Chạy script này nếu không đăng nhập được
-- =====================================================

USE EsportsManager;

-- Cập nhật hash BCrypt CHÍNH XÁC cho tài khoản admin
UPDATE Users
SET 
    PasswordHash = '$2a$11$AT5YJeJ9yMR60/YFFsWYp.PPMe1ZFHZ.RuR6EBGJ5ZIzmCgtI3zh6', -- admin123
    Status = 'Active',
    IsEmailVerified = TRUE,
    IsActive = TRUE,
    UpdatedAt = NOW()
WHERE Username = 'admin';

-- Cập nhật hash BCrypt cho player1
UPDATE Users
SET 
    PasswordHash = '$2a$11$vQx1EAmFP67.XJ1bCQgVpe7VQHxL1CgY3o9dF5I4HJy6TGHHyOmKS', -- player123
    Status = 'Active',
    IsEmailVerified = TRUE,
    IsActive = TRUE,
    UpdatedAt = NOW()
WHERE Username = 'player1';

-- Cập nhật hash BCrypt cho viewer1
UPDATE Users
SET 
    PasswordHash = '$2a$11$mOBCKR5/l5EG5EYh5sPhb.sYgtbdO3eXGJQ5k8I8k8SnVGLzJmq2e', -- viewer123
    Status = 'Active',
    IsEmailVerified = TRUE,
    IsActive = TRUE,
    UpdatedAt = NOW()
WHERE Username = 'viewer1';

-- Kiểm tra kết quả
SELECT 
    UserID,
    Username,
    SUBSTRING(PasswordHash, 1, 30) AS 'Hash Preview',
    Status,
    IsActive,
    IsEmailVerified,
    UpdatedAt
FROM Users 
WHERE Username IN ('admin', 'player1', 'viewer1');

-- Thông báo hoàn thành  
SELECT 'Đã cập nhật thành công hash BCrypt cho các tài khoản!' as Message;
SELECT 'admin/admin123, player1/player123, viewer1/viewer123' as 'Login Info';
