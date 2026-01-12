# Module 09: Booking Management (Partner Portal)

## Authorization & Permissions

> **Hệ thống phân quyền động**: Sử dụng `[Authorize(Policy = "Permission:xxx")]` thay vì role-based cứng

### Permission Policies

| Policy | Mô tả | Cho phép |
|--------|-------|----------|
| `Permission:bookings.read` | Xem bookings | Tất cả staff của hotel |
| `Permission:bookings.create` | Tạo booking (walk-in) | HotelManager |
| `Permission:bookings.update` | Sửa booking | HotelManager |
| `Permission:bookings.delete` | Hủy booking | HotelManager |
| `Permission:bookings.checkin` | Check-in khách | HotelManager, Receptionist |
| `Permission:bookings.checkout` | Check-out khách | HotelManager, Receptionist |

### Role-Based Access Matrix

| Action | SuperAdmin | BrandAdmin | HotelManager | Receptionist | Staff |
|--------|------------|------------|--------------|--------------|-------|
| Xem bookings | Read (all) | Read (own) | Read (own) | Read (own) | Read (own) |
| Tạo booking | ❌ | ❌ | ✅ | ❌ | ❌ |
| Check-in | ❌ | ❌ | ✅ | ✅ | ❌ |
| Check-out | ❌ | ❌ | ✅ | ✅ | ❌ |
| Hủy booking | ❌ | ❌ | ✅ | ❌ | ❌ |

### JWT Claims

Token chứa các claims liên quan đến permissions:
```json
{
  "permissions": "hotels.read,bookings.read,bookings.checkin,...",
  "scope": "hotel",
  "hotelId": "uuid-cua-hotel-duoc-assign",
  "brandId": "uuid-cua-brand-neu-co"
}
```

---

## Screens

| Screen | Route | M� t? |
|--------|-------|-------|
| Bookings List | `/manage/bookings` | Danh s�ch t?t c? bookings |
| Booking Detail | `/manage/bookings/[id]` | Chi ti?t booking |
| Today's Arrivals | `/manage/bookings/arrivals` | Check-in h�m nay |
| Today's Departures | `/manage/bookings/departures` | Check-out h�m nay |
| Create Booking | `/manage/bookings/new` | T?o booking m?i (walk-in) |

---

## API Endpoints

### 1. Get Hotel Bookings
```http
GET /api/hotels/{hotelId}/bookings
Authorization: Bearer {token}
```
**Required Policy:** `Permission:bookings.read`

**Query Parameters:**
| Param | Type | Default | M� t? |
|-------|------|---------|-------|
| `status` | string | - | `Pending`, `Confirmed`, `CheckedIn`, etc. |
| `fromDate` | date | - | Filter from date |
| `toDate` | date | - | Filter to date |
| `search` | string | - | Search by guest name or confirmation # |
| `page` | int | 1 | |
| `pageSize` | int | 20 | |

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "uuid",
        "confirmationNumber": "BK-20240201-ABC123",
        "status": "Confirmed",
        "guestName": "John Doe",
        "guestEmail": "john@example.com",
        "guestPhoneNumber": "+84901234567",
        "checkInDate": "2024-02-01T14:00:00Z",
        "checkOutDate": "2024-02-03T12:00:00Z",
        "numberOfGuests": 2,
        "numberOfRooms": 1,
        "rooms": [
          { "roomNumber": "301", "roomType": "Deluxe King" }
        ],
        "totalAmount": 6850000,
        "isPaid": true,
        "source": "Website",
        "createdAt": "2024-01-20T10:00:00Z"
      }
    ],
    "totalCount": 150,
    "summary": {
      "totalBookings": 150,
      "pendingCount": 5,
      "confirmedCount": 120,
      "checkedInCount": 15,
      "cancelledCount": 10
    }
  }
}
```

---

### 2. Get Today's Arrivals
```http
GET /api/hotels/{hotelId}/bookings/arrivals
Authorization: Bearer {token}
```

**Query Parameters:**
| Param | Type | Default |
|-------|------|---------|
| `date` | date | today |

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": "uuid",
      "confirmationNumber": "BK-20240201-ABC123",
      "status": "Confirmed",
      "guestName": "John Doe",
      "guestPhoneNumber": "+84901234567",
      "checkInDate": "2024-02-01T14:00:00Z",
      "checkOutDate": "2024-02-03T12:00:00Z",
      "expectedArrivalTime": "15:00",
      "rooms": [
        { "roomNumber": "301", "roomType": "Deluxe King" }
      ],
      "specialRequests": "Late check-in around 10pm",
      "isPaid": true,
      "isVIP": false
    }
  ]
}
```

---

### 3. Get Today's Departures
```http
GET /api/hotels/{hotelId}/bookings/departures
Authorization: Bearer {token}
```

---

### 4. Get Booking Detail
```http
GET /api/bookings/{id}
Authorization: Bearer {token}
```

---

### 5. Check-In Guest
```http
POST /api/bookings/{id}/check-in
Authorization: Bearer {token}
```
**Required Policy:** `Permission:bookings.checkin`

**Request:**
```json
{
  "roomNumber": "301",
  "idDocumentType": "Passport",
  "idDocumentNumber": "C12345678",
  "notes": "Guest requested extra pillows"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Guest checked in successfully",
  "data": {
    "status": "CheckedIn",
    "checkedInAt": "2024-02-01T15:30:00Z",
    "checkedInBy": "Staff Name"
  }
}
```

---

### 6. Check-Out Guest
```http
POST /api/bookings/{id}/check-out
Authorization: Bearer {token}
```
**Required Policy:** `Permission:bookings.checkout`

**Request:**
```json
{
  "additionalCharges": [
    { "description": "Mini bar", "amount": 625000 },
    { "description": "Room service", "amount": 1125000 }
  ],
  "notes": "All good, guest satisfied"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Guest checked out successfully",
  "data": {
    "status": "CheckedOut",
    "checkedOutAt": "2024-02-03T11:00:00Z",
    "finalBill": {
      "roomCharges": 6850000,
      "additionalCharges": 1750000,
      "totalAmount": 8600000,
      "amountPaid": 6850000,
      "balanceDue": 1750000
    }
  }
}
```

---

### 7. Cancel Booking (by Hotel)
```http
POST /api/bookings/{id}/cancel
Authorization: Bearer {token}
```
**Required Policy:** `Permission:bookings.delete`

**Request:**
```json
{
  "reason": "Guest requested cancellation",
  "refundAmount": 6850000,
  "isFullRefund": true
}
```

---

### 8. Modify Booking
```http
PUT /api/bookings/{id}
Authorization: Bearer {token}
```

**Request:**
```json
{
  "checkInDate": "2024-02-02T14:00:00Z",
  "checkOutDate": "2024-02-04T12:00:00Z",
  "rooms": [
    { "roomId": "uuid", "numberOfAdults": 2 }
  ],
  "specialRequests": "Updated request"
}
```

---

### 9. Change Room Assignment
```http
PUT /api/bookings/{id}/rooms
Authorization: Bearer {token}
```

**Request:**
```json
{
  "oldRoomId": "uuid-301",
  "newRoomId": "uuid-305",
  "reason": "Guest requested higher floor"
}
```

---

### 10. Create Walk-in Booking
```http
POST /api/hotels/{hotelId}/bookings
Authorization: Bearer {token}
```
**Required Policy:** `Permission:bookings.create`

**Request:**
```json
{
  "roomId": "uuid",
  "guestName": "Jane Smith",
  "guestEmail": "jane@example.com",
  "guestPhoneNumber": "+84901234567",
  "checkInDate": "2024-02-01T14:00:00Z",
  "checkOutDate": "2024-02-03T12:00:00Z",
  "numberOfAdults": 2,
  "numberOfChildren": 0,
  "paymentMethod": "Cash",
  "isPaid": true,
  "source": "WalkIn"
}
```

---

### 11. Add Booking Note
```http
POST /api/bookings/{id}/notes
Authorization: Bearer {token}
```

**Request:**
```json
{
  "content": "Guest called to confirm late arrival",
  "isInternal": true
}
```

---

### 12. Get Booking History/Timeline
```http
GET /api/bookings/{id}/timeline
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "timestamp": "2024-01-20T10:00:00Z",
      "action": "Created",
      "description": "Booking created via Website",
      "performedBy": "System"
    },
    {
      "timestamp": "2024-01-20T10:05:00Z",
      "action": "PaymentReceived",
      "description": "Payment of 6.850.000 VND received via Stripe",
      "performedBy": "System"
    },
    {
      "timestamp": "2024-02-01T15:30:00Z",
      "action": "CheckedIn",
      "description": "Guest checked in to Room 301",
      "performedBy": "Staff Name"
    }
  ]
}
```

---

## Change Room API

### 13. Change Room (Đổi phòng cho guest đang ở)
```http
POST /api/bookings/{id}/change-room
Authorization: Bearer {token}
```
Chỉ áp dụng cho bookings đã **CheckedIn**

**Request:**
```json
{
  "oldRoomId": "uuid-cua-phong-cu",
  "newRoomId": "uuid-cua-phong-moi",
  "reason": "AC not working, moved to higher floor"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Room changed successfully",
  "data": {
    "bookingId": "uuid",
    "oldRoom": { "id": "uuid", "roomNumber": "301", "type": "Deluxe" },
    "newRoom": { "id": "uuid", "roomNumber": "401", "type": "Deluxe" },
    "priceDifference": 500000,
    "newTotalAmount": 7350000
  }
}
```

---

## Late Checkout API

### 14. Calculate Late Checkout Fee (Tính phụ phí trước khi xác nhận)
```http
POST /api/bookings/{id}/late-checkout/calculate
Authorization: Bearer {token}
```
Tính phụ phí late checkout mà không lưu vào hệ thống

**Request:**
```json
{
  "newCheckOutTime": "2024-02-03T15:00:00Z",
  "reason": "Guest requested late checkout"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "lateFeeAmount": 250000,
    "extraHours": 3,
    "hourlyRate": 83333,
    "originalCheckOut": "2024-02-03T12:00:00Z",
    "newCheckOut": "2024-02-03T15:00:00Z",
    "originalTotal": 6850000,
    "newTotal": 7100000
  }
}
```

**Cách tính phụ phí:**
- 10% giá phòng/giờ
- Tối đa 50% giá phòng/ngày
- Tối thiểu 1 giờ, tối đa 24 giờ

### 15. Process Late Checkout (Xác nhận late checkout)
```http
POST /api/bookings/{id}/late-checkout
Authorization: Bearer {token}
```
Xác nhận late checkout và thêm phụ phí vào booking

**Request:**
```json
{
  "newCheckOutTime": "2024-02-03T15:00:00Z",
  "reason": "Guest requested late checkout"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Late checkout processed successfully",
  "data": {
    "bookingId": "uuid",
    "newCheckOutTime": "2024-02-03T15:00:00Z",
    "lateFeeAmount": 250000,
    "extraHours": 3,
    "newTotalAmount": 7100000
  }
}
```

---

## Additional Charges API

### 16. Get Additional Charges (Lấy danh sách phụ phí)
```http
GET /api/bookings/{id}/charges
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": "uuid",
      "type": "LateCheckout",
      "description": "Late checkout fee: 15:00 (3 hours late)",
      "amount": 250000,
      "isPaid": false,
      "paidAt": null,
      "paymentMethod": null,
      "createdAt": "2024-02-03T11:00:00Z"
    },
    {
      "id": "uuid",
      "type": "Minibar",
      "description": "Minibar consumption",
      "amount": 625000,
      "isPaid": true,
      "paidAt": "2024-02-03T11:30:00Z",
      "paymentMethod": "Cash",
      "createdAt": "2024-02-02T20:00:00Z"
    },
    {
      "id": "uuid",
      "type": "RoomService",
      "description": "Dinner - Room service",
      "amount": 1125000,
      "isPaid": false,
      "paidAt": null,
      "paymentMethod": null,
      "createdAt": "2024-02-02T19:30:00Z"
    }
  ]
}
```

### 17. Add Additional Charge (Thêm phụ phí)
```http
POST /api/bookings/charges
Authorization: Bearer {token}
```

**Request:**
```json
{
  "bookingId": "uuid",
  "type": "Minibar",  // LateCheckout, Minibar, RoomService, Damages, Other
  "description": "Minibar - Beer and snacks",
  "amount": 450000,
  "notes": "Guest consumed beer and snacks from minibar"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Additional charge added successfully",
  "data": {
    "id": "uuid",
    "type": "Minibar",
    "description": "Minibar - Beer and snacks",
    "amount": 450000,
    "isPaid": false,
    "createdAt": "2024-02-03T10:00:00Z"
  }
}
```

### 18. Remove Additional Charge (Xóa phụ phí)
```http
DELETE /api/bookings/charges/{chargeId}
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "message": "Charge removed successfully",
  "data": true
}
```

---

## Enhanced Check-Out API

### 19. Check-Out với Additional Charges
```http
POST /api/bookings/{id}/checkout
Authorization: Bearer {token}
```

**Request:**
```json
{
  "additionalCharges": [
    {
      "type": "Minibar",
      "description": "Minibar consumption",
      "amount": 625000
    },
    {
      "type": "RoomService",
      "description": "Dinner - Room service",
      "amount": 1125000
    },
    {
      "type": "LateCheckout",
      "description": "Late checkout 3 hours",
      "amount": 250000
    }
  ],
  "paymentMethod": "Cash",
  "notes": "Guest satisfied, no damages"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Check-out successful",
  "data": {
    "bookingId": "uuid",
    "status": "CheckedOut",
    "checkedOutAt": "2024-02-03T11:30:00Z",
    "roomCharges": 6850000,
    "additionalCharges": 2000000,
    "totalAmount": 8850000,
    "amountPaid": 6850000,
    "balanceDue": 2000000,
    "charges": [
      {
        "id": "uuid",
        "type": "Minibar",
        "description": "Minibar consumption",
        "amount": 625000,
        "isPaid": true,
        "paidAt": "2024-02-03T11:30:00Z",
        "paymentMethod": "Cash"
      },
      {
        "id": "uuid",
        "type": "RoomService",
        "description": "Dinner - Room service",
        "amount": 1125000,
        "isPaid": true,
        "paidAt": "2024-02-03T11:30:00Z",
        "paymentMethod": "Cash"
      },
      {
        "id": "uuid",
        "type": "LateCheckout",
        "description": "Late checkout 3 hours",
        "amount": 250000,
        "isPaid": true,
        "paidAt": "2024-02-03T11:30:00Z",
        "paymentMethod": "Cash"
      }
    ]
  }
}
```

```typescript
interface BookingManagementDto {
  id: string;
  confirmationNumber: string;
  status: BookingStatus;
  guestName: string;
  guestEmail: string;
  guestPhoneNumber: string;
  checkInDate: string;
  checkOutDate: string;
  numberOfGuests: number;
  numberOfRooms: number;
  rooms: BookingRoomSummary[];
  totalAmount: number;
  isPaid: boolean;
  source: BookingSource;
  specialRequests?: string;
  createdAt: string;
}

interface BookingRoomSummary {
  roomId: string;
  roomNumber: string;
  roomType: string;
  guestName?: string;
}

enum BookingStatus {
  Pending = 0,
  Confirmed = 1,
  CheckedIn = 2,
  CheckedOut = 3,
  Cancelled = 4,
  NoShow = 5
}

enum BookingSource {
  Website = 0,
  Mobile = 1,
  WalkIn = 2,
  Phone = 3,
  OTA = 4,
  Corporate = 5
}

interface CheckInRequest {
  roomNumber?: string;
  idDocumentType?: string;
  idDocumentNumber?: string;
  notes?: string;
}

interface CheckOutRequest {
  additionalCharges?: AdditionalCharge[];
  notes?: string;
}

interface AdditionalCharge {
  description: string;
  amount: number;
}

interface BookingTimelineItem {
  timestamp: string;
  action: string;
  description: string;
  performedBy: string;
}

interface CreateWalkInBookingRequest {
  roomId: string;
  guestName: string;
  guestEmail?: string;
  guestPhoneNumber?: string;
  checkInDate: string;
  checkOutDate: string;
  numberOfAdults: number;
  numberOfChildren?: number;
  paymentMethod: string;
  isPaid: boolean;
  source: BookingSource;
}

// ============ CHANGE ROOM TYPES ============

interface ChangeRoomRequestDto {
  oldRoomId: string;
  newRoomId: string;
  reason?: string;
}

interface ChangeRoomResponseDto {
  bookingId: string;
  oldRoom: { id: string; roomNumber: string; type: string };
  newRoom: { id: string; roomNumber: string; type: string };
  priceDifference: number;
  newTotalAmount: number;
}

// ============ LATE CHECKOUT TYPES ============

interface LateCheckoutRequestDto {
  newCheckOutTime: string;  // ISO 8601 datetime
  reason?: string;
}

interface LateCheckoutResponseDto {
  lateFeeAmount: number;
  extraHours: number;
  hourlyRate: number;
  originalCheckOut: string;
  newCheckOut: string;
  originalTotal: number;
  newTotal: number;
}

// ============ ADDITIONAL CHARGES TYPES ============

enum AdditionalChargeType {
  LateCheckout = 'LateCheckout',
  Minibar = 'Minibar',
  RoomService = 'RoomService',
  Damages = 'Damages',
  Other = 'Other'
}

interface AdditionalChargeDto {
  id: string;
  type: AdditionalChargeType;
  description: string;
  amount: number;
  isPaid: boolean;
  paidAt?: string;
  paymentMethod?: string;
  createdAt: string;
}

interface CreateAdditionalChargeDto {
  bookingId: string;
  type: AdditionalChargeType;
  description: string;
  amount: number;
  notes?: string;
}

interface CheckOutResponseDto {
  bookingId: string;
  status: BookingStatus;
  checkedOutAt: string;
  roomCharges: number;
  additionalCharges: number;
  totalAmount: number;
  amountPaid: number;
  balanceDue: number;
  charges: AdditionalChargeDto[];
}

// ============ ROOM STATUS TYPES ============

enum RoomMaintenanceIssue {
  Plumbing = 'Plumbing',
  Electrical = 'Electrical',
  Cleaning = 'Cleaning',
  Damages = 'Damages',
  Other = 'Other'
}

enum RoomMaintenancePriority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Urgent = 'Urgent'
}

interface RoomMaintenanceReportDto {
  roomId: string;
  issue: RoomMaintenanceIssue;
  description: string;
  priority: RoomMaintenancePriority;
  reportedBy?: string;
}

interface RoomStatusSummaryDto {
  totalRooms: number;
  available: number;
  occupied: number;
  maintenance: number;
  cleaning: number;
  outOfOrder: number;
}

// ============ PERMISSION TYPES ============

interface UserPermissions {
  permissions: string[];
  scope: 'system' | 'brand' | 'hotel';
  brandId?: string;
  hotelId?: string;
}

interface PermissionCheckResult {
  hasPermission: boolean;
  permission: string;
  scope: string;
}

const PERMISSION_POLICIES = {
  BOOKINGS_READ: 'Permission:bookings.read',
  BOOKINGS_CREATE: 'Permission:bookings.create',
  BOOKINGS_UPDATE: 'Permission:bookings.update',
  BOOKINGS_DELETE: 'Permission:bookings.delete',
  BOOKINGS_CHECKIN: 'Permission:bookings.checkin',
  BOOKINGS_CHECKOUT: 'Permission:bookings.checkout',
  DASHBOARD_VIEW: 'Permission:dashboard.view',
  USERS_READ: 'Permission:users.read',
} as const;

---

## Screen Specifications

### Bookings List Screen
```
????????????????????????????????????????????????????
?  ?? Bookings                      [+ Walk-in]    ?
????????????????????????????????????????????????????
?  [All] [Pending 5] [Confirmed] [CheckedIn 15]    ?
?                                                  ?
?  ?? Search by guest name or confirmation #       ?
?  Date: [Feb 1] to [Feb 28]        [?? Filter]    ?
????????????????????????????????????????????????????
?  ??? Today's Overview ?????????????????????????  ?
?  ? 15 Arrivals ? 12 Departures ? 85 In-House ?  ?
????????????????????????????????????????????????????
?  ?????????????????????????????????????????????? ?
?  ? BK-20240201-ABC123        ? Confirmed     ? ?
?  ? John Doe � +84901234567                   ? ?
?  ? Feb 1 - Feb 3 � Room 301 (Deluxe King)    ? ?
?  ? 2 guests � 6,850,000 VND ?? Paid          ? ?
?  ? ?????????????????????????????????????     ? ?
?  ? [View] [Check-In] [���]                   ? ?
?  ?????????????????????????????????????????????? ?
?  ?????????????????????????????????????????????? ?
?  ? BK-20240201-XYZ789        ? CheckedIn     ? ?
?  ? Jane Smith � +84901234567                 ? ?
?  ? Jan 30 - Feb 2 � Room 205 (Suite)         ? ?
?  ? 2 guests � 11,250,000 VND ?? Paid         ? ?
?  ? ?????????????????????????????????????     ? ?
?  ? [View] [Check-Out] [���]                  ? ?
?  ?????????????????????????????????????????????? ?
????????????????????????????????????????????????????

### Booking Detail Screen
```
????????????????????????????????????????????????????
?  ? Back     BK-20240201-ABC123     [Check-In]    ?
????????????????????????????????????????????????????
?                                                  ?
?  Status: ? Confirmed                             ?
?                                                  ?
?  ??? Guest Information ????????????????????????  ?
?  ?? John Doe                                     ?
?  ?? john@example.com                             ?
?  ?? +84 901 234 567                              ?
?  ?? Passport: C12345678                          ?
?                                                  ?
?  ??? Stay Details ?????????????????????????????  ?
?  ?? Check-in:  Thu, Feb 1, 2024 at 14:00        ?
?  ?? Check-out: Sat, Feb 3, 2024 at 12:00        ?
?  ?? 2 nights                                     ?
?                                                  ?
?  ??? Room Assignment ??????????????????????????  ?
?  ?????????????????????????????????????????????? ?
?  ? Room 301 - Deluxe King                     ? ?
?  ? ?? 2 Adults                                ? ?
?  ?                           [Change Room]    ? ?
?  ?????????????????????????????????????????????? ?
?                                                  ?
?  ??? Special Requests ?????????????????????????  ?
?  Late check-in around 10pm                       ?
?  High floor if possible                          ?
?                                                  ?
?  ??? Payment ??????????????????????????????????  ?
?  Room charges:         7,500,000 VND             ?
?  Discount (SUMMER20): -1,500,000 VND             ?
?  Taxes & Fees:           850,000 VND             ?
?  ????????????????????????????????                ?
?  Total:                6,850,000 VND             ?
?  Paid via VNPay:       6,850,000 VND ?           ?
?  Balance:                      0 VND             ?
?                                                  ?
?  ??? Timeline ?????????????????????????????????  ?
?  � Jan 20, 10:00 - Booking created (Website)     ?
?  � Jan 20, 10:05 - Payment received (6.85M)      ?
?  � Jan 20, 10:05 - Confirmation email sent       ?
?                                                  ?
?  ??? Notes ?????????????????????? [+ Add Note]   ?
?  ?? Guest called to confirm late arrival         ?
?     - Staff Name, Jan 25, 14:00                  ?
?                                                  ?
????????????????????????????????????????????????????

### Check-In Modal
```
????????????????????????????????????????
?           Check-In Guest             ?
????????????????????????????????????????
?  Guest: John Doe                     ?
?  Booking: BK-20240201-ABC123         ?
?                                      ?
?  ??? Room Assignment ???????????????  ?
?  ??????????????????????????????????  ?
?  ? Room Number                    ?  ?
?  ? 301 (Deluxe King)           ?  ?  ?
?  ??????????????????????????????????  ?
?                                      ?
?  ??? ID Verification ???????????????  ?
?  ??????????????????????????????????  ?
?  ? Document Type                  ?  ?
?  ? Passport                    ?  ?  ?
?  ??????????????????????????????????  ?
?  ??????????????????????????????????  ?
?  ? Document Number                ?  ?
?  ? C12345678                      ?  ?
?  ??????????????????????????????????  ?
?                                      ?
?  ??? Notes ?????????????????????????  ?
?  ??????????????????????????????????  ?
?  ? Guest requested extra pillows  ?  ?
?  ??????????????????????????????????  ?
?                                      ?
?  ??????????????????????????????????  ?
?  ?        Complete Check-In       ?  ?
?  ??????????????????????????????????  ?
?  [Cancel]                            ?
????????????????????????????????????????
```

### Check-Out Modal
```
????????????????????????????????????????
?          Check-Out Guest             ?
????????????????????????????????????????
?  Guest: John Doe                     ?
?  Room: 301 � Feb 1-3, 2024           ?
?                                      ?
?  ??? Charges Summary ???????????????  ?
?  Room charges:        6,850,000 VND  ?
?  Already paid:        6,850,000 VND  ?
?                                      ?
?  ??? Additional Charges ?? [+ Add]   ?
?  ??????????????????????????????????  ?
?  ? Mini bar             625,000  ?  ?
?  ? Room service       1,125,000  ?  ?
?  ??????????????????????????????????  ?
?  Subtotal:            1,750,000      ?
?                                      ?
?  ??? Balance Due ???????????????????  ?
?  Total:               8,600,000      ?
?  Paid:                6,850,000      ?
?  ?????????????????????????????????   ?
?  Balance:             1,750,000      ?
?                                      ?
?  Payment Method: [Cash ?]            ?
?                                      ?
?  ??????????????????????????????????  ?
?  ?       Complete Check-Out       ?  ?
?  ??????????????????????????????????  ?
?  [Cancel]                            ?
????????????????????????????????????????

---
