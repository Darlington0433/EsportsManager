# 🎮 Esports Manager - Optimized Architecture

## 📋 Tổng quan

Esports Manager là một hệ thống quản lý giải đấu esports được thiết kế theo kiến trúc 3 lớp (3-Layer Architecture) và tuân thủ các nguyên lý SOLID.

## 🏗️ Kiến trúc

### 3-Layer Architecture
```
┌─────────────────────────────────────┐
│           UI Layer (Presentation)   │
│  • Console Interface                │
│  • Input/Output Handling            │
│  • Menu Management                  │
│  • User Interaction                 │
└─────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────┐
│        BL Layer (Business Logic)    │
│  • Business Rules                   │
│  • Data Validation                  │
│  • Authentication                   │
│  • Service Operations               │
└─────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────┐
│      DAL Layer (Data Access)        │
│  • Database Operations              │
│  • Repository Pattern               │
│  • Data Context                     │
│  • Entity Models                    │
└─────────────────────────────────────┘
```

### SOLID Principles Implementation

#### 1. **S**ingle Responsibility Principle (SRP)
- Mỗi class chỉ có một trách nhiệm duy nhất
- `UserService` chỉ xử lý business logic của User
- `UserRepository` chỉ xử lý data access của User
- `ConsoleHelper` chỉ xử lý UI rendering

#### 2. **O**pen/Closed Principle (OCP)
- Classes mở để mở rộng, đóng để sửa đổi
- `BaseRepository` có thể extend cho các entity khác
- Constants classes (`UserRoles`, `UserStatus`) dễ mở rộng

#### 3. **L**iskov Substitution Principle (LSP)
- Derived classes có thể thay thế base classes
- `UserRepository` có thể thay thế `BaseRepository`
- Implementations có thể thay thế interfaces

#### 4. **I**nterface Segregation Principle (ISP)
- Interfaces nhỏ và tập trung
- `IUserService` chỉ chứa methods liên quan đến User
- `IUserRepository` chỉ chứa data access methods

#### 5. **D**ependency Inversion Principle (DIP)
- Depend on abstractions, not concretions
- Services depend on `IRepository` interfaces
- UI depends on `IService` interfaces
- Dependency Injection container

## 📁 Cấu trúc thư mục

```
src/
├── EsportsManager.UI/              # Presentation Layer
│   ├── Configuration/              # DI & Configuration
│   │   └── DIContainer.cs
│   ├── Menus/                      # Menu classes
│   │   └── MainMenu.cs
│   ├── Utilities/                  # UI utilities
│   │   ├── ConsoleHelper.cs
│   │   └── ConsoleInput.cs
│   ├── appsettings.json
│   └── Program.cs
│
├── EsportsManager.BL/              # Business Logic Layer
│   ├── DTOs/                       # Data Transfer Objects
│   │   └── UserDto.cs
│   ├── Interfaces/                 # Service contracts
│   │   └── IUserService.cs
│   ├── Models/                     # Business models
│   │   └── BusinessModels.cs
│   ├── Services/                   # Business logic
│   │   └── UserService.cs
│   └── Utilities/                  # Business utilities
│       ├── PasswordHasher.cs
│       └── InputValidator.cs
│
└── EsportsManager.DAL/             # Data Access Layer
    ├── Context/                    # Database context
    │   └── DataContext.cs
    ├── Interfaces/                 # Repository contracts
    │   ├── IRepository.cs
    │   └── IUserRepository.cs
    ├── Models/                     # Entity models
    │   └── User.cs
    └── Repositories/               # Data access
        ├── Base/
        │   └── BaseRepository.cs
        └── UserRepository.cs
```

## 🔧 Công nghệ sử dụng

- **.NET 9.0** - Framework chính
- **C#** - Ngôn ngữ lập trình
- **SQL Server** - Cơ sở dữ liệu
- **Microsoft.Extensions.DependencyInjection** - Dependency Injection
- **Microsoft.Extensions.Configuration** - Configuration management
- **Microsoft.Extensions.Logging** - Logging
- **BCrypt.Net** - Password hashing
- **FluentValidation** - Input validation

## 🚀 Cài đặt và chạy

### Prerequisites
- .NET 9.0 SDK
- SQL Server (LocalDB hoặc SQL Server Express)
- Visual Studio 2022 hoặc VS Code

### Bước 1: Clone repository
```bash
git clone <repository-url>
cd EsportsManager_Backup
```

### Bước 2: Restore packages
```bash
dotnet restore src/EsportsManager.UI/EsportsManager.UI.csproj
```

### Bước 3: Setup database
1. Tạo database `EsportsManager` trong SQL Server
2. Chạy script `database/scripts/esportsmanager.sql`
3. Cập nhật connection string trong `appsettings.json`

### Bước 4: Build và chạy
```bash
cd src/EsportsManager.UI
dotnet build
dotnet run
```

## 📊 Database Schema

### Users Table
```sql
CREATE TABLE Users (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Username nvarchar(50) UNIQUE NOT NULL,
    Email nvarchar(255) NULL,
    PasswordHash nvarchar(255) NOT NULL,
    Role nvarchar(20) NOT NULL,
    Status nvarchar(20) DEFAULT 'Active',
    CreatedAt datetime2 DEFAULT GETUTCDATE(),
    UpdatedAt datetime2 NULL,
    LastLoginAt datetime2 NULL
);
```

## 🎯 Features

### Hiện tại
- ✅ User Management (Register, Login, Profile)
- ✅ Role-based Authentication (Admin, Player, Viewer)
- ✅ Password Hashing & Validation
- ✅ Input Validation
- ✅ Error Handling & Logging
- ✅ Console UI with colored output

### Sắp tới
- 🔄 Tournament Management
- 🔄 Team Management
- 🔄 Achievement System
- 🔄 Statistics & Reporting
- 🔄 Email Notifications

## 🧪 Testing

```bash
# Run unit tests
dotnet test tests/EsportsManager.Tests.Unit/

# Run integration tests
dotnet test tests/EsportsManager.Tests.Integration/
```

## 📝 Logging

Logs được lưu tại:
- Console output
- File logs (nếu được cấu hình)

## 🔒 Security

- Password hashing với BCrypt
- Input validation tất cả inputs
- SQL injection protection với parameterized queries
- Role-based access control

## 🤝 Contributing

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Authors

- **Developer** - *Initial work* - [Your Name]

## 🙏 Acknowledgments

- Inspiration from modern software architecture patterns
- SOLID principles implementation
- Clean Architecture concepts
