# Error Codes & Handling

## Response Format

Khi có lỗi, API trả về:

```json
{
  "success": false,
  "message": "Mô tả lỗi bằng tiếng Việt",
  "errors": ["validation_error", "not_found"],
  "errorCode": "BOOKING_NOT_FOUND",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

## HTTP Status Codes

| Status | Meaning | Usage |
|--------|---------|-------|
| 200 | OK | Thành công |
| 201 | Created | Tạo mới thành công |
| 400 | Bad Request | Lỗi validation, bad data |
| 401 | Unauthorized | Token không hợp lệ/hết hạn |
| 403 | Forbidden | Không có quyền |
| 404 | Not Found | Resource không tồn tại |
| 409 | Conflict | Conflict (duplicate, etc.) |
| 422 | Unprocessable Entity | Validation failed |
| 500 | Internal Server Error | Lỗi server |

---

## Error Codes List

### Authentication (AUTH_xxx)

| Code | HTTP | Message | Solution |
|------|------|---------|----------|
| `AUTH_INVALID_TOKEN` | 401 | Token không hợp lệ | Đăng nhập lại |
| `AUTH_TOKEN_EXPIRED` | 401 | Token đã hết hạn | Refresh token |
| `AUTH_REFRESH_TOKEN_INVALID` | 401 | Refresh token không hợp lệ | Đăng nhập lại |
| `AUTH_INVALID_CREDENTIALS` | 401 | Email hoặc mật khẩu không đúng | Kiểm tra credentials |
| `AUTH_ACCOUNT_LOCKED` | 401 | Tài khoản đã bị khóa | Liên hệ support |
| `AUTH_EMAIL_NOT_VERIFIED` | 401 | Email chưa được xác thực | Verify email |
| `AUTH_FORBIDDEN` | 403 | Bạn không có quyền thực hiện | Kiểm tra permissions |

### Booking (BOOKING_xxx)

| Code | HTTP | Message | Solution |
|------|------|---------|----------|
| `BOOKING_NOT_FOUND` | 404 | Booking không tồn tại | Kiểm tra booking ID |
| `BOOKING_INVALID_STATUS` | 400 | Trạng thái booking không hợp lệ | Kiểm tra status hiện tại |
| `BOOKING_ALREADY_CHECKED_IN` | 409 | Booking đã check-in | Không thể check-in lại |
| `BOOKING_ALREADY_CHECKED_OUT` | 409 | Booking đã check-out | Không thể check-out lại |
| `BOOKING_ALREADY_CANCELLED` | 409 | Booking đã bị hủy | Booking đã cancelled |
| `BOOKING_CANNOT_CANCEL` | 400 | Không thể hủy booking | Kiểm tra cancellation policy |
| `BOOKING_INVALID_DATES` | 400 | Ngày check-in/check-out không hợp lệ | Kiểm tra date range |
| `BOOKING_ROOM_NOT_AVAILABLE` | 409 | Phòng không còn trống | Chọn phòng khác |
| `BOOKING_MAX_NIGHTS_EXCEEDED` | 400 | Vượt quá số đêm tối đa | Giảm số đêm |
| `BOOKING_MIN_NIGHTS_NOT_MET` | 400 | Chưa đủ số đêm tối thiểu | Tăng số đêm |
| `BOOKING_ADVANCE_BOOKING_LIMIT` | 400 | Vượt quá thời gian đặt trước | Kiểm tra hotel settings |
| `BOOKING_GUEST_LIMIT_EXCEEDED` | 400 | Vượt quá số khách cho phép | Giảm số khách |
| `BOOKING_CHANGE_ROOM_INVALID` | 400 | Không thể đổi phòng | Chỉ áp dụng cho checked-in bookings |
| `BOOKING_LATE_CHECKOUT_INVALID` | 400 | Không thể late checkout | Chỉ áp dụng cho checked-in bookings |

### Hotel (HOTEL_xxx)

| Code | HTTP | Message | Solution |
|------|------|---------|----------|
| `HOTEL_NOT_FOUND` | 404 | Khách sạn không tồn tại | Kiểm tra hotel ID |
| `HOTEL_INACTIVE` | 400 | Khách sạn đang không hoạt động | Liên hệ hotel |
| `HOTEL_NOT_VERIFIED` | 403 | Khách sạn chưa được xác thực | Chờ SuperAdmin duyệt |
| `HOTEL_NO_AVAILABLE_ROOMS` | 404 | Không có phòng trống | Chọn ngày khác |

### Room (ROOM_xxx)

| Code | HTTP | Message | Solution |
|------|------|---------|----------|
| `ROOM_NOT_FOUND` | 404 | Phòng không tồn tại | Kiểm tra room ID |
| `ROOM_NOT_AVAILABLE` | 409 | Phòng không khả dụng | Kiểm tra room status |
| `ROOM_OCCUPIED` | 409 | Phòng đang có khách | Chọn phòng khác |
| `ROOM_UNDER_MAINTENANCE` | 409 | Phòng đang bảo trì | Chọn phòng khác |
| `ROOM_OUT_OF_ORDER` | 409 | Phòng không hoạt động | Chọn phòng khác |
| `ROOM_INVALID_STATUS` | 400 | Trạng thái phòng không hợp lệ | Kiểm tra status |

### Payment (PAYMENT_xxx)

| Code | HTTP | Message | Solution |
|------|------|---------|----------|
| `PAYMENT_NOT_FOUND` | 404 | Payment không tồn tại | Kiểm tra payment ID |
| `PAYMENT_FAILED` | 400 | Thanh toán thất bại | Thử lại hoặc liên hệ bank |
| `PAYMENT_ALREADY_PROCESSED` | 409 | Payment đã được xử lý | Không thể process lại |
| `PAYMENT_STRIPE_ERROR` | 400 | Lỗi Stripe | Kiểm tra Stripe response |
| `PAYMENT_METHOD_NOT_ALLOWED` | 400 | Phương thức thanh toán không được chấp nhận | Chọn phương thức khác |
| `PAYMENT_HOTEL_STRIPE_NOT_CONFIGURED` | 400 | Hotel chưa cấu hình Stripe | Liên hệ hotel |
| `PAYMENT_INSUFFICIENT_FUNDS` | 400 | Số dư không đủ | Thông báo cho guest |

### User (USER_xxx)

| Code | HTTP | Message | Solution |
|------|------|---------|----------|
| `USER_NOT_FOUND` | 404 | User không tồn tại | Kiểm tra user ID |
| `USER_EMAIL_EXISTS` | 409 | Email đã được sử dụng | Dùng email khác |
| `USER_PHONE_EXISTS` | 409 | Số điện thoại đã được sử dụng | Dùng SĐT khác |
| `USER_INACTIVE` | 403 | Tài khoản đã bị vô hiệu hóa | Liên hệ support |
| `USER_ROLE_INVALID` | 400 | Role không hợp lệ | Kiểm tra role enum |
| `USER_HOTEL_MISMATCH` | 403 | User không thuộc hotel này | Kiểm tra user assignment |

### Validation (VALIDATION_xxx)

| Code | HTTP | Message | Solution |
|------|------|---------|----------|
| `VALIDATION_REQUIRED` | 400 | Trường {field} là bắt buộc | Điền đầy đủ thông tin |
| `VALIDATION_INVALID_FORMAT` | 400 | Định dạng {field} không hợp lệ | Kiểm tra format |
| `VALIDATION_INVALID_EMAIL` | 400 | Email không hợp lệ | Nhập email đúng format |
| `VALIDATION_INVALID_PHONE` | 400 | Số điện thoại không hợp lệ | Nhập SĐT đúng format |
| `VALIDATION_INVALID_DATE` | 400 | Ngày không hợp lệ | Kiểm tra date format |
| `VALIDATION_DATE_RANGE` | 400 | Ngày bắt đầu phải trước ngày kết thúc | Kiểm tra date range |
| `VALIDATION_MIN_VALUE` | 400 | {field} phải lớn hơn {min} | Tăng giá trị |
| `VALIDATION_MAX_VALUE` | 400 | {field} phải nhỏ hơn {max} | Giảm giá trị |
| `VALIDATION_MIN_LENGTH` | 400 | {field} phải có ít nhất {min} ký tự | Nhập thêm ký tự |
| `VALIDATION_MAX_LENGTH` | 400 | {field} phải có tối đa {max} ký tự | Rút ngắn |

### System (SYSTEM_xxx)

| Code | HTTP | Message | Solution |
|------|------|---------|----------|
| `SYSTEM_ERROR` | 500 | Lỗi hệ thống | Liên hệ support |
| `SYSTEM_MAINTENANCE` | 503 | Hệ thống đang bảo trì | Thử lại sau |
| `SYSTEM_DATABASE_ERROR` | 500 | Lỗi database | Liên hệ support |

---

## Frontend Error Handling Pattern

```typescript
// API Error Response
interface ApiErrorResponse {
  success: false;
  message: string;
  errors?: string[];
  errorCode?: string;
  timestamp: string;
}

// Error Handler Hook
function useErrorHandler() {
  const handleApiError = (error: ApiErrorResponse) => {
    // Show toast notification
    toast.error(error.message);

    // Handle specific error codes
    switch (error.errorCode) {
      case 'AUTH_TOKEN_EXPIRED':
        // Redirect to login
        router.push('/login');
        break;
      case 'BOOKING_NOT_FOUND':
        // Show booking not found modal
        setShowNotFoundModal(true);
        break;
      case 'ROOM_NOT_AVAILABLE':
        // Refresh room list
        refreshRooms();
        break;
      case 'PAYMENT_FAILED':
        // Show payment error modal
        setShowPaymentError(true);
        break;
    }
  };

  return { handleApiError };
}
```

---

## Common Error Messages (Vietnamese)

| Error | Vietnamese Message |
|-------|-------------------|
| Invalid credentials | Email hoặc mật khẩu không đúng |
| Token expired | Phiên đăng nhập đã hết hãy đăng nhập lại |
| Booking not found | Không tìm thấy đặt phòng |
| Room not available | Phòng này đã được đặt, vui lòng chọn phòng khác |
| Check-in failed | Check-in thất bại, vui lòng thử lại |
| Check-out failed | Check-out thất bại, vui lòng thử lại |
| Payment failed | Thanh toán thất bại, vui lòng thử lại |
| Hotel not found | Không tìm thấy khách sạn |
| Date range invalid | Ngày check-in phải trước ngày check-out |
| Guest limit exceeded | Vượt quá số khách cho phép |
