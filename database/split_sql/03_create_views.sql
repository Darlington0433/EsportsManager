-- =====================================================
-- 03_CREATE_VIEWS.sql
-- Tạo các views để query dữ liệu dễ dàng hơn
-- Run Order: 3
-- Prerequisites: 01_create_database_and_tables.sql, 02_create_indexes.sql
-- =====================================================

USE EsportsManager;

-- =====================================================
-- VIEWS FOR COMMON QUERIES
-- =====================================================

-- View: Team statistics
DROP VIEW IF EXISTS v_team_stats;

CREATE VIEW v_team_stats AS
SELECT 
    t.TeamID,
    t.TeamName,
    g.GameName,
    COUNT(tm.UserID) as MemberCount,
    u.DisplayName as LeaderName,
    t.CreatedAt
FROM Teams t
LEFT JOIN TeamMembers tm ON t.TeamID = tm.TeamID AND tm.Status = 'Active'
LEFT JOIN Games g ON t.GameID = g.GameID
LEFT JOIN Users u ON t.CreatedBy = u.UserID
GROUP BY t.TeamID;

-- View: Tournament statistics with registration count
DROP VIEW IF EXISTS v_tournament_stats;

CREATE VIEW v_tournament_stats AS
SELECT 
    t.TournamentID,
    t.TournamentName,
    g.GameName,
    COUNT(tr.TeamID) as RegisteredTeams,
    t.MaxTeams,
    t.PrizePool,
    t.Status,
    u.DisplayName as CreatedBy
FROM Tournaments t
LEFT JOIN TournamentRegistrations tr ON t.TournamentID = tr.TournamentID AND tr.Status = 'Approved'
LEFT JOIN Games g ON t.GameID = g.GameID
LEFT JOIN Users u ON t.CreatedBy = u.UserID
GROUP BY t.TournamentID;

-- View: Player statistics
DROP VIEW IF EXISTS v_player_stats;

CREATE VIEW v_player_stats AS
SELECT 
    u.UserID,
    u.Username,
    u.DisplayName,
    COUNT(DISTINCT tm.TeamID) as TeamsJoined,
    COUNT(DISTINCT tr.TournamentID) as TournamentsPlayed,
    COALESCE(w.Balance, 0.00) as WalletBalance,
    COALESCE(w.TotalReceived, 0.00) as TotalDonationsReceived
FROM Users u
LEFT JOIN TeamMembers tm ON u.UserID = tm.UserID AND tm.Status = 'Active'
LEFT JOIN TournamentRegistrations tr ON tm.TeamID = tr.TeamID AND tr.Status = 'Approved'
LEFT JOIN Wallets w ON u.UserID = w.UserID
WHERE u.Role = 'Player'
GROUP BY u.UserID;

-- View: User wallet summary for detailed wallet operations
DROP VIEW IF EXISTS v_user_wallet_summary;

CREATE VIEW v_user_wallet_summary AS
SELECT 
    u.UserID,
    u.Username,
    u.DisplayName,
    w.WalletID,
    COALESCE(w.Balance, 0.00) as Balance,
    COALESCE(w.TotalReceived, 0.00) as TotalReceived,
    COALESCE(w.TotalWithdrawn, 0.00) as TotalWithdrawn,
    COUNT(DISTINCT wt_in.TransactionID) as TotalIncomingTransactions,
    COUNT(DISTINCT wt_out.TransactionID) as TotalOutgoingTransactions,
    w.LastUpdated
FROM Users u
LEFT JOIN Wallets w ON u.UserID = w.UserID
LEFT JOIN WalletTransactions wt_in ON w.WalletID = wt_in.WalletID 
    AND wt_in.TransactionType IN ('Donation_Received', 'Prize_Money', 'Deposit')
LEFT JOIN WalletTransactions wt_out ON w.WalletID = wt_out.WalletID 
    AND wt_out.TransactionType IN ('Withdrawal')
WHERE u.Role = 'Player'
GROUP BY u.UserID, w.WalletID;

SELECT 'Database views created successfully!' as Message;
