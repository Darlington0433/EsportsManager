# EsportsManager Implementation Notes

## Current Issues and How to Fix

### Program Entry Point
The project has two entry points:
- `Program.cs` (Legacy UI implementation)
- `Program_Simple.cs` (New dependency injection implementation)

To use the new implementation properly:
1. Rename `Program_Simple.cs` to `Program.cs` (backup the original first if needed)
2. Use the new `Program.cs` which utilizes dependency injection

### Known Warnings

#### Async Methods Without Await
Many async methods have warnings about lacking await operators. This is expected for now because:
- The current implementation uses in-memory collections and doesn't have real database access
- The methods are designed to be asynchronous for future implementation
- These warnings can be safely ignored until real database access is implemented

To silence these warnings, you can add the following pragma at the top of the service files:
```csharp
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
```

#### ConsoleInput Issues
There are several error messages related to ConsoleInput methods:
- Some methods like `GetDateTime()` don't exist
- Some methods have incorrect parameter types

To fix these, update the ConsoleInput class to include these methods or update the parameters in method calls.

#### WalletService Null Reference Warnings
Some warnings about possible null references in WalletService. These have been partially fixed, but a full review is needed to ensure all potential null references are handled.

#### Donation Model Issues
The Donation model seems to be missing some properties like RecipientType and RecipientId. Update the Donation model to include these properties.

## How to Run the Application

1. Use the provided `run.bat` file which:
   - Builds the application
   - Runs it from the correct directory
   - Provides sample login credentials

2. Sample login credentials:
   - Admin: username: admin, password: admin123
   - Player: username: player1, password: player123
   - Viewer: username: viewer1, password: viewer123
