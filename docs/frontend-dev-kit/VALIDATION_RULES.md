# Form Validation Rules

## Validation Patterns

### Email
```regex
/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/
```
- Tối thiểu 3 ký tự trước @
- Tối thiểu 2 ký tự sau @
- Không có ký tự đặc biệt ngoài `.`, `_`, `%`, `+`, `-`

### Phone Number (Vietnam)
```regex
/^(\+84|0)[0-9]{9,10}$/
```
- Bắt đầu bằng `+84` hoặc `0`
- Theo sau là 9-10 chữ số
- Ví dụ: `+84901234567` hoặc `0901234567`

### Price/Amount
```regex
/^[0-9]+(\.[0-9]{1,2})?$/
```
- Số nguyên hoặc số thập phân (tối đa 2 chữ số sau dấu chấm)
- Không có dấu phẩy (dùng `.` cho decimal)

### Date (ISO 8601)
```
YYYY-MM-DD hoặc YYYY-MM-DDTHH:mm:ssZ
```
- Năm: 4 chữ số (1900 - 2100)
- Tháng: 01 - 12
- Ngày: 01 - 31 (tùy tháng)

### Time (24h format)
```
HH:mm
```
- Giờ: 00 - 23
- Phút: 00 - 59

---

## Field Validation Rules

### User Fields

| Field | Required | Type | Min | Max | Pattern |
|-------|----------|------|-----|-----|---------|
| `email` | ✅ | email | - | 255 | Email regex |
| `password` | ✅ | string | 8 | 128 | At least 1 uppercase, 1 lowercase, 1 number, 1 special |
| `firstName` | ✅ | string | 1 | 50 | Vietnamese, English letters |
| `lastName` | ✅ | string | 1 | 50 | Vietnamese, English letters |
| `phoneNumber` | ❌ | phone | 10 | 15 | Vietnam phone regex |
| `dateOfBirth` | ❌ | date | - | - | Must be in past, at least 18 years old |

### Hotel Fields

| Field | Required | Type | Min | Max | Pattern |
|-------|----------|------|-----|-----|---------|
| `name` | ✅ | string | 2 | 200 | Vietnamese, English, numbers, basic symbols |
| `email` | ✅ | email | - | 255 | Email regex |
| `phoneNumber` | ✅ | string | 10 | 20 | Phone regex |
| `address` | ✅ | string | 5 | 500 | - |
| `city` | ✅ | string | 2 | 100 | - |
| `country` | ✅ | string | 2 | 100 | - |
| `starRating` | ✅ | number | 1 | 5 | Integer |
| `totalRooms` | ❌ | number | 1 | 1000 | Integer |
| `checkInTime` | ✅ | time | - | 5 | HH:mm format |
| `checkOutTime` | ✅ | time | - | 5 | HH:mm format |

### Room Fields

| Field | Required | Type | Min | Max | Pattern |
|-------|----------|------|-----|-----|---------|
| `roomNumber` | ✅ | string | 1 | 20 | Numbers, letters, dash |
| `type` | ✅ | enum | - | - | Standard, Deluxe, Suite, Family... |
| `bedType` | ✅ | enum | - | - | Single, Double, Queen, King, Twin... |
| `maxOccupancy` | ✅ | number | 1 | 10 | Integer |
| `basePrice` | ✅ | number | 10000 | 100000000 | Min 10,000 VND |
| `floor` | ❌ | number | - | 100 | Integer |
| `sizeInSquareMeters` | ❌ | number | 5 | 1000 | Integer |

### Booking Fields

| Field | Required | Type | Min | Max | Pattern |
|-------|----------|------|-----|-----|---------|
| `checkInDate` | ✅ | datetime | - | - | Future or today |
| `checkOutDate` | ✅ | datetime | - | - | After checkInDate |
| `numberOfAdults` | ✅ | number | 1 | 10 | Integer |
| `numberOfChildren` | ✅ | number | 0 | 10 | Integer |
| `guestName` | ✅ | string | 2 | 100 | Vietnamese, English |
| `guestEmail` | ✅ | email | - | 255 | Email regex |
| `guestPhoneNumber` | ✅ | phone | 10 | 15 | Phone regex |

### Payment Fields

| Field | Required | Type | Min | Max | Pattern |
|-------|----------|------|-----|-----|---------|
| `amount` | ✅ | number | 10000 | - | Min 10,000 VND |
| `currency` | ✅ | string | 3 | 3 | ISO 4217 (VND, USD) |
| `paymentMethod` | ✅ | enum | - | - | Cash, Card, BankTransfer... |

---

## Form-specific Validation

### Search Hotel Form
```typescript
interface SearchHotelForm {
  destination: string;      // Required, min 2 chars
  checkInDate: Date;        // Required, >= today
  checkOutDate: Date;       // Required, > checkInDate
  numberOfGuests: number;   // Required, 1-10
  numberOfRooms: number;    // Required, 1-10
}

// Validation rules:
- checkInDate >= Today
- checkOutDate > checkInDate
- Minimum stay: 1 night
- Maximum stay: 30 nights (or hotel config)
```

### Booking Form
```typescript
interface BookingForm {
  guestInfo: {
    firstName: string;      // Required, 2-50 chars
    lastName: string;       // Required, 2-50 chars
    email: string;          // Required, valid email
    phoneNumber: string;    // Required, valid phone
  };
  stayInfo: {
    checkIn: Date;          // Required
    checkOut: Date;         // Required, > checkIn
    adults: number;         // Required, 1-10
    children: number;       // Required, 0-10
  };
  rooms: RoomSelection[];   // Required, at least 1
  specialRequests?: string; // Optional, max 500 chars
}

// Validation rules:
- Guest count <= Room max occupancy
- Special requests max 500 characters
```

### Hotel Settings Form
```typescript
interface HotelSettingsForm {
  timeConfig: {
    checkInTime: string;    // Required, HH:mm
    checkOutTime: string;   // Required, HH:mm
  };
  guestConfig: {
    maxAdultsPerRoom: number;  // Required, 1-10
    maxChildrenPerRoom: number;// Required, 0-10
    maxGuestsPerRoom: number;  // Required, 1-20
    allowExtraBed: boolean;    // Optional
    extraBedPrice?: number;    // Required if allowExtraBed
  };
  bookingRules: {
    minNights: number;         // Required, 1-30
    maxNights: number;         // Required, 1-365
    minAdvanceBookingHours: number;  // Required, 0-720
    maxAdvanceBookingDays: number;   // Required, 1-365
  };
  paymentConfig: {
    enableStripePayment: boolean;
    enablePayAtHotel: boolean;
  };
}
```

---

## Validation Error Messages (Vietnamese)

| Field | Error | Message |
|-------|-------|---------|
| email | required | Email là bắt buộc |
| email | email | Email không hợp lệ |
| email | maxLength | Email không được quá 255 ký tự |
| password | required | Mật khẩu là bắt buộc |
| password | minLength | Mật khẩu phải có ít nhất 8 ký tự |
| password | pattern | Mật khẩu phải có chữ hoa, chữ thường, số và ký tự đặc biệt |
| phoneNumber | pattern | Số điện thoại không hợp lệ |
| checkInDate | required | Ngày check-in là bắt buộc |
| checkInDate | futureDate | Ngày check-in phải từ hôm nay trở đi |
| checkOutDate | required | Ngày check-out là bắt buộc |
| checkOutDate | afterCheckIn | Ngày check-out phải sau ngày check-in |
| numberOfGuests | min | Số khách phải từ 1 trở lên |
| numberOfGuests | max | Số khách không được vượt quá 10 |
| roomNumber | required | Số phòng là bắt buộc |
| basePrice | min | Giá phòng phải từ 10,000 VND |
| starRating | min | Xếp hạng sao phải từ 1 |
| starRating | max | Xếp hạng sao không được quá 5 |

---

## React Hook Form Integration Example

```typescript
import { useForm } from 'react-hook-form';

const schema = z.object({
  email: z.string().email('Email không hợp lệ').min(1, 'Email là bắt buộc'),
  password: z.string()
    .min(8, 'Mật khẩu phải có ít nhất 8 ký tự')
    .regex(/[A-Z]/, 'Mật khẩu phải có ít nhất 1 chữ hoa')
    .regex(/[a-z]/, 'Mật khẩu phải có ít nhất 1 chữ thường')
    .regex(/[0-9]/, 'Mật khẩu phải có ít nhất 1 số')
    .regex(/[!@#$%^&*]/, 'Mật khẩu phải có ít nhất 1 ký tự đặc biệt'),
  checkInDate: z.string().min(1, 'Ngày check-in là bắt buộc'),
  checkOutDate: z.string().min(1, 'Ngày check-out là bắt buộc'),
  numberOfGuests: z.number().min(1, 'Số khách phải từ 1').max(10),
});

function BookingForm() {
  const { register, handleSubmit, formState: { errors } } = useForm();

  const onSubmit = (data) => {
    console.log(data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <input {...register('email', { required: true })} />
      {errors.email && <span>Email là bắt buộc</span>}

      <input type="password" {...register('password', { required: true, minLength: 8 })} />
      {errors.password && <span>Mật khẩu không hợp lệ</span>}

      <button type="submit">Submit</button>
    </form>
  );
}
```
