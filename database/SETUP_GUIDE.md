# Hướng dẫn cài đặt Database EsportsManager

## Yêu cầu
- MySQL Server hoặc MariaDB đã được cài đặt
- Quyền admin trên MySQL Server để tạo database và thực thi các câu lệnh SQL

## Cách 1: Import toàn bộ database

1. Mở MySQL Workbench hoặc phpmyadmin
2. Tạo một database mới tên là `EsportsManager` (nếu chưa có)
3. Chọn database EsportsManager
4. Import file `database/esportsmanager.sql` để tạo toàn bộ database với cấu trúc và dữ liệu mẫu

## Cách 2: Cài đặt từng bước (khuyến nghị cho nhà phát triển)

### Chạy các file SQL theo thứ tự

1. Mở MySQL Workbench hoặc MySQL Command Line
2. Trong thư mục `database/split_sql/`, **bắt buộc** phải chạy các file SQL theo đúng thứ tự:
   - `01_create_database_and_tables.sql`: Tạo database và tất cả các bảng cơ bản (Users, Wallets, WalletTransactions, Teams, Tournaments,...)
   - `02_create_indexes.sql`: Tạo các indexes để tối ưu hiệu năng
   - `03_create_views.sql`: Tạo các views (phụ thuộc vào các bảng đã tạo ở file 01)
   - `04_create_triggers.sql`: Tạo các triggers tự động hóa
   - `05_create_procedures.sql`: Tạo stored procedures cơ bản
   - `06_add_constraints.sql`: Thêm các ràng buộc dữ liệu
   - `07_sample_data.sql`: Thêm dữ liệu mẫu và tài khoản admin
   - `08_tournament_procedures.sql`: Tạo stored procedures giải đấu

> **LƯU Ý QUAN TRỌNG**: Thứ tự import các file SQL là rất quan trọng vì các file sau phụ thuộc vào các file trước đó. Nếu bạn chạy không đúng thứ tự, có thể gặp lỗi như "Table doesn't exist" hoặc "Column doesn't exist".

## Sửa lỗi thường gặp

### Lỗi "Access denied for user..."
Cần đảm bảo rằng thông tin đăng nhập MySQL trong file `appsettings.json` của ứng dụng là chính xác:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=EsportsManager;Uid=root;Pwd=yourpassword;CharSet=utf8mb4;"
}
```

### Lỗi "Unknown database 'EsportsManager'"
Database chưa được tạo. Hãy chạy file `01_create_database.sql` trước.

### Lỗi "Table already exists"
Nếu bạn đã chạy một số file SQL và gặp lỗi bảng đã tồn tại, hãy sử dụng cú pháp `DROP TABLE IF EXISTS` trước khi tạo bảng mới. Cú pháp này đã được sử dụng trong các file SQL của chúng tôi.

## Tài khoản mặc định sau khi cài đặt

### Admin
- Username: admin
- Password: admin123

### Player
- Username: player1
- Password: player123

### Viewer
- Username: viewer1
- Password: viewer123

## Kiểm tra cài đặt thành công

Sau khi cài đặt, bạn nên kiểm tra xem database đã hoạt động đúng chưa:

1. Đăng nhập với tài khoản admin
2. Vào menu "Quản lý người dùng" > "Xem danh sách người dùng"
3. Hệ thống nên hiển thị danh sách người dùng đã import

Nếu gặp lỗi "Không thể kết nối đến cơ sở dữ liệu", hãy kiểm tra lại các bước cài đặt trên.

## Khắc phục sự cố chung

### MySQL Server không chạy
- Kiểm tra xem MySQL service đã được khởi động chưa
- Trên Windows: Mở Services.msc và khởi động MySQL service
- Trên Linux: Sử dụng lệnh `sudo service mysql start` hoặc `sudo systemctl start mysql`

### Lỗi khi import SQL
- Kiểm tra xem file SQL có lỗi cú pháp không
- Đảm bảo đã import các file theo đúng thứ tự
- Kiểm tra phiên bản MySQL (nên dùng MySQL 5.7 trở lên hoặc MariaDB 10.2 trở lên)

### Lỗi kết nối từ ứng dụng
- Kiểm tra connection string trong `appsettings.json`
- Đảm bảo MySQL Server chấp nhận kết nối từ localhost
- Kiểm tra username và password kết nối đến MySQL
