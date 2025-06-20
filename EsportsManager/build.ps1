Write-Host "=======================================================" -ForegroundColor Yellow
Write-Host "          ESPORTS MANAGER PROJECT SETUP SCRIPT           " -ForegroundColor Yellow
Write-Host "=======================================================" -ForegroundColor Yellow
Write-Host ""

# Summary of what the script will do
Write-Host "This script will help you set up the project by:" -ForegroundColor Cyan
Write-Host " 1. Adding required NuGet packages" -ForegroundColor Cyan
Write-Host " 2. Creating a simplified project that can be built" -ForegroundColor Cyan
Write-Host " 3. Providing instructions to fix remaining issues" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press any key to continue..." -ForegroundColor Green
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Add required NuGet packages
Write-Host "`nAdding required NuGet packages..." -ForegroundColor Yellow
$packages = @(
    "Microsoft.Extensions.Hosting", 
    "Microsoft.Extensions.DependencyInjection", 
    "Microsoft.Extensions.Configuration", 
    "Microsoft.Extensions.Configuration.Json"
)

foreach ($package in $packages) {
    Write-Host "Adding package: $package" -ForegroundColor Cyan
    dotnet add src/EsportsManager.UI/EsportsManager.UI.csproj package $package
}

# Create a simplified Program.cs that will build
Write-Host "`nCreating a simplified Program.cs for building..." -ForegroundColor Yellow

# Backup the current Program.cs
if (Test-Path "src/EsportsManager.UI/Program.cs") {
    Copy-Item "src/EsportsManager.UI/Program.cs" -Destination "src/EsportsManager.UI/Program.cs.backup" -Force
    Write-Host "Original Program.cs backed up to Program.cs.backup" -ForegroundColor Green
}

# Create simplified Program.cs
$simpleProgramContent = @"
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace EsportsManager.UI
{
    class Program
    {
        // Make the ServiceProvider accessible for menu classes
        public static IServiceProvider ServiceProvider { get; private set; }

        static async Task Main(string[] args)
        {
            try
            {
                // Simple service provider
                ServiceProvider = new ServiceCollection()
                    .AddLogging()
                    .BuildServiceProvider();

                Console.WriteLine("Esports Manager");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(\$"Error: {ex.Message}");
                Console.ResetColor();
                Console.ReadKey();
            }
        }
    }
}
"@

Set-Content -Path "src/EsportsManager.UI/Program.cs" -Value $simpleProgramContent
Write-Host "Created simplified Program.cs" -ForegroundColor Green

# Try to build
Write-Host "`nAttempting to build project..." -ForegroundColor Yellow
$buildResult = dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nBuild successful!" -ForegroundColor Green
} else {
    Write-Host "`nBuild failed with errors." -ForegroundColor Red
}

# Provide instructions
Write-Host "`n=======================================================" -ForegroundColor Yellow
Write-Host "                  NEXT STEPS                          " -ForegroundColor Yellow
Write-Host "=======================================================" -ForegroundColor Yellow
Write-Host @"
1. To restore your original Program.cs:
   - Copy src/EsportsManager.UI/Program.cs.backup back to Program.cs

2. For a complete implementation:
   - Fix the errors identified in the business logic layer
   - Add missing properties to models
   - Update service interfaces and implementations

3. To run a simplified version:
   - Use the current simplified Program.cs
   - Run using: dotnet run --project src/EsportsManager.UI/EsportsManager.UI.csproj

"@ -ForegroundColor White

Write-Host "=======================================================" -ForegroundColor Yellow
