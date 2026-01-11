using System.Net;
using System.Net.Mail;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.Options;
using Microsoft.Extensions.Options;

namespace Hotel_SAAS_Backend.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _options;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailOptions> options,
            IBookingRepository bookingRepository,
            ILogger<EmailService> logger)
        {
            _options = options.Value;
            _bookingRepository = bookingRepository;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                // Skip if SMTP not configured
                if (string.IsNullOrEmpty(_options.SmtpHost))
                {
                    _logger.LogWarning("SMTP not configured, skipping email send");
                    return false;
                }

                using var client = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
                {
                    Credentials = new NetworkCredential(_options.SmtpUsername, _options.SmtpPassword),
                    EnableSsl = _options.EnableSsl
                };

                var message = new MailMessage
                {
                    From = new MailAddress(_options.FromEmail, _options.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(to);

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent to {Email}", to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", to);
                return false;
            }
        }

        public async Task<bool> SendBookingConfirmationAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null || string.IsNullOrEmpty(booking.GuestEmail))
                return false;

            var subject = $"Booking Confirmed - {booking.ConfirmationNumber}";
            var body = BuildBookingConfirmationEmail(booking);

            return await SendEmailAsync(booking.GuestEmail, subject, body);
        }

        public async Task<bool> SendBookingCancellationAsync(Guid bookingId, string? reason)
        {
            var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null || string.IsNullOrEmpty(booking.GuestEmail))
                return false;

            var subject = $"Booking Cancelled - {booking.ConfirmationNumber}";
            var body = BuildBookingCancellationEmail(booking, reason);

            return await SendEmailAsync(booking.GuestEmail, subject, body);
        }

        public async Task<bool> SendCheckInReminderAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null || string.IsNullOrEmpty(booking.GuestEmail))
                return false;

            var subject = $"Check-in Reminder - {booking.Hotel?.Name}";
            var body = BuildCheckInReminderEmail(booking);

            return await SendEmailAsync(booking.GuestEmail, subject, body);
        }

        private static string BuildBookingConfirmationEmail(Models.Entities.Booking booking)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Booking Confirmed!</h2>
                <p>Dear {booking.GuestName},</p>
                <p>Your booking has been confirmed.</p>
                <div style='background: #f5f5f5; padding: 20px; margin: 20px 0;'>
                    <p><strong>Confirmation Number:</strong> {booking.ConfirmationNumber}</p>
                    <p><strong>Hotel:</strong> {booking.Hotel?.Name}</p>
                    <p><strong>Check-in:</strong> {booking.CheckInDate:MMMM dd, yyyy}</p>
                    <p><strong>Check-out:</strong> {booking.CheckOutDate:MMMM dd, yyyy}</p>
                    <p><strong>Total Amount:</strong> {booking.Currency} {booking.TotalAmount:N2}</p>
                </div>
                <p>Thank you for choosing us!</p>
            </body>
            </html>";
        }

        private static string BuildBookingCancellationEmail(Models.Entities.Booking booking, string? reason)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Booking Cancelled</h2>
                <p>Dear {booking.GuestName},</p>
                <p>Your booking has been cancelled.</p>
                <div style='background: #f5f5f5; padding: 20px; margin: 20px 0;'>
                    <p><strong>Confirmation Number:</strong> {booking.ConfirmationNumber}</p>
                    <p><strong>Hotel:</strong> {booking.Hotel?.Name}</p>
                    {(string.IsNullOrEmpty(reason) ? "" : $"<p><strong>Reason:</strong> {reason}</p>")}
                </div>
                <p>If you have any questions, please contact us.</p>
            </body>
            </html>";
        }

        private static string BuildCheckInReminderEmail(Models.Entities.Booking booking)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Check-in Reminder</h2>
                <p>Dear {booking.GuestName},</p>
                <p>This is a reminder that your check-in is coming up!</p>
                <div style='background: #f5f5f5; padding: 20px; margin: 20px 0;'>
                    <p><strong>Hotel:</strong> {booking.Hotel?.Name}</p>
                    <p><strong>Address:</strong> {booking.Hotel?.Address}, {booking.Hotel?.City}</p>
                    <p><strong>Check-in Date:</strong> {booking.CheckInDate:MMMM dd, yyyy}</p>
                    <p><strong>Check-in Time:</strong> {booking.Hotel?.CheckInTime ?? "14:00"}</p>
                </div>
                <p>We look forward to welcoming you!</p>
            </body>
            </html>";
        }
    }
}
