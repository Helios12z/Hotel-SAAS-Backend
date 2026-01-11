# ?? Hotel SAAS - Frontend Development Kit

## M?c L?c

1. [T?ng Quan Ki?n Trúc](#t?ng-quan-ki?n-trúc)
2. [Danh Sách Modules](#danh-sách-modules)
3. [API Base URL & Authentication](#api-base-url--authentication)
4. [Th? T? Implement ?u Tiên](#th?-t?-implement-?u-tiên)
5. [File Chi Ti?t T?ng Module](#file-chi-ti?t-t?ng-module)
6. [TypeScript DTOs](#typescript-dtos)

---

## T?ng Quan Ki?n Trúc

### User Roles & Access

| Role | Mô t? | Modules Truy C?p |
|------|-------|------------------|
| `SuperAdmin` | Platform Admin | T?t c? |
| `BrandAdmin` | Ch? chu?i khách s?n | Brand, Hotels, Subscriptions, Dashboard |
| `HotelManager` | Qu?n lý khách s?n | Hotel Management, Bookings, Reviews |
| `Receptionist` | Nhân viên l? tân | Bookings, Check-in/out |
| `Guest` | Khách ??t phòng | Search, Booking, Profile, Reviews |

### Frontend Apps ?? Xu?t

```
hotel-saas-frontend/
??? apps/
?   ??? guest-web/          # Website cho Guest (Next.js)
?   ??? admin-portal/       # Portal cho Admin/Manager (React)
?   ??? mobile-app/         # Mobile App (React Native/Flutter)
??? packages/
?   ??? api-client/         # Shared API client
?   ??? ui-components/      # Shared UI components
?   ??? types/              # TypeScript types (copy t? ph?n DTOs bên d??i)
```

---

## Danh Sách Modules

| # | Module | File Chi Ti?t | Priority |
|---|--------|---------------|----------|
| 1 | Authentication | [01-auth.md](./modules/01-auth.md) | ?? Critical |
| 2 | Hotel Search & Discovery | [02-hotel-search.md](./modules/02-hotel-search.md) | ?? Critical |
| 3 | Booking & Checkout | [03-booking.md](./modules/03-booking.md) | ?? Critical |
| 4 | Guest Profile | [04-guest-profile.md](./modules/04-guest-profile.md) | ?? High |
| 5 | Reviews & Ratings | [05-reviews.md](./modules/05-reviews.md) | ?? High |
| 6 | Partner Onboarding | [06-partner-onboarding.md](./modules/06-partner-onboarding.md) | ?? High |
| 7 | Hotel Management | [07-hotel-management.md](./modules/07-hotel-management.md) | ?? High |
| 8 | Room Management | [08-room-management.md](./modules/08-room-management.md) | ?? High |
| 9 | Booking Management | [09-booking-management.md](./modules/09-booking-management.md) | ?? High |
| 10 | Dashboard & Analytics | [10-dashboard.md](./modules/10-dashboard.md) | ?? Medium |
| 11 | Promotions & Coupons | [11-promotions.md](./modules/11-promotions.md) | ?? Medium |
| 12 | Subscriptions | [12-subscriptions.md](./modules/12-subscriptions.md) | ?? Medium |
| 13 | Admin Portal | [13-admin-portal.md](./modules/13-admin-portal.md) | ?? Medium |
| 14 | Notifications | [14-notifications.md](./modules/14-notifications.md) | ?? Low |
| 15 | AI Chatbot | [15-chatbot.md](./modules/15-chatbot.md) | ?? Low |

---

## API Base URL & Authentication

### Base Configuration

```
Base URL: http://localhost:5000/api
Swagger UI: http://localhost:5000/
Timeout: 30000ms
```

### Authentication Flow

```
1. Login ? Nh?n tokens
   POST /api/auth/login
   Request: { email, password }
   Response: { accessToken, refreshToken, user }

2. L?u tokens vào localStorage ho?c secure storage

3. G?i request v?i Bearer token
   Header: Authorization: Bearer {accessToken}

4. Refresh token khi h?t h?n (401 response)
   POST /api/auth/refresh
   Request: { refreshToken }
   Response: { accessToken, refreshToken }
```

---

## Th? T? Implement ?u Tiên

### Phase 1: MVP Core (2-3 tu?n)
```
Week 1:
??? 01-auth (Login, Register, Forgot Password)
??? 02-hotel-search (Search, Filter, Hotel Detail)
??? 03-booking (Booking Flow, Payment)

Week 2-3:
??? 04-guest-profile (Profile, Bookings History)
??? 05-reviews (View, Write Reviews)
??? Basic UI/UX Polish
```

### Phase 2: Partner Portal (2-3 tu?n)
```
Week 4:
??? 06-partner-onboarding (Registration Flow)
??? 07-hotel-management (CRUD Hotels)
??? 08-room-management (CRUD Rooms)

Week 5-6:
??? 09-booking-management (View, Manage Bookings)
??? 10-dashboard (Analytics)
??? 11-promotions (Create, Manage Promos)
```

### Phase 3: Admin & Advanced (2 tu?n)
```
Week 7-8:
??? 12-subscriptions (Plan Selection, Billing)
??? 13-admin-portal (Onboarding Review, System Config)
??? 14-notifications (In-App Notifications)
??? 15-chatbot (AI Assistant)
```

---

## File Chi Ti?t T?ng Module

Xem th? m?c `./modules/` ?? có API spec và screens chi ti?t cho t?ng module.

---

## Quick Reference

### Common Response Format

```typescript
interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: string[];
  timestamp: string; // ISO 8601
}
```

### Pagination

```typescript
// Request
interface PaginatedRequest {
  page?: number;      // default: 1
  pageSize?: number;  // default: 10, max: 100
  sortBy?: string;
  sortDescending?: boolean;
}

// Response
interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
```

### Date Format
- T?t c? dates s? d?ng **ISO 8601**: `2024-01-15T14:30:00Z`
- Timezone: **UTC**
- Frontend nên convert sang local timezone khi hi?n th?

### Currency
- **Default**: `VND` (Vi?t Nam ??ng)
- Backend h? tr? multi-currency, truy?n `currency` field trong request
- Format s?: Không có ph?n th?p phân cho VND (ví d?: 1,500,000 VND)

---

## TypeScript DTOs

> Copy các interfaces bên d??i vào project frontend c?a b?n

### Common Types

```typescript
// Base DTO
interface BaseDto {
  id: string;
  createdAt: string;
  updatedAt: string;
}

// API Response wrapper
interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: string[];
  timestamp: string;
}

// Paginated response
interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
```

### Enums

```typescript
// User
enum UserRole {
  SuperAdmin = 0,
  BrandAdmin = 1,
  HotelManager = 2,
  Receptionist = 3,
  Guest = 4
}

enum UserStatus {
  Active = 0,
  Inactive = 1,
  Suspended = 2,
  PendingVerification = 3
}

// Room
enum RoomType {
  Standard = 0,
  Deluxe = 1,
  Suite = 2,
  Family = 3,
  Presidential = 4,
  Dormitory = 5,
  Studio = 6,
  Penthouse = 7
}

enum RoomStatus {
  Available = 0,
  Occupied = 1,
  Maintenance = 2,
  Cleaning = 3,
  OutOfOrder = 4,
  Reserved = 5
}

enum BedType {
  Single = 0,
  Double = 1,
  Queen = 2,
  King = 3,
  Twin = 4,
  Bunk = 5,
  SofaBed = 6
}

// Booking
enum BookingStatus {
  Pending = 0,
  Confirmed = 1,
  CheckedIn = 2,
  CheckedOut = 3,
  Cancelled = 4,
  NoShow = 5
}

// Payment
enum PaymentMethod {
  CreditCard = 0,
  DebitCard = 1,
  BankTransfer = 2,
  EWallet = 3,
  Cash = 4,
  Check = 5,
  OnlinePayment = 6
}

enum PaymentStatus {
  Pending = 0,
  Completed = 1,
  Failed = 2,
  Refunded = 3,
  PartiallyRefunded = 4
}

// Review
enum ReviewStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
  Flagged = 3
}

// Amenity
enum AmenityType {
  General = 0,
  Room = 1,
  Bathroom = 2,
  Kitchen = 3,
  Entertainment = 4,
  Service = 5,
  Facilities = 6
}

// Promotion
enum PromotionType {
  Percentage = 0,
  FixedAmount = 1,
  FreeNight = 2,
  Upgrade = 3
}

enum PromotionStatus {
  Draft = 0,
  Active = 1,
  Paused = 2,
  Expired = 3,
  Cancelled = 4
}

enum CouponStatus {
  Available = 0,
  Used = 1,
  Expired = 2,
  Cancelled = 3
}

// Subscription
enum SubscriptionPlanType {
  Basic = 0,
  Standard = 1,
  Premium = 2,
  Enterprise = 3
}

enum SubscriptionStatus {
  Active = 0,
  Cancelled = 1,
  Expired = 2,
  PastDue = 3,
  Trialing = 4,
  Paused = 5
}

enum BillingCycle {
  Monthly = 0,
  Quarterly = 1,
  Yearly = 2
}

enum InvoiceStatus {
  Draft = 0,
  Pending = 1,
  Paid = 2,
  Overdue = 3,
  Cancelled = 4
}

// Onboarding
enum OnboardingStatus {
  Draft = 0,
  PendingReview = 1,
  UnderReview = 2,
  RequiresChanges = 3,
  Approved = 4,
  Rejected = 5
}

enum DocumentType {
  BusinessLicense = 0,
  TaxCertificate = 1,
  OwnershipProof = 2,
  HotelPhotos = 3,
  ManagerId = 4,
  Other = 5
}

enum DocumentStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2
}

// Notification
enum NotificationType {
  BookingConfirmation = 0,
  BookingCancellation = 1,
  BookingReminder = 2,
  CheckInReminder = 3,
  CheckOutReminder = 4,
  PaymentReceived = 5,
  PaymentFailed = 6,
  ReviewRequest = 7,
  PromotionAlert = 8,
  SystemAlert = 9
}

enum NotificationChannel {
  InApp = 0,
  Email = 1,
  SMS = 2,
  Push = 3
}

enum NotificationStatus {
  Pending = 0,
  Sent = 1,
  Read = 2,
  Failed = 3
}
