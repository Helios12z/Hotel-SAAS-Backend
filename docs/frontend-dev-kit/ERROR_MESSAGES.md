# Danh mục Thông báo Hệ thống (System Messages) - 100% Synchronization

Tài liệu này liệt kê TOÀN BỘ các hằng số thông báo được định nghĩa trong `Messages.cs`. Đây là nguồn tham khảo duy nhất cho Frontend khi cần hiển thị thông báo.

---

## 1. Xác thực (Authentication)
| Key | Nội dung (Tiếng Việt) |
|-----|----------------------|
| `Auth.InvalidCredentials` | Email hoặc mật khẩu không đúng |
| `Auth.LoginSuccess` | Đăng nhập thành công |
| `Auth.EmailExists` | Email đã được sử dụng |
| `Auth.RegistrationSuccess` | Đăng ký tài khoản mới thành công |
| `Auth.InvalidRefreshToken` | Phiên làm việc không hợp lệ hoặc đã hết hạn |
| `Auth.RefreshTokenSuccess` | Làm mới mã xác thực thành công |
| `Auth.LogoutSuccess` | Đăng xuất thành công |
| `Auth.LogoutFailed` | Đăng xuất thất bại |
| `Auth.PasswordChangeSuccess` | Thay đổi mật khẩu thành công |
| `Auth.PasswordChangeFailed` | Thay đổi mật khẩu thất bại. Vui lòng kiểm tra lại mật khẩu hiện tại. |
| `Auth.ForgotPasswordSuccess` | Nếu email tồn tại trong hệ thống, hướng dẫn khôi phục mật khẩu sẽ được gửi đi. |
| `Auth.ResetPasswordSuccess` | Đặt lại mật khẩu thành công |
| `Auth.ResetPasswordFailed` | Đặt lại mật khẩu thất bại |
| `Auth.EmailRequired` | Email là bắt buộc |

## 2. Đặt phòng (Booking)
| Key | Nội dung (Tiếng Việt) |
|-----|----------------------|
| `Booking.NotFound` | Không tìm thấy thông tin đặt phòng |
| `Booking.Created` | Đặt phòng thành công |
| `Booking.Updated` | Cập nhật thông tin đặt phòng thành công |
| `Booking.Cancelled` | Hủy đặt phòng thành công |
| `Booking.CancelFailed` | Hủy đặt phòng thất bại |
| `Booking.Confirmed` | Xác nhận đặt phòng thành công |
| `Booking.ConfirmFailed` | Xác nhận đặt phòng thất bại |
| `Booking.CheckInSuccess` | Nhận phòng (Check-in) thành công |
| `Booking.CheckInFailed` | Nhận phòng thất bại |
| `Booking.CheckOutSuccess` | Trả phòng (Check-out) thành công |
| `Booking.CheckOutFailed` | Trả phòng thất bại |
| `Booking.RoomChanged` | Đổi phòng thành công |
| `Booking.LateCheckOutProcessed` | Xử lý trả phòng muộn thành công |
| `Booking.AdditionalChargeAdded` | Đã thêm chi phí phát sinh thành công |
| `Booking.AdditionalChargeRemoved` | Đã xóa chi phí phát sinh |
| `Booking.AdditionalChargeRemoveFailed` | Xóa chi phí phát sinh thất bại |
| `Booking.CannotChangeRoomStatus` | Chỉ có thể đổi phòng cho các đặt phòng đã nhận phòng |
| `Booking.CannotCalculateLateCheckOut` | Chỉ có thể tính phí trả phòng muộn cho các đặt phòng đã nhận phòng |
| `Booking.NewCheckOutTimeInvalid` | Thời gian trả phòng mới phải sau thời gian trả phòng hiện tại |
| `Booking.NoRoomsInBooking` | Đặt phòng này không có thông tin phòng |
| `Booking.OldRoomNotFound` | Không tìm thấy phòng cũ |
| `Booking.NewRoomNotFound` | Không tìm thấy phòng mới |
| `Booking.NewRoomNotAvailable` | Phòng mới hiện không khả dụng |
| `Booking.BookingRoomNotFound` | Không tìm thấy thông tin chi tiết phòng trong đặt phòng |

## 3. Khách sạn & Tiện ích (Hotel & Amenities)
| Key | Nội dung (Tiếng Việt) |
|-----|----------------------|
| `Hotel.NotFound` | Không tìm thấy khách sạn |
| `Hotel.Created` | Tạo khách sạn thành công |
| `Hotel.Updated` | Cập nhật thông tin khách sạn thành công |
| `Hotel.Deleted` | Xóa khách sạn thành công |
| `Hotel.DeleteFailed` | Xóa khách sạn thất bại |
| `Hotel.InvalidDates` | Ngày trả phòng phải sau ngày nhận phòng |
| `Hotel.CheckInPast` | Ngày nhận phòng không thể ở trong quá khứ |
| `Hotel.BrandAdminOnly` | Chỉ Quản trị viên Thương hiệu mới có quyền thực hiện hành động này |
| `Hotel.BrandIdRequired` | Mã thương hiệu (Brand ID) là bắt buộc |
| `Hotel.HotelIdRequired` | Mã khách sạn (Hotel ID) là bắt buộc |
| `Hotel.AmenityNotFound` | Không tìm thấy tiện ích |
| `Hotel.AmenityCreated` | Tạo tiện ích thành công |
| `Hotel.AmenityUpdated` | Cập nhật tiện ích thành công |

## 4. Người dùng (User)
| Key | Nội dung (Tiếng Việt) |
|-----|----------------------|
| `User.NotFound` | Không tìm thấy người dùng |
| `User.ProfileUpdated` | Cập nhật thông tin cá nhân thành công |
| `User.UpdateSuccess` | Cập nhật người dùng thành công |
| `User.UpdateFailed` | Cập nhật người dùng thất bại |
| `User.NoPermission` | Bạn không có quyền thực hiện hành động này |
| `User.CreatedSuccess` | Tạo người dùng thành công |
| `User.UserMismatch` | Người dùng không thuộc về đơn vị này |
| `User.ProfileNotFound` | Không tìm thấy thông tin hồ sơ khách |
| `User.PreferencesUpdated` | Cập nhật sở thích thành công |
| `User.HistoryCleared` | Đã xóa lịch sử |
| `User.SuperAdminOnlyBrandAdmin` | SuperAdmin chỉ có quyền tạo tài khoản Quản trị viên Thương hiệu (BrandAdmin) |
| `User.BrandIdRequiredForBrandAdmin` | Mã Thương hiệu (Brand ID) là bắt buộc khi tạo BrandAdmin |
| `User.BrandAdminCreated` | Tạo Quản trị viên Thương hiệu thành công |
| `User.BrandAdminOnlyHotelManager` | Quản trị viên Thương hiệu chỉ có quyền tạo tài khoản Quản lý Khách sạn (HotelManager) |
| `User.CannotCreateForDifferentBrand` | Không thể tạo người dùng cho thương hiệu khác |
| `User.HotelIdRequiredForHotelManager` | Mã Khách sạn (Hotel ID) là bắt buộc khi tạo HotelManager |
| `User.HotelManagerCreated` | Tạo Quản lý Khách sạn thành công |
| `User.HotelManagerOnlyStaff` | Quản lý Khách sạn chỉ có quyền tạo tài khoản Lễ tân hoặc Nhân viên |
| `User.CannotCreateForDifferentHotel` | Không thể tạo người dùng cho khách sạn khác |
| `User.SuperAdminOnly` | Chỉ SuperAdmin mới có quyền thực hiện hành động này |

## 5. Quản trị Nền tảng (Platform)
| Key | Nội dung (Tiếng Việt) |
|-----|----------------------|
| `Platform.BrandNotFound` | Không tìm thấy thương hiệu |
| `Platform.BrandCreated` | Tạo thương hiệu thành công |
| `Platform.BrandUpdated` | Cập nhật thương hiệu thành công |
| `Platform.BrandDeleted` | Xóa thương hiệu thành công |
| `Platform.BrandDeleteFailed` | Xóa thương hiệu thất bại |
| `Platform.PlanNotFound` | Không tìm thấy gói cước |
| `Platform.PlanCreated` | Tạo gói cước mới thành công |
| `Platform.PlanUpdated` | Cập nhật gói cước thành công |
| `Platform.PlanDeleted` | Xóa gói cước thành công |
| `Platform.PlanDeleteFailed` | Xóa gói cước thất bại |
| `Platform.PlanActivated` | Đã kích hoạt gói cước |
| `Platform.PlanDeactivated` | Đã tạm dừng gói cước |
| `Platform.SettingUpdated` | Cập nhật cài đặt hệ thống thành công |
| `Platform.PolicyUpdated` | Cập nhật chính sách hệ thống thành công |

## 6. Gói dịch vụ (Subscription)
| Key | Nội dung (Tiếng Việt) |
|-----|----------------------|
| `Subscription.NotFound` | Không tìm thấy gói dịch vụ |
| `Subscription.ActiveNotFound` | Không tìm thấy gói dịch vụ đang hoạt động cho thương hiệu này |
| `Subscription.Created` | Đăng ký gói dịch vụ thành công |
| `Subscription.Updated` | Cập nhật gói dịch vụ thành công |
| `Subscription.PlanChanged` | Thay đổi gói dịch vụ thành công |
| `Subscription.Cancelled` | Đã hủy gói dịch vụ |
| `Subscription.CancelFailed` | Hủy gói dịch vụ thất bại |
| `Subscription.Renewed` | Gia hạn gói dịch vụ thành công |
| `Subscription.RenewFailed` | Gia hạn gói dịch vụ thất bại |
| `Subscription.InvoiceNotFound` | Không tìm thấy hóa đơn |
| `Subscription.InvoicePaid` | Thanh toán hóa đơn thành công |
| `Subscription.InvoicePayFailed` | Thanh toán hóa đơn thất bại |
| `Subscription.NewPlanNotFound` | Không tìm thấy gói cước mới |
| `Subscription.PlanLimitReached` | Đã đạt giới hạn tối đa của gói dịch vụ |
| `Subscription.CanAddHotel` | Bạn có thể thêm khách sạn mới |
| `Subscription.CanAddRoom` | Bạn có thể thêm phòng mới |
| `Subscription.CanAddUser` | Bạn có thể thêm người dùng mới |
| `Subscription.ExistingBrandNotFound` | Không tìm thấy thương hiệu hiện có |
| `Subscription.BookingNotFound` | Không tìm thấy thông tin đặt phòng |
| `Subscription.BrandNotFound` | Không tìm thấy thương hiệu |
| `Subscription.BrandCreated` | Tạo thương hiệu thành công |
| `Subscription.BrandUpdated` | Cập nhật thương hiệu thành công |
| `Subscription.BrandIdRequired` | Mã thương hiệu (Brand ID) là bắt buộc |
| `Subscription.BrandAlreadyHasSubscription` | Thương hiệu này đã có gói dịch vụ đang hoạt động |
| `Subscription.PlanNotFound` | Không tìm thấy gói cước |

## 7. Phòng (Room)
| Key | Nội dung (Tiếng Việt) |
|-----|----------------------|
| `Room.NotFound` | Không tìm thấy phòng |
| `Room.Created` | Tạo phòng thành công |
| `Room.Updated` | Cập nhật thông tin phòng thành công |
| `Room.Deleted` | Xóa phòng thành công |
| `Room.DeleteFailed` | Xóa phòng thất bại |
| `Room.StatusUpdated` | Cập nhật trạng thái phòng thành công |
| `Room.StatusUpdateFailed` | Cập nhật trạng thái phòng thất bại |
| `Room.MaintenanceReported` | Báo cáo bảo trì thành công |
| `Room.MaintenanceReportFailed` | Báo cáo bảo trì thất bại |
| `Room.MarkedAvailable` | Đã đánh dấu phòng sẵn sàng |

## 8. Hồ sơ Hợp tác (Onboarding)
| Key | Nội dung (Tiếng Việt) |
|-----|----------------------|
| `Onboarding.Created` | Tạo hồ sơ đăng ký thành công. Vui lòng hoàn tất thông tin và tải lên tài liệu cần thiết. |
| `Onboarding.NotFound` | Không tìm thấy hồ sơ đăng ký |
| `Onboarding.Updated` | Cập nhật hồ sơ đăng ký thành công |
| `Onboarding.Submitted` | Gửi hồ sơ đăng ký thành công. Chúng tôi sẽ sớm xem xét hồ sơ của bạn. |
| `Onboarding.Approved` | Hồ sơ đã được duyệt! Thương hiệu, khách sạn và gói dịch vụ đã được khởi tạo. |
| `Onboarding.DocumentUploaded` | Tải lên tài liệu thành công |
| `Onboarding.DocumentNotFound` | Không tìm thấy tài liệu |
| `Onboarding.NoPermissionUpdate` | Bạn không có quyền cập nhật hồ sơ đăng ký này |
| `Onboarding.CannotUpdateStatus` | Không thể cập nhật hồ sơ ở trạng thái hiện tại |
| `Onboarding.NoPermissionDelete` | Bạn không có quyền xóa hồ sơ đăng ký này |
| `Onboarding.OnlyDraftDelete` | Chỉ có thể xóa hồ sơ ở trạng thái nháp |
| `Onboarding.NoPermissionSubmit` | Bạn không có quyền gửi hồ sơ đăng ký này |
| `Onboarding.CannotSubmitStatus` | Không thể gửi hồ sơ ở trạng thái hiện tại |
| `Onboarding.TermsRequired` | Bạn cần phải đồng ý với các điều khoản và điều kiện |
| `Onboarding.NoPermissionDocUpload` | Bạn không có quyền tải tài liệu lên hồ sơ này |
| `Onboarding.NoPermissionDocDelete` | Bạn không có quyền xóa tài liệu này |
| `Onboarding.ApprovedDocDeleteFailed` | Không thể xóa tài liệu đã được duyệt |
| `Onboarding.OnlyReviewApprove` | Chỉ có thể duyệt hồ sơ đang trong quá trình xem xét |
| `Onboarding.SubmittedNotificationTitle` | Hồ sơ đăng ký đối tác mới |
| `Onboarding.SubmittedNotificationMessage` | Có một hồ sơ đăng ký đối tác mới vừa được nộp cho khách sạn {0} |
| `Onboarding.StatusReview` | Hồ sơ của bạn đang được đánh giá. |
| `Onboarding.StatusDocRequired` | Hồ sơ của bạn cần bổ sung thêm tài liệu. |
| `Onboarding.StatusRejected` | Hồ sơ của bạn đã bị từ chối. Lý do: {0} |
| `Onboarding.StatusUpdated` | Trạng thái hồ sơ của bạn đã được cập nhật. |
| `Onboarding.ApprovedTitle` | 🎉 Chúc mừng! Hồ sơ của bạn đã được phê duyệt |
| `Onboarding.ApprovedMessage` | Khách sạn {0} của bạn đã được phê duyệt và hiện đã chính thức hoạt động trên hệ thống! |

## 9. Khác & Hệ thống (Misc & System)
| Key | Nội dung (Tiếng Việt) |
|-----|----------------------|
| `Misc.SystemError` | Hệ thống gặp sự cố, vui lòng thử lại sau |
| `Misc.ValidationFailed` | Dữ liệu không hợp lệ |
| `Misc.AddedToWishlist` | Đã thêm vào danh sách yêu thích |
| `Misc.ItemNotFoundInWishlist` | Không tìm thấy mục này trong danh sách yêu thích |
| `Misc.WishlistNoteUpdated` | Đã cập nhật ghi chú |
| `Misc.ReviewSubmitted` | Đánh giá của bạn đã được gửi và sẽ hiển thị sau khi được duyệt |
| `Misc.ReviewNotFound` | Không tìm thấy đánh giá |
| `Misc.ReviewUpdated` | Cập nhật đánh giá thành công |
| `Misc.PromotionNotFound` | Không tìm thấy chương trình khuyến mãi |
| `Misc.PromotionCreated` | Tạo chương trình khuyến mãi thành công |
| `Misc.PromotionUpdated` | Cập nhật chương trình khuyến mãi thành công |
| `Misc.PromotionDeleted` | Đã xóa chương trình khuyến mãi |
| `Misc.PromotionExists` | Mã khuyến mãi đã tồn tại |
| `Misc.CouponInvalid` | Mã giảm giá không hợp lệ |
| `Misc.MarkedAsRead` | Đã đánh dấu là đã đọc |
| `Misc.PaymentNotFound` | Không tìm thấy thông tin thanh toán |
| `Misc.PaymentCreated` | Tạo thanh toán thành công |
| `Misc.PaymentProcessed` | Xử lý thanh toán thành công |
| `Misc.ConversationNotFound` | Không tìm thấy cuộc hội thoại |
| `Misc.HotelAlreadyInWishlist` | Khách sạn này đã có trong danh sách yêu thích |
| `Misc.InternalServerError` | Hệ thống gặp sự cố, vui lòng thử lại sau |
| `Misc.VectorDimensionMismatch` | Kích thước vector không khớp |

---
*Lưu ý: Bảng này được đồng bộ 1:1 với hằng số code. Một số thông báo nội bộ (System/Vector) có thể không xuất hiện trực tiếp cho người dùng cuối nhưng được ghi lại để debug.*
