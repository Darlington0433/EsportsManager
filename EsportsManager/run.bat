@echo off
echo.
echo ===================================================
echo         ESPORTS MANAGER APPLICATION
echo ===================================================
echo.
echo Adding required NuGet packages...
echo.
dotnet add src\EsportsManager.UI\EsportsManager.UI.csproj package Microsoft.Extensions.Hosting
dotnet add src\EsportsManager.UI\EsportsManager.UI.csproj package Microsoft.Extensions.DependencyInjection
dotnet add src\EsportsManager.UI\EsportsManager.UI.csproj package Microsoft.Extensions.Configuration
dotnet add src\EsportsManager.UI\EsportsManager.UI.csproj package Microsoft.Extensions.Configuration.Json

echo.
echo Building Esports Manager Application...
dotnet build
if %ERRORLEVEL% NEQ 0 (
    echo Build failed. Please check the errors above.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo Running Esports Manager Application...
echo.
echo Sample login credentials:
echo - Admin:    username: admin,    password: admin123
echo - Player:   username: player1,  password: player123
echo - Viewer:   username: viewer1,  password: viewer123
echo.
cd src\EsportsManager.UI
dotnet run
pause
