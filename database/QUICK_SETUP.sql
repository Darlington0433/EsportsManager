-- =====================================================
-- SETUP NHANH ESPORTS MANAGER DATABASE
-- =====================================================
-- Chỉ cần chạy file này để có database hoàn chỉnh!
-- 
-- Cách sử dụng:
-- 1. Mở MySQL Workbench
-- 2. Chạy file này
-- 3. Hoàn thành!
-- =====================================================

-- Import toàn bộ database
SOURCE esportsmanager.sql;

-- Hiển thị thông báo hoàn thành
SELECT '🎉 Database EsportsManager đã được tạo thành công!' as Message;
SELECT 'Tài khoản admin: username=admin, password=admin123' as AdminAccount;
SELECT 'Có thể đăng nhập ứng dụng ngay bây giờ!' as Status;
