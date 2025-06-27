-- =====================================================
-- RUN ALL - COMPLETE DATABASE SETUP
-- =====================================================
-- Description: Execute all SQL modules in the correct order
-- Usage: Run this file to create the complete EsportsManager database
-- =====================================================

-- 01. Database Setup
SOURCE 01_database_setup.sql;

-- 02. Games Module
SOURCE 02_games.sql;

-- 03. Users Module  
SOURCE 03_users.sql;

-- 04. Teams Module
SOURCE 04_teams.sql;

-- 05. Tournaments Module
SOURCE 05_tournaments.sql;

-- 06. Wallet and Donations Module
SOURCE 06_wallet_donations.sql;

-- 07. Votes and Feedback Module
SOURCE 07_votes_feedback.sql;

-- 08. Admin Actions Module
SOURCE 08_admin_actions.sql;

-- 09. Rankings and Results Module
SOURCE 09_rankings_results.sql;

-- 10. Indexes Module
SOURCE 10_indexes.sql;

-- 11. Views Module
SOURCE 11_views.sql;

-- 12. Triggers Module
SOURCE 12_triggers.sql;

-- 13. Stored Procedures Module (Part 1)
SOURCE 13_procedures_part1.sql;

-- 13B. Wallet and Donation Procedures Module
SOURCE 13b_wallet_procedures.sql;

-- 14. Tournament Procedures Module
SOURCE 14_tournament_procedures.sql;

-- 15. Constraints Module
SOURCE 15_constraints.sql;

-- 16. Sample Data Modules (Split by Data Type)
SOURCE 16a_games_sample_data.sql;
SOURCE 16b_users_sample_data.sql;
SOURCE 16c_user_profiles_sample_data.sql;
SOURCE 16d_teams_sample_data.sql;
SOURCE 16e_tournaments_sample_data.sql;
SOURCE 16f_wallet_donations_sample_data.sql;
SOURCE 16g_votes_feedback_sample_data.sql;

SELECT '🎉 COMPLETE DATABASE SETUP FINISHED SUCCESSFULLY!' as FinalMessage;
SELECT '📊 Database: EsportsManager' as Info;
SELECT '🔧 Total Modules: 23' as ModuleCount;
SELECT '📅 Setup Date:', NOW() as SetupDate;
