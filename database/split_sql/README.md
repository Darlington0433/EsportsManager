# 📁 EsportsManager Database Split Scripts

## 📖 Mô tả

Thư mục này chứa các file SQL được tách ra từ `COMPLETE_DATABASE_SETUP.sql` để dễ quản lý và bảo trì.

## 🗂️ Cấu trúc Files

| File                                   | Mô tả                                       | Phụ thuộc |
| -------------------------------------- | ------------------------------------------- | --------- |
| `01_create_database_and_tables.sql`    | Tạo database và tất cả các tables           | -         |
| `02_create_indexes.sql`                | Tạo indexes để tối ưu hiệu suất             | 01        |
| `03_create_views.sql`                  | Tạo views để query dễ dàng                  | 01, 02    |
| `04_create_triggers.sql`               | Tạo triggers tự động xử lý business logic   | 01        |
| `05_create_procedures_basic.sql`       | Stored procedures cơ bản (tournament, team) | 01-04     |
| `06_create_procedures_achievement.sql` | Stored procedures cho achievement system    | 01-05     |
| `07_create_procedures_wallet.sql`      | Stored procedures cho wallet system         | 01-06     |
| `08_add_constraints.sql`               | Thêm constraints đảm bảo tính toàn vẹn      | 01-07     |
| `09_insert_sample_data.sql`            | Chèn dữ liệu mẫu để test                    | 01-08     |
| `10_run_all_scripts.sql`               | Chạy tất cả scripts theo thứ tự             | -         |

## 🚀 Cách sử dụng

### Phương án 1: Chạy từng file riêng lẻ

```bash
mysql -u root -p < 01_create_database_and_tables.sql
mysql -u root -p < 02_create_indexes.sql
mysql -u root -p < 03_create_views.sql
mysql -u root -p < 04_create_triggers.sql
mysql -u root -p < 05_create_procedures_basic.sql
mysql -u root -p < 06_create_procedures_achievement.sql
mysql -u root -p < 07_create_procedures_wallet.sql
mysql -u root -p < 08_add_constraints.sql
mysql -u root -p < 09_insert_sample_data.sql
```

### Phương án 2: Chạy file tổng hợp

```bash
mysql -u root -p < 10_run_all_scripts.sql
```

### Phương án 3: Sử dụng MySQL Workbench

1. Mở MySQL Workbench
2. Chạy từng file theo thứ tự từ 01 đến 09
3. Hoặc chạy file `10_run_all_scripts.sql`

## 🎯 Lợi ích của việc tách files

### ✅ **Quản lý dễ dàng**

- Mỗi file có chức năng riêng biệt
- Dễ tìm và sửa lỗi
- Code có tổ chức tốt hơn

### ✅ **Bảo trì linh hoạt**

- Có thể chạy từng phần độc lập
- Dễ debug khi có lỗi
- Có thể skip những phần không cần thiết

### ✅ **Version Control tốt hơn**

- Git diff rõ ràng khi có thay đổi
- Merge conflicts ít hơn
- History thay đổi dễ theo dõi

### ✅ **Deployment linh hoạt**

- Có thể deploy từng phần
- Rollback dễ dàng khi cần
- Update incremental

### ✅ **Teamwork hiệu quả**

- Nhiều developer có thể làm việc song song
- Ít conflict khi nhiều người sửa
- Review code dễ dàng hơn

## 📊 Nội dung từng file

### 01_create_database_and_tables.sql

- Tạo database `EsportsManager`
- Tạo tất cả tables: Users, Games, Teams, Tournaments, Wallets, Achievements, etc.
- Thiết lập foreign keys và basic indexes

### 02_create_indexes.sql

- Indexes cho performance: Users, Teams, Tournaments
- Indexes cho search: Email, Username, Status
- Indexes cho reporting: Dates, Amounts

### 03_create_views.sql

- `v_team_stats`: Thống kê team
- `v_tournament_stats`: Thống kê tournament
- `v_player_stats`: Thống kê player
- `v_user_wallet_summary`: Tóm tắt wallet

### 04_create_triggers.sql

- Auto create wallet cho Player
- Auto create profile cho User
- Auto update wallet khi donation/withdrawal
- Auto log transactions

### 05_create_procedures_basic.sql

- Tournament procedures: Get, Register, Create
- Team procedures: Management, Registration
- Basic CRUD operations

### 06_create_procedures_achievement.sql

- `sp_AssignAchievement`: Gán thành tích cho player
- `sp_GetPlayerAchievements`: Lấy danh sách thành tích
- `sp_GetPlayerStats`: Thống kê player
- `sp_GetPlayerTournamentHistory`: Lịch sử tournament

### 07_create_procedures_wallet.sql

- `sp_CreateWithdrawal`: Tạo withdrawal (tức thì)
- `sp_CreateDeposit`: Tạo deposit transaction
- `sp_GetWalletTransactionHistory`: Lịch sử giao dịch
- Donation procedures

### 08_add_constraints.sql

- Check constraints cho amounts, dates
- Data validation rules
- Business logic constraints

### 09_insert_sample_data.sql

- Sample games: LoL, CS2, Valorant, Dota 2
- Sample users: Admin, Players, Viewers (với BCrypt passwords)
- Sample teams và tournaments
- Sample achievements và donations

## 🔧 Troubleshooting

### Lỗi Foreign Key

```sql
SET FOREIGN_KEY_CHECKS = 0;
-- Chạy scripts
SET FOREIGN_KEY_CHECKS = 1;
```

### Lỗi Permissions

```sql
GRANT ALL PRIVILEGES ON EsportsManager.* TO 'username'@'localhost';
FLUSH PRIVILEGES;
```

### Kiểm tra setup thành công

```sql
USE EsportsManager;
SHOW TABLES;
SHOW PROCEDURE STATUS WHERE Db = 'EsportsManager';
SELECT COUNT(*) FROM Users;
```

## 📝 Notes

- **MySQL Version**: Được test trên MySQL 8.0+
- **Character Set**: UTF8MB4 để support Unicode đầy đủ
- **Engine**: InnoDB cho ACID compliance
- **Passwords**: Sử dụng BCrypt hashing
- **Sample Data**: Có sẵn để test ngay

## 🤝 Contributing

Khi thêm/sửa database schema:

1. Tạo file migration mới với số thứ tự tiếp theo
2. Update file `10_run_all_scripts.sql`
3. Test trên database clean
4. Document changes trong README

## 📞 Support

Nếu có vấn đề với database setup, kiểm tra:

1. MySQL server đang chạy
2. User có quyền CREATE DATABASE
3. Chạy scripts theo đúng thứ tự
4. Kiểm tra MySQL error log
