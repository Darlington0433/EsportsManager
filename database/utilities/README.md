# 🛠️ Database Utilities - Công cụ hỗ trợ Database

Thư mục này chứa các file SQL hỗ trợ để khắc phục sự cố và kiểm tra database.

## 📋 Danh sách file

### `check_passwords.sql`
**Mục đích**: Kiểm tra thông tin hash mật khẩu của các tài khoản
**Cách dùng**: Chạy khi muốn xem hash mật khẩu hiện tại trong database
```sql
SOURCE check_passwords.sql;
```

### `fix_passwords.sql`  
**Mục đích**: Sửa lỗi không đăng nhập được - cập nhật hash BCrypt chính xác
**Cách dùng**: Chạy khi không thể đăng nhập với tài khoản mặc định
```sql
SOURCE fix_passwords.sql;
```

## 🚨 Khi nào cần sử dụng

### Lỗi đăng nhập
Nếu không thể đăng nhập với:
- admin/admin123
- player1/player123  
- viewer1/viewer123

→ Chạy `fix_passwords.sql`

### Kiểm tra hash
Muốn xem hash hiện tại trong database để debug
→ Chạy `check_passwords.sql`

## ⚡ Lưu ý
- Tất cả file đã được test và hoạt động ổn định
- Hash BCrypt được tạo bằng BCrypt.Net (tương thích với ứng dụng)
- Chỉ sử dụng khi cần thiết, không chạy liên tục
