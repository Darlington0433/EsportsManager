# Add required NuGet package for dependency injection extensions
cd "d:\EsportsManager-ddat (1)\EsportsManager-ddat"
dotnet add .\src\EsportsManager.UI\EsportsManager.UI.csproj package Microsoft.Extensions.DependencyInjection.Abstractions

# Build the solution to make sure all references are updated
dotnet build
