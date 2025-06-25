-- =====================================================
-- CREATE INDEXES FOR PERFORMANCE
-- =====================================================

USE EsportsManager;

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
CREATE INDEX idx_donations_from ON Donations(FromUserID);
CREATE INDEX idx_donations_to ON Donations(ToUserID);
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

SELECT 'Database indexes created successfully!' as Message;
