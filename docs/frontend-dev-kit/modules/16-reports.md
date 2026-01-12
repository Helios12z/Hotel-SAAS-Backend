# Module 16: Reports & Exporting

## Tổng quan

Module Reports cung cấp các API endpoint để xuất báo cáo dưới dạng file Excel (.xlsx) và PDF. Các báo cáo bao gồm: Doanh thu, Đặt phòng, Tỷ lệ lấp đầy, và Tồn kho phòng.

---

## Authorization & Permissions

### Required Policy

| Policy | Mô tả | Roles |
|--------|-------|-------|
| `Permission:reports.revenue` | Export báo cáo doanh thu | BrandAdmin, HotelManager |
| `Permission:reports.bookings` | Export báo cáo đặt phòng | BrandAdmin, HotelManager |
| `Permission:reports.occupancy` | Export báo cáo tỷ lệ lấp đầy | BrandAdmin, HotelManager |
| `Permission:reports.inventory` | Export báo cáo tồn kho | BrandAdmin, HotelManager |
| `Permission:reports.hotel` | Export báo cáo tổng hợp khách sạn | BrandAdmin, HotelManager |

### Role-Based Access Matrix

| Action | SuperAdmin | BrandAdmin | HotelManager | Receptionist | Staff |
|--------|:----------:|:----------:|:------------:|:------------:|:-----:|
| Export Revenue | ✅ | ✅ (own) | ✅ (own) | ❌ | ❌ |
| Export Bookings | ✅ | ✅ (own) | ✅ (own) | ❌ | ❌ |
| Export Occupancy | ✅ | ✅ (own) | ✅ (own) | ❌ | ❌ |
| Export Inventory | ✅ | ✅ (own) | ✅ (own) | ❌ | ❌ |
| Export Full Hotel | ✅ | ✅ (own) | ✅ (own) | ❌ | ❌ |

> **Lưu ý:** `own` = chỉ xem được dữ liệu của brand/hotel mình quản lý

---

## API Endpoints

### 1. Export Revenue Report (Excel)

Xuất báo cáo doanh thu dưới dạng file Excel.

```http
GET /api/reports/revenue/excel
Authorization: Bearer {token}
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|:--------:|-------------|
| `hotelId` | UUID | ✅ | ID của khách sạn |
| `startDate` | Date | ✅ | Ngày bắt đầu (yyyy-MM-dd) |
| `endDate` | Date | ✅ | Ngày kết thúc (yyyy-MM-dd) |

**Response:**

| Field | Value |
|-------|-------|
| Content-Type | `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` |
| Content-Disposition | `attachment; filename="revenue_report_{hotelId}_{startDate}_{endDate}.xlsx"` |
| Body | Binary file |

**Status Codes:**

| Code | Description |
|------|-------------|
| 200 | OK - File downloaded successfully |
| 400 | Bad Request - Thiếu hoặc sai tham số |
| 401 | Unauthorized - Token không hợp lệ |
| 403 | Forbidden - Không có quyền `reports.revenue` |
| 404 | Not Found - Hotel không tồn tại |
| 500 | Internal Server Error |

---

### 2. Export Revenue Report (PDF)

Xuất báo cáo doanh thu dưới dạng PDF.

```http
GET /api/reports/revenue/pdf
Authorization: Bearer {token}
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|:--------:|-------------|
| `hotelId` | UUID | ✅ | ID của khách sạn |
| `startDate` | Date | ✅ | Ngày bắt đầu (yyyy-MM-dd) |
| `endDate` | Date | ✅ | Ngày kết thúc (yyyy-MM-dd) |

**Response:**

| Field | Value |
|-------|-------|
| Content-Type | `application/pdf` |
| Content-Disposition | `attachment; filename="revenue_report_{hotelId}_{startDate}_{endDate}.pdf"` |
| Body | Binary file |

---

### 3. Export Bookings Report (Excel)

Xuất báo cáo đặt phòng dưới dạng file Excel.

```http
GET /api/reports/bookings/excel
Authorization: Bearer {token}
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|:--------:|-------------|
| `hotelId` | UUID | ✅ | ID của khách sạn |
| `startDate` | Date | ✅ | Ngày bắt đầu (yyyy-MM-dd) |
| `endDate` | Date | ✅ | Ngày kết thúc (yyyy-MM-dd) |

**Response:**

| Field | Value |
|-------|-------|
| Content-Type | `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` |
| Content-Disposition | `attachment; filename="bookings_report_{hotelId}_{startDate}_{endDate}.xlsx"` |
| Body | Binary file |

---

### 4. Export Bookings Report (PDF)

Xuất báo cáo đặt phòng dưới dạng PDF.

```http
GET /api/reports/bookings/pdf
Authorization: Bearer {token}
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|:--------:|-------------|
| `hotelId` | UUID | ✅ | ID của khách sạn |
| `startDate` | Date | ✅ | Ngày bắt đầu (yyyy-MM-dd) |
| `endDate` | Date | ✅ | Ngày kết thúc (yyyy-MM-dd) |

**Response:**

| Field | Value |
|-------|-------|
| Content-Type | `application/pdf` |
| Content-Disposition | `attachment; filename="bookings_report_{hotelId}_{startDate}_{endDate}.pdf"` |
| Body | Binary file |

---

### 5. Export Occupancy Report (Excel)

Xuất báo cáo tỷ lệ lấp đầy dưới dạng file Excel.

```http
GET /api/reports/occupancy/excel
Authorization: Bearer {token}
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|:--------:|-------------|
| `hotelId` | UUID | ✅ | ID của khách sạn |
| `startDate` | Date | ✅ | Ngày bắt đầu (yyyy-MM-dd) |
| `endDate` | Date | ✅ | Ngày kết thúc (yyyy-MM-dd) |

**Response:**

| Field | Value |
|-------|-------|
| Content-Type | `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` |
| Content-Disposition | `attachment; filename="occupancy_report_{hotelId}_{startDate}_{endDate}.xlsx"` |
| Body | Binary file |

---

### 6. Export Occupancy Report (PDF)

Xuất báo cáo tỷ lệ lấp đầy dưới dạng PDF.

```http
GET /api/reports/occupancy/pdf
Authorization: Bearer {token}
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|:--------:|-------------|
| `hotelId` | UUID | ✅ | ID của khách sạn |
| `startDate` | Date | ✅ | Ngày bắt đầu (yyyy-MM-dd) |
| `endDate` | Date | ✅ | Ngày kết thúc (yyyy-MM-dd) |

**Response:**

| Field | Value |
|-------|-------|
| Content-Type | `application/pdf` |
| Content-Disposition | `attachment; filename="occupancy_report_{hotelId}_{startDate}_{endDate}.pdf"` |
| Body | Binary file |

---

### 7. Export Inventory Report (Excel)

Xuất báo cáo tồn kho phòng dưới dạng file Excel.

```http
GET /api/reports/inventory/excel
Authorization: Bearer {token}
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|:--------:|-------------|
| `hotelId` | UUID | ✅ | ID của khách sạn |

**Response:**

| Field | Value |
|-------|-------|
| Content-Type | `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` |
| Content-Disposition | `attachment; filename="inventory_report_{hotelId}_{yyyyMMdd}.xlsx"` |
| Body | Binary file |

---

### 8. Export Full Hotel Report (PDF)

Xuất báo cáo tổng hợp khách sạn dưới dạng PDF.

```http
GET /api/reports/hotel/full
Authorization: Bearer {token}
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|:--------:|-------------|
| `hotelId` | UUID | ✅ | ID của khách sạn |
| `startDate` | Date | ✅ | Ngày bắt đầu (yyyy-MM-dd) |
| `endDate` | Date | ✅ | Ngày kết thúc (yyyy-MM-dd) |

**Response:**

| Field | Value |
|-------|-------|
| Content-Type | `application/pdf` |
| Content-Disposition | `attachment; filename="hotel_report_{hotelId}_{startDate}_{endDate}.pdf"` |
| Body | Binary file |

---

## Quick Reference

| # | Endpoint | Format | Required Permission | Filename |
|---|----------|--------|:-------------------|----------|
| 1 | `GET /api/reports/revenue/excel` | XLSX | `reports.revenue` | `revenue_report_{hotelId}_{startDate}_{endDate}.xlsx` |
| 2 | `GET /api/reports/revenue/pdf` | PDF | `reports.revenue` | `revenue_report_{hotelId}_{startDate}_{endDate}.pdf` |
| 3 | `GET /api/reports/bookings/excel` | XLSX | `reports.bookings` | `bookings_report_{hotelId}_{startDate}_{endDate}.xlsx` |
| 4 | `GET /api/reports/bookings/pdf` | PDF | `reports.bookings` | `bookings_report_{hotelId}_{startDate}_{endDate}.pdf` |
| 5 | `GET /api/reports/occupancy/excel` | XLSX | `reports.occupancy` | `occupancy_report_{hotelId}_{startDate}_{endDate}.xlsx` |
| 6 | `GET /api/reports/occupancy/pdf` | PDF | `reports.occupancy` | `occupancy_report_{hotelId}_{startDate}_{endDate}.pdf` |
| 7 | `GET /api/reports/inventory/excel` | XLSX | `reports.inventory` | `inventory_report_{hotelId}_{date}.xlsx` |
| 8 | `GET /api/reports/hotel/full` | PDF | `reports.hotel` | `hotel_report_{hotelId}_{startDate}_{endDate}.pdf` |

---

## Excel Report Structures

### 1. Revenue Report (Sheet: "Revenue Report")

| Column | Header | Data Type | Description |
|--------|--------|-----------|-------------|
| A | Booking ID | String | UUID truncated (8 chars) |
| B | Confirmation | String | Mã xác nhận booking |
| C | Guest Name | String | Tên khách |
| D | Check-in | Date (yyyy-MM-dd) | Ngày nhận phòng |
| E | Check-out | Date (yyyy-MM-dd) | Ngày trả phòng |
| F | Rooms | Integer | Số lượng phòng |
| G | Total | Currency ($#,##0.00) | Tổng tiền |
| H | Paid | Currency ($#,##0.00) | Đã thanh toán |
| I | Status | String | Trạng thái booking |

**Summary Section:**

| Cell | Content |
|------|---------|
| F (last row + 1) | "TOTAL REVENUE:" |
| G (last row + 1) | Sum của Total column |
| F (last row + 2) | "TOTAL PAID:" |
| G (last row + 2) | Sum của Paid column |

---

### 2. Bookings Report (Sheet: "Bookings Report")

| Column | Header | Data Type | Description |
|--------|--------|-----------|-------------|
| A | Booking ID | String | UUID truncated (8 chars) |
| B | Confirmation | String | Mã xác nhận booking |
| C | Guest Name | String | Tên khách |
| D | Email | String | Email khách |
| E | Phone | String | Số điện thoại |
| F | Check-in | Date (yyyy-MM-dd) | Ngày nhận phòng |
| G | Check-out | Date (yyyy-MM-dd) | Ngày trả phòng |
| H | Nights | Integer | Số đêm |
| I | Rooms | Integer | Số phòng |
| J | Amount | Currency ($#,##0.00) | Tổng tiền |
| K | Status | String | Trạng thái booking |

---

### 3. Occupancy Report (Sheet: "Occupancy Report")

| Column | Header | Data Type | Description |
|--------|--------|-----------|-------------|
| A | Room Number | String | Số phòng |
| B | Type | String | Loại phòng (Single, Double, Suite...) |
| C | Base Price | Currency ($#,##0.00) | Giá cơ bản |
| D | Bookings | Integer | Số booking |
| E | Occupancy % | String | Tỷ lệ lấp đầy (format: "xx.x%") |
| F | Revenue | Currency ($#,##0.00) | Doanh thu phòng |

---

### 4. Inventory Report (Sheet: "Inventory Report")

| Column | Header | Data Type | Description |
|--------|--------|-----------|-------------|
| A | Room Number | String | Số phòng |
| B | Type | String | Loại phòng |
| C | Floor | Integer | Tầng |
| D | Base Price | Currency ($#,##0.00) | Giá cơ bản |
| E | Status | String | Trạng thái (Available, Occupied, Maintenance...) |
| F | Max Guests | Integer | Số khách tối đa |
| G | Amenities | String | Danh sách tiện ích (comma-separated) |

---

## PDF Report Structures

### 1. Revenue Report PDF

```
+--------------------------------------------------+
|                    HEADER                        |
|              REVENUE REPORT                      |
|            Hotel Name (nếu có)                  |
+--------------------------------------------------+
| Period: yyyy-MM-dd to yyyy-MM-dd                |
| Total Bookings: N                                |
+--------------------------------------------------+
|                  DATA TABLE                      |
| Confirmation | Guest | Check-in | ... | Amount  |
+--------------------------------------------------+
|                  SUMMARY                         |
| Total Revenue: $X,XXX,XXX.XX                     |
| Total Paid:    $X,XXX,XXX.XX                     |
+--------------------------------------------------+
| Footer: Generated: yyyy-MM-dd HH:mm:ss UTC      |
|         Page X of Y                             |
+--------------------------------------------------+
```

---

### 2. Bookings Report PDF

```
+--------------------------------------------------+
|                    HEADER                        |
|                BOOKINGS REPORT                   |
|            Hotel Name (nếu có)                  |
+--------------------------------------------------+
| Period: yyyy-MM-dd to yyyy-MM-dd                |
| Total Bookings: N                                |
+--------------------------------------------------+
|                  DATA TABLE                      |
| Confirmation | Guest Name | Dates | ... | Status |
+--------------------------------------------------+
```

---

### 3. Occupancy Report PDF

```
+--------------------------------------------------+
|                    HEADER                        |
|               OCCUPANCY REPORT                   |
|            Hotel Name (nếu có)                  |
+--------------------------------------------------+
| Period: yyyy-MM-dd to yyyy-MM-dd                |
| Total Rooms: N                                   |
+--------------------------------------------------+
|                  DATA TABLE                      |
| Room Number | Type | Bookings | Status           |
+--------------------------------------------------+
```

---

### 4. Full Hotel Report PDF

```
+--------------------------------------------------+
|               HOTEL SUMMARY REPORT               |
|            Hotel Name (nếu có)                  |
+--------------------------------------------------+
| Report Period: yyyy-MM-dd to yyyy-MM-dd         |
+--------------------------------------------------+
|                  SUMMARY                         |
| Total Rooms: N     | Total Bookings: N          |
| Revenue: $XXX      | Avg Occupancy: XX.X%       |
+--------------------------------------------------+
|               ROOM INVENTORY                     |
| Number | Type | Floor | Status                   |
+--------------------------------------------------+
| Footer: Generated: yyyy-MM-dd HH:mm:ss UTC      |
|         Page X of Y                             |
+--------------------------------------------------+
```

---

## Error Handling

### Error Response Format

```json
{
  "success": false,
  "message": "Error description",
  "errors": null
}
```

### Error Codes

| HTTP | Message | Nguyên nhân |
|------|---------|-------------|
| 400 | "Invalid parameters" | Thiếu hotelId, startDate, hoặc endDate |
| 401 | "Unauthorized" | Token không hợp lệ hoặc hết hạn |
| 403 | "Forbidden" | Không có permission cần thiết |
| 404 | "Hotel not found" | HotelId không tồn tại |
| 500 | "Internal server error" | Lỗi server khi generate report |

---

## Related Documentation

- [Module 10: Dashboard](../modules/10-dashboard.md) - Dashboard analytics
- [Module 13: Admin Portal](../modules/13-admin-portal.md) - Admin permissions
- [DYNAMIC_CONFIGURATION.md](../../DYNAMIC_CONFIGURATION.md) - Permission system
