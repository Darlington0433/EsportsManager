# Hướng dẫn sửa lỗi và thêm dữ liệu mẫu

## Tổng quan

Bộ scripts này được tạo để:

1. Sửa các thủ tục liên quan đến donations đang bị lỗi
2. Thêm dữ liệu mẫu cho các bảng chính của hệ thống

## Cấu trúc files

- `fix_donation_procedures.sql`: Sửa lại các stored procedures liên quan đến donations
- `sample_donations_data.sql`: Thêm 30 mẫu donations
- `sample_teams_data.sql`: Thêm 10 teams mẫu
- `sample_tournaments_data.sql`: Thêm 10 tournaments mẫu
- `sample_wallets_data.sql`: Tạo ví cho 10 người dùng đầu tiên
- `sample_wallet_transactions_data.sql`: Thêm 30 giao dịch ví mẫu
- `run_all_fixes_and_samples.sql`: Script chạy tất cả các file trên

## Cách sử dụng

### 1. Sửa lỗi thủ tục donations

Để chỉ sửa các thủ tục đang bị lỗi mà không thêm dữ liệu mẫu:

```bash
mysql -u username -p EsportsManager < fix_donation_procedures.sql
```

### 2. Thêm dữ liệu mẫu cho một bảng cụ thể

Ví dụ, để thêm dữ liệu mẫu chỉ cho bảng Donations:

```bash
mysql -u username -p EsportsManager < sample_donations_data.sql
```

### 3. Chạy tất cả các scripts

Để thực hiện tất cả các sửa chữa và thêm dữ liệu mẫu:

```bash
mysql -u username -p EsportsManager < run_all_fixes_and_samples.sql
```

Hoặc chạy từng file một theo thứ tự:

```bash
mysql -u username -p EsportsManager < fix_donation_procedures.sql
mysql -u username -p EsportsManager < sample_teams_data.sql
mysql -u username -p EsportsManager < sample_tournaments_data.sql
mysql -u username -p EsportsManager < sample_wallets_data.sql
mysql -u username -p EsportsManager < sample_wallet_transactions_data.sql
mysql -u username -p EsportsManager < sample_donations_data.sql
```

## Chú ý quan trọng

1. File `fix_donation_procedures.sql` chỉ sửa lại các thủ tục liên quan đến donations mà không thay đổi cấu trúc bảng.
2. Các file thêm dữ liệu mẫu chứa kiểm tra để tránh thêm dữ liệu trùng lặp hoặc không cần thiết.
3. Các thủ tục được thiết kế để tương thích với cấu trúc bảng trong file `01_create_database_and_tables.sql`.
4. Thứ tự chạy các file có ý nghĩa quan trọng vì một số dữ liệu phụ thuộc vào dữ liệu khác.
