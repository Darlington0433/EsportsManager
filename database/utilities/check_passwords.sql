USE EsportsManager;

-- Hiển thị thông tin hash của các tài khoản
SELECT 
    UserID, 
    Username, 
    PasswordHash,
    Status, 
    IsActive, 
    IsEmailVerified 
FROM 
    Users 
WHERE 
    Username IN ('admin', 'superadmin') 
    OR Username LIKE 'player%' 
    OR Username LIKE 'viewer%'
LIMIT 10;
