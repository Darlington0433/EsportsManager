-- =====================================================
-- 01. DATABASE SETUP AND BASIC CONFIGURATION
-- =====================================================
-- Module: Database Setup
-- Description: Creates the main database and basic configuration
-- Dependencies: None
-- =====================================================

-- Create database
DROP DATABASE IF EXISTS EsportsManager;
CREATE DATABASE IF NOT EXISTS EsportsManager 
CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE EsportsManager;

-- Disable foreign key checks temporarily
SET FOREIGN_KEY_CHECKS = 0;

SELECT '01. Database setup completed successfully!' as Message;
