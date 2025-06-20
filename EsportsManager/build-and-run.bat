@echo off
echo ========================================
echo  Building Esports Manager (Optimized)
echo ========================================

echo.
echo [1/4] Cleaning previous builds...
dotnet clean src/EsportsManager.UI/EsportsManager.UI.csproj
dotnet clean src/EsportsManager.BL/EsportsManager.BL.csproj
dotnet clean src/EsportsManager.DAL/EsportsManager.DAL.csproj

echo.
echo [2/4] Restoring NuGet packages...
dotnet restore EsportsManager.Optimized.sln

echo.
echo [3/4] Building solution...
dotnet build EsportsManager.Optimized.sln --configuration Release --no-restore

echo.
echo [4/4] Running application...
cd src/EsportsManager.UI
dotnet run --configuration Release

pause
