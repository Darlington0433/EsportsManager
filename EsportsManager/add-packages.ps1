# Change to the project directory
Set-Location "d:\EsportsManager-ddat (1)\EsportsManager-ddat"

# Add package references
dotnet add src/EsportsManager.UI/EsportsManager.UI.csproj package Microsoft.Extensions.Hosting
dotnet add src/EsportsManager.UI/EsportsManager.UI.csproj package Microsoft.Extensions.DependencyInjection
dotnet add src/EsportsManager.UI/EsportsManager.UI.csproj package Microsoft.Extensions.Configuration
dotnet add src/EsportsManager.UI/EsportsManager.UI.csproj package Microsoft.Extensions.Configuration.Json

Write-Host "Packages added successfully!" -ForegroundColor Green
