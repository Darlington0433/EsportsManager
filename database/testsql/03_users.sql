-- =====================================================
-- 03. USERS MODULE
-- =====================================================
-- Module: Users and User Profiles
-- Description: User management with 3 roles (Admin, Player, Viewer) and personal profiles
-- Dependencies: 01_database_setup.sql
-- =====================================================

USE EsportsManager;

-- Table: Users (3 roles: Admin, Player, Viewer)
CREATE TABLE IF NOT EXISTS Users (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    FullName VARCHAR(100),
    PhoneNumber VARCHAR(20),
    DisplayName VARCHAR(100),
    Role ENUM('Admin', 'Player', 'Viewer') NOT NULL DEFAULT 'Viewer',
    IsActive BOOLEAN DEFAULT FALSE,
    Status ENUM('Active', 'Suspended', 'Inactive', 'Pending', 'Deleted') NOT NULL DEFAULT 'Pending',
    IsEmailVerified BOOLEAN DEFAULT FALSE,
    EmailVerificationToken VARCHAR(255),
    PasswordResetToken VARCHAR(255),
    PasswordResetExpiry DATETIME,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP NULL ON UPDATE CURRENT_TIMESTAMP,
    LastLoginAt DATETIME,
    SecurityQuestion VARCHAR(255),
    SecurityAnswer VARCHAR(255)
) ENGINE=InnoDB COMMENT='Users table';

-- Table: UserProfiles (Detailed personal information)
CREATE TABLE IF NOT EXISTS UserProfiles (
    ProfileID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL UNIQUE,
    DateOfBirth DATE,
    Gender ENUM('Male', 'Female', 'Other'),
    Address TEXT,
    Country VARCHAR(50),
    City VARCHAR(50),
    Bio TEXT,
    AvatarURL VARCHAR(255),
    SocialLinks JSON,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
) ENGINE=InnoDB COMMENT='User profiles table';

SELECT '03. Users module created successfully!' as Message;
