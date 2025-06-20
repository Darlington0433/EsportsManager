# Change to the project directory
Set-Location "d:\EsportsManager-ddat (1)\EsportsManager-ddat"

# Add required NuGet packages
Write-Host "Adding required NuGet packages..." -ForegroundColor Cyan
dotnet add src/EsportsManager.UI/EsportsManager.UI.csproj package Microsoft.Extensions.Hosting
dotnet add src/EsportsManager.UI/EsportsManager.UI.csproj package Microsoft.Extensions.DependencyInjection
dotnet add src/EsportsManager.UI/EsportsManager.UI.csproj package Microsoft.Extensions.Configuration
dotnet add src/EsportsManager.UI/EsportsManager.UI.csproj package Microsoft.Extensions.Configuration.Json

# Build the project
Write-Host "Building the project..." -ForegroundColor Cyan
dotnet build

# Check if build was successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "`nSample login credentials:" -ForegroundColor Yellow
    Write-Host "- Admin:    username: admin,    password: admin123" -ForegroundColor White
    Write-Host "- Player:   username: player1,  password: player123" -ForegroundColor White
    Write-Host "- Viewer:   username: viewer1,  password: viewer123" -ForegroundColor White
    Write-Host "`nRunning the application..." -ForegroundColor Green
    
    # Run the application
    dotnet run --project src/EsportsManager.UI/EsportsManager.UI.csproj
} else {
    Write-Host "Build failed. Please check the errors above." -ForegroundColor Red
}
