-- =====================================================
-- 06. WALLET AND DONATIONS MODULE
-- =====================================================
-- Module: Wallets, Donations, Withdrawals, and Transactions
-- Description: Financial system for player donations and withdrawals
-- Dependencies: 01_database_setup.sql, 03_users.sql
-- =====================================================

USE EsportsManager;

-- Table: Wallets (Player wallets for receiving donations)
CREATE TABLE IF NOT EXISTS Wallets (
    WalletID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL UNIQUE,
    Balance DECIMAL(12,2) DEFAULT 0.00,
    TotalReceived DECIMAL(12,2) DEFAULT 0.00,
    TotalWithdrawn DECIMAL(12,2) DEFAULT 0.00,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    LastUpdated TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
) ENGINE=InnoDB COMMENT='Wallets table';

-- Table: Donations (Donations from Viewers to Players)
CREATE TABLE IF NOT EXISTS Donations (
    DonationID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL, -- User who donates (FromUserID)
    Amount DECIMAL(10,2) NOT NULL,
    Message TEXT,
    Status ENUM('Completed', 'Failed', 'Refunded') DEFAULT 'Completed',
    TargetType VARCHAR(50) NOT NULL, -- 'Tournament', 'Team', 'Player', etc.
    TargetID INT, -- ID of the target entity
    TransactionID INT NULL, -- Link to WalletTransactions (nullable to avoid circular dependency)
    DonationDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE RESTRICT
) ENGINE=InnoDB COMMENT='Donations table';

-- Table: Withdrawals (Money withdrawal by Players)
CREATE TABLE IF NOT EXISTS Withdrawals (
    WithdrawalID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    BankAccountNumber VARCHAR(50) NOT NULL,
    BankName VARCHAR(100) NOT NULL,
    AccountHolderName VARCHAR(100) NOT NULL,
    Status ENUM('Pending', 'Approved', 'Processing', 'Completed', 'Rejected') DEFAULT 'Pending',
    RequestDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ProcessedDate DATETIME,
    ProcessedBy INT, -- Admin processes
    Notes TEXT,
    
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE RESTRICT,
    FOREIGN KEY (ProcessedBy) REFERENCES Users(UserID) ON DELETE SET NULL
) ENGINE=InnoDB COMMENT='Withdrawals table';

-- Table: WalletTransactions (Based on ERD - detailed transaction history)
CREATE TABLE IF NOT EXISTS WalletTransactions (
    TransactionID INT AUTO_INCREMENT PRIMARY KEY,
    WalletID INT NOT NULL,
    UserID INT NOT NULL,
    TransactionType VARCHAR(50) NOT NULL,
    Amount DECIMAL(12,2) NOT NULL,
    BalanceAfter DECIMAL(12,2) NOT NULL,
    Status VARCHAR(20) DEFAULT 'Completed',
    ReferenceCode VARCHAR(100),
    Note TEXT,
    RelatedEntityType VARCHAR(50),
    RelatedEntityID INT,
    RelatedUserID INT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (WalletID) REFERENCES Wallets(WalletID) ON DELETE CASCADE,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (RelatedUserID) REFERENCES Users(UserID) ON DELETE SET NULL
) ENGINE=InnoDB COMMENT='Detailed wallet transaction history based on ERD';

SELECT '06. Wallet and donations module created successfully!' as Message;
