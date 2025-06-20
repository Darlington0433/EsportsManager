# Hướng dẫn sửa lỗi xung đột namespace trong dự án EsportsManager

## Vấn đề

Dự án hiện đang gặp vấn đề xung đột giữa các namespace. Có hai vấn đề chính:

1. **Không đồng bộ namespace**: Một số file sử dụng `EsportManager` (thiếu "s") và một số sử dụng `EsportsManager` (có "s").
2. **Xung đột với assembly đã biên dịch**: Các kiểu đã được biên dịch trong file UI.dll đang xung đột với các kiểu trong mã nguồn.

## Giải pháp

### Bước 1: Xóa các file build đã biên dịch

```powershell
# Xóa thư mục bin và obj trong cả project chính và UI project
cd c:\Users\tvmar\Desktop\LearnVTC\ASM\EsportsManager
Remove-Item -Recurse -Force bin,obj
cd UI
Remove-Item -Recurse -Force bin,obj
```

### Bước 2: Đồng bộ namespace trong tất cả các file

Đổi tất cả namespace từ `EsportManager` thành `EsportsManager` trong các file sau:

1. `UI\Menus\BaseMenu.cs`
2. `UI\Menus\AdminMenu.cs`
3. `UI\Menus\PlayerMenu.cs`
4. `UI\Forms\LoginForm.cs`
5. `UI\Forms\RegisterForm.cs`
6. `UI\Forms\ForgotPasswordForm.cs`
7. `UI\Utilities\ConsoleDrawing.cs`
8. `UI\Utilities\ConsoleInput.cs`
9. `UI\Utilities\MenuManager.cs`
10. `UI\UIHelper.cs`
11. `Program.cs`
12. `BL\Models\User.cs`
13. `BL\Services\UserService.cs`
14. `BL\Utilities\PasswordHasher.cs`
15. `DAL\DataContext.cs`
16. `DAL\Repositories\UserRepository.cs`

### Bước 3: Đảm bảo các using statement đúng

Thêm các using statement phù hợp vào đầu mỗi file, ví dụ:

```csharp
using System;
using EsportsManager.UI.Utilities;
```

### Bước 4: Tái cấu trúc code (Nếu cần)

1. Tạo lớp `BaseForm` làm lớp cơ sở cho tất cả các form
2. Cập nhật các form hiện có để kế thừa từ `BaseForm`
3. Thêm XML documentation cho tất cả public method/class

### Bước 5: Build lại dự án

```powershell
cd c:\Users\tvmar\Desktop\LearnVTC\ASM\EsportsManager
dotnet build
```

## Cải tiến đề xuất sau khi sửa lỗi

1. Thêm logging để theo dõi và gỡ lỗi hiệu quả hơn
2. Sử dụng Dependency Injection để quản lý các dependency
3. Tách các settings vào file cấu hình riêng
4. Thêm unit test để đảm bảo chất lượng code
5. Đảm bảo tất cả public method/class đều có XML doc
