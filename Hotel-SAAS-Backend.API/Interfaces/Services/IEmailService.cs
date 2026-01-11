namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
        Task<bool> SendBookingConfirmationAsync(Guid bookingId);
        Task<bool> SendBookingCancellationAsync(Guid bookingId, string? reason);
        Task<bool> SendCheckInReminderAsync(Guid bookingId);
    }
}
