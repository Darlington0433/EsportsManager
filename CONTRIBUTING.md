# Contributing to Esports Manager

🎉 Thank you for your interest in contributing to Esports Manager!

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022 or VS Code
- Git
- SQL Server (LocalDB or Express)

### Development Setup

1. Fork the repository
2. Clone your fork: `git clone https://github.com/Darlington0433/EsportsManager.git`
3. Create a new branch: `git checkout -b feature/your-feature-name`
4. Install dependencies: `dotnet restore`
5. Build the project: `dotnet build`

## 🎯 How to Contribute

### 🐛 Bug Reports

- Use the [Bug Report template](.github/ISSUE_TEMPLATE/bug_report.md)
- Include steps to reproduce
- Attach screenshots if applicable
- Specify your environment (OS, .NET version, etc.)

### ✨ Feature Requests

- Use the [Feature Request template](.github/ISSUE_TEMPLATE/feature_request.md)
- Explain the use case and benefits
- Provide mockups or examples if possible

### 💻 Code Contributions

#### Code Style Guidelines

- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public methods
- Keep methods small and focused (Single Responsibility)
- Use Vietnamese comments for internal logic

#### Architecture Guidelines

- Maintain 3-layer architecture (UI/BL/DAL)
- Follow SOLID principles
- Use Repository pattern for data access
- Implement proper error handling
- Add unit tests for business logic

#### Pull Request Process

1. Ensure your code builds without warnings
2. Add tests for new functionality
3. Update documentation if needed
4. Create a clear PR description with:
   - What changes were made
   - Why the changes were needed
   - How to test the changes
5. Link related issues

## 🔧 Development Guidelines

### Project Structure

```
src/
├── EsportsManager.UI/      # Presentation Layer
├── EsportsManager.BL/      # Business Logic Layer
└── EsportsManager.DAL/     # Data Access Layer
```

### Coding Standards

- **Classes**: PascalCase (`UserService`)
- **Methods**: PascalCase (`GetUserById`)
- **Variables**: camelCase (`userId`)
- **Constants**: UPPER_CASE (`MAX_PLAYERS`)
- **Private fields**: \_camelCase (`_userRepository`)

### Commit Messages

Use conventional commits format:

```
type(scope): description

feat(ui): add tournament registration form
fix(auth): resolve login validation issue
docs(readme): update installation instructions
refactor(dal): optimize user repository queries
test(bl): add unit tests for user service
```

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Guidelines

- Write unit tests for business logic
- Use descriptive test method names
- Follow AAA pattern (Arrange, Act, Assert)
- Mock external dependencies

## 📝 Documentation

### Code Documentation

- Add XML documentation for public APIs
- Use Vietnamese comments for complex logic
- Update README.md for new features
- Include code examples in documentation

## 🏆 Recognition

Contributors will be:

- Added to the contributors list
- Mentioned in release notes
- Recognized in the project documentation

## 📞 Getting Help

- � Email: quannnd2004@gmail.com
- 🐛 Issues: [GitHub Issues](https://github.com/Darlington0433/EsportsManager/issues)
- � Repository: [GitHub Repository](https://github.com/Darlington0433/EsportsManager)

## 📜 Code of Conduct

This project follows the [Contributor Covenant Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

---

**Thank you for contributing to Esports Manager! 🎮✨**

_Developed with ❤️ by Phan Nhật Quân và mọi người - VTC Academy Team_
