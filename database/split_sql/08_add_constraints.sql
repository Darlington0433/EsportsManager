-- =====================================================
-- 08_ADD_CONSTRAINTS.sql
-- Thêm các constraints để đảm bảo tính toàn vẹn dữ liệu
-- Run Order: 8
-- Prerequisites: 01-07 (all previous files)
-- =====================================================

USE EsportsManager;

-- =====================================================
-- DATA INTEGRITY CONSTRAINTS
-- =====================================================

-- Add constraints to ensure data integrity based on ERD
ALTER TABLE Teams ADD CONSTRAINT chk_max_members CHECK (MaxMembers > 0 AND MaxMembers <= 10);
ALTER TABLE Tournaments ADD CONSTRAINT chk_tournament_dates CHECK (StartDate < EndDate);
ALTER TABLE Tournaments ADD CONSTRAINT chk_registration_deadline CHECK (RegistrationDeadline <= StartDate);
ALTER TABLE TournamentResults ADD CONSTRAINT chk_position_positive CHECK (Position > 0);
ALTER TABLE Donations ADD CONSTRAINT chk_donation_amount CHECK (Amount > 0);
ALTER TABLE Withdrawals ADD CONSTRAINT chk_withdrawal_amount CHECK (Amount > 0);

-- Additional constraints for Achievements
ALTER TABLE Achievements ADD CONSTRAINT chk_achievement_date CHECK (DateAchieved <= NOW());

-- Additional constraints for Wallets
ALTER TABLE Wallets ADD CONSTRAINT chk_wallet_balance CHECK (Balance >= 0);
ALTER TABLE Wallets ADD CONSTRAINT chk_wallet_total_received CHECK (TotalReceived >= 0);
ALTER TABLE Wallets ADD CONSTRAINT chk_wallet_total_withdrawn CHECK (TotalWithdrawn >= 0);

-- Additional constraints for WalletTransactions
ALTER TABLE WalletTransactions ADD CONSTRAINT chk_transaction_amount CHECK (Amount != 0);

SELECT 'Database constraints added successfully!' as Message;
