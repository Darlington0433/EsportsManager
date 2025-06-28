# 🏗️ OPTIMIZED FOLDER STRUCTURE

**Repository**: [https://github.com/Darlington0433/EsportsManager](https://github.com/Darlington0433/EsportsManager)  
**Authors**: Phan Nhật Quân và mọi người - VTC Academy Team  
**Contact**: quannnd2004@gmail.com

## 📁 Cấu trúc thư mục tối ưu cho EsportsManager

```
EsportsManager/                           # Root project
├── src/                                  # Source code
│   ├── EsportsManager.UI/               # UI Layer (Console Application)
│   │   ├── Program.cs                   # Entry point
│   │   ├── Configuration/               # App configuration
│   │   │   ├── AppSettings.cs
│   │   │   └── DIContainer.cs           # Dependency injection setup
│   │   ├── Menus/                       # Role-based menus
│   │   │   ├── Base/
│   │   │   │   └── BaseMenu.cs          # Base menu class
│   │   │   ├── AdminMenu.cs
│   │   │   ├── PlayerMenu.cs
│   │   │   └── ViewerMenu.cs
│   │   ├── Forms/                       # Console forms
│   │   │   ├── Base/
│   │   │   │   └── BaseForm.cs
│   │   │   ├── Auth/
│   │   │   │   ├── LoginForm.cs
│   │   │   │   ├── RegisterForm.cs
│   │   │   │   └── ForgotPasswordForm.cs
│   │   │   ├── Admin/
│   │   │   │   ├── TournamentManagementForm.cs
│   │   │   │   └── UserManagementForm.cs
│   │   │   ├── Player/
│   │   │   │   ├── TeamManagementForm.cs
│   │   │   │   └── TournamentRegistrationForm.cs
│   │   │   └── Viewer/
│   │   │       ├── TournamentViewForm.cs
│   │   │       └── DonationForm.cs
│   │   ├── Controls/                    # Reusable UI components
│   │   │   ├── TableRenderer.cs
│   │   │   ├── MenuRenderer.cs
│   │   │   ├── FormRenderer.cs
│   │   │   └── ProgressBar.cs
│   │   ├── Utilities/                   # UI utilities
│   │   │   ├── ConsoleDrawing.cs
│   │   │   ├── ConsoleInput.cs
│   │   │   ├── MenuManager.cs
│   │   │   └── UIHelper.cs
│   │   └── Resources/                   # Static resources
│   │       ├── Messages.cs              # UI messages
│   │       └── Constants.cs             # UI constants
│   │
│   ├── EsportsManager.BL/              # Business Logic Layer
│   │   ├── Interfaces/                 # Contracts
│   │   │   ├── Services/
│   │   │   │   ├── IUserService.cs
│   │   │   │   ├── ITournamentService.cs
│   │   │   │   ├── ITeamService.cs
│   │   │   │   ├── IAchievementService.cs
│   │   │   │   ├── IFeedbackService.cs
│   │   │   │   ├── IWalletService.cs
│   │   │   │   ├── IDonationService.cs
│   │   │   │   ├── IWithdrawalService.cs
│   │   │   │   ├── IVoteService.cs
│   │   │   │   └── IStatisticsService.cs
│   │   │   └── Repositories/
│   │   │       ├── IUserRepository.cs
│   │   │       ├── ITournamentRepository.cs
│   │   │       └── ... (other repository interfaces)
│   │   ├── Models/                     # Data models
│   │   │   ├── Core/                   # Core entities
│   │   │   │   ├── User.cs
│   │   │   │   ├── Tournament.cs
│   │   │   │   ├── Team.cs
│   │   │   │   └── TeamMember.cs
│   │   │   ├── Financial/              # Financial entities
│   │   │   │   ├── Wallet.cs
│   │   │   │   ├── Donation.cs
│   │   │   │   └── Withdrawal.cs
│   │   │   ├── Competition/            # Competition entities
│   │   │   │   ├── Registration.cs
│   │   │   │   ├── Achievement.cs
│   │   │   │   ├── TournamentResult.cs
│   │   │   │   └── Vote.cs
│   │   │   ├── Communication/          # Communication entities
│   │   │   │   ├── Feedback.cs
│   │   │   │   └── RoleChangeRequest.cs
│   │   │   └── DTOs/                   # Data Transfer Objects
│   │   │       ├── UserDTO.cs
│   │   │       ├── TournamentDTO.cs
│   │   │       └── StatisticsDTO.cs
│   │   ├── Services/                   # Business logic services
│   │   │   ├── Core/
│   │   │   │   ├── UserService.cs
│   │   │   │   ├── TournamentService.cs
│   │   │   │   └── TeamService.cs
│   │   │   ├── Financial/
│   │   │   │   ├── WalletService.cs
│   │   │   │   ├── DonationService.cs
│   │   │   │   └── WithdrawalService.cs
│   │   │   ├── Competition/
│   │   │   │   ├── AchievementService.cs
│   │   │   │   ├── VoteService.cs
│   │   │   │   └── StatisticsService.cs
│   │   │   └── Communication/
│   │   │       └── FeedbackService.cs
│   │   ├── Utilities/                  # Business utilities
│   │   │   ├── PasswordHasher.cs
│   │   │   ├── InputValidator.cs
│   │   │   ├── Logger.cs
│   │   │   ├── EmailService.cs
│   │   │   └── FileHelper.cs
│   │   ├── Exceptions/                 # Custom exceptions
│   │   │   ├── BusinessException.cs
│   │   │   ├── ValidationException.cs
│   │   │   └── AuthenticationException.cs
│   │   └── Extensions/                 # Extension methods
│   │       ├── StringExtensions.cs
│   │       ├── DateTimeExtensions.cs
│   │       └── CollectionExtensions.cs
│   │
│   └── EsportsManager.DAL/             # Data Access Layer
│       ├── Interfaces/                 # Repository interfaces
│       │   ├── IRepository.cs          # Base repository interface
│       │   ├── IUserRepository.cs
│       │   ├── ITournamentRepository.cs
│       │   └── ... (other interfaces)
│       ├── Repositories/               # Data access implementations
│       │   ├── Base/
│       │   │   └── BaseRepository.cs   # Base repository
│       │   ├── Core/
│       │   │   ├── UserRepository.cs
│       │   │   ├── TournamentRepository.cs
│       │   │   ├── TeamRepository.cs
│       │   │   └── TeamMemberRepository.cs
│       │   ├── Financial/
│       │   │   ├── WalletRepository.cs
│       │   │   ├── DonationRepository.cs
│       │   │   └── WithdrawalRepository.cs
│       │   └── Competition/
│       │       ├── RegistrationRepository.cs
│       │       ├── AchievementRepository.cs
│       │       ├── TournamentResultRepository.cs
│       │       ├── VoteRepository.cs
│       │       └── FeedbackRepository.cs
│       ├── Context/                    # Database context
│       │   ├── DataContext.cs          # Main database context
│       │   └── Configurations/         # Entity configurations
│       │       ├── UserConfiguration.cs
│       │       ├── TournamentConfiguration.cs
│       │       └── ... (other configurations)
│       ├── Migrations/                 # Database migrations
│       │   ├── 001_Initial.sql
│       │   ├── 002_AddWallets.sql
│       │   └── ... (other migrations)
│       └── Utilities/                  # Data utilities
│           ├── ConnectionHelper.cs
│           ├── SqlHelper.cs
│           └── DatabaseSeeder.cs
│
├── tests/                              # Test projects
│   ├── EsportsManager.Tests.Unit/     # Unit tests
│   │   ├── Services/
│   │   ├── Repositories/
│   │   └── Utilities/
│   ├── EsportsManager.Tests.Integration/ # Integration tests
│   │   ├── Database/
│   │   └── Services/
│   └── EsportsManager.Tests.UI/       # UI tests
│       ├── Menus/
│       └── Forms/
│
├── database/                           # Database scripts
│   ├── scripts/
│   │   ├── esportsmanager.sql         # Main database script
│   │   ├── sample-data.sql            # Sample data
│   │   ├── stored-procedures.sql      # Stored procedures
│   │   └── views.sql                  # Database views
│   ├── migrations/                    # Migration scripts
│   │   ├── 001_initial_schema.sql
│   │   └── 002_add_wallets.sql
│   └── diagrams/                      # Database diagrams
│       ├── erd.drawio                 # Entity Relationship Diagram
│       └── database-schema.png
│
├── docs/                              # Documentation
│   ├── README.md                      # Main documentation
│   ├── INSTALLATION.md               # Setup instructions
│   ├── API-DOCUMENTATION.md          # API documentation
│   ├── ARCHITECTURE.md               # Architecture overview
│   ├── DEVELOPER-GUIDE.md            # Development guide
│   ├── TROUBLESHOOTING.md            # Common issues
│   ├── CHANGELOG.md                  # Version history
│   ├── architecture/                 # Architecture diagrams
│   │   ├── system-overview.png
│   │   ├── class-diagram.png
│   │   └── use-case-diagram.png
│   └── images/                       # Screenshots & images
│       ├── login-screen.png
│       ├── admin-dashboard.png
│       └── player-interface.png
│
├── tools/                             # Development tools
│   ├── scripts/                      # Build & deployment scripts
│   │   ├── build.bat
│   │   ├── deploy.bat
│   │   └── database-backup.bat
│   ├── generators/                   # Code generators
│   │   ├── model-generator.cs
│   │   └── repository-generator.cs
│   └── analyzers/                    # Code analyzers
│       └── custom-rules.json
│
├── config/                           # Configuration files
│   ├── appsettings.json             # Application settings
│   ├── database.config             # Database configuration
│   └── logging.config              # Logging configuration
│
├── .gitignore                        # Git ignore rules
├── .editorconfig                     # Editor configuration
├── EsportsManager.sln               # Solution file
├── Directory.Build.props            # MSBuild properties
├── nuget.config                     # NuGet configuration
├── LICENSE                          # License file
└── CONTRIBUTING.md                  # Contribution guidelines
```

## 🎯 Key Benefits của cấu trúc này:

### ✅ **Separation of Concerns**

- Mỗi layer có trách nhiệm rõ ràng
- Business logic tách biệt khỏi UI và Data Access

### ✅ **Scalability**

- Dễ dàng thêm features mới
- Có thể mở rộng sang Web/Mobile UI

### ✅ **Maintainability**

- Code được tổ chức logic
- Dễ debug và maintain

### ✅ **Testability**

- Có thể test từng layer riêng biệt
- Mock dependencies dễ dàng

### ✅ **Documentation**

- Tài liệu đầy đủ cho developers
- Hướng dẫn setup và troubleshooting

### ✅ **Professional Standards**

- Tuân thủ best practices
- Sẵn sàng cho production

## 📝 Notes:

1. **Interfaces**: Cho phép Dependency Injection và Unit Testing
2. **DTOs**: Tách biệt internal models với external data transfer
3. **Configuration**: Centralized configuration management
4. **Migrations**: Version control cho database schema
5. **Documentation**: Comprehensive documentation cho team collaboration
6. **Testing**: Complete testing strategy
7. **Tools**: Development productivity tools
