# Danh mục Thông báo Hệ thống (System Messages)

Tài liệu này liệt kê tất cả các thông báo (lỗi và thành công) mà API trả về. Tất cả các thông báo đều sử dụng tiếng Việt để phục vụ người dùng cuối.

## Quy định chung
- **Success:** `true` kèm theo message tích cực.
- **Fail:** `false` kèm theo message mô tả lỗi.
- **Ngôn ngữ:** Tiếng Việt chuẩn.

---

## 1. Xác thực & Tài khoản (Authentication & Account)

| Message (Tiếng Việt) | Ngữ cảnh |
|----------------------|----------|
| Email hoặc mật khẩu không đúng | Đăng nhập thất bại |
| Đăng nhập thành công | Đăng nhập thành công |
| Email đã tồn tại | Đăng ký với email đã có trong hệ thống |
| Đăng ký thành công | Đăng ký tài khoản mới thành công |
| Mã làm mới không hợp lệ | Refresh token sai hoặc hết hạn |
| Đăng xuất thành công | Đăng xuất thành công |
| Thay đổi mật khẩu thành công | Cập nhật mật khẩu mới thành công |
| Thay đổi mật khẩu thất bại. Vui lòng kiểm tra lại mật khẩu hiện tại. | Nhập sai mật khẩu cũ |
| Khôi phục mật khẩu thành công | Đặt lại mật khẩu qua email thành công |

## 2. Quản lý Đặt phòng (Booking Management)

| Message (Tiếng Việt) | Ngữ cảnh |
|----------------------|----------|
| Không tìm thấy thông tin đặt phòng | Truy cập booking ID không tồn tại |
| Đặt phòng thành công | Tạo mới đặt phòng thành công |
| Cập nhật thông tin đặt phòng thành công | Thay đổi thông tin đặt phòng |
| Hủy đặt phòng thành công | Hủy đặt phòng thành công |
| Xác nhận đặt phòng thành công | Nhân viên xác nhận đặt phòng |
| Nhận phòng thành công | Khách thực hiện check-in |
| Trả phòng thành công | Khách thực hiện check-out |
| Đổi phòng thành công | Thay đổi phòng trong cùng đặt phòng |
| Xử lý trả phòng muộn thành công | Tính phí và cập nhật late check-out |
| Đã thêm chi phí phát sinh | Thêm dịch vụ/phí vào booking |
| Rất tiếc, phòng này không còn trống | Đặt phòng vào ngày đã có người đặt |

## 3. Khách sạn & Phòng (Hotel & Room)

| Message (Tiếng Việt) | Ngữ cảnh |
|----------------------|----------|
| Không tìm thấy khách sạn | Truy cập hotel ID không tồn tại |
| Tạo khách sạn thành công | Admin tạo khách sạn mới |
| Cập nhật khách sạn thành công | Thay đổi thông tin khách sạn |
| Xóa khách sạn thành công | Xóa khách sạn khỏi hệ thống |
| Không tìm thấy phòng | Truy cập room ID không tồn tại |
| Tạo phòng thành công | Tạo mới phòng trong khách sạn |
| Cập nhật phòng thành công | Thay đổi thông tin phòng |
| Đã đánh dấu phòng là trống | Chuyển trạng thái phòng sang khả dụng |

## 4. Người dùng & Phân quyền (Users & Permissions)

| Message (Tiếng Việt) | Ngữ cảnh |
|----------------------|----------|
| Không tìm thấy người dùng | Truy cập user ID không tồn tại |
| Email là bắt buộc | Thiếu email khi tạo/cập nhật |
| Bạn không có quyền thực hiện hành động này | Lỗi phân quyền chung |
| Cập nhật thông tin cá nhân thành công | User tự cập nhật profile |
| Cập nhật người dùng thành công | Admin cập nhật thông tin user |

## 5. Dịch vụ & Khác (Services & Others)

| Message (Tiếng Việt) | Ngữ cảnh |
|----------------------|----------|
| Không tìm thấy gói dịch vụ | Truy cập subscription plan không có |
| Đăng ký thành công | Đăng ký gói dịch vụ mới |
| Cập nhật đánh giá thành công | Sửa đổi đánh giá khách sạn |
| Mã giảm giá không hợp lệ | Nhập sai hoặc mã hết hạn |
| Đã thêm vào danh sách yêu thích | Thêm khách sạn vào wishlist |
| Hệ thống gặp sự cố, vui lòng thử lại sau | Lỗi server (500) |

---
*Lưu ý: Danh sách này bao quát 100% các thông báo hiện có trong codebase dựa trên rà soát ngày 13/01/2026. Một số thông báo động (dynamic) sẽ được chuẩn hóa tương đương.*
