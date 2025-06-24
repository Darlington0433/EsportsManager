# Hướng dẫn sử dụng SQL Database

## Tổng quan

Database EsportsManager đã được chia thành nhiều file SQL nhỏ để dễ quản lý và thực thi theo thứ tự. Việc này giúp việc cài đặt database trở nên có cấu trúc và có thể tùy chỉnh từng phần riêng biệt.

## Thứ tự chạy các file SQL

Hãy chạy các file theo thứ tự sau trong MySQL Workbench hoặc MySQL Command Line:

1. **01_create_database_and_tables.sql**: Tạo database và các bảng cơ bản (bao gồm Users, Teams, Tournaments, Wallets, WalletTransactions, ...)
2. **02_create_indexes.sql**: Tạo các indexes để tối ưu hiệu năng truy vấn
3. **03_create_views.sql**: Tạo các views để đơn giản hóa các truy vấn phức tạp
4. **04_create_triggers.sql**: Tạo các triggers tự động hóa nghiệp vụ
5. **05_create_procedures.sql**: Tạo các stored procedures cơ bản
6. **06_add_constraints.sql**: Thêm các ràng buộc dữ liệu
7. **07_sample_data.sql**: Thêm dữ liệu mẫu, bao gồm tài khoản admin
8. **08_tournament_procedures.sql**: Tạo các stored procedures liên quan đến giải đấu

**QUAN TRỌNG**: Phải đảm bảo thực hiện theo đúng thứ tự trên. Nếu không các view và trigger sẽ gặp lỗi do thiếu bảng hoặc cấu trúc bảng không đúng. Các file đã được thiết kế để chạy tuần tự, với mỗi file phụ thuộc vào các file trước đó.

## Chạy từ Command Line

Để chạy từng file SQL theo thứ tự từ dòng lệnh MySQL:

### Windows
```
mysql -u root -p < 01_create_database_and_tables.sql
mysql -u root -p EsportsManager < 02_create_indexes.sql
mysql -u root -p EsportsManager < 03_create_views.sql
mysql -u root -p EsportsManager < 04_create_triggers.sql
mysql -u root -p EsportsManager < 05_create_procedures.sql
mysql -u root -p EsportsManager < 06_add_constraints.sql
mysql -u root -p EsportsManager < 07_sample_data.sql
mysql -u root -p EsportsManager < 08_tournament_procedures.sql
```

### Linux/Mac
```
mysql -u root -p < 01_create_database_and_tables.sql
mysql -u root -p EsportsManager < 02_create_indexes.sql
mysql -u root -p EsportsManager < 03_create_views.sql
mysql -u root -p EsportsManager < 04_create_triggers.sql
mysql -u root -p EsportsManager < 05_create_procedures.sql
mysql -u root -p EsportsManager < 06_add_constraints.sql
mysql -u root -p EsportsManager < 07_sample_data.sql
mysql -u root -p EsportsManager < 08_tournament_procedures.sql
```

Lưu ý: File đầu tiên sẽ tạo database nên không cần chỉ định tên database. Các file còn lại phải chỉ định database EsportsManager.

## Đồng bộ với file tổng hợp

Tất cả nội dung trong các file SQL riêng lẻ này đã được tích hợp đầy đủ trong file `../esportsmanager.sql`. Hai cách cài đặt này hoàn toàn tương đương và sẽ tạo ra cùng một cấu trúc cơ sở dữ liệu:

1. Chạy file `esportsmanager.sql` trong thư mục gốc
2. Chạy tuần tự 8 file trong thư mục `split_sql/` theo thứ tự đã nêu

Nếu bạn muốn tạo lại file tổng hợp sau khi sửa đổi các file riêng lẻ, bạn có thể kết hợp chúng bằng lệnh:

### Windows
```
type 01_create_database_and_tables.sql 02_create_indexes.sql 03_create_views.sql 04_create_triggers.sql 05_create_procedures.sql 06_add_constraints.sql 07_sample_data.sql 08_tournament_procedures.sql > ..\esportsmanager.sql
```

### Linux/Mac
```
cat 01_create_database_and_tables.sql 02_create_indexes.sql 03_create_views.sql 04_create_triggers.sql 05_create_procedures.sql 06_add_constraints.sql 07_sample_data.sql 08_tournament_procedures.sql > ../esportsmanager.sql
```

## Tài khoản có sẵn

### Tài khoản Admin

- **Username**: admin
- **Password**: admin123
- **Email**: admin@esportmanager.com

### Tài khoản Super Admin

- **Username**: superadmin
- **Password**: admin123
- **Email**: superadmin@esportmanager.com

### Tài khoản Player

- **Username**: player1, player2, player3, player4, player5
- **Password**: player123

### Tài khoản Viewer

- **Username**: viewer1, viewer2, viewer3
- **Password**: viewer123

## Kết nối trong C#

Để kết nối với database MySQL trong code C#, đảm bảo rằng:

1. Đã cài đặt package MySql.Data trong project EsportsManager.DAL
2. Cung cấp connection string đúng trong appsettings.json hoặc trong biến môi trường
3. Sử dụng DataContext để thực thi các stored procedures và truy vấn

Ví dụ sử dụng stored procedure trong code:

```csharp
// Lấy thống kê hệ thống
var systemStats = _dataContext.ExecuteStoredProcedure("sp_GetSystemStats");

// Lấy top players dựa trên donations
var topPlayers = _dataContext.ExecuteStoredProcedure("sp_GetTopPlayersByDonations",
    _dataContext.CreateParameter("p_Limit", 10));
```

## Lưu ý về từ khóa dành riêng trong MySQL

Khi làm việc với MySQL, hãy cẩn thận với các từ khóa dành riêng như:

- `RANK`
- `ORDER`
- `GROUP`
- `KEY`
- `LIMIT`
- `VALUES`

Nếu bạn muốn sử dụng các từ khóa này làm tên cột hoặc bảng, hãy đặt chúng trong dấu backticks:

```sql
-- Cách sử dụng từ khóa dành riêng an toàn
SELECT ROW_NUMBER() OVER (ORDER BY score DESC) as `Rank`,
       username, score
FROM leaderboard
ORDER BY `Rank` ASC;
```

Trong file 05_create_procedures.sql, chúng tôi đã thay đổi tên cột "Rank" thành "RankPosition" trong stored procedure sp_GetTournamentLeaderboard để tránh lỗi cú pháp.

## Lưu ý về Triggers và dữ liệu mẫu

Trong hệ thống có các triggers tự động tạo một số dữ liệu:

1. **tr_create_player_wallet**: Tự động tạo ví (Wallet) khi thêm người dùng với role "Player"
2. **tr_create_user_profile**: Tự động tạo UserProfile khi thêm bất kỳ người dùng nào

Với những dữ liệu được tạo tự động bởi trigger, chúng ta không nên INSERT trực tiếp vào những bảng đó (sẽ gây lỗi duplicate key). Thay vào đó, chúng ta nên sử dụng UPDATE để cập nhật dữ liệu đã được tạo ra.

Ví dụ, khi cần thiết lập số dư ban đầu cho ví của Player, thay vì:

```sql
-- KHÔNG NÊN LÀM ĐIỀU NÀY (sẽ gây lỗi do trigger đã tạo ví)
INSERT INTO Wallets (UserID, Balance, TotalReceived) VALUES (3, 1000.00, 1500.00);
```

Nên sử dụng:

```sql
-- Cách đúng - cập nhật ví đã được tạo bởi trigger
UPDATE Wallets SET
    Balance = 1000.00,
    TotalReceived = 1500.00
WHERE UserID = 3;
```
