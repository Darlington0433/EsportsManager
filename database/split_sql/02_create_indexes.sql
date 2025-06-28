-- =====================================================
-- 02_CREATE_INDEXES.sql
-- Tạo các indexes để tối ưu hiệu suất query
-- Run Order: 2
-- Prerequisites: 01_create_database_and_tables.sql
-- =====================================================

USE EsportsManager;

-- =====================================================
-- CREATE INDEXES FOR PERFORMANCE
-- =====================================================

-- Indexes for Users
CREATE INDEX idx_users_role ON Users(Role);
CREATE INDEX idx_users_active ON Users(IsActive);
CREATE INDEX idx_users_email ON Users(Email);

-- Indexes for Teams
CREATE INDEX idx_teams_game ON Teams(GameID);
CREATE INDEX idx_teams_creator ON Teams(CreatedBy);
CREATE INDEX idx_teams_status ON Teams(Status);

-- Indexes for Tournaments
CREATE INDEX idx_tournaments_game ON Tournaments(GameID);
CREATE INDEX idx_tournaments_status ON Tournaments(Status);
CREATE INDEX idx_tournaments_dates ON Tournaments(StartDate, EndDate);

-- Indexes for Donations
CREATE INDEX idx_donations_user ON Donations(UserID);
CREATE INDEX idx_donations_target ON Donations(TargetType, TargetID);
CREATE INDEX idx_donations_date ON Donations(DonationDate);

-- Indexes for Votes
CREATE INDEX idx_votes_target ON Votes(VoteType, TargetID);
CREATE INDEX idx_votes_voter ON Votes(VoterID);

-- Additional indexes for TournamentResults
CREATE INDEX idx_tournament_results_tournament ON TournamentResults(TournamentID);
CREATE INDEX idx_tournament_results_team ON TournamentResults(TeamID);

-- Additional indexes for Teams (based on ERD relationships)
CREATE INDEX idx_teams_created_by ON Teams(CreatedBy);

-- Indexes for Wallet Transactions
CREATE INDEX idx_wallet_transactions_type ON WalletTransactions(TransactionType);
CREATE INDEX idx_wallet_transactions_date ON WalletTransactions(CreatedAt);

-- Additional indexes for better performance
CREATE INDEX idx_users_role_status ON Users(Role, Status);
CREATE INDEX idx_users_created ON Users(CreatedAt);
CREATE INDEX idx_tournaments_created ON Tournaments(CreatedAt);
CREATE INDEX idx_teams_created ON Teams(CreatedAt);

-- Add indexes for better withdrawal performance
CREATE INDEX idx_withdrawals_user_date ON Withdrawals(UserID, RequestDate);
CREATE INDEX idx_withdrawals_status ON Withdrawals(Status);
CREATE INDEX idx_withdrawals_reference ON Withdrawals(ReferenceCode);

SELECT 'Database indexes created successfully!' as Message;
