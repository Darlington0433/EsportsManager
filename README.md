# 🎮 Esports Manager - Hệ Thống Quản Lý Giải Đấu Chuyên Nghiệp

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![Trạng Thái Build](https://img.shields.io/badge/build-passing-brightgreen.svg)](#)
[![Giấy Phép](https://img.shields.io/badge/license-Custom-red.svg)](https://github.com/Darlington0433/EsportsManager/blob/main/LICENSE)
[![C#](https://img.shields.io/badge/C%23-11.0-purple.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![GitHub Stars](https://img.shields.io/github/stars/Darlington0433/EsportsManager?style=social)](https://github.com/Darlington0433/EsportsManager)

> **Hệ thống quản lý giải đấu Esports chuyên nghiệp** được phát triển theo kiến trúc 3 tầng chuẩn và tuân thủ nguyên lý SOLID.

## ✨ Tính năng nổi bật

- 🎯 **Giao diện Console đẹp mắt** - ASCII Art, Menu tương tác, Thiết kế responsive
- 🏗️ **Kiến trúc chuẩn** - Kiến trúc 3 tầng, Nguyên lý SOLID, Clean Code
- 🔐 **Xác thực & Phân quyền** - Truy cập theo vai trò (Admin/Player/Viewer)
- 📊 **Quản lý toàn diện** - Giải đấu, Đội tuyển, Người chơi, Thống kê
- 💰 **Hệ thống ví điện tử** - Thanh toán, Ủng hộ, Lịch sử giao dịch
- 📈 **Thống kê & Báo cáo** - Phân tích hiệu suất, Theo dõi thành tích

## 🖼️ Hình ảnh giao diện

<details>
<summary>Nhấn để xem giao diện</summary>

### Menu Chính

```
╔══════════════════════════════════════════════════════════════╗
║                    🎮 ESPORTS MANAGER 🎮                    ║
║                      Phiên Bản Chuyên Nghiệp                 ║
╠══════════════════════════════════════════════════════════════╣
║                                                              ║
║  ┌─ CHỨC NĂNG CHÍNH ─────────────────────────────────────┐   ║
║  │                                                       │   ║
║  │  ► Đăng nhập hệ thống                                 │   ║
║  │    Đăng ký tài khoản mới                              │   ║
║  │    Quên mật khẩu                                      │   ║
║  │    Về chúng tôi                                       │   ║
║  │    Thoát                                              │   ║
║  │                                                       │   ║
║  └───────────────────────────────────────────────────────┘   ║
║                                                              ║
║            ↑↓: Di chuyển   Enter: Chọn   Esc: Thoát          ║
╚══════════════════════════════════════════════════════════════╝
```

### Form Đăng Nhập

```
╔═══════════════════════════════════════════════════════════════╗
║                      [ĐĂNG NHẬP HỆ THỐNG]                     ║
╠═══════════════════════════════════════════════════════════════╣
║                                                               ║
║  Tên đăng nhập:  [████████████████████████████████████]       ║
║                                                               ║
║  Mật khẩu:       [████████████████████████████████████]       ║
║                                                               ║
║                                                               ║
║        ↑↓/Tab: Chọn   Enter: Nhập   F1: Đăng nhập   Esc: Thoát║
╚═══════════════════════════════════════════════════════════════╝
```

</details>

## 🚀 Hướng dẫn cài đặt

### Yêu cầu hệ thống

- **.NET 9.0 SDK** hoặc cao hơn
- **Visual Studio 2022** hoặc **VS Code**
- **SQL Server** (LocalDB hoặc SQL Server Express)
- **Windows 10/11** (khuyến nghị)

### Cài đặt và chạy

```bash
# Tải về repository
git clone https://github.com/Darlington0433/EsportsManager
cd EsportsManager

# Khôi phục các package
dotnet restore

# Build solution
dotnet build

# Chạy ứng dụng
dotnet run --project src/EsportsManager.UI
```

### Tài khoản demo

```
Admin:  admin / admin
Player: player / player
Viewer: viewer / viewer
```

## 🏗️ Kiến trúc hệ thống

### Kiến trúc 3 tầng

```
┌─────────────────────────────────────┐
│        Tầng Giao Diện (UI)          │
│  • Console Interface                │
│  • Input/Output Handling            │
│  • Menu Management                  │
│  • User Interaction                 │
└─────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────┐
│      Tầng Logic Nghiệp Vụ (BL)      │
│  • Business Rules                   │
│  • Data Validation                  │
│  • Authentication                   │
│  • Service Operations               │
└─────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────┐
│     Tầng Truy Cập Dữ Liệu (DAL)     │
│  • Database Operations              │
│  • Repository Pattern               │
│  • Data Context                     │
│  • Entity Models                    │
└─────────────────────────────────────┘
```

### Triển khai Nguyên lý SOLID

#### 1. **S**ingle Responsibility Principle (SRP)

- Mỗi class chỉ có một trách nhiệm duy nhất
- `UserService` chỉ xử lý logic nghiệp vụ của User
- `UserRepository` chỉ xử lý truy cập dữ liệu của User

#### 2. **O**pen/Closed Principle (OCP)

- Classes mở để mở rộng, đóng để sửa đổi
- `BaseRepository` có thể extend cho các entity khác

#### 3. **L**iskov Substitution Principle (LSP)

- Derived classes có thể thay thế base classes
- `UserRepository` có thể thay thế `BaseRepository`

#### 4. **I**nterface Segregation Principle (ISP)

- Interfaces nhỏ và tập trung
- `IUserService` chỉ chứa methods liên quan đến User

#### 5. **D**ependency Inversion Principle (DIP)

- Phụ thuộc vào abstractions, không phải concretions
- Services phụ thuộc vào `IRepository` interfaces

## 🛠️ Công nghệ sử dụng

| Tầng                 | Công nghệ                       | Mục đích                         |
| -------------------- | ------------------------------- | -------------------------------- |
| **Giao diện**        | C# Console Application          | Giao diện người dùng & Tương tác |
| **Logic nghiệp vụ**  | C# Services & DTOs              | Quy tắc nghiệp vụ & Validation   |
| **Truy cập dữ liệu** | Repository Pattern + SQL Server | Thao tác cơ sở dữ liệu           |
| **Framework**        | .NET 9.0                        | Môi trường chạy                  |
| **Kiến trúc**        | 3-Layer + SOLID                 | Tổ chức code                     |

## 🎯 Tính năng chính

### 👑 Tính năng Admin

- 👥 **Quản lý người dùng** - Tạo, Sửa, Xóa, Quản lý trạng thái
- 🏆 **Quản lý giải đấu** - Thiết lập giải đấu, Quản lý trận đấu
- 📊 **Thống kê hệ thống** - Phân tích người dùng, Báo cáo hệ thống
- 💸 **Báo cáo ủng hộ** - Lịch sử giao dịch, Báo cáo tài chính
- 🗳️ **Kết quả bình chọn** - Thăm dò cộng đồng, Bỏ phiếu sự kiện
- 📝 **Quản lý phản hồi** - Quản lý phản hồi người dùng
- ⚙️ **Cài đặt hệ thống** - Cấu hình hệ thống

### 🎮 Tính năng Player

- 📝 **Đăng ký giải đấu** - Đăng ký tham gia giải đấu
- 👥 **Quản lý đội** - Tạo/Tham gia đội, Quản lý thành viên
- 👤 **Hồ sơ cá nhân** - Quản lý hồ sơ, Thông tin cá nhân
- 🏆 **Danh sách giải đấu** - Duyệt các giải đấu có sẵn
- 💬 **Gửi phản hồi** - Hệ thống phản hồi
- 💰 **Quản lý ví điện tử** - Quản lý ví, Giao dịch
- 🏅 **Thành tích cá nhân** - Theo dõi thành tích

### 👁️ Tính năng Viewer

- 📺 **Xem giải đấu** - Xem giải đấu
- 📊 **Thống kê** - Thống kê trận đấu, Thống kê người chơi
- 🗳️ **Bình chọn** - Bỏ phiếu cho trận đấu/người chơi
- 💸 **Ủng hộ** - Hỗ trợ người chơi/đội
- 📝 **Phản hồi** - Phản hồi hệ thống
- 📈 **Xếp hạng** - Bảng xếp hạng & Rankings

## 📁 Cấu trúc dự án

```
src/
├── EsportsManager.UI/              # Tầng Giao Diện
│   ├── ConsoleUI/                  # Console Interface
│   ├── Controllers/                # Controllers cho từng role
│   ├── Forms/                      # Authentication Forms
│   ├── Utilities/                  # UI Utilities
│   └── Program.cs
│
├── EsportsManager.BL/              # Tầng Logic Nghiệp Vụ
│   ├── DTOs/                       # Data Transfer Objects
│   ├── Interfaces/                 # Service Contracts
│   ├── Models/                     # Business Models
│   ├── Services/                   # Business Logic
│   └── Utilities/                  # Business Utilities
│
└── EsportsManager.DAL/             # Tầng Truy Cập Dữ Liệu
    ├── Context/                    # Database Context
    ├── Interfaces/                 # Repository Contracts
    ├── Models/                     # Entity Models
    └── Repositories/               # Data Access
```

## 🤝 Đóng góp

Chúng tôi hoan nghênh mọi đóng góp! Vui lòng đọc [CONTRIBUTING.md](CONTRIBUTING.md) để biết chi tiết.

### Quy trình đóng góp nhanh:

1. Fork repository này
2. Tạo branch cho tính năng (`git checkout -b tinh-nang/TinhNangMoi`)
3. Commit thay đổi (`git commit -m 'Thêm tính năng mới'`)
4. Push lên branch (`git push origin tinh-nang/TinhNangMoi`)
5. Tạo Pull Request

## 🚧 Lộ trình phát triển

- [ ] **v1.1.0** - Tích hợp cơ sở dữ liệu

  - Kết nối SQL Server thực tế
  - Triển khai Entity Framework
  - Hệ thống migration

- [ ] **v1.2.0** - Tính năng nâng cao

  - Hệ thống bracket giải đấu
  - Theo dõi trận đấu real-time
  - Thống kê nâng cao

- [ ] **v1.3.0** - Giao diện Web

  - ASP.NET Core Web API
  - Frontend React/Angular
  - Mobile responsive

- [ ] **v2.0.0** - Cloud & Scale
  - Triển khai Azure
  - Kiến trúc Microservices
  - Thông báo real-time

## 🐛 Vấn đề đã biết

- Connection string SQL Server cần cấu hình cho production
- Một số method controller vẫn đang phát triển (đánh dấu là stubs)
- Cảnh báo bảo mật cho System.Data.SqlClient (dự kiến cập nhật)

## 📊 Thống kê dự án

![Kích thước Code](https://img.shields.io/github/languages/code-size/Darlington0433/EsportsManager)
![Số dòng Code](https://img.shields.io/tokei/lines/github/Darlington0433/EsportsManager)
![Số File](https://img.shields.io/github/directory-file-count/Darlington0433/EsportsManager)

## 💬 Hỗ trợ

- 📧 Email: quannnd2004@gmail.com
- � Issues: [GitHub Issues](https://github.com/Darlington0433/EsportsManager/issues)
- � Repository: [GitHub Repository](https://github.com/Darlington0433/EsportsManager)

## 📄 Giấy phép

Dự án này được cấp phép theo giấy phép Custom (Non-commercial) - xem file [LICENSE](LICENSE) để biết chi tiết.
## 👥 Tác giả

- **Phan Nhật Quân và mọi người** - _Sinh viên VTC Academy_ - [GitHub Profile](https://github.com/Darlington0433)
- **Email**: quannnd2004@gmail.com
- **Trường**: VTC Academy
- **Contributors** - Xem [danh sách người đóng góp](https://github.com/Darlington0433/EsportsManager/graphs/contributors)

## 🙏 Lời cảm ơn

- 🎓 **VTC Academy** - Giáo dục và hướng dẫn
- 🏗️ **Clean Architecture** - Nguyên lý thiết kế của Robert C. Martin
- 📚 **Nguyên lý SOLID** - Best practices thiết kế hướng đối tượng
- 🎮 **Cộng đồng Esports** - Cảm hứng và thu thập yêu cầu
- 💻 **Cộng đồng Open Source** - Công cụ và thư viện được sử dụng

---

<div align="center">

**🌟 Nếu bạn thấy dự án này hữu ích, hãy cho chúng tôi một sao! 🌟**

[![GitHub stars](https://img.shields.io/github/stars/Darlington0433/EsportsManager?style=social)](https://github.com/Darlington0433/EsportsManager/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/Darlington0433/EsportsManager?style=social)](https://github.com/Darlington0433/EsportsManager/network)

Được tạo với ❤️ bởi Phan Nhật Quân và mọi người - Sinh viên VTC Academy

</div>
