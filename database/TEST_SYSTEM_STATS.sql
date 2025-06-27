-- =====================================================
-- TEST SYSTEM STATS PROCEDURES
-- =====================================================

USE EsportsManager;

-- Test basic connection
SELECT 'Database connection successful!' as Status;

-- Check if main tables exist
SELECT 
    'Table Check' as Test,
    (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'EsportsManager' AND table_name = 'Users') as Users_Table,
    (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'EsportsManager' AND table_name = 'Tournaments') as Tournaments_Table,
    (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'EsportsManager' AND table_name = 'Teams') as Teams_Table,
    (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'EsportsManager' AND table_name = 'TournamentRegistrations') as TournamentRegistrations_Table;

-- Test basic counts
SELECT 
    'Current Data' as Test,
    (SELECT COUNT(*) FROM Users) as Total_Users,
    (SELECT COUNT(*) FROM Users WHERE Status = 'Active') as Active_Users,
    (SELECT COUNT(*) FROM Tournaments) as Total_Tournaments,
    (SELECT COUNT(*) FROM Teams) as Total_Teams,
    (SELECT COUNT(*) FROM TournamentRegistrations) as Total_Registrations;

-- Test the RegisteredTeams calculation
SELECT 
    'Tournament Registration Test' as Test,
    t.TournamentID,
    t.TournamentName,
    t.Status,
    (SELECT COUNT(*) FROM TournamentRegistrations tr WHERE tr.TournamentID = t.TournamentID AND tr.Status = 'Approved') as RegisteredTeams
FROM Tournaments t
LIMIT 5;

-- Test if stored procedures exist
SELECT 
    'Stored Procedures Check' as Test,
    (SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema = 'EsportsManager' AND routine_name = 'sp_GetSystemStats') as sp_GetSystemStats,
    (SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema = 'EsportsManager' AND routine_name = 'sp_GetAllTournaments') as sp_GetAllTournaments,
    (SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema = 'EsportsManager' AND routine_name = 'sp_FixSystemData') as sp_FixSystemData;

-- Test the sp_GetAllTournaments procedure (which the service uses)
SELECT 'Testing sp_GetAllTournaments...' as Test;
CALL sp_GetAllTournaments();

-- Test the system stats procedure
SELECT 'Testing sp_GetSystemStats...' as Test;
CALL sp_GetSystemStats();

SELECT 'All tests completed!' as Status;
