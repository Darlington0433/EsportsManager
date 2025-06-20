# Esports Manager System

A comprehensive C# .NET application for managing esports tournaments, teams, players, and competitions.

## Overview

Esports Manager is a console-based application that provides management capabilities for different user roles:

- **Admin**: Manage users, tournaments, teams, and system statistics
- **Player**: Participate in tournaments, manage teams, track achievements
- **Viewer**: View tournaments, donate to players/teams, provide feedback, vote on events

## Project Structure

The application follows a 3-Layer Architecture:

1. **User Interface (UI) Layer**
   - Console-based menus and user interaction
   - Role-specific menu handling

2. **Business Logic (BL) Layer**
   - Services for business operations
   - Data validation and processing

3. **Data Access (DAL) Layer**
   - Repositories for data storage and retrieval
   - Database context and connections

## Features

- User management with role-based access
- Tournament creation and management
- Team management
- Achievement tracking
- Wallet system with transactions
- Donation capabilities
- Feedback submission
- Voting system
- Statistics and reporting

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022+ or VS Code

## Troubleshooting

### Using the Simplified Build Script

If you encounter issues with building the project, you can use the provided `build.ps1` PowerShell script to create a simplified version that will build:

```powershell
# Run the build script
.\build.ps1
```

This script will:
1. Add all required NuGet packages
2. Back up your current Program.cs
3. Create a simplified Program.cs that will build without errors
4. Attempt to build the project
5. Provide instructions for next steps

You can then gradually fix the issues in the codebase while maintaining a buildable project.

### PowerShell Execution Policy

If you encounter issues with running PowerShell scripts, you might need to adjust the execution policy. Run PowerShell as Administrator and use:

```powershell
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
```

Then navigate to the project directory and run the script.

### Build Errors

If you encounter build errors, make sure you have:

1. The correct .NET SDK version installed
2. All required NuGet packages added (as specified in the run scripts)
3. No syntax errors in your code

### Command Chaining in PowerShell

If you're using PowerShell and encounter issues with the `&&` operator for command chaining, use the `;` operator instead, or use:

```powershell
cmd1; if ($?) { cmd2 }
```

### Running the Application

#### Using Batch Script (Windows CMD)

1. Double-click on `run.bat` or `run-esports-manager.bat` in the root directory
   - This will add required NuGet packages, build, and run the application

#### Using PowerShell Script

1. Right-click on `run-esports-manager.ps1` and select "Run with PowerShell"
   - If you encounter execution policy issues, you can run PowerShell as Administrator and use:
     ```powershell
     Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
     ```
     Then navigate to the project directory and run:
     ```powershell
     .\run-esports-manager.ps1
     ```

#### Manual Run

1. Open a terminal/command prompt
2. Navigate to the project root directory
3. Run the following commands:

```bash
# Add required NuGet packages
dotnet add src/EsportsManager.UI/EsportsManager.UI.csproj package Microsoft.Extensions.Hosting
dotnet add src/EsportsManager.UI/EsportsManager.UI.csproj package Microsoft.Extensions.DependencyInjection
dotnet add src/EsportsManager.UI/EsportsManager.UI.csproj package Microsoft.Extensions.Configuration
dotnet add src/EsportsManager.UI/EsportsManager.UI.csproj package Microsoft.Extensions.Configuration.Json

# Build the project
dotnet build

# Run the application
cd src/EsportsManager.UI
dotnet run
```

### Sample Login Credentials

- **Admin**: username: admin, password: admin123
- **Player**: username: player1, password: player123
- **Viewer**: username: viewer1, password: viewer123

## Development Notes

- Services use in-memory collections for development purposes
- Async methods are prepared for future database implementation
- Dependency Injection is used throughout the application

## Project Structure

```
src/
├── EsportsManager.BL/          # Business Logic Layer
│   ├── DTOs/                   # Data Transfer Objects
│   ├── Interfaces/             # Service interfaces
│   ├── Models/                 # Business models
│   └── Services/               # Service implementations
│
├── EsportsManager.DAL/         # Data Access Layer
│   ├── Context/                # Database context
│   ├── Interfaces/             # Repository interfaces
│   ├── Models/                 # Data models
│   └── Repositories/           # Repository implementations
│
└── EsportsManager.UI/          # User Interface Layer
    ├── Configuration/          # Application configuration
    ├── Menus/                  # Menu implementations
    └── Utilities/              # UI helper classes
```
