-- =============================================
-- MySQL Compatibility Test Script
-- This script tests basic MySQL functionality and syntax
-- Run this first to ensure MySQL compatibility
-- =============================================

-- Test 1: Database creation
DROP DATABASE IF EXISTS `EsportsManager_Test`;
CREATE DATABASE `EsportsManager_Test` 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE `EsportsManager_Test`;

-- Test 2: Basic table with common data types
CREATE TABLE `TestTable` (
    `ID` INT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(100) NOT NULL,
    `Description` TEXT,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `Amount` DECIMAL(18,2) DEFAULT 0,
    `Status` ENUM('Active', 'Inactive') DEFAULT 'Active',
    `CreatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Test 3: Insert data with boolean values
INSERT INTO `TestTable` (`Name`, `Description`, `IsActive`, `Amount`, `Status`) VALUES
('Test Item 1', 'This is a test item', TRUE, 100.50, 'Active'),
('Test Item 2', 'Another test item', FALSE, 200.75, 'Inactive');

-- Test 4: Query with functions
SELECT 
    `ID`,
    `Name`,
    `IsActive`,
    `Amount`,
    `Status`,
    `CreatedAt`,
    NOW() as CurrentTime,
    DATE_SUB(NOW(), INTERVAL 1 HOUR) as OneHourAgo
FROM `TestTable`;

-- Test 5: Update with timestamp
UPDATE `TestTable` SET 
    `Description` = 'Updated description',
    `Amount` = 150.25 
WHERE `ID` = 1;

-- Test 6: Foreign key constraint test
CREATE TABLE `TestParent` (
    `ParentID` INT AUTO_INCREMENT PRIMARY KEY,
    `ParentName` VARCHAR(50) NOT NULL
);

CREATE TABLE `TestChild` (
    `ChildID` INT AUTO_INCREMENT PRIMARY KEY,
    `ParentID` INT NOT NULL,
    `ChildName` VARCHAR(50) NOT NULL,
    FOREIGN KEY (`ParentID`) REFERENCES `TestParent`(`ParentID`)
);

INSERT INTO `TestParent` (`ParentName`) VALUES ('Parent 1');
INSERT INTO `TestChild` (`ParentID`, `ChildName`) VALUES (1, 'Child 1');

-- Test 7: Check results
SELECT 'MySQL compatibility test completed successfully!' as TestResult;
SELECT COUNT(*) as TotalRecords FROM `TestTable`;
SELECT 'All basic MySQL features are working properly.' as Status;

-- Clean up test database
DROP DATABASE `EsportsManager_Test`;

SELECT 'Test database cleaned up. MySQL is ready for EsportsManager!' as FinalResult;
