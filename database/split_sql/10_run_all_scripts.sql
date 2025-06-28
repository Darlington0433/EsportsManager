-- =====================================================
-- 10_RUN_ALL_SCRIPTS.sql  
-- Chạy tất cả các scripts theo thứ tự đúng
-- Run Order: 10 (hoặc chạy file này để setup toàn bộ database)
-- =====================================================

-- File này sẽ chạy tất cả các scripts theo thứ tự
-- Bạn có thể chạy từng file riêng lẻ hoặc chạy file này để setup toàn bộ

SELECT 'Starting complete database setup...' as Message;

-- Script 1: Create database and tables
SOURCE 01_create_database_and_tables.sql;

-- Script 2: Create indexes  
SOURCE 02_create_indexes.sql;

-- Script 3: Create views
SOURCE 03_create_views.sql;

-- Script 4: Create triggers
SOURCE 04_create_triggers.sql;

-- Script 5: Create basic procedures
SOURCE 05_create_procedures_basic.sql;

-- Script 6: Create achievement procedures
SOURCE 06_create_procedures_achievement.sql;

-- Script 7: Create wallet procedures  
SOURCE 07_create_procedures_wallet.sql;

-- Script 8: Add constraints
SOURCE 08_add_constraints.sql;

-- Script 9: Insert sample data
SOURCE 09_insert_sample_data.sql;

SELECT 'Complete database setup finished successfully!' as Message;

-- Final verification
SELECT 'Database verification:' as Message;
SELECT CONCAT('Tables created: ', COUNT(*)) as TableCount 
FROM information_schema.tables 
WHERE table_schema = 'EsportsManager';

SELECT CONCAT('Stored procedures created: ', COUNT(*)) as ProcedureCount
FROM information_schema.routines 
WHERE routine_schema = 'EsportsManager' AND routine_type = 'PROCEDURE';

SELECT CONCAT('Views created: ', COUNT(*)) as ViewCount
FROM information_schema.views 
WHERE table_schema = 'EsportsManager';

SELECT CONCAT('Triggers created: ', COUNT(*)) as TriggerCount
FROM information_schema.triggers 
WHERE trigger_schema = 'EsportsManager';
