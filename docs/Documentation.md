# Tài liệu hướng dẫn EsportsManager

**Repository**: [https://github.com/Darlington0433/EsportsManager](https://github.com/Darlington0433/EsportsManager)  
**Authors**: Phan Nhật Quân và mọi người - VTC Academy Team  
**Contact**: quannnd2004@gmail.com

## Giới thiệu

EsportsManager là một ứng dụng quản lý Esports được phát triển trên nền tảng .NET, cho phép quản lý giải đấu, người chơi, đội tuyển và người xem. Ứng dụng được xây dựng với giao diện console đơn giản nhưng trực quan, dễ sử dụng.

## Cấu trúc dự án

Dự án được tổ chức theo mô hình kiến trúc 3 lớp:

```
EsportsManager/
├── BL/                    # Business Logic - Xử lý nghiệp vụ
│   ├── Models/            # Các model đại diện cho dữ liệu
│   ├── Services/          # Các dịch vụ xử lý nghiệp vụ
│   └── Utilities/         # Các tiện ích cho BL
├── DAL/                   # Data Access Layer - Tầng truy cập dữ liệu
│   ├── Repositories/      # Các repository giao tiếp với dữ liệu
│   └── DataContext.cs     # Ngữ cảnh dữ liệu
├── UI/                    # User Interface - Giao diện người dùng
│   ├── Forms/             # Các form giao diện
│   ├── Menus/             # Các menu của ứng dụng
│   ├── Utilities/         # Các tiện ích liên quan đến UI
│   └── UIHelper.cs        # Lớp hỗ trợ UI
└── Program.cs             # Điểm khởi đầu của ứng dụng
```

## Chi tiết các thành phần

### Business Logic (BL)

#### Models

- **User.cs**: Đối tượng người dùng trong hệ thống với các thuộc tính:
  - Id: Định danh người dùng
  - Username: Tên đăng nhập
  - Role: Vai trò (Admin, Player, Viewer)

#### Services

- **UserService.cs**: Xử lý các nghiệp vụ liên quan đến người dùng:
  - Đăng ký
  - Đăng nhập
  - Quản lý thông tin

#### Utilities

- **PasswordHasher.cs**: Xử lý mã hóa mật khẩu và xác thực

### Data Access Layer (DAL)

#### Repositories

- **UserRepository.cs**: Truy xuất và lưu trữ dữ liệu người dùng

#### DataContext

- **DataContext.cs**: Quản lý kết nối dữ liệu và cung cấp ngữ cảnh truy xuất

### User Interface (UI)

#### Forms

- **LoginForm.cs**: Form đăng nhập với các tính năng:

  - Nhập tên đăng nhập (chỉ chấp nhận ký tự alphanumeric)
  - Nhập mật khẩu (ẩn ký tự, chấp nhận ký tự đặc biệt)
  - Hỗ trợ phím Escape để hủy
  - Hiển thị thông báo thành công/lỗi

- **RegisterForm.cs**: Form đăng ký với các tính năng:

  - Nhập thông tin cá nhân
  - Xác thực email
  - Đặt câu hỏi bảo mật
  - Xác nhận mật khẩu

- **ForgotPasswordForm.cs**: Form quên mật khẩu với các tính năng:
  - Nhập tên đăng nhập
  - Nhập email
  - Nhập câu trả lời bảo mật
  - Gửi yêu cầu đặt lại mật khẩu

#### Menus

- **BaseMenu.cs**: Lớp cơ sở cho tất cả các menu, định nghĩa:

  - Thuộc tính chung (tiêu đề, danh sách lựa chọn)
  - Phương thức hiển thị menu
  - Phương thức ảo xử lý lựa chọn menu

- **AdminMenu.cs**: Menu dành cho quản trị viên:

  - Quản lý người dùng
  - Quản lý giải đấu
  - Quản lý đội
  - Thống kê

- **PlayerMenu.cs**: Menu dành cho người chơi:

  - Xem giải đấu
  - Đăng ký tham gia
  - Quản lý đội
  - Xem thành tích

- **ViewerMenu.cs**: Menu dành cho người xem:
  - Xem giải đấu
  - Donate cho đội/giải đấu
  - Gửi phản hồi

#### Utilities

- **ConsoleDrawing.cs**: Cung cấp các phương thức vẽ giao diện:

  - Vẽ hộp với viền
  - Vẽ ASCII art
  - Căn giữa văn bản
  - Cập nhật dòng menu

- **ConsoleInput.cs**: Xử lý nhập liệu từ người dùng:

  - Đọc chuỗi alphanumeric (cho tên đăng nhập)
  - Đọc mật khẩu (ẩn ký tự)
  - Đọc chuỗi bất kỳ
  - Xác thực email

- **MenuManager.cs**: Quản lý hiển thị và tương tác với menu:
  - Hiển thị menu
  - Xử lý phím tắt
  - Hiển thị màn hình chào mừng

#### UIHelper

- **UIHelper.cs**: Lớp cầu nối giữa các thành phần UI:
  - Chuyển hướng đến các form
  - Chuyển hướng đến MenuManager
  - Chuyển hướng đến các phương thức vẽ

## Hướng dẫn sử dụng

### Khởi động ứng dụng

- Chạy tệp `ESportsMgr.exe` hoặc thông qua lệnh `dotnet run` trong thư mục dự án.
- Màn hình chính sẽ hiển thị với menu chính.

### Đăng nhập

1. Chọn "Đăng nhập" trong menu chính
2. Nhập tên đăng nhập và mật khẩu
3. Sử dụng phím mũi tên để di chuyển giữa các trường
4. Nhấn Enter để xác nhận, Esc để hủy

### Đăng ký

1. Chọn "Đăng ký" trong menu chính
2. Điền thông tin đăng ký:
   - Tên đăng nhập (chỉ chấp nhận chữ cái và số)
   - Email (phải có định dạng email hợp lệ)
   - Mật khẩu
   - Xác nhận mật khẩu (phải khớp với mật khẩu)
   - Câu trả lời bảo mật
3. Sử dụng phím mũi tên để di chuyển giữa các trường
4. Nhấn Enter để xác nhận, Esc để hủy

### Quên mật khẩu

1. Chọn "Quên mật khẩu" trong menu chính
2. Điền thông tin:
   - Tên đăng nhập
   - Email
   - Câu trả lời bảo mật
3. Nhấn Enter để gửi yêu cầu đặt lại mật khẩu

### Sử dụng menu

- Sử dụng phím mũi tên lên/xuống để chọn các mục
- Nhấn Enter để chọn mục đã highlight
- Nhấn Esc để quay lại menu trước hoặc thoát
- Tùy theo vai trò (Admin, Player, Viewer), menu hiển thị sẽ khác nhau

## Mẹo sử dụng giao diện

- Giao diện được căn giữa tự động theo kích thước cửa sổ console
- Phím tắt:
  - ↑/↓: Di chuyển giữa các lựa chọn
  - Enter: Xác nhận lựa chọn
  - Esc: Hủy/quay lại
- Các thông báo lỗi được hiển thị ngay trong form
- Thông báo thành công hiển thị bên dưới khung form

## Các loại người dùng và chức năng

### Quản trị viên (Admin)

- Quản lý người dùng (thêm, sửa, xóa)
- Quản lý giải đấu (tạo, chỉnh sửa, hủy)
- Quản lý đội tham gia
- Xem thống kê tổng quan

### Người chơi (Player)

- Xem giải đấu hiện có
- Đăng ký tham gia giải đấu
- Quản lý đội của mình
- Theo dõi thành tích

### Người xem (Viewer)

- Xem thông tin giải đấu
- Ủng hộ/donate cho đội hoặc giải đấu
- Gửi phản hồi về giải đấu

## Thông tin kỹ thuật

- Ngôn ngữ: C# 9.0
- Framework: .NET 9.0
- Kiến trúc: 3-layer architecture
- Giao diện: Console Application với UI nâng cao
