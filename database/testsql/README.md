# EsportsManager Database - Split SQL Modules

## Mô tả

Đây là hệ thống database được tách từ file `testsql.sql` thành các module riêng biệt theo chức năng và loại dữ liệu. Mỗi module được đánh số thứ tự để đảm bảo chạy đúng thứ tự dependencies.

## Cấu trúc Module

### 📋 Danh sách các Module (theo thứ tự thực thi)

| STT | File                                   | Mô tả                                                | Dependencies       |
| --- | -------------------------------------- | ---------------------------------------------------- | ------------------ |
| 01  | `01_database_setup.sql`                | Tạo database và cấu hình cơ bản                      | Không              |
| 02  | `02_games.sql`                         | Bảng Games và quản lý trò chơi                       | 01                 |
| 03  | `03_users.sql`                         | Bảng Users và UserProfiles                           | 01                 |
| 04  | `04_teams.sql`                         | Bảng Teams và TeamMembers                            | 01, 02, 03         |
| 05  | `05_tournaments.sql`                   | Bảng Tournaments và TournamentRegistrations          | 01, 02, 03, 04     |
| 06  | `06_wallet_donations.sql`              | Hệ thống tài chính (Wallets, Donations, Withdrawals) | 01, 03             |
| 07  | `07_votes_feedback.sql`                | Hệ thống voting và feedback                          | 01, 03, 05         |
| 08  | `08_admin_actions.sql`                 | Audit log cho Admin                                  | 01, 03             |
| 09  | `09_rankings_results.sql`              | Bảng xếp hạng và kết quả giải đấu                    | 01, 02, 03, 04, 05 |
| 10  | `10_indexes.sql`                       | Tạo indexes cho performance                          | 02-09              |
| 11  | `11_views.sql`                         | Tạo views cho queries thường dùng                    | 02-09              |
| 12  | `12_triggers.sql`                      | Tự động hóa với triggers                             | 02-09              |
| 13  | `13_procedures_part1.sql`              | Stored procedures cơ bản                             | 02-09              |
| 13B | `13b_wallet_procedures.sql`            | Stored procedures cho wallet và donation             | 06, 03             |
| 14  | `14_tournament_procedures.sql`         | Stored procedures cho tournaments                    | 02-09              |
| 15  | `15_constraints.sql`                   | Data integrity constraints                           | 02-09              |
| 16A | `16a_games_sample_data.sql`            | Dữ liệu mẫu Games                                    | 16, 02             |
| 16B | `16b_users_sample_data.sql`            | Dữ liệu mẫu Users (Admin, Player, Viewer)            | 16, 03             |
| 16C | `16c_user_profiles_sample_data.sql`    | Dữ liệu mẫu UserProfiles                             | 16B                |
| 16D | `16d_teams_sample_data.sql`            | Dữ liệu mẫu Teams và TeamMembers                     | 16A, 16B           |
| 16E | `16e_tournaments_sample_data.sql`      | Dữ liệu mẫu Tournaments và Results                   | 16A, 16B, 16D      |
| 16F | `16f_wallet_donations_sample_data.sql` | Dữ liệu mẫu Wallets và Donations                     | 16B                |
| 16G | `16g_votes_feedback_sample_data.sql`   | Dữ liệu mẫu Votes và AdminActions                    | 16B, 16E           |

## 🎯 Phân loại theo chức năng

### 🎮 **Core System Tables**

- **Games** (`02_games.sql`): Quản lý các trò chơi esports
- **Users** (`03_users.sql`): Hệ thống người dùng với 3 roles (Admin, Player, Viewer)

### 👥 **Team Management**

- **Teams** (`04_teams.sql`): Quản lý đội và thành viên đội

### 🏆 **Tournament System**

- **Tournaments** (`05_tournaments.sql`): Hệ thống giải đấu và đăng ký
- **Rankings & Results** (`09_rankings_results.sql`): Xếp hạng và kết quả

### 💰 **Financial System**

- **Wallet & Donations** (`06_wallet_donations.sql`): Hệ thống ví và donate

### 📊 **Engagement Features**

- **Votes & Feedback** (`07_votes_feedback.sql`): Voting và feedback

### 🔧 **Admin Features**

- **Admin Actions** (`08_admin_actions.sql`): Audit trail cho admin

### ⚡ **Performance & Logic**

- **Indexes** (`10_indexes.sql`): Tối ưu hóa performance
- **Views** (`11_views.sql`): Views cho queries phức tạp
- **Triggers** (`12_triggers.sql`): Tự động hóa business logic
- **Procedures** (`13_procedures_part1.sql`, `13b_wallet_procedures.sql`, `14_tournament_procedures.sql`): Stored procedures

### 📝 **Sample Data Modules**

- **16A** (`16a_games_sample_data.sql`): Dữ liệu mẫu Games
- **16B** (`16b_users_sample_data.sql`): Dữ liệu mẫu Users
- **16C** (`16c_user_profiles_sample_data.sql`): Dữ liệu mẫu UserProfiles
- **16D** (`16d_teams_sample_data.sql`): Dữ liệu mẫu Teams
- **16E** (`16e_tournaments_sample_data.sql`): Dữ liệu mẫu Tournaments
- **16F** (`16f_wallet_donations_sample_data.sql`): Dữ liệu mẫu Wallets
- **16G** (`16g_votes_feedback_sample_data.sql`): Dữ liệu mẫu Votes

## 🚀 Cách sử dụng

### Option 1: Chạy từng file thủ công

```sql
-- Kết nối MySQL và chạy từng file theo thứ tự 01-16G
mysql -u root -p < 01_database_setup.sql
mysql -u root -p < 02_games.sql
mysql -u root -p < 03_users.sql
-- ... tiếp tục cho đến file 16g
```

### Option 2: Chạy tất cả cùng lúc

```sql
-- Chạy file RUN_ALL.sql (không khuyến khích vì khó debug)
mysql -u root -p < RUN_ALL.sql
```

### Option 3: Khuyến khích - Chạy từng module và kiểm tra

```sql
-- 1. Setup cơ bản
mysql -u root -p < 01_database_setup.sql

-- 2. Core tables
mysql -u root -p < 02_games.sql
mysql -u root -p < 03_users.sql

-- 3. Business logic tables
mysql -u root -p < 04_teams.sql
mysql -u root -p < 05_tournaments.sql
mysql -u root -p < 06_wallet_donations.sql
mysql -u root -p < 07_votes_feedback.sql
mysql -u root -p < 08_admin_actions.sql
mysql -u root -p < 09_rankings_results.sql

-- 4. Performance và logic
mysql -u root -p < 10_indexes.sql
mysql -u root -p < 11_views.sql
mysql -u root -p < 12_triggers.sql
mysql -u root -p < 13_procedures_part1.sql
mysql -u root -p < 14_tournament_procedures.sql
mysql -u root -p < 15_constraints.sql

-- 5. Sample data (optional - run specific modules as needed)
mysql -u root -p < 16a_games_sample_data.sql
mysql -u root -p < 16b_users_sample_data.sql
mysql -u root -p < 16c_user_profiles_sample_data.sql
mysql -u root -p < 16d_teams_sample_data.sql
mysql -u root -p < 16e_tournaments_sample_data.sql
mysql -u root -p < 16f_wallet_donations_sample_data.sql
mysql -u root -p < 16g_votes_feedback_sample_data.sql
```

## 📝 Lưu ý quan trọng

### ⚠️ Dependencies

- **BẮT BUỘC** chạy đúng thứ tự từ 01-16
- Module sau phụ thuộc vào module trước (foreign keys)
- Không được skip module nào trong 01-15

### 🔐 Sample Data (Modules 16A-16G)

- **16A**: Games data (7 games: LoL, CS2, Valorant, Dota 2, FIFA 24, Rocket League, Overwatch 2)
- **16B**: Users data (2 Admin, 5 Player, 3 Viewer)
  - Admin accounts: `admin/admin123`, `superadmin/admin123`
  - Player accounts: `player1/player123` đến `player5/player123`
  - Viewer accounts: `viewer1/viewer123` đến `viewer3/viewer123`
- **16C**: User profiles with detailed information (bio, avatar, personal details)
- **16D**: Teams and team members (5 teams: Dragons Gaming, Phoenix Valorant, Dota Masters, Football Kings, Rocket Stars)
- **16E**: Tournaments, registrations, and rankings (5 tournaments with different statuses)
- **16F**: Wallet balances and donations (Total $140 in sample donations)
- **16G**: Votes, feedback, and admin actions (5 votes, 7 admin actions for audit trail)
- **Password đã được hash bằng BCrypt và test thành công**

### 🛠️ Troubleshooting

- Nếu gặp lỗi foreign key: Kiểm tra xem đã chạy đủ các module dependencies chưa
- Nếu gặp lỗi syntax: Đảm bảo đang dùng MySQL (không phải SQL Server)
- Nếu muốn reset: Xóa database và chạy lại từ đầu

## 🎯 Ưu điểm của việc tách module

1. **Dễ bảo trì**: Mỗi module chỉ tập trung vào 1 chức năng cụ thể
2. **Dễ debug**: Lỗi xảy ra ở module nào thì dễ dàng xác định
3. **Tái sử dụng**: Có thể chỉ chạy các module cần thiết
4. **Phát triển parallel**: Team có thể làm việc trên các module khác nhau
5. **Testing**: Có thể test từng module riêng lẻ
6. **Documentation**: Mỗi module có mô tả rõ ràng về chức năng

## 🔄 Cập nhật và Maintenance

- Khi thêm tính năng mới: Tạo module mới với số thứ tự tiếp theo
- Khi sửa lỗi: Chỉ cần sửa module liên quan
- Khi backup: Có thể backup từng module riêng biệt
- Khi migrate: Dễ dàng điều chỉnh từng module

---

**📌 Created by**: EsportsManager Development Team  
**📅 Date**: 2025-06-27  
**🔄 Version**: 2.0 (Split Sample Data Edition)  
**📧 Contact**: Phan Nhat Quan - EsportsManager Project  
**📊 Total Modules**: 22 (15 structure + 7 sample data)
