-- =====================================================
-- 11. VIEWS MODULE
-- =====================================================
-- Module: Database Views for Common Queries
-- Description: Create views for frequently used queries
-- Dependencies: All table modules (02-09)
-- =====================================================

USE EsportsManager;

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

-- View: Tournament results with team details
DROP VIEW IF EXISTS v_tournament_results;

CREATE VIEW v_tournament_results AS
SELECT 
    tr.ResultID,
    t.TournamentName,
    g.GameName,
    team.TeamName,
    tr.Position,
    tr.PrizeMoney,
    tr.Notes,
    t.StartDate,
    t.EndDate
FROM TournamentResults tr
JOIN Tournaments t ON tr.TournamentID = t.TournamentID
JOIN Teams team ON tr.TeamID = team.TeamID
JOIN Games g ON t.GameID = g.GameID
ORDER BY t.TournamentID, tr.Position;

-- View: Complete team information with position ranking
DROP VIEW IF EXISTS v_team_rankings;

CREATE VIEW v_team_rankings AS
SELECT 
    t.TeamID,
    t.TeamName,
    g.GameName,
    COUNT(DISTINCT tm.UserID) as ActiveMembers,
    COUNT(DISTINCT tr.TournamentID) as TournamentsParticipated,
    AVG(tres.Position) as AveragePosition,
    SUM(tres.PrizeMoney) as TotalPrizeMoneyWon,
    RANK() OVER (PARTITION BY t.GameID ORDER BY SUM(tres.PrizeMoney) DESC) as GameRanking
FROM Teams t
JOIN Games g ON t.GameID = g.GameID
LEFT JOIN TeamMembers tm ON t.TeamID = tm.TeamID AND tm.Status = 'Active'
LEFT JOIN TournamentRegistrations tr ON t.TeamID = tr.TeamID AND tr.Status = 'Approved'
LEFT JOIN TournamentResults tres ON t.TeamID = tres.TeamID
WHERE t.Status = 'Active'
GROUP BY t.TeamID, t.TeamName, g.GameName;

-- View: Enhanced user wallet summary
DROP VIEW IF EXISTS v_user_wallet_summary;

CREATE VIEW v_user_wallet_summary AS
SELECT 
    u.UserID,
    u.Username,
    u.DisplayName,
    w.Balance,
    w.TotalReceived,
    w.TotalWithdrawn,
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
GROUP BY u.UserID;

SELECT '11. Database views created successfully!' as Message;
