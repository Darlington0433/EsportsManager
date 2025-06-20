# Troubleshooting Guide for Esports Manager

This document outlines common issues and solutions for the Esports Manager project.

## Build Errors

### UserService Errors

Error:
```
Cannot implicitly convert type 'EsportsManager.DAL.Models.User' to 'bool'
```

Solution:
- Fix the return type in UserRepository.UpdateAsync to return bool instead of User
- Or update the UserService implementation to handle User return type correctly

### WalletTransaction Properties

Error:
```
'WalletTransaction' does not contain a definition for 'UserId' and 'Type'
```

Solution:
- Add UserId property to WalletTransaction class
- Add Type property as an alias for TransactionType

### Missing Methods in Services

Error:
```
'IDonationService' does not contain a definition for 'CreateAsync'
```

Solution:
- Add missing methods to service interfaces
- Implement the methods in the service classes

### ConsoleInput.GetDateTime

Error:
```
'ConsoleInput' does not contain a definition for 'GetDateTime'
```

Solution:
- Add GetDateTime method to ConsoleInput class:

```csharp
public static DateTime GetDateTime(string prompt, DateTime? min = null, DateTime? max = null)
{
    min ??= DateTime.MinValue;
    max ??= DateTime.MaxValue;
    
    while (true)
    {
        Console.Write($"{prompt} (dd/MM/yyyy HH:mm): ");
        var input = Console.ReadLine()?.Trim();

        if (DateTime.TryParse(input, out var value))
        {
            if (value >= min && value <= max)
            {
                return value;
            }
            else
            {
                ConsoleHelper.ShowError($"Please enter a date between {min:dd/MM/yyyy HH:mm} and {max:dd/MM/yyyy HH:mm}.");
            }
        }
        else
        {
            ConsoleHelper.ShowError("Please enter a valid date in format dd/MM/yyyy HH:mm.");
        }
    }
}
```

### Missing Properties in Models

Error:
```
'Donation' does not contain a definition for 'UserId', 'RecipientType', etc.
```

Solution:
- Add missing properties to Donation class:

```csharp
public int UserId { get; set; }  // User who made the donation
public string RecipientType { get; set; } = "User";  // User, Team, Tournament
public int RecipientId { get; set; }  // UserId, TeamId, or TournamentId
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
```

### ConsoleInput Parameter Type Mismatch

Error:
```
Argument 2: cannot convert from 'string' to 'bool'
```

Solution:
- Check and fix the ConsoleInput method signatures to match usage in menu classes
- Update menu class calls to match the ConsoleInput method signatures

## Warnings

### Async Methods Without Await

Warning:
```
This async method lacks 'await' operators and will run synchronously
```

These warnings can be ignored for now as they don't prevent the code from running. In a production environment, you might want to:
1. Add actual async calls where appropriate
2. Or remove the async/await keywords from methods that don't need them

## Running a Simplified Version

To run a simplified version of the application:

1. Use the `build.ps1` script to create a simplified Program.cs
2. Build the project
3. Run using: `dotnet run --project src/EsportsManager.UI/EsportsManager.UI.csproj`

This will allow you to gradually fix issues while maintaining a buildable project.
