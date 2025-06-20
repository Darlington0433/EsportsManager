@echo off
echo.
echo ===================================================
echo         ESPORTS MANAGER APPLICATION
echo ===================================================
echo.
echo Starting Esports Manager Application...
echo.
echo Sample login credentials:
echo - Admin:    username: admin,    password: admin123
echo - Player:   username: player1,  password: player123
echo - Viewer:   username: viewer1,  password: viewer123
echo.
echo ===================================================
echo.
dotnet run --project .\src\EsportsManager.UI\EsportsManager.UI.csproj
echo.
echo Thank you for using Esports Manager!
pause
