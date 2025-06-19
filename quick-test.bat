@echo off
echo Building and running EsportsManager...
cd /d "c:\Users\tvmar\Desktop\LearnVTC\EsportsManager_Backup"
dotnet build --configuration Release
if %ERRORLEVEL% EQU 0 (
    echo Build successful! Running application...
    dotnet run --project "src\EsportsManager.UI\EsportsManager.UI.csproj" --configuration Release
) else (
    echo Build failed!
    pause
)
