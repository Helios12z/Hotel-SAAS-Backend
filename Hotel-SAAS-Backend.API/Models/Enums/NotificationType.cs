namespace Hotel_SAAS_Backend.API.Models.Enums
{
    public enum NotificationType
    {
        BookingConfirmation,
        BookingCancellation,
        BookingReminder,
        CheckInReminder,
        CheckOutReminder,
        PaymentReceived,
        PaymentFailed,
        ReviewRequest,
        PromotionAlert,
        SystemAlert
    }

    public enum NotificationChannel
    {
        InApp,
        Email,
        SMS,
        Push
    }

    public enum NotificationStatus
    {
        Pending,
        Sent,
        Read,
        Failed
    }
}
