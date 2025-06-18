# Hướng dẫn sửa lỗi xung đột với BaseForm

## Vấn đề
Sau khi tạo lớp `BaseForm` làm lớp cơ sở cho các form, các form hiện tại (LoginForm, RegisterForm, ForgotPasswordForm) gặp vấn đề khi kế thừa từ `BaseForm` do xung đột với UI.dll đã được biên dịch.

## Giải pháp 1: Cập nhật các form hiện tại
1. Thêm constructor phù hợp
2. Override ProcessForm
3. Thêm từ khóa 'new' trước phương thức Show()

### Ví dụ cho LoginForm:
```csharp
public class LoginForm : BaseForm
{
    // Constructor
    public LoginForm() : base("[ĐĂNG NHẬP]", 60, 9)
    {
    }
    
    // Override ProcessForm từ BaseForm
    protected override object? ProcessForm()
    {
        // Logic xử lý form
    }
    
    // Thêm từ khóa new để ghi đè lên Show() của BaseForm
    public new (string Username, string Password)? Show()
    {
        return (base.Show() as (string, string)?);
    }
}
```

### Ví dụ cho RegisterForm:
```csharp
public class RegisterForm : BaseForm
{
    // Constructor
    public RegisterForm() : base("[ĐĂNG KÝ]", 60, 15)
    {
    }
    
    // Override ProcessForm từ BaseForm
    protected override object? ProcessForm()
    {
        // Logic xử lý form
    }
    
    // Thêm từ khóa new
    public new (string Username, string Email, string Password, string SecurityAnswer)? Show()
    {
        return (base.Show() as (string, string, string, string)?);
    }
}
```

### Ví dụ cho ForgotPasswordForm:
```csharp
public class ForgotPasswordForm : BaseForm
{
    // Constructor
    public ForgotPasswordForm() : base("[QUÊN MẬT KHẨU]", 60, 12)
    {
    }
    
    // Override ProcessForm từ BaseForm
    protected override object? ProcessForm()
    {
        // Logic xử lý form
    }
    
    // Thêm từ khóa new
    public new (string Username, string Email, string SecurityAnswer)? Show()
    {
        return (base.Show() as (string, string, string)?);
    }
}
```

## Giải pháp 2: Xóa tất cả bin/obj và rebuild
1. Xóa thư mục bin và obj trong cả hai project
2. Xóa file UI.dll đã biên dịch
3. Build lại dự án

## Giải pháp 3: Sử dụng tên BaseForm khác
Nếu vẫn gặp xung đột, bạn có thể đổi tên BaseForm thành:
- FormBase
- CustomFormBase
- EsportsManagerFormBase

Điều này sẽ tránh xung đột với bất kỳ BaseForm nào đã tồn tại trong assembly.
