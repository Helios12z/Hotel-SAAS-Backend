using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IReportService
    {
        /// <summary>
        /// Generate revenue report as Excel file
        /// </summary>
        Task<byte[]> GenerateRevenueReportExcelAsync(Guid hotelId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Generate revenue report as PDF file
        /// </summary>
        Task<byte[]> GenerateRevenueReportPdfAsync(Guid hotelId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Generate bookings report as Excel file
        /// </summary>
        Task<byte[]> GenerateBookingsReportExcelAsync(Guid hotelId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Generate bookings report as PDF file
        /// </summary>
        Task<byte[]> GenerateBookingsReportPdfAsync(Guid hotelId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Generate occupancy report as Excel file
        /// </summary>
        Task<byte[]> GenerateOccupancyReportExcelAsync(Guid hotelId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Generate occupancy report as PDF file
        /// </summary>
        Task<byte[]> GenerateOccupancyReportPdfAsync(Guid hotelId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Generate inventory report as Excel file
        /// </summary>
        Task<byte[]> GenerateInventoryReportExcelAsync(Guid hotelId);

        /// <summary>
        /// Generate full hotel report as PDF
        /// </summary>
        Task<byte[]> GenerateFullHotelReportPdfAsync(Guid hotelId, DateTime startDate, DateTime endDate);
    }

    public class ReportFilterDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? HotelId { get; set; }
        public Guid? BrandId { get; set; }
        public string? Format { get; set; } = "excel"; // excel or pdf
    }
}
