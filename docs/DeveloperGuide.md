# Hướng dẫn kỹ thuật cho nhà phát triển EsportsManager

**Repository**: [https://github.com/Darlington0433/EsportsManager](https://github.com/Darlington0433/EsportsManager)  
**Authors**: Phan Nhật Quân và mọi người - VTC Academy Team  
**Contact**: quannnd2004@gmail.com

## Tổng quan kiến trúc

EsportsManager được xây dựng dựa trên kiến trúc 3 lớp rõ ràng:

```
[ Presentation Layer (UI) ] → [ Business Logic Layer (BL) ] → [ Data Access Layer (DAL) ]
```

## Cấu trúc thư mục và mục đích

```
EsportsManager/
├── BL/                    # Business Logic - Xử lý nghiệp vụ
│   ├── Models/            # Định nghĩa mô hình dữ liệu
│   ├── Services/          # Xử lý nghiệp vụ và quy tắc
│   └── Utilities/         # Các công cụ hỗ trợ nghiệp vụ
├── DAL/                   # Data Access Layer - Truy cập dữ liệu
│   ├── Repositories/      # Các Repository pattern
│   └── DataContext.cs     # Khởi tạo và kết nối dữ liệu
├── UI/                    # User Interface - Giao diện người dùng
│   ├── Forms/             # Xử lý các biểu mẫu nhập liệu
│   ├── Menus/             # Định nghĩa các menu
│   ├── Utilities/         # Công cụ UI (vẽ, nhập liệu...)
│   └── UIHelper.cs        # Facade cho UI
└── Program.cs             # Điểm khởi đầu ứng dụng
```

## Các lớp chính và chức năng

### UI Layer

#### 1. Lớp UIHelper (Facade)

`UIHelper.cs` là một facade pattern giúp Program.cs chỉ cần quan tâm tới interface đơn giản. Nó chuyển hướng các lời gọi đến:

- MenuManager: Xử lý hiển thị menu
- Forms: Các biểu mẫu cụ thể
- ConsoleDrawing: Vẽ giao diện

#### 2. Forms

- **LoginForm**: Xử lý quá trình đăng nhập

  - Phương thức chính: `Show()` → trả về tuple `(string Username, string Password)?`
  - Xác thực đầu vào: Chỉ chấp nhận ký tự alphanumeric cho tên đăng nhập

- **RegisterForm**: Xử lý quá trình đăng ký

  - Phương thức chính: `Show()` → trả về tuple `(string Username, string Email, string Password, string SecurityAnswer)?`
  - Xác thực email thông qua regex
  - Xác nhận mật khẩu khớp nhau

- **ForgotPasswordForm**: Xử lý quá trình khôi phục mật khẩu
  - Phương thức chính: `Show()` → trả về tuple `(string Username, string Email, string SecurityAnswer)?`

#### 3. Menus

- **BaseMenu**: Lớp trừu tượng định nghĩa cấu trúc chung cho tất cả menu

  - Phương thức: `Show()` - hiển thị menu
  - Phương thức trừu tượng: `HandleMenuSelection(int selected)` - xử lý khi người dùng chọn một mục

- **AdminMenu**: Hiển thị và xử lý tương tác với menu admin

  - Phương thức: `HandleUserManagement()`, `HandleTournamentManagement()`, v.v.

- **PlayerMenu**: Hiển thị và xử lý tương tác với menu người chơi

  - Phương thức: `ViewTournaments()`, `RegisterForTournament()`, v.v.

- **ViewerMenu**: Hiển thị và xử lý tương tác với menu người xem
  - Phương thức: `ViewTournaments()`, `MakeDonation()`, `SendFeedback()`

#### 4. Utilities

- **ConsoleDrawing**: Các phương thức vẽ giao diện

  - `DrawBox(width, height, left, top)`: Vẽ khung
  - `DrawTitleArt(artLines, contentWidth)`: Vẽ ASCII Art
  - `CenterText(text, width)`: Căn giữa văn bản

- **ConsoleInput**: Các phương thức xử lý nhập liệu

  - `ReadAlphaNumeric(maxLen)`: Đọc chuỗi alphanumeric
  - `ReadPassword(maxLen)`: Đọc mật khẩu (hiển thị dấu \*)
  - `ReadAnyString(maxLen)`: Đọc chuỗi bất kỳ
  - `IsValidEmail(email)`: Kiểm tra email hợp lệ

- **MenuManager**: Xử lý hiển thị và điều hướng menu
  - `ShowMenu(menuTitle, options, selectedIndex)`: Hiển thị menu và trả về lựa chọn
  - `ShowWelcomeScreen()`: Hiển thị màn hình chào mừng ban đầu

### Business Logic Layer

#### 1. Models

- **User**: Đại diện cho người dùng trong hệ thống
  - Thuộc tính: Id, Username, Role

#### 2. Services

- **UserService**: Xử lý nghiệp vụ liên quan đến người dùng
  - Phương thức: `ShowInfo()` (hiện đang là stub)

#### 3. Utilities

- **PasswordHasher**: Mã hóa và xác thực mật khẩu

### Data Access Layer

#### 1. Repositories

- **UserRepository**: Truy xuất và lưu trữ dữ liệu người dùng

#### 2. DataContext

- Quản lý kết nối với nguồn dữ liệu

## Luồng dữ liệu trong ứng dụng

1. **Người dùng tương tác với UI**: Nhập thông tin qua Forms hoặc chọn lựa chọn từ Menu
2. **UI gọi đến Business Logic**: Các form/menu gọi đến các services tương ứng
3. **Business Logic xử lý nghiệp vụ**: Áp dụng các quy tắc nghiệp vụ và xác thực
4. **BL gọi xuống Data Access Layer**: Để lưu trữ hoặc truy vấn dữ liệu
5. **DAL tương tác với dữ liệu**: Chuyển đổi giữa mô hình đối tượng và dữ liệu lưu trữ
6. **Kết quả trả về theo luồng ngược lại**: DAL → BL → UI → Người dùng

## Xử lý đầu vào

- **Validation**: Kiểm tra định dạng và giá trị nhập vào

  - Tên đăng nhập: Chỉ chấp nhận ký tự alphanumeric
  - Email: Phải đúng định dạng email (pattern regex)
  - Mật khẩu: Chấp nhận tất cả ký tự

- **Phím tắt**:
  - Enter: Xác nhận giá trị nhập
  - Escape: Hủy thao tác/quay lại
  - Mũi tên lên/xuống: Di chuyển giữa các lựa chọn

## Cách mở rộng dự án

### Thêm chức năng mới

1. Thêm Model mới trong `BL/Models`
2. Thêm Repository tương ứng trong `DAL/Repositories`
3. Thêm Service xử lý nghiệp vụ trong `BL/Services`
4. Cập nhật UI để hiển thị và tương tác với chức năng mới

### Thêm form mới

1. Tạo lớp form mới trong thư mục `UI/Forms`
2. Sử dụng các tiện ích từ `ConsoleDrawing` và `ConsoleInput`
3. Cập nhật `UIHelper` để cung cấp truy cập đến form mới

### Thêm menu mới

1. Tạo lớp kế thừa từ `BaseMenu` trong thư mục `UI/Menus`
2. Ghi đè phương thức `HandleMenuSelection()`
3. Thêm các phương thức xử lý cho từng mục menu

## Quy tắc lập trình và chuẩn code

### Quy tắc đặt tên

- **Classes và Types**: PascalCase (VD: `UserService`, `LoginForm`)
- **Methods**: PascalCase (VD: `GetUser()`, `ShowMenu()`)
- **Variables**: camelCase (VD: `userName`, `passwordHash`)
- **Constants**: UPPER_CASE (VD: `MAX_LOGIN_ATTEMPTS`)
- **Private fields**: \_camelCase (VD: `_repository`, `_userManager`)

### Chuẩn mã nguồn

- Sử dụng namespace đúng: `EsportsManager.[Layer].[SubLayer]`
- Thêm XML documentation cho tất cả public members
- Sử dụng properties thay vì public fields
- Sử dụng các accessibility modifiers phù hợp (public, private, protected, internal)
- Xử lý ngoại lệ đúng cách với try-catch
- Tách các phương thức lớn thành các phương thức nhỏ hơn, mỗi phương thức chỉ nên thực hiện một chức năng

### Hiệu suất và bảo mật

- Sử dụng hashing cho mật khẩu (đã có trong `PasswordHasher.cs`)
- Kiểm tra đầu vào từ người dùng
- Tránh SQL injection trong các câu truy vấn
- Tránh các vòng lặp lồng nhau sâu
- Tránh sử dụng biến toàn cục không cần thiết

### Đề xuất cải tiến

1. **Dependency Injection**: Sử dụng DI để giảm sự phụ thuộc chặt chẽ giữa các thành phần
2. **Logging**: Thêm logging để ghi lại lỗi và hoạt động của hệ thống
3. **Cấu hình**: Tách các giá trị cố định vào file cấu hình
4. **Unit Testing**: Thêm unit test để đảm bảo code hoạt động đúng
5. **Validation**: Thêm validation cho dữ liệu đầu vào
6. **Internationalization**: Hỗ trợ đa ngôn ngữ

### Giải quyết xung đột namespace

Xem file `FixNamespaceConflicts.md` để biết chi tiết về cách giải quyết xung đột namespace trong dự án.
