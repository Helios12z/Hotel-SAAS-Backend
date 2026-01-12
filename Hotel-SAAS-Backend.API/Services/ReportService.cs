using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Hotel_SAAS_Backend.API.Services
{
    public class ReportService(ApplicationDbContext context) : IReportService
    {
        public async Task<byte[]> GenerateRevenueReportExcelAsync(Guid hotelId, DateTime startDate, DateTime endDate)
        {
            var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId);
            var bookings = await context.Bookings
                .Include(b => b.Payments)
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                .Where(b => b.HotelId == hotelId &&
                            b.CheckOutDate >= startDate &&
                            b.CheckOutDate <= endDate &&
                            !b.IsDeleted &&
                            (b.Status == BookingStatus.CheckedOut || b.Status == BookingStatus.Confirmed))
                .OrderByDescending(b => b.CheckOutDate)
                .ToListAsync();

            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Revenue Report");

            // Header
            worksheet.Cells["A1"].Value = "REVENUE REPORT";
            worksheet.Cells["A1:I1"].Merge = true;
            worksheet.Cells["A1"].Style.Font.Size = 18;
            worksheet.Cells["A1"].Style.Font.Bold = true;

            worksheet.Cells["A2"].Value = $"Hotel: {hotel?.Name ?? "N/A"}";
            worksheet.Cells["A3"].Value = $"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}";
            worksheet.Cells["A4"].Value = $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";

            // Column headers
            var headers = new[] { "Booking ID", "Confirmation", "Guest Name", "Check-in", "Check-out", "Rooms", "Total", "Paid", "Status" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[6, i + 1].Value = headers[i];
                worksheet.Cells[6, i + 1].Style.Font.Bold = true;
                worksheet.Cells[6, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[6, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            decimal totalRevenue = 0;
            decimal totalPaid = 0;

            // Data rows
            for (int i = 0; i < bookings.Count; i++)
            {
                var booking = bookings[i];
                var row = i + 7;
                worksheet.Cells[row, 1].Value = booking.Id.ToString()[..8];
                worksheet.Cells[row, 2].Value = booking.ConfirmationNumber;
                worksheet.Cells[row, 3].Value = booking.GuestName;
                worksheet.Cells[row, 4].Value = booking.CheckInDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 5].Value = booking.CheckOutDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 6].Value = booking.BookingRooms.Count;
                worksheet.Cells[row, 7].Value = (double)booking.TotalAmount;
                worksheet.Cells[row, 7].Style.Numberformat.Format = "$#,##0.00";

                var paidAmount = booking.Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
                worksheet.Cells[row, 8].Value = (double)paidAmount;
                worksheet.Cells[row, 8].Style.Numberformat.Format = "$#,##0.00";

                worksheet.Cells[row, 9].Value = booking.Status.ToString();

                totalRevenue += booking.TotalAmount;
                totalPaid += paidAmount;
            }

            // Summary
            var summaryRow = bookings.Count + 8;
            worksheet.Cells[summaryRow, 6].Value = "TOTAL REVENUE:";
            worksheet.Cells[summaryRow, 6].Style.Font.Bold = true;
            worksheet.Cells[summaryRow, 7].Value = (double)totalRevenue;
            worksheet.Cells[summaryRow, 7].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[summaryRow, 7].Style.Font.Bold = true;

            worksheet.Cells[summaryRow + 1, 6].Value = "TOTAL PAID:";
            worksheet.Cells[summaryRow + 1, 6].Style.Font.Bold = true;
            worksheet.Cells[summaryRow + 1, 7].Value = (double)totalPaid;
            worksheet.Cells[summaryRow + 1, 7].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[summaryRow + 1, 7].Style.Font.Bold = true;

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }

        public async Task<byte[]> GenerateRevenueReportPdfAsync(Guid hotelId, DateTime startDate, DateTime endDate)
        {
            var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId);
            var bookings = await context.Bookings
                .Include(b => b.Payments)
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                .Where(b => b.HotelId == hotelId &&
                            b.CheckOutDate >= startDate &&
                            b.CheckOutDate <= endDate &&
                            !b.IsDeleted &&
                            (b.Status == BookingStatus.CheckedOut || b.Status == BookingStatus.Confirmed))
                .OrderByDescending(b => b.CheckOutDate)
                .ToListAsync();

            var totalRevenue = bookings.Sum(b => b.TotalAmount);
            var totalPaid = bookings.SelectMany(b => b.Payments).Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.Header().Element(header => ComposeHeader(header, "REVENUE REPORT", hotel?.Name));
                    page.Content().Element(content => ComposeRevenueContent(content, bookings, startDate, endDate, totalRevenue, totalPaid));
                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerateBookingsReportExcelAsync(Guid hotelId, DateTime startDate, DateTime endDate)
        {
            var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId);
            var bookings = await context.Bookings
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                .Where(b => b.HotelId == hotelId &&
                            b.CreatedAt >= startDate &&
                            b.CreatedAt <= endDate &&
                            !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Bookings Report");

            // Header
            worksheet.Cells["A1"].Value = "BOOKINGS REPORT";
            worksheet.Cells["A1:K1"].Merge = true;
            worksheet.Cells["A1"].Style.Font.Size = 18;
            worksheet.Cells["A1"].Style.Font.Bold = true;

            worksheet.Cells["A2"].Value = $"Hotel: {hotel?.Name ?? "N/A"}";
            worksheet.Cells["A3"].Value = $"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}";

            // Column headers
            var headers = new[] { "Booking ID", "Confirmation", "Guest Name", "Email", "Phone", "Check-in", "Check-out", "Nights", "Rooms", "Amount", "Status" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[5, i + 1].Value = headers[i];
                worksheet.Cells[5, i + 1].Style.Font.Bold = true;
                worksheet.Cells[5, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[5, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
            }

            int row = 6;
            foreach (var booking in bookings)
            {
                worksheet.Cells[row, 1].Value = booking.Id.ToString()[..8];
                worksheet.Cells[row, 2].Value = booking.ConfirmationNumber;
                worksheet.Cells[row, 3].Value = booking.GuestName;
                worksheet.Cells[row, 4].Value = booking.GuestEmail;
                worksheet.Cells[row, 5].Value = booking.GuestPhoneNumber;
                worksheet.Cells[row, 6].Value = booking.CheckInDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 7].Value = booking.CheckOutDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 8].Value = (booking.CheckOutDate - booking.CheckInDate).Days;
                worksheet.Cells[row, 9].Value = booking.BookingRooms.Count;
                worksheet.Cells[row, 10].Value = (double)booking.TotalAmount;
                worksheet.Cells[row, 10].Style.Numberformat.Format = "$#,##0.00";
                worksheet.Cells[row, 11].Value = booking.Status.ToString();
                row++;
            }

            // Summary
            worksheet.Cells[row, 9].Value = "TOTAL BOOKINGS:";
            worksheet.Cells[row, 9].Style.Font.Bold = true;
            worksheet.Cells[row, 10].Value = bookings.Count;

            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        public async Task<byte[]> GenerateBookingsReportPdfAsync(Guid hotelId, DateTime startDate, DateTime endDate)
        {
            var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId);
            var bookings = await context.Bookings
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                .Where(b => b.HotelId == hotelId &&
                            b.CreatedAt >= startDate &&
                            b.CreatedAt <= endDate &&
                            !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.Header().Element(header => ComposeHeader(header, "BOOKINGS REPORT", hotel?.Name));
                    page.Content().Element(content => ComposeBookingsContent(content, bookings, startDate, endDate));
                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerateOccupancyReportExcelAsync(Guid hotelId, DateTime startDate, DateTime endDate)
        {
            var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId);
            var rooms = await context.Rooms.Where(r => r.HotelId == hotelId && !r.IsDeleted).ToListAsync();
            var bookings = await context.Bookings
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                .Where(b => b.HotelId == hotelId &&
                            !b.IsDeleted &&
                            b.Status != BookingStatus.Cancelled &&
                            b.Status != BookingStatus.NoShow)
                .ToListAsync();

            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Occupancy Report");

            // Header
            worksheet.Cells["A1"].Value = "OCCUPANCY REPORT";
            worksheet.Cells["A1:F1"].Merge = true;
            worksheet.Cells["A1"].Style.Font.Size = 18;
            worksheet.Cells["A1"].Style.Font.Bold = true;

            worksheet.Cells["A2"].Value = $"Hotel: {hotel?.Name ?? "N/A"}";
            worksheet.Cells["A3"].Value = $"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}";
            worksheet.Cells["A4"].Value = $"Total Rooms: {rooms.Count}";

            // Room details
            var headers = new[] { "Room Number", "Type", "Base Price", "Bookings", "Occupancy %", "Revenue" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[6, i + 1].Value = headers[i];
                worksheet.Cells[6, i + 1].Style.Font.Bold = true;
                worksheet.Cells[6, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[6, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
            }

            int row = 7;
            decimal totalRevenue = 0;
            int totalBookings = 0;
            int totalDays = (endDate - startDate).Days;

            foreach (var room in rooms)
            {
                var roomBookings = bookings.Where(b => b.BookingRooms.Any(br => br.RoomId == room.Id)).ToList();
                var roomNights = roomBookings.Sum(b => (b.CheckOutDate - b.CheckInDate).Days);
                var occupancyRate = totalDays > 0 ? (double)roomNights / totalDays * 100 : 0;
                var roomRevenue = roomBookings.Sum(b => b.BookingRooms.Where(br => br.RoomId == room.Id).Sum(br => br.Price));

                worksheet.Cells[row, 1].Value = room.RoomNumber;
                worksheet.Cells[row, 2].Value = room.Type.ToString();
                worksheet.Cells[row, 3].Value = (double)room.BasePrice;
                worksheet.Cells[row, 3].Style.Numberformat.Format = "$#,##0.00";
                worksheet.Cells[row, 4].Value = roomBookings.Count;
                worksheet.Cells[row, 5].Value = $"{occupancyRate:F1}%";
                worksheet.Cells[row, 6].Value = (double)roomRevenue;
                worksheet.Cells[row, 6].Style.Numberformat.Format = "$#,##0.00";

                totalRevenue += roomRevenue;
                totalBookings += roomBookings.Count;
                row++;
            }

            // Summary
            worksheet.Cells[row, 1].Value = "TOTAL";
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 4].Value = totalBookings;
            worksheet.Cells[row, 4].Style.Font.Bold = true;
            worksheet.Cells[row, 6].Value = (double)totalRevenue;
            worksheet.Cells[row, 6].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[row, 6].Style.Font.Bold = true;

            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        public async Task<byte[]> GenerateOccupancyReportPdfAsync(Guid hotelId, DateTime startDate, DateTime endDate)
        {
            var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId);
            var rooms = await context.Rooms.Where(r => r.HotelId == hotelId && !r.IsDeleted).ToListAsync();
            var bookings = await context.Bookings
                .Include(b => b.BookingRooms)
                .Where(b => b.HotelId == hotelId && !b.IsDeleted && b.Status != BookingStatus.Cancelled)
                .ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.Header().Element(header => ComposeHeader(header, "OCCUPANCY REPORT", hotel?.Name));
                    page.Content().Element(content => ComposeOccupancyContent(content, rooms, bookings, startDate, endDate));
                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerateInventoryReportExcelAsync(Guid hotelId)
        {
            var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId);
            var rooms = await context.Rooms
                .Include(r => r.Amenities)
                    .ThenInclude(ra => ra.Amenity)
                .Where(r => r.HotelId == hotelId && !r.IsDeleted)
                .ToListAsync();

            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Inventory Report");

            worksheet.Cells["A1"].Value = "INVENTORY REPORT";
            worksheet.Cells["A1:G1"].Merge = true;
            worksheet.Cells["A1"].Style.Font.Size = 18;
            worksheet.Cells["A1"].Style.Font.Bold = true;

            worksheet.Cells["A2"].Value = $"Hotel: {hotel?.Name ?? "N/A"}";
            worksheet.Cells["A3"].Value = $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";

            var headers = new[] { "Room Number", "Type", "Floor", "Base Price", "Status", "Max Guests", "Amenities" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[5, i + 1].Value = headers[i];
                worksheet.Cells[5, i + 1].Style.Font.Bold = true;
                worksheet.Cells[5, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[5, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCoral);
            }

            int row = 6;
            foreach (var room in rooms)
            {
                worksheet.Cells[row, 1].Value = room.RoomNumber;
                worksheet.Cells[row, 2].Value = room.Type.ToString();
                worksheet.Cells[row, 3].Value = room.Floor;
                worksheet.Cells[row, 4].Value = (double)room.BasePrice;
                worksheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00";
                worksheet.Cells[row, 5].Value = room.Status.ToString();
                worksheet.Cells[row, 6].Value = room.MaxOccupancy;
                worksheet.Cells[row, 7].Value = string.Join(", ", room.Amenities.Select(a => a.Amenity?.Name));
                row++;
            }

            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        public async Task<byte[]> GenerateFullHotelReportPdfAsync(Guid hotelId, DateTime startDate, DateTime endDate)
        {
            var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId);
            var rooms = await context.Rooms.Where(r => r.HotelId == hotelId && !r.IsDeleted).ToListAsync();
            var bookings = await context.Bookings
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                .Include(b => b.Payments)
                .Where(b => b.HotelId == hotelId && !b.IsDeleted)
                .ToListAsync();

            var periodBookings = bookings.Where(b => b.CreatedAt >= startDate && b.CreatedAt <= endDate).ToList();
            var totalRevenue = periodBookings.Sum(b => b.TotalAmount);
            var avgOccupancy = rooms.Any() ? (double)periodBookings.Sum(b => (b.CheckOutDate - b.CheckInDate).Days) / rooms.Count / (endDate - startDate).Days * 100 : 0;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.Header().Element(header => ComposeHeader(header, "HOTEL SUMMARY REPORT", hotel?.Name));
                    page.Content().Element(content => ComposeFullReportContent(content, hotel, rooms, periodBookings, totalRevenue, avgOccupancy, startDate, endDate));
                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        #region PDF Helpers

        private static void ComposeHeader(IContainer container, string title, string? hotelName)
        {
            container.Column(column =>
            {
                column.Item().Text(title).FontSize(24).Bold().AlignCenter();
                if (!string.IsNullOrEmpty(hotelName))
                {
                    column.Item().Text(hotelName).FontSize(14).AlignCenter();
                }
                column.Item().PaddingTop(10).LineHorizontal(1);
            });
        }

        private static void ComposeRevenueContent(IContainer container, List<Booking> bookings, DateTime start, DateTime end, decimal total, decimal paid)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Item().Text($"Period: {start:yyyy-MM-dd} to {end:yyyy-MM-dd}").FontSize(10);
                column.Item().Text($"Total Bookings: {bookings.Count}").FontSize(10);
                column.Item().PaddingTop(5);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Cell().Element(CellStyle).Text("Confirmation");
                    table.Cell().Element(CellStyle).Text("Guest");
                    table.Cell().Element(CellStyle).Text("Check-in");
                    table.Cell().Element(CellStyle).Text("Check-out");
                    table.Cell().Element(CellStyle).Text("Amount");
                    table.Cell().Element(CellStyle).Text("Status");

                    foreach (var booking in bookings.Take(50))
                    {
                        table.Cell().Text(booking.ConfirmationNumber);
                        table.Cell().Text(booking.GuestName ?? "N/A");
                        table.Cell().Text(booking.CheckInDate.ToString("yyyy-MM-dd"));
                        table.Cell().Text(booking.CheckOutDate.ToString("yyyy-MM-dd"));
                        table.Cell().Text($"${booking.TotalAmount:N2}");
                        table.Cell().Text(booking.Status.ToString());
                    }
                });

                column.Item().PaddingTop(20).Row(row =>
                {
                    row.RelativeItem().Text($"Total Revenue: ${total:N2}").FontSize(14).Bold();
                    row.RelativeItem().Text($"Total Paid: ${paid:N2}").FontSize(14).Bold();
                });
            });
        }

        private static void ComposeBookingsContent(IContainer container, List<Booking> bookings, DateTime start, DateTime end)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Item().Text($"Period: {start:yyyy-MM-dd} to {end:yyyy-MM-dd}").FontSize(10);
                column.Item().Text($"Total Bookings: {bookings.Count}").FontSize(10);

                column.Item().PaddingTop(5).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Cell().Element(CellStyle).Text("Confirmation");
                    table.Cell().Element(CellStyle).Text("Guest Name");
                    table.Cell().Element(CellStyle).Text("Dates");
                    table.Cell().Element(CellStyle).Text("Rooms");
                    table.Cell().Element(CellStyle).Text("Amount");
                    table.Cell().Element(CellStyle).Text("Status");

                    foreach (var booking in bookings.Take(50))
                    {
                        table.Cell().Text(booking.ConfirmationNumber);
                        table.Cell().Text(booking.GuestName ?? "N/A");
                        table.Cell().Text($"{booking.CheckInDate:MM/dd} - {booking.CheckOutDate:MM/dd}");
                        table.Cell().Text(booking.BookingRooms.Count.ToString());
                        table.Cell().Text($"${booking.TotalAmount:N2}");
                        table.Cell().Text(booking.Status.ToString());
                    }
                });
            });
        }

        private static void ComposeOccupancyContent(IContainer container, List<Room> rooms, List<Booking> bookings, DateTime start, DateTime end)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Item().Text($"Period: {start:yyyy-MM-dd} to {end:yyyy-MM-dd}").FontSize(10);
                column.Item().Text($"Total Rooms: {rooms.Count}").FontSize(10);

                column.Item().PaddingTop(5).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Cell().Element(CellStyle).Text("Room Number");
                    table.Cell().Element(CellStyle).Text("Type");
                    table.Cell().Element(CellStyle).Text("Bookings");
                    table.Cell().Element(CellStyle).Text("Status");

                    foreach (var room in rooms)
                    {
                        var roomBookings = bookings.Where(b => b.BookingRooms.Any(br => br.RoomId == room.Id)).ToList();
                        table.Cell().Text(room.RoomNumber);
                        table.Cell().Text(room.Type.ToString());
                        table.Cell().Text(roomBookings.Count.ToString());
                        table.Cell().Text(room.Status.ToString());
                    }
                });
            });
        }

        private static void ComposeFullReportContent(IContainer container, Hotel? hotel, List<Room> rooms, List<Booking> bookings, decimal revenue, double occupancy, DateTime start, DateTime end)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Item().Text($"Report Period: {start:yyyy-MM-dd} to {end:yyyy-MM-dd}").FontSize(10);
                column.Item().PaddingTop(10).Text("SUMMARY").FontSize(14).Bold();

                column.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text($"Total Rooms: {rooms.Count}");
                    row.RelativeItem().Text($"Total Bookings: {bookings.Count}");
                    row.RelativeItem().Text($"Revenue: ${revenue:N2}");
                    row.RelativeItem().Text($"Avg Occupancy: {occupancy:F1}%");
                });

                column.Item().PaddingTop(15).Text("ROOM INVENTORY").FontSize(14).Bold();
                column.Item().PaddingTop(5).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(2);
                    });

                    table.Cell().Element(CellStyle).Text("Number");
                    table.Cell().Element(CellStyle).Text("Type");
                    table.Cell().Element(CellStyle).Text("Floor");
                    table.Cell().Element(CellStyle).Text("Status");

                    foreach (var room in rooms)
                    {
                        table.Cell().Text(room.RoomNumber);
                        table.Cell().Text(room.Type.ToString());
                        table.Cell().Text(room.Floor ?? "");
                        table.Cell().Text(room.Status.ToString());
                    }
                });
            });
        }

        private static void ComposeFooter(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Text(x =>
                {
                    x.Span("Generated: ").FontSize(8);
                    x.Span($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC").FontSize(8);
                });
                row.RelativeItem().AlignRight().Text(x =>
                {
                    x.Span("Page ").FontSize(8);
                    x.CurrentPageNumber().FontSize(8);
                    x.Span(" of ").FontSize(8);
                    x.TotalPages().FontSize(8);
                });
            });
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container.Background("#f0f0f0").Padding(5).Border(1).BorderColor("#ccc");
        }

        #endregion
    }
}
