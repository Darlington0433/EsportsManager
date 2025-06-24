# THƯA MỤC TESTS - CÁC FILE KIỂM THỬ

Thư mục này chứa các file và project kiểm thử cho hệ thống EsportsManager.

## Các file có sẵn:

### `DebugLogin.cs`
- **Mục đích**: Kiểm tra toàn bộ quy trình đăng nhập từ BCrypt đến DAL và BL layers
- **Chức năng**:
  - Kiểm tra BCrypt hash trực tiếp
  - Test kết nối database (DataContext)
  - Test User Repository (DAL layer)
  - Test User Service (BL layer)

### `BCryptTest.cs`
- **Mục đích**: Kiểm tra xác thực BCrypt đơn giản
- **Chức năng**: Xác minh mật khẩu admin với hash từ database

### `BCryptTestProject/`
- **Mục đích**: Project tạo hash BCrypt mới
- **Chức năng**: 
  - Tạo hash BCrypt cho các tài khoản mới
  - Kiểm tra và xác minh hash
  - Tạo SQL updates cho database

## Cách sử dụng:

1. **Chạy DebugLogin.cs**: 
   ```bash
   cd tests
   dotnet run DebugLogin.cs
   ```

2. **Chạy BCryptTest.cs**:
   ```bash
   cd tests  
   dotnet run BCryptTest.cs
   ```

3. **Chạy BCryptTestProject**:
   ```bash
   cd tests/BCryptTestProject
   dotnet run
   ```

## Lưu ý:

- Tất cả các file test đã được di chuyển từ thư mục `database/` về đây
- Các file trống và không cần thiết đã được xóa
- Thư mục `database/` giờ chỉ chứa file SQL và documentation
- Code đã được cập nhật để sử dụng tiếng Việt và có documentation rõ ràng
