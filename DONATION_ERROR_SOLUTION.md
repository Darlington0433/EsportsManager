# 🚨 SỬA LỖI DONATION - HƯỚNG DẪN TOÀN DIỆN

## ❌ THÔNG BÁO LỖI HIỆN TẠI

```
Lỗi khi tải tổng quan donation: Không thể lấy tổng quan donation.
Vui lòng kiểm tra cài đặt cơ sở dữ liệu và thử lại.
```

## 🔍 NGUYÊN NHÂN CHÍNH

Các **stored procedures** cho chức năng donation chưa được tạo trong database MySQL. Cụ thể thiếu:

- `sp_GetDonationOverview`
- `sp_GetDonationsByType`
- `sp_GetTopDonationReceivers`
- `sp_GetTopDonators`
- `sp_CreateDonation`

## 🚀 CÁCH SỬA NGAY (3 BƯỚC ĐỀN GIẢN)

### BƯỚC 1: Mở MySQL Workbench

1. Mở **MySQL Workbench**
2. Kết nối với server MySQL (localhost)
3. Username: `root`, Password: `quan2004` (hoặc password bạn đã đặt)

### BƯỚC 2: Chạy File Sửa Lỗi HOÀN CHỈNH

1. Trong MySQL Workbench, chọn **File > Open SQL Script**
2. Navigate đến: `e:\featUI\EsportsManager\database\DONATION_FIX_COMPLETE.sql`
3. Nhấn **Open**
4. Nhấn **Execute** (⚡ biểu tượng tia chớp) hoặc Ctrl+Shift+Enter
5. Đợi 10-30 giây để hoàn thành

> ⚠️ **LƯU Ý**: Sử dụng file `DONATION_FIX_COMPLETE.sql` thay vì `DONATION_FIX.sql` để có đầy đủ tất cả stored procedures cần thiết!

### BƯỚC 3: Kiểm Tra Kết Quả

1. Chạy ứng dụng:
   ```bash
   cd e:\featUI\EsportsManager
   dotnet run --project src/EsportsManager.UI/
   ```
2. Đăng nhập với `admin / admin123`
3. Chọn **"Báo cáo và thống kê"**
4. Chọn **"Tổng quan donation"**
5. ✅ Nếu thấy số liệu thay vì lỗi → **ĐÃ SỬA THÀNH CÔNG!**

## 📋 WHAT THE FIX DOES

File `DONATION_FIX.sql` sẽ tạo:

- ✅ 5 stored procedures cần thiết cho donation
- ✅ Sửa lỗi schema compatibility
- ✅ Thêm sample data nếu cần
- ✅ Kiểm tra và tạo các indexes cần thiết

## 🔧 NẾU VẪN LỖI SAU KHI CHẠY FIX

### Lỗi 1: "Table 'Donations' doesn't exist"

**Nguyên nhân:** Database chưa được setup đầy đủ
**Cách sửa:**

```sql
-- Chạy trong MySQL Workbench
USE EsportsManager;
SOURCE e:/featUI/EsportsManager/database/testsql/RUN_ALL.sql;
```

### Lỗi 2: "Can't connect to MySQL server"

**Nguyên nhân:** MySQL service chưa chạy
**Cách sửa:**

1. Mở **Services** (Windows+R → services.msc)
2. Tìm **MySQL** service
3. Right-click → **Start**

### Lỗi 3: "Access denied for user 'root'"

**Nguyên nhân:** Sai password MySQL
**Cách sửa:**

1. Mở `src/EsportsManager.UI/appsettings.json`
2. Sửa password trong ConnectionStrings:
   ```json
   "DefaultConnection": "Server=localhost;Database=EsportsManager;Uid=root;Pwd=YOUR_ACTUAL_PASSWORD;CharSet=utf8mb4;"
   ```

## 📞 TÀI KHOẢN TEST SAU KHI SỬA

- **Admin:** `admin / admin123`
- **Player:** `player1 / player123`
- **Viewer:** `viewer1 / viewer123`

## 🎯 KIỂM TRA THÀNH CÔNG

Sau khi sửa, bạn sẽ thấy:

- Tổng quan donation hiển thị số liệu
- Top người donation
- Top người nhận donation
- Lịch sử donation
- Tìm kiếm donation

## ⚡ QUICK TEST

Để test nhanh donation có hoạt động:

1. Đăng nhập với `viewer1 / viewer123`
2. Chọn **"Donation cho Player"**
3. Donation cho player1 số tiền 50000 VND
4. Quay lại admin → Báo cáo → Tổng quan donation
5. Kiểm tra có hiện donation mới không

---

**💡 LƯU Ý:** Nếu vẫn gặp vấn đề, hãy chạy file `database/testsql/RUN_ALL.sql` để setup toàn bộ database từ đầu.
