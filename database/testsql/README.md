# EsportsManager Test Database Scripts

This folder contains MySQL-compatible database scripts for the EsportsManager project. These scripts provide a complete, clean database schema and comprehensive sample data for testing and development.

## Files Overview

### Core Scripts

1. **`test_mysql_compatibility.sql`** - MySQL compatibility test (run this first)
2. **`01_create_database_schema_mysql.sql`** - Complete MySQL database schema
3. **`02_insert_sample_data_mysql.sql`** - Comprehensive sample data
4. **`MYSQL_SETUP_GUIDE.md`** - Detailed setup instructions

### Legacy Scripts (Deprecated)

- `01_create_database_schema.sql` - Original SQL Server version
- `02_insert_sample_data.sql` - Original SQL Server version

## Quick Start

### 1. Test MySQL Compatibility

```bash
mysql -u root -p < test_mysql_compatibility.sql
```

### 2. Create Database Schema

```bash
mysql -u root -p < 01_create_database_schema_mysql.sql
```

### 3. Insert Sample Data

```bash
mysql -u root -p < 02_insert_sample_data_mysql.sql
```

## Test Accounts

| Username | Password   | Role   | Purpose                 |
| -------- | ---------- | ------ | ----------------------- |
| admin    | Admin@123  | Admin  | Primary administrator   |
| admin2   | Admin@123  | Admin  | Secondary administrator |
| player1  | Player@123 | Player | Tournament champion     |
| player2  | Player@123 | Player | Tournament runner-up    |
| viewer1  | Viewer@123 | Viewer | Community supporter     |

## Usage

1. **Create Database**:

   ```sql
   -- Run 01_create_database_schema.sql
   -- This will drop existing database and create fresh schema
   ```

2. **Insert Test Data**:

   ```sql
   -- Run 02_insert_sample_data.sql
   -- This will populate all tables with realistic test data
   ```

3. **Verify Installation**:
   ```sql
   -- Check data counts
   SELECT 'Users' as TableName, COUNT(*) as Count FROM Users
   UNION ALL
   SELECT 'Teams', COUNT(*) FROM Teams
   UNION ALL
   SELECT 'Tournaments', COUNT(*) FROM Tournaments
   UNION ALL
   SELECT 'Transactions', COUNT(*) FROM Transactions;
   ```

## Database Features

### Security

- BCrypt password hashing for all users
- Security questions and answers
- Role-based access control (Admin, Player, Viewer)

### Financial System

- Complete wallet management
- Transaction tracking with status
- Donation system with anonymous options
- Prize money distribution

### Tournament Management

- Multiple tournament types (Elimination, RoundRobin, Swiss)
- Team registration and approval workflow
- Match scheduling and results
- Tournament brackets and rankings

### Social Features

- Voting system for tournaments, teams, players
- Feedback system with categories and priorities
- Achievement system with multiple types
- Community interaction features

### System Management

- Comprehensive system settings
- Activity logging with different levels
- User statistics and analytics
- Data integrity constraints

## Development Notes

- All foreign key relationships are properly defined
- Indexes are optimized for common queries
- Data types are appropriate for Vietnamese currency (VND)
- Sample data includes realistic tournament scenarios
- All features have been tested with the provided data

## Integration with Application

These scripts are designed to work seamlessly with the EsportsManager application's:

- Entity Framework models
- Repository pattern implementation
- Service layer business logic
- Authentication and authorization
- All CRUD operations

The sample data provides complete test scenarios for:

- User management and authentication
- Tournament creation and management
- Team formation and competition
- Financial transactions and donations
- Community voting and feedback
- Achievement earning and tracking
- System administration tasks
