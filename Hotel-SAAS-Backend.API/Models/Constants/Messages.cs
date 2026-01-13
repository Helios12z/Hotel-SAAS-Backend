namespace Hotel_SAAS_Backend.API.Models.Constants
{
    public static class Messages
    {
        public static class Auth
        {
            public const string InvalidCredentials = "Email hoặc mật khẩu không đúng";
            public const string LoginSuccess = "Đăng nhập thành công";
            public const string EmailExists = "Email đã được sử dụng";
            public const string RegistrationSuccess = "Đăng ký tài khoản mới thành công";
            public const string InvalidRefreshToken = "Phiên làm việc không hợp lệ hoặc đã hết hạn";
            public const string RefreshTokenSuccess = "Làm mới mã xác thực thành công";
            public const string LogoutSuccess = "Đăng xuất thành công";
            public const string LogoutFailed = "Đăng xuất thất bại";
            public const string PasswordChangeSuccess = "Thay đổi mật khẩu thành công";
            public const string PasswordChangeFailed = "Thay đổi mật khẩu thất bại. Vui lòng kiểm tra lại mật khẩu hiện tại.";
            public const string ForgotPasswordSuccess = "Nếu email tồn tại trong hệ thống, hướng dẫn khôi phục mật khẩu sẽ được gửi đi.";
            public const string ResetPasswordSuccess = "Đặt lại mật khẩu thành công";
            public const string ResetPasswordFailed = "Đặt lại mật khẩu thất bại";
            public const string EmailRequired = "Email là bắt buộc";
        }

        public static class Booking
        {
            public const string NotFound = "Không tìm thấy thông tin đặt phòng";
            public const string Created = "Đặt phòng thành công";
            public const string Updated = "Cập nhật thông tin đặt phòng thành công";
            public const string Cancelled = "Hủy đặt phòng thành công";
            public const string CancelFailed = "Hủy đặt phòng thất bại";
            public const string Confirmed = "Xác nhận đặt phòng thành công";
            public const string ConfirmFailed = "Xác nhận đặt phòng thất bại";
            public const string CheckInSuccess = "Nhận phòng (Check-in) thành công";
            public const string CheckInFailed = "Nhận phòng thất bại";
            public const string CheckOutSuccess = "Trả phòng (Check-out) thành công";
            public const string CheckOutFailed = "Trả phòng thất bại";
            public const string RoomChanged = "Đổi phòng thành công";
            public const string LateCheckOutProcessed = "Xử lý trả phòng muộn thành công";
            public const string AdditionalChargeAdded = "Đã thêm chi phí phát sinh thành công";
            public const string AdditionalChargeRemoved = "Đã xóa chi phí phát sinh";
            public const string AdditionalChargeRemoveFailed = "Xóa chi phí phát sinh thất bại";
            public const string CannotChangeRoomStatus = "Chỉ có thể đổi phòng cho các đặt phòng đã nhận phòng";
            public const string CannotCalculateLateCheckOut = "Chỉ có thể tính phí trả phòng muộn cho các đặt phòng đã nhận phòng";
            public const string NewCheckOutTimeInvalid = "Thời gian trả phòng mới phải sau thời gian trả phòng hiện tại";
            public const string NoRoomsInBooking = "Đặt phòng này không có thông tin phòng";
            public const string OldRoomNotFound = "Không tìm thấy phòng cũ";
            public const string NewRoomNotFound = "Không tìm thấy phòng mới";
            public const string NewRoomNotAvailable = "Phòng mới hiện không khả dụng";
            public const string BookingRoomNotFound = "Không tìm thấy thông tin chi tiết phòng trong đặt phòng";
        }

        public static class Hotel
        {
            public const string NotFound = "Không tìm thấy khách sạn";
            public const string Created = "Tạo khách sạn thành công";
            public const string Updated = "Cập nhật thông tin khách sạn thành công";
            public const string Deleted = "Xóa khách sạn thành công";
            public const string DeleteFailed = "Xóa khách sạn thất bại";
            public const string InvalidDates = "Ngày trả phòng phải sau ngày nhận phòng";
            public const string CheckInPast = "Ngày nhận phòng không thể ở trong quá khứ";
            public const string BrandAdminOnly = "Chỉ Quản trị viên Thương hiệu mới có quyền thực hiện hành động này";
            public const string BrandIdRequired = "Mã thương hiệu (Brand ID) là bắt buộc";
            public const string HotelIdRequired = "Mã khách sạn (Hotel ID) là bắt buộc";
            public const string AmenityNotFound = "Không tìm thấy tiện ích";
            public const string AmenityCreated = "Tạo tiện ích thành công";
            public const string AmenityUpdated = "Cập nhật tiện ích thành công";
        }

        public static class User
        {
            public const string NotFound = "Không tìm thấy người dùng";
            public const string ProfileUpdated = "Cập nhật thông tin cá nhân thành công";
            public const string UpdateSuccess = "Cập nhật người dùng thành công";
            public const string UpdateFailed = "Cập nhật người dùng thất bại";
            public const string NoPermission = "Bạn không có quyền thực hiện hành động này";
            public const string CreatedSuccess = "Tạo người dùng thành công";
            public const string UserMismatch = "Người dùng không thuộc về đơn vị này";
            public const string ProfileNotFound = "Không tìm thấy thông tin hồ sơ khách";
            public const string PreferencesUpdated = "Cập nhật sở thích thành công";
            public const string HistoryCleared = "Đã xóa lịch sử";
            public const string SuperAdminOnlyBrandAdmin = "SuperAdmin chỉ có quyền tạo tài khoản Quản trị viên Thương hiệu (BrandAdmin)";
            public const string BrandIdRequiredForBrandAdmin = "Mã Thương hiệu (Brand ID) là bắt buộc khi tạo BrandAdmin";
            public const string BrandAdminCreated = "Tạo Quản trị viên Thương hiệu thành công";
            public const string BrandAdminOnlyHotelManager = "Quản trị viên Thương hiệu chỉ có quyền tạo tài khoản Quản lý Khách sạn (HotelManager)";
            public const string CannotCreateForDifferentBrand = "Không thể tạo người dùng cho thương hiệu khác";
            public const string HotelIdRequiredForHotelManager = "Mã Khách sạn (Hotel ID) là bắt buộc khi tạo HotelManager";
            public const string HotelManagerCreated = "Tạo Quản lý Khách sạn thành công";
            public const string HotelManagerOnlyStaff = "Quản lý Khách sạn chỉ có quyền tạo tài khoản Lễ tân hoặc Nhân viên";
            public const string CannotCreateForDifferentHotel = "Không thể tạo người dùng cho khách sạn khác";
            public const string SuperAdminOnly = "Chỉ SuperAdmin mới có quyền thực hiện hành động này";
        }

        public static class Platform
        {
            public const string BrandNotFound = "Không tìm thấy thương hiệu";
            public const string BrandCreated = "Tạo thương hiệu thành công";
            public const string BrandUpdated = "Cập nhật thương hiệu thành công";
            public const string BrandDeleted = "Xóa thương hiệu thành công";
            public const string BrandDeleteFailed = "Xóa thương hiệu thất bại";
            public const string PlanNotFound = "Không tìm thấy gói cước";
            public const string PlanCreated = "Tạo gói cước mới thành công";
            public const string PlanUpdated = "Cập nhật gói cước thành công";
            public const string PlanDeleted = "Xóa gói cước thành công";
            public const string PlanDeleteFailed = "Xóa gói cước thất bại";
            public const string PlanActivated = "Đã kích hoạt gói cước";
            public const string PlanDeactivated = "Đã tạm dừng gói cước";
            public const string SettingUpdated = "Cập nhật cài đặt hệ thống thành công";
            public const string PolicyUpdated = "Cập nhật chính sách hệ thống thành công";
        }

        public static class Subscription
        {
            public const string NotFound = "Không tìm thấy gói dịch vụ";
            public const string ActiveNotFound = "Không tìm thấy gói dịch vụ đang hoạt động cho thương hiệu này";
            public const string Created = "Đăng ký gói dịch vụ thành công";
            public const string Updated = "Cập nhật gói dịch vụ thành công";
            public const string PlanChanged = "Thay đổi gói dịch vụ thành công";
            public const string Cancelled = "Đã hủy gói dịch vụ";
            public const string CancelFailed = "Hủy gói dịch vụ thất bại";
            public const string Renewed = "Gia hạn gói dịch vụ thành công";
            public const string RenewFailed = "Gia hạn gói dịch vụ thất bại";
            public const string InvoiceNotFound = "Không tìm thấy hóa đơn";
            public const string InvoicePaid = "Thanh toán hóa đơn thành công";
            public const string InvoicePayFailed = "Thanh toán hóa đơn thất bại";
            public const string NewPlanNotFound = "Không tìm thấy gói cước mới";
            public const string PlanLimitReached = "Đã đạt giới hạn tối đa của gói dịch vụ";
            public const string CanAddHotel = "Bạn có thể thêm khách sạn mới";
            public const string CanAddRoom = "Bạn có thể thêm phòng mới";
            public const string CanAddUser = "Bạn có thể thêm người dùng mới";
            public const string ExistingBrandNotFound = "Không tìm thấy thương hiệu hiện có";
            public const string BookingNotFound = "Không tìm thấy thông tin đặt phòng"; // For StripeService
        }

        public static class Onboarding
        {
            public const string Created = "Tạo hồ sơ đăng ký thành công. Vui lòng hoàn tất thông tin và tải lên tài liệu cần thiết.";
            public const string NotFound = "Không tìm thấy hồ sơ đăng ký";
            public const string Updated = "Cập nhật hồ sơ đăng ký thành công";
            public const string Submitted = "Gửi hồ sơ đăng ký thành công. Chúng tôi sẽ sớm xem xét hồ sơ của bạn.";
            public const string Approved = "Hồ sơ đã được duyệt! Thương hiệu, khách sạn và gói dịch vụ đã được khởi tạo.";
            public const string DocumentUploaded = "Tải lên tài liệu thành công";
            public const string DocumentNotFound = "Không tìm thấy tài liệu";
            public const string NoPermissionUpdate = "Bạn không có quyền cập nhật hồ sơ đăng ký này";
            public const string CannotUpdateStatus = "Không thể cập nhật hồ sơ ở trạng thái hiện tại";
            public const string NoPermissionDelete = "Bạn không có quyền xóa hồ sơ đăng ký này";
            public const string OnlyDraftDelete = "Chỉ có thể xóa hồ sơ ở trạng thái nháp";
            public const string NoPermissionSubmit = "Bạn không có quyền gửi hồ sơ đăng ký này";
            public const string CannotSubmitStatus = "Không thể gửi hồ sơ ở trạng thái hiện tại";
            public const string TermsRequired = "Bạn cần phải đồng ý với các điều khoản và điều kiện";
            public const string NoPermissionDocUpload = "Bạn không có quyền tải tài liệu lên hồ sơ này";
            public const string NoPermissionDocDelete = "Bạn không có quyền xóa tài liệu này";
            public const string ApprovedDocDeleteFailed = "Không thể xóa tài liệu đã được duyệt";
            public const string OnlyReviewApprove = "Chỉ có thể duyệt hồ sơ đang trong quá trình xem xét";
            public const string SubmittedNotificationTitle = "Hồ sơ đăng ký đối tác mới";
            public const string SubmittedNotificationMessage = "Có một hồ sơ đăng ký đối tác mới vừa được nộp cho khách sạn {0}";
            public const string StatusReview = "Hồ sơ của bạn đang được đánh giá.";
            public const string StatusDocRequired = "Hồ sơ của bạn cần bổ sung thêm tài liệu.";
            public const string StatusRejected = "Hồ sơ của bạn đã bị từ chối. Lý do: {0}";
            public const string StatusUpdated = "Trạng thái hồ sơ của bạn đã được cập nhật.";
            public const string ApprovedTitle = "🎉 Chúc mừng! Hồ sơ của bạn đã được phê duyệt";
            public const string ApprovedMessage = "Khách sạn {0} của bạn đã được phê duyệt và hiện đã chính thức hoạt động trên hệ thống!";
        }

        public static class Misc
        {
            public const string SystemError = "Hệ thống gặp sự cố, vui lòng thử lại sau";
            public const string ValidationFailed = "Dữ liệu không hợp lệ";
            public const string AddedToWishlist = "Đã thêm vào danh sách yêu thích";
            public const string ItemNotFoundInWishlist = "Không tìm thấy mục này trong danh sách yêu thích";
            public const string WishlistNoteUpdated = "Đã cập nhật ghi chú";
            public const string ReviewSubmitted = "Đánh giá của bạn đã được gửi và sẽ hiển thị sau khi được duyệt";
            public const string ReviewNotFound = "Không tìm thấy đánh giá";
            public const string ReviewUpdated = "Cập nhật đánh giá thành công";
            public const string PromotionNotFound = "Không tìm thấy chương trình khuyến mãi";
            public const string PromotionCreated = "Tạo chương trình khuyến mãi thành công";
            public const string PromotionUpdated = "Cập nhật chương trình khuyến mãi thành công";
            public const string PromotionDeleted = "Đã xóa chương trình khuyến mãi";
            public const string PromotionExists = "Mã khuyến mãi đã tồn tại";
            public const string CouponInvalid = "Mã giảm giá không hợp lệ";
            public const string MarkedAsRead = "Đã đánh dấu là đã đọc";
            public const string PaymentNotFound = "Không tìm thấy thông tin thanh toán";
            public const string PaymentCreated = "Tạo thanh toán thành công";
            public const string PaymentProcessed = "Xử lý thanh toán thành công";
            public const string ConversationNotFound = "Không tìm thấy cuộc hội thoại";
            public const string HotelAlreadyInWishlist = "Khách sạn này đã có trong danh sách yêu thích";
            public const string InternalServerError = "Hệ thống gặp sự cố, vui lòng thử lại sau";
            public const string VectorDimensionMismatch = "Kích thước vector không khớp";
        }
    }
}
