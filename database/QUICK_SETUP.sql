-- =====================================================
-- SETUP NHANH ESPORTS MANAGER DATABASE
-- =====================================================
-- Chỉ cần chạy file này để có database hoàn chỉnh!
-- 
-- Cách sử dụng:
-- 1. Mở MySQL Workbench
-- 2. Chạy file này
-- 3. Hoàn thành!
-- 
-- Tài khoản đăng nhập sau khi setup:
-- admin/admin123, player1/player123, viewer1/viewer123
-- =====================================================

-- Import toàn bộ database
SOURCE esportsmanager.sql;

-- Hiển thị thông báo hoàn thành
SELECT '🎉 Database EsportsManager đã được tạo thành công!' as Message;
SELECT 'admin/admin123, player1/player123, viewer1/viewer123' as 'Tài khoản mặc định';
SELECT 'Hash BCrypt đã được chuẩn hóa - sẵn sàng clone sang máy khác!' as Status;
