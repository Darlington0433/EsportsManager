-- =====================================================
-- DATA INTEGRITY ENHANCEMENTS
-- =====================================================

USE EsportsManager;

-- Add constraints to ensure data integrity based on ERD
ALTER TABLE Teams ADD CONSTRAINT chk_max_members CHECK (MaxMembers > 0 AND MaxMembers <= 10);
ALTER TABLE Tournaments ADD CONSTRAINT chk_tournament_dates CHECK (StartDate < EndDate);
ALTER TABLE Tournaments ADD CONSTRAINT chk_registration_deadline CHECK (RegistrationDeadline <= StartDate);
ALTER TABLE TournamentResults ADD CONSTRAINT chk_position_positive CHECK (Position > 0);
ALTER TABLE Donations ADD CONSTRAINT chk_donation_amount CHECK (Amount > 0);
ALTER TABLE Withdrawals ADD CONSTRAINT chk_withdrawal_amount CHECK (Amount > 0);

SELECT 'Database constraints added successfully!' as Message;
