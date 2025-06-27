-- =====================================================
-- RUN ALL SCRIPTS IN ORDER - Chạy toàn bộ theo thứ tự
-- =====================================================
-- File này sẽ chạy tất cả 8 file trong split_sql theo đúng thứ tự
-- Sử dụng khi muốn setup từng bước hoặc debug

-- 1. Tạo database và tables
SOURCE 01_create_database_and_tables.sql;

-- 2. Tạo indexes để tối ưu hiệu năng
SOURCE 02_create_indexes.sql;

-- 3. Tạo views
SOURCE 03_create_views.sql;

-- 4. Tạo triggers
SOURCE 04_create_triggers.sql;

-- 5. Tạo procedures cơ bản
SOURCE 05_create_procedures.sql;

-- 6. Thêm constraints
SOURCE 06_add_constraints.sql;

-- 7. Thêm dữ liệu mẫu (QUAN TRỌNG - chứa tài khoản đăng nhập)
SOURCE 07_sample_data.sql;

-- 8. Tạo tournament procedures
SOURCE 08_tournament_procedures.sql;

-- 9. Tạo wallet và donation procedures
SOURCE 09_wallet_procedures.sql;

-- Thông báo hoàn thành
SELECT '✅ Đã hoàn thành tất cả 9 bước setup!' as Message;
SELECT 'admin/admin123, player1/player123, viewer1/viewer123' as 'Tài khoản đăng nhập';
SELECT 'Database EsportsManager sẵn sàng sử dụng!' as Status;
