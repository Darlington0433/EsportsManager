# MySQL Setup Guide for EsportsManager

## Prerequisites

1. **MySQL Server 8.0+** installed and running
2. **MySQL Client** or **MySQL Workbench** for executing scripts
3. Proper user permissions to create databases

## Quick Setup Steps

### Step 1: Test MySQL Compatibility

```bash
mysql -u root -p < test_mysql_compatibility.sql
```

### Step 2: Create Database Schema

```bash
mysql -u root -p < 01_create_database_schema_mysql.sql
```

### Step 3: Insert Sample Data

```bash
mysql -u root -p < 02_insert_sample_data_mysql.sql
```

## Connection Configuration

### For Application (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EsportsManager;Uid=root;Pwd=your_password;charset=utf8mb4;"
  }
}
```

### For Development

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=EsportsManager;Uid=esports_user;Pwd=esports_password;charset=utf8mb4;SslMode=none;"
  }
}
```

## Test Accounts

After running the sample data script, you can use these accounts:

### Admin Accounts

- **Username:** `admin` | **Password:** `Admin@123`
- **Username:** `admin2` | **Password:** `Admin@123`

### Player Accounts

- **Username:** `player1` | **Password:** `Player@123`
- **Username:** `player2` | **Password:** `Player@123`
- **Username:** `player3` | **Password:** `Player@123`

### Viewer Accounts

- **Username:** `viewer1` | **Password:** `Viewer@123`
- **Username:** `viewer2` | **Password:** `Viewer@123`

## Database Features

### Created Tables

- `Games` - Game definitions (LoL, CS2, Valorant, etc.)
- `Users` - User accounts with roles and authentication
- `Teams` - Team management with captain system
- `TeamMembers` - Team membership relationships
- `Tournaments` - Tournament management with multiple statuses
- `TournamentTeams` - Team registrations for tournaments
- `Matches` - Match scheduling and results
- `TournamentResults` - Final tournament rankings
- `Wallets` - User financial accounts
- `Transactions` - Financial transaction history
- `Donations` - Community donations system
- `Votes` - Community voting system
- `Feedback` - User feedback collection
- `SystemSettings` - Application configuration
- `SystemLogs` - System activity logging
- `Achievements` - Achievement definitions
- `UserAchievements` - User achievement progress

### Sample Data Includes

- **13 Users** (2 Admins, 6 Players, 5 Viewers)
- **4 Teams** with realistic membership
- **5 Tournaments** (2 active, 3 completed)
- **6 Matches** with complete results
- **15 Transactions** including prizes and donations
- **12 Community votes**
- **8 User feedback entries**
- **18 Achievement types**
- **15 User achievements**
- **25 System configuration settings**

## Troubleshooting

### Common Issues

1. **Character Set Error**

   ```sql
   ALTER DATABASE EsportsManager CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   ```

2. **Foreign Key Constraint Error**

   ```sql
   SET FOREIGN_KEY_CHECKS = 0;
   -- Run your script
   SET FOREIGN_KEY_CHECKS = 1;
   ```

3. **Timezone Issues**

   ```sql
   SET time_zone = '+07:00'; -- For Vietnam timezone
   ```

4. **Boolean Values**
   - MySQL uses `TRUE`/`FALSE` or `1`/`0` for boolean values
   - Our scripts use `TRUE`/`FALSE` for clarity

### Performance Tips

1. **Enable Query Cache** (if available)
2. **Use appropriate indexes** (already included in schema)
3. **Monitor slow queries** with MySQL slow query log
4. **Regular ANALYZE TABLE** for large datasets

## Security Recommendations

1. **Create dedicated database user**

   ```sql
   CREATE USER 'esports_user'@'localhost' IDENTIFIED BY 'strong_password';
   GRANT ALL PRIVILEGES ON EsportsManager.* TO 'esports_user'@'localhost';
   FLUSH PRIVILEGES;
   ```

2. **Use SSL connections** in production
3. **Regular backups** with mysqldump
4. **Update MySQL** to latest stable version

## Backup and Restore

### Create Backup

```bash
mysqldump -u root -p EsportsManager > esportsmanager_backup.sql
```

### Restore from Backup

```bash
mysql -u root -p EsportsManager < esportsmanager_backup.sql
```

## Production Considerations

1. **Use connection pooling** in application
2. **Implement proper indexing** for large datasets
3. **Monitor database performance** regularly
4. **Set up replication** for high availability
5. **Configure proper backup strategy**

## Support

If you encounter any issues:

1. Check MySQL error logs
2. Verify user permissions
3. Confirm MySQL version compatibility
4. Test with the compatibility script first

The database is now ready for use with the EsportsManager application!
