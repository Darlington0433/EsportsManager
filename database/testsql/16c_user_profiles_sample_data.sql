-- =====================================================
-- 16C. USER PROFILES SAMPLE DATA
-- =====================================================
-- Module: Sample User Profiles Data
-- Description: Update user profiles with detailed personal information
-- Dependencies: 16b_users_sample_data.sql
-- =====================================================

USE EsportsManager;

-- Update profiles for users with more detailed information (they're already created by the trigger)
UPDATE UserProfiles SET
    DateOfBirth = '1990-01-15',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Ho Chi Minh City',
    Bio = 'System Administrator',
    AvatarURL = '/images/avatars/admin.png'
WHERE UserID = 1;

UPDATE UserProfiles SET
    DateOfBirth = '1988-05-20',
    Gender = 'Female',
    Country = 'Vietnam',
    City = 'Hanoi',
    Bio = 'Super Administrator',
    AvatarURL = '/images/avatars/superadmin.png'
WHERE UserID = 2;

UPDATE UserProfiles SET
    DateOfBirth = '1995-03-24',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Hanoi',
    Bio = 'Professional League of Legends player with 5 years experience',
    AvatarURL = '/images/avatars/player1.png'
WHERE UserID = 3;

UPDATE UserProfiles SET
    DateOfBirth = '1997-07-12',
    Gender = 'Female',
    Country = 'Vietnam',
    City = 'Ho Chi Minh City',
    Bio = 'Former CS:GO champion, now playing Valorant professionally',
    AvatarURL = '/images/avatars/player2.png'
WHERE UserID = 4;

UPDATE UserProfiles SET
    DateOfBirth = '1998-11-03',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Da Nang',
    Bio = 'Dota 2 specialist, twice finalist in national tournaments',
    AvatarURL = '/images/avatars/player3.png'
WHERE UserID = 5;

UPDATE UserProfiles SET
    DateOfBirth = '1996-08-18',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Can Tho',
    Bio = 'FIFA professional player, represented Vietnam in Asia Cup',
    AvatarURL = '/images/avatars/player4.png'
WHERE UserID = 6;

UPDATE UserProfiles SET
    DateOfBirth = '1999-02-27',
    Gender = 'Female',
    Country = 'Vietnam',
    City = 'Hai Phong',
    Bio = 'Rocket League champion, streaming on Twitch',
    AvatarURL = '/images/avatars/player5.png'
WHERE UserID = 7;

UPDATE UserProfiles SET
    DateOfBirth = '1994-04-15',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Ho Chi Minh City',
    Bio = 'Esports enthusiast and supporter',
    AvatarURL = '/images/avatars/viewer1.png'
WHERE UserID = 8;

UPDATE UserProfiles SET
    DateOfBirth = '1992-09-22',
    Gender = 'Female',
    Country = 'Vietnam',
    City = 'Hanoi',
    Bio = 'Regular tournament viewer and commentator',
    AvatarURL = '/images/avatars/viewer2.png'
WHERE UserID = 9;

UPDATE UserProfiles SET
    DateOfBirth = '1997-12-05',
    Gender = 'Male',
    Country = 'Vietnam',
    City = 'Da Nang',
    Bio = 'Esports blogger and fan',
    AvatarURL = '/images/avatars/viewer3.png'
WHERE UserID = 10;

SELECT '16C. User profiles sample data updated successfully!' as Message;
