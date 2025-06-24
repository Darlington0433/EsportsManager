# Hướng dẫn cài đặt Database EsportsManager

## Cấu trúc file
Dự án EsportsManager sử dụng hai cấu trúc SQL chính:
- `database/esportsmanager.sql`: File SQL tổng hợp, bao gồm tất cả các bảng, ràng buộc, procedures và dữ liệu mẫu (khuyến nghị cho người dùng mới)
- `database/split_sql/`: Thư mục chứa các file SQL đã được tách nhỏ theo chức năng (dành cho nhà phát triển muốn tùy chỉnh từng phần)

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
Database chưa được tạo. Hãy chạy file `split_sql/01_create_database_and_tables.sql` trước.

### Lỗi "Table already exists"
Nếu bạn đã chạy một số file SQL và gặp lỗi bảng đã tồn tại, hãy sử dụng cú pháp `DROP TABLE IF EXISTS` trước khi tạo bảng mới. Cú pháp này đã được sử dụng trong các file SQL của chúng tôi.

## Tài khoản mặc định sau khi cài đặt

**Lưu ý**: Nếu bạn gặp vấn đề đăng nhập, hãy chạy file `standardize_passwords.sql` để đảm bảo mật khẩu được hash đúng cách với BCrypt.

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

### Chạy SQL từ command line
- Trên Windows: `mysql -u root -p < database\esportsmanager.sql`
- Trên Linux/Mac: `mysql -u root -p < database/esportsmanager.sql`
- Hoặc từng file riêng lẻ: `mysql -u root -p < database/split_sql/01_create_database_and_tables.sql` và tiếp tục theo thứ tự

### Lỗi khi import SQL
- Kiểm tra xem file SQL có lỗi cú pháp không
- Đảm bảo đã import các file theo đúng thứ tự
- Kiểm tra phiên bản MySQL (nên dùng MySQL 5.7 trở lên hoặc MariaDB 10.2 trở lên)

### Lỗi kết nối từ ứng dụng
- Kiểm tra connection string trong `appsettings.json`
- Đảm bảo MySQL Server chấp nhận kết nối từ localhost
- Kiểm tra username và password kết nối đến MySQL

### Lỗi "Unknown column 'Status' in 'field list'" hoặc "Unknown column 'IsEmailVerified' in 'field list'"
- Nếu gặp lỗi này khi đăng nhập, thiếu các cột trong bảng Users
- Tạo lại toàn bộ database từ đầu bằng file `database/esportsmanager.sql` mới nhất
- Hoặc có thể cập nhật bảng Users thủ công thông qua MySQL với ALTER TABLE 

### Lỗi "Table 'esportsmanager.wallettransactions' doesn't exist"
- Thiếu bảng WalletTransactions trong database
- Tạo lại toàn bộ database từ đầu bằng file `database/esportsmanager.sql` mới nhất
- Hoặc có thể chạy file `database/split_sql/01_create_database_and_tables.sql` để thêm các bảng còn thiếu

### Lỗi "TÀI KHOẢN MẶC ĐỊNH KHÔNG TỒN TẠI"
- Dữ liệu mẫu chưa được import vào database
- Chạy trực tiếp file `database/split_sql/07_sample_data.sql` để thêm tài khoản mặc định
- Hoặc tạo lại toàn bộ database từ đầu bằng file `database/esportsmanager.sql` mới nhất để có sẵn dữ liệu mẫu

## Lưu ý về tính đồng bộ

Dự án EsportsManager SQL đã được đồng bộ hoàn toàn giữa code và database:
- Tất cả các cấu trúc bảng đã khớp với mô hình trong code
- Các cột Status, IsEmailVerified, và các trường quan trọng khác đã được thêm vào
- Bảng WalletTransactions và các bảng khác đã được đồng bộ đầy đủ
- Dữ liệu mẫu với các tài khoản mặc định đã được tích hợp sẵn

Nếu gặp lỗi không đồng bộ giữa code và database, hãy luôn tạo database mới từ file esportsmanager.sql hoặc chạy tuần tự các file trong thư mục split_sql/ theo đúng thứ tự.
