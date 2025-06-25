# Authentication and Configurations in EsportsManager

## Authentication Changes

### Overview

The application now uses real user authentication instead of the demo role selection that was previously in place. This means that upon login, the system checks the database for valid credentials and assigns the corresponding role to the user session.

### Key Changes

- Removed `UserRoleSelector` class and all related demo role selection functionality
- Updated `UserAuthenticationForm` to store authenticated user roles from database
- Modified `ConsoleAppRunner` to use real user roles for menu routing
- Added proper session management via `UserSessionManager`

### Authentication Flow

1. User enters credentials in `UserAuthenticationForm`
2. `UserService.AuthenticateAsync()` validates credentials against the database
3. On successful authentication, user profile with actual role is stored in `UserSessionManager.CurrentUser`
4. `ConsoleAppRunner` uses the authenticated role to route to the appropriate menu
5. Menu services (`AdminMenuService`, `PlayerMenuService`, `ViewerMenuService`) have access to the real user profile

## Configuration System

### Overview

The application now uses a flexible configuration system that can be easily adapted for different environments. Configuration values can be set through:

1. `.env` file (for local development and sensitive values)
2. `appsettings.json` (for default configurations)
3. `appsettings.{Environment}.json` (for environment-specific settings)
4. Environment variables (for overriding any value at runtime)

### Configuration File Structure

```
# .env (sensitive values, not committed to version control)
ESPORTS_DB_TYPE=sqlserver
ESPORTS_DB_CONNECTION=Server=localhost;Database=EsportsManager;Trusted_Connection=true;TrustServerCertificate=true;
ASPNETCORE_ENVIRONMENT=Development
ENABLE_LOGGING=true
LOG_LEVEL=Debug

# appsettings.json (application defaults)
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### How to Add New Configuration

1. Add your setting to the appropriate configuration file
2. Access it in code through the `IConfiguration` interface:
   ```csharp
   var value = _configuration["SectionName:SettingName"];
   ```
   or
   ```csharp
   var section = _configuration.GetSection("SectionName");
   ```

### Environment-Specific Configuration

- Development: Use `appsettings.Development.json` and `.env`
- Production: Use `appsettings.json` and environment variables

## Database Configuration

### Connection String Management

Database connection string is now managed through configuration:

1. Default is set in `appsettings.json`
2. Can be overridden in `.env` file via `ESPORTS_DB_CONNECTION`
3. For production, should be set as an environment variable

### Connecting to Different Database Types

The application supports different database types:

- SQL Server: Set `ESPORTS_DB_TYPE=sqlserver` in configuration
- MySQL: Set `ESPORTS_DB_TYPE=mysql` in configuration (requires additional code changes)
