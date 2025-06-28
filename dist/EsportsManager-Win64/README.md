# 🎮 ESPORTS MANAGER - Console Application

**Repository**: [https://github.com/Darlington0433/EsportsManager](https://github.com/Darlington0433/EsportsManager)  
**Authors**: Phan Nhật Quân và mọi người - VTC Academy Team  
**Contact**: quannnd2004@gmail.com

## 📦 **DISTRIBUTION PACKAGE**

Đây là phiên bản đóng gói sẵn sàng để chạy của **Esports Manager Console Application**.

## 🚀 **CÁCH CHẠY ỨNG DỤNG**

### **Cách 1: Chạy trực tiếp (Khuyến nghị)**

```bash
EsportsManager.exe
```

### **Cách 2: Sử dụng Batch Script**

```bash
LaunchEsportsManager.bat
```

### **Cách 3: Sử dụng PowerShell Script**

```powershell
.\LaunchEsportsManager.ps1
```

## 📋 **YÊU CẦU HỆ THỐNG**

- **OS**: Windows 10/11 (64-bit)
- **Runtime**: .NET 9.0 (Self-contained - không cần cài đặt thêm)
- **RAM**: Tối thiểu 512MB
- **Disk**: ~50MB trống

## 📁 **CẤU TRÚC FILES**

```
📁 EsportsManager-Win64/
├── 🎮 EsportsManager.exe          # File chính của ứng dụng
├── ⚙️ appsettings.json            # Cấu hình ứng dụng
├── 🚀 LaunchEsportsManager.bat    # Script khởi động (CMD)
├── 🚀 LaunchEsportsManager.ps1    # Script khởi động (PowerShell)
├── 📖 README.md                   # Tài liệu này
├── 🔧 sni.dll                     # SQL Server Native Client
├── 🔧 SQLite.Interop.dll          # SQLite Database Engine
├── 🐛 EsportsManager.BL.pdb       # Debug symbols (Business Layer)
└── 🐛 EsportsManager.DAL.pdb      # Debug symbols (Data Access Layer)
```

## ✨ **TÍNH NĂNG CHÍNH**

- 🔐 **Hệ thống xác thực** với mã hóa BCrypt
- 👥 **Phân quyền người dùng**: Player, Admin, Viewer
- 🎯 **Giao diện console** tương tác hiện đại
- 📊 **Quản lý đội tuyển** Esports chuyên nghiệp
- 🗄️ **Database SQLite** nhúng sẵn
- ⚡ **Hiệu năng cao** với kiến trúc 3-layer

## 🎨 **CONSOLE FEATURES**

- ✅ **Auto-resize** console window (120x30)
- ✅ **UTF-8 Encoding** hỗ trợ tiếng Việt
- ✅ **Colorful UI** với theme chuyên nghiệp
- ✅ **Interactive Menu** với navigation dễ dàng
- ✅ **Loading Animation** và welcome banner
- ✅ **Error Handling** và recovery tự động

## 🛠️ **TROUBLESHOOTING**

### **Lỗi không chạy được EXE:**

1. Click chuột phải → Properties → Unblock
2. Chạy với quyền Administrator
3. Tắt Windows Defender tạm thời

### **Console hiển thị lỗi font:**

1. Cài đặt font Cascadia Code hoặc Consolas
2. Thay đổi font trong Properties của console

### **Lỗi database:**

1. Đảm bảo SQLite.Interop.dll nằm cùng folder
2. Kiểm tra quyền write vào folder

## 📞 **HỖ TRỢ**

- **Repository**: [GitHub](https://github.com/Darlington0433/EsportsManager)
- **Issues**: [Bug Reports](https://github.com/Darlington0433/EsportsManager/issues)
- **Documentation**: [Wiki](https://github.com/Darlington0433/EsportsManager/wiki)

---

🎮 **Chúc bạn có trải nghiệm tuyệt vời với Esports Manager!** 🎮
