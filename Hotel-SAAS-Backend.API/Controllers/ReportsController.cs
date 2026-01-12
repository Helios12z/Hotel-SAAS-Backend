using Hotel_SAAS_Backend.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Export revenue report as Excel
        /// </summary>
        [HttpGet("revenue/excel")]
        [Authorize(Policy = "Permission:reports.revenue")]
        public async Task<IActionResult> ExportRevenueExcel(
            [FromQuery] Guid hotelId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var bytes = await _reportService.GenerateRevenueReportExcelAsync(hotelId, startDate, endDate);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"revenue_report_{hotelId}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx");
        }

        /// <summary>
        /// Export revenue report as PDF
        /// </summary>
        [HttpGet("revenue/pdf")]
        [Authorize(Policy = "Permission:reports.revenue")]
        public async Task<IActionResult> ExportRevenuePdf(
            [FromQuery] Guid hotelId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var bytes = await _reportService.GenerateRevenueReportPdfAsync(hotelId, startDate, endDate);
            return File(bytes, "application/pdf",
                $"revenue_report_{hotelId}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf");
        }

        /// <summary>
        /// Export bookings report as Excel
        /// </summary>
        [HttpGet("bookings/excel")]
        [Authorize(Policy = "Permission:reports.bookings")]
        public async Task<IActionResult> ExportBookingsExcel(
            [FromQuery] Guid hotelId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var bytes = await _reportService.GenerateBookingsReportExcelAsync(hotelId, startDate, endDate);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"bookings_report_{hotelId}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx");
        }

        /// <summary>
        /// Export bookings report as PDF
        /// </summary>
        [HttpGet("bookings/pdf")]
        [Authorize(Policy = "Permission:reports.bookings")]
        public async Task<IActionResult> ExportBookingsPdf(
            [FromQuery] Guid hotelId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var bytes = await _reportService.GenerateBookingsReportPdfAsync(hotelId, startDate, endDate);
            return File(bytes, "application/pdf",
                $"bookings_report_{hotelId}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf");
        }

        /// <summary>
        /// Export occupancy report as Excel
        /// </summary>
        [HttpGet("occupancy/excel")]
        [Authorize(Policy = "Permission:reports.occupancy")]
        public async Task<IActionResult> ExportOccupancyExcel(
            [FromQuery] Guid hotelId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var bytes = await _reportService.GenerateOccupancyReportExcelAsync(hotelId, startDate, endDate);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"occupancy_report_{hotelId}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx");
        }

        /// <summary>
        /// Export occupancy report as PDF
        /// </summary>
        [HttpGet("occupancy/pdf")]
        [Authorize(Policy = "Permission:reports.occupancy")]
        public async Task<IActionResult> ExportOccupancyPdf(
            [FromQuery] Guid hotelId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var bytes = await _reportService.GenerateOccupancyReportPdfAsync(hotelId, startDate, endDate);
            return File(bytes, "application/pdf",
                $"occupancy_report_{hotelId}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf");
        }

        /// <summary>
        /// Export inventory report as Excel
        /// </summary>
        [HttpGet("inventory/excel")]
        [Authorize(Policy = "Permission:reports.inventory")]
        public async Task<IActionResult> ExportInventoryExcel([FromQuery] Guid hotelId)
        {
            var bytes = await _reportService.GenerateInventoryReportExcelAsync(hotelId);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"inventory_report_{hotelId}_{DateTime.UtcNow:yyyyMMdd}.xlsx");
        }

        /// <summary>
        /// Export full hotel report as PDF
        /// </summary>
        [HttpGet("hotel/full")]
        [Authorize(Policy = "Permission:reports.hotel")]
        public async Task<IActionResult> ExportFullHotelReport(
            [FromQuery] Guid hotelId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var bytes = await _reportService.GenerateFullHotelReportPdfAsync(hotelId, startDate, endDate);
            return File(bytes, "application/pdf",
                $"hotel_report_{hotelId}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf");
        }
    }
}
