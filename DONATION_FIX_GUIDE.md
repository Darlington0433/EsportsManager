# SỬA LỖI DONATION - HƯỚNG DẪN CHI TIẾT

## ❌ LỖI HIỆN TẠI

```
Lỗi khi tải tổng quan donation: Không thể lấy tổng quan donation.
Vui lòng kiểm tra cài đặt cơ sở dữ liệu và thử lại.
```

## 🔍 NGUYÊN NHÂN

- Các stored procedures cho donation chưa được tạo trong database
- Database chưa được setup đầy đủ

## 🚀 CÁCH SỬA (CHỌN 1 TRONG 3 CÁCH)

### CÁCH 1: SỬA NHANH (KHUYẾN NGHỊ)

1. Mở MySQL Workbench hoặc phpMyAdmin
2. Chạy file `database/DONATION_FIX.sql`
3. ✅ XONG! Donation đã hoạt động

### CÁCH 2: SETUP ĐẦY ĐỦ DATABASE

1. Mở MySQL Workbench
2. Chạy file `database/testsql/RUN_ALL.sql`
3. Đợi hoàn thành (khoảng 1-2 phút)
4. ✅ XONG! Toàn bộ database đã được setup

### CÁCH 3: SỬA BẰNG LỆNH MYSQL

```sql
-- Chạy các lệnh này trong MySQL Workbench
USE EsportsManager;
SOURCE database/DONATION_FIX.sql;
```

## ✅ KIỂM TRA SAU KHI SỬA

1. Chạy ứng dụng: `dotnet run --project src/EsportsManager.UI/`
2. Đăng nhập bằng admin/admin123
3. Chọn "Báo cáo và thống kê"
4. Chọn "Tổng quan donation"
5. Nếu thấy số liệu thay vì lỗi → ✅ ĐÃ SỬA THÀNH CÔNG

## 📋 STORED PROCEDURES ĐƯỢC TẠO

- `sp_GetDonationOverview` - Lấy tổng quan donation
- `sp_GetDonationsByType` - Thống kê theo loại donation
- `sp_GetTopDonationReceivers` - Top người nhận donation
- `sp_GetTopDonators` - Top người donation
- `sp_CreateDonation` - Tạo giao dịch donation

## 🔧 NẾU VẪN LỖI

1. Kiểm tra MySQL đã chạy chưa
2. Kiểm tra connection string trong `appsettings.json`
3. Đảm bảo database `EsportsManager` đã tồn tại
4. Chạy lại file `DONATION_FIX.sql`

## 📞 TÀI KHOẢN TEST

- Admin: `admin/admin123`
- Player: `player1/player123`
- Viewer: `viewer1/viewer123`
