# Hướng dẫn khắc phục sự cố - EsportsManager

## Các lỗi thường gặp và cách khắc phục

### 1. Không thể kết nối đến MySQL Server

**Hiện tượng:**
- Thông báo lỗi "Không thể kết nối đến cơ sở dữ liệu MySQL"
- Không thể đăng nhập vào hệ thống

**Nguyên nhân:**
- MySQL Server chưa được khởi động
- Thông tin kết nối trong appsettings.json không chính xác
- Database chưa được tạo

**Cách khắc phục:**
1. Kiểm tra xem MySQL Server đã chạy chưa
   - Windows: Mở Services.msc và kiểm tra MySQL/MariaDB service
   - Linux: Sử dụng lệnh `sudo systemctl status mysql`
2. Kiểm tra file `appsettings.json` trong thư mục `src/EsportsManager.UI/`
   - Đảm bảo thông tin server, username và password chính xác
3. Import database từ file SQL trong thư mục `database/`
   - Xem hướng dẫn cài đặt database trong `database/SETUP_GUIDE.md`

### 2. Lỗi "Access denied for user..."

**Hiện tượng:**
- Ứng dụng hiển thị lỗi "Access denied for user 'root'@'localhost'"

**Nguyên nhân:**
- Sai mật khẩu MySQL trong file cấu hình
- Người dùng MySQL không có quyền truy cập database

**Cách khắc phục:**
1. Mở file `appsettings.json` và cập nhật mật khẩu chính xác:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=EsportsManager;Uid=root;Pwd=yourpassword;CharSet=utf8mb4;"
   }
   ```
2. Hoặc tạo người dùng MySQL mới có quyền truy cập database:
   ```sql
   CREATE USER 'esports_user'@'localhost' IDENTIFIED BY 'password';
   GRANT ALL PRIVILEGES ON EsportsManager.* TO 'esports_user'@'localhost';
   FLUSH PRIVILEGES;
   ```

### 3. Lỗi "Unknown database 'EsportsManager'"

**Hiện tượng:**
- Ứng dụng hiển thị lỗi "Unknown database 'EsportsManager'"

**Nguyên nhân:**
- Database chưa được tạo

**Cách khắc phục:**
1. Tạo database trước:
   ```sql
   CREATE DATABASE EsportsManager CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   ```
2. Import cấu trúc và dữ liệu từ file SQL trong thư mục database/

### 4. Lỗi "Tài khoản admin không tồn tại"

**Hiện tượng:**
- Ứng dụng hiển thị thông báo về việc không tìm thấy tài khoản mặc định

**Nguyên nhân:**
- File dữ liệu mẫu chưa được import

**Cách khắc phục:**
1. Chạy file `database/split_sql/07_sample_data.sql`
2. Hoặc tạo người dùng admin thủ công:
   ```sql
   INSERT INTO Users (Username, PasswordHash, Email, Role, IsActive) 
   VALUES ('admin', '$2a$11$jytLhMkiejhdiugfkrjgnyoerpw3riue0r8urhw9478/djfeiorj', 'admin@esportmanager.com', 'Admin', TRUE);
   ```

### 5. Lỗi "Lỗi đăng nhập: Column 'xxx' does not exist"

**Hiện tượng:**
- Khi đăng nhập gặp lỗi về cột không tồn tại

**Nguyên nhân:**
- Cấu trúc database không khớp với code
- Database chưa được cập nhật đến phiên bản mới nhất

**Cách khắc phục:**
1. Import lại toàn bộ database từ file `database/esportsmanager.sql`
2. Hoặc chạy lại tất cả các file SQL trong thư mục `database/split_sql/` theo đúng thứ tự

### 6. Lỗi "Error Code: 1146. Table 'esportsmanager.wallettransactions' doesn't exist"

**Hiện tượng:**
- Gặp lỗi khi chạy SQL hoặc khi ứng dụng cố gắng truy cập vào bảng WalletTransactions
- Thường xuất hiện khi tạo view v_user_wallet_summary

**Nguyên nhân:**
- Bảng WalletTransactions chưa được tạo
- Các file SQL không được chạy theo đúng thứ tự
- File 01_create_database_and_tables.sql chưa được chạy hoặc chạy không thành công

**Cách khắc phục:**
1. Import lại toàn bộ database theo đúng thứ tự (đây là cách đảm bảo nhất):
   ```sql
   -- Import theo thứ tự
   source [đường_dẫn]/database/split_sql/01_create_database_and_tables.sql
   source [đường_dẫn]/database/split_sql/02_create_indexes.sql
   source [đường_dẫn]/database/split_sql/03_create_views.sql
   source [đường_dẫn]/database/split_sql/04_create_triggers.sql
   source [đường_dẫn]/database/split_sql/05_create_procedures.sql
   source [đường_dẫn]/database/split_sql/06_add_constraints.sql
   source [đường_dẫn]/database/split_sql/07_sample_data.sql
   source [đường_dẫn]/database/split_sql/08_tournament_procedures.sql
   ```
   
2. Hoặc tạo thủ công bảng WalletTransactions nếu thiếu:
   ```sql
   USE EsportsManager;
   
   CREATE TABLE IF NOT EXISTS WalletTransactions (
     TransactionID INT AUTO_INCREMENT PRIMARY KEY,
     WalletID INT NOT NULL,
     TransactionType ENUM('Deposit', 'Withdrawal', 'Donation_Received', 'Prize_Money', 'Refund') NOT NULL,
     Amount DECIMAL(12,2) NOT NULL,
     Description TEXT,
     ReferenceID INT,
     CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
     
     FOREIGN KEY (WalletID) REFERENCES Wallets(WalletID) ON DELETE CASCADE
   ) ENGINE=InnoDB COMMENT='Detailed wallet transaction history';
   ```
   
3. Sau đó import lại các view từ file `03_create_views.sql`:
   ```sql
   USE EsportsManager;
   source [đường_dẫn]/database/split_sql/03_create_views.sql
   ```

### 7. Lỗi khác

Nếu bạn gặp các lỗi khác không được liệt kê ở đây, vui lòng kiểm tra:

1. File log trong thư mục `logs/`
2. Xem tài liệu phát triển trong thư mục `docs/`
3. Kiểm tra console output để biết chi tiết lỗi

## Hướng dẫn sử dụng các tài khoản có sẵn

### Admin
- Username: admin
- Password: admin123

### Player
- Username: player1
- Password: player123

### Viewer
- Username: viewer1
- Password: viewer123

## Liên hệ hỗ trợ

Nếu bạn gặp sự cố không thể tự khắc phục, vui lòng liên hệ:

- Email: support@esportsmanager.com
- Discord: https://discord.gg/esportsmanager
