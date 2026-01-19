using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Mapping
{
    /// <summary>
    /// Simple custom mapper for converting between entities and DTOs.
    /// Use this instead of AutoMapper for better performance and explicit mapping control.
    /// </summary>
    public static class Mapper
    {
        #region User Mappings

        public static UserDto ToDto(User entity)
        {
            return new UserDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Email = entity.Email,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                PhoneNumber = entity.PhoneNumber,
                AvatarUrl = entity.AvatarUrl,
                Role = entity.Role,
                Status = entity.Status,
                Nationality = entity.Nationality,
                BrandId = entity.BrandId,
                HotelId = entity.HotelId,
                LastLoginAt = entity.LastLoginAt
            };
        }

        public static User ToEntity(RegisterDto dto)
        {
            return new User
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Role,
                BrandId = dto.BrandId,
                HotelId = dto.HotelId,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(UpdateUserDto dto, User entity)
        {
            if (dto.FirstName != null) entity.FirstName = dto.FirstName;
            if (dto.LastName != null) entity.LastName = dto.LastName;
            if (dto.PhoneNumber != null) entity.PhoneNumber = dto.PhoneNumber;
            if (dto.Role.HasValue) entity.Role = dto.Role.Value;
            if (dto.Status.HasValue) entity.Status = dto.Status.Value;
            if (dto.BrandId.HasValue) entity.BrandId = dto.BrandId.Value;
            if (dto.HotelId.HasValue) entity.HotelId = dto.HotelId.Value;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        #endregion

        #region Brand Mappings

        public static BrandDto ToDto(Brand entity)
        {
            var activeSubscription = entity.Subscriptions?.OrderByDescending(s => s.CreatedAt).FirstOrDefault();
            
            return new BrandDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Name = entity.Name,
                Description = entity.Description,
                LogoUrl = entity.LogoUrl,
                Website = entity.Website,
                PhoneNumber = entity.PhoneNumber,
                Email = entity.Email,
                Address = entity.Address,
                City = entity.City,
                Country = entity.Country,
                IsActive = entity.IsActive,
                HotelCount = entity.Hotels != null ? entity.Hotels.Count : 0,
                CommissionRate = entity.CommissionRate,
                SubscriptionPlan = activeSubscription?.Plan?.Name,
                SubscriptionStatus = activeSubscription?.Status.ToString()
            };
        }

        public static Brand ToEntity(CreateBrandDto dto)
        {
            return new Brand
            {
                Name = dto.Name,
                Description = dto.Description,
                LogoUrl = dto.LogoUrl,
                Website = dto.Website,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                TaxId = dto.TaxId,
                BusinessLicense = dto.BusinessLicense,
                CommissionRate = dto.CommissionRate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(UpdateBrandDto dto, Brand entity)
        {
            if (dto.Name != null) entity.Name = dto.Name;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.LogoUrl != null) entity.LogoUrl = dto.LogoUrl;
            if (dto.Website != null) entity.Website = dto.Website;
            if (dto.PhoneNumber != null) entity.PhoneNumber = dto.PhoneNumber;
            if (dto.Email != null) entity.Email = dto.Email;
            if (dto.Address != null) entity.Address = dto.Address;
            if (dto.City != null) entity.City = dto.City;
            if (dto.Country != null) entity.Country = dto.Country;
            if (dto.PostalCode != null) entity.PostalCode = dto.PostalCode;
            if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;
            if (dto.CommissionRate != null) entity.CommissionRate = dto.CommissionRate;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        #endregion

        #region Hotel Mappings

        public static HotelDto ToDto(Hotel entity)
        {
            return new HotelDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                BrandId = entity.BrandId,
                BrandName = entity.Brand?.Name ?? "",
                Name = entity.Name,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl,
                City = entity.City,
                Country = entity.Country,
                StarRating = entity.StarRating,
                IsActive = entity.IsActive,
                IsVerified = entity.IsVerified,
                AverageRating = entity.AverageRating,
                ReviewCount = entity.ReviewCount,
                MinPrice = entity.Rooms?.Any() == true ? entity.Rooms.Min(r => r.BasePrice) : null
            };
        }

        public static HotelDetailDto ToDetailDto(Hotel entity)
        {
            return new HotelDetailDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                BrandId = entity.BrandId,
                BrandName = entity.Brand?.Name ?? "",
                Name = entity.Name,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl,
                City = entity.City,
                Country = entity.Country,
                StarRating = entity.StarRating,
                IsActive = entity.IsActive,
                IsVerified = entity.IsVerified,
                AverageRating = entity.AverageRating,
                ReviewCount = entity.ReviewCount,
                MinPrice = entity.Rooms?.Any() == true ? entity.Rooms.Min(r => r.BasePrice) : null,
                Address = entity.Address,
                State = entity.State,
                PostalCode = entity.PostalCode,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                PhoneNumber = entity.PhoneNumber,
                Email = entity.Email,
                Website = entity.Website,
                TotalRooms = entity.TotalRooms,
                NumberOfFloors = entity.NumberOfFloors,
                TaxId = entity.TaxId,
                SmokingPolicy = entity.SmokingPolicy,

                // Settings - All config
                Settings = new HotelSettingsDto
                {
                    CheckInTime = entity.CheckInTime,
                    CheckOutTime = entity.CheckOutTime,
                    MaxAdultsPerRoom = entity.MaxAdultsPerRoom,
                    MaxChildrenPerRoom = entity.MaxChildrenPerRoom,
                    MaxGuestsPerRoom = entity.MaxGuestsPerRoom,
                    AllowExtraBed = entity.AllowExtraBed,
                    ExtraBedPrice = entity.ExtraBedPrice,
                    MinNights = entity.MinNights,
                    MaxNights = entity.MaxNights,
                    MinAdvanceBookingHours = entity.MinAdvanceBookingHours,
                    MaxAdvanceBookingDays = entity.MaxAdvanceBookingDays,
                    EnableStripePayment = entity.EnableStripePayment,
                    EnablePayAtHotel = entity.EnablePayAtHotel,
                    StripeAccountId = entity.StripeAccountId,
                    TaxRate = entity.TaxRate,
                    ServiceFeeRate = entity.ServiceFeeRate,
                    CancellationPolicy = entity.CancellationPolicy,
                    ChildPolicy = entity.ChildPolicy,
                    PetPolicy = entity.PetPolicy
                },

                // Public Settings - Show to guests
                PublicSettings = new HotelPublicSettingsDto
                {
                    CheckInTime = entity.CheckInTime,
                    CheckOutTime = entity.CheckOutTime,
                    MaxGuestsPerRoom = entity.MaxGuestsPerRoom,
                    AllowExtraBed = entity.AllowExtraBed,
                    ExtraBedPrice = entity.ExtraBedPrice,
                    CancellationPolicy = entity.CancellationPolicy,
                    ChildPolicy = entity.ChildPolicy,
                    PetPolicy = entity.PetPolicy,
                    SmokingPolicy = entity.SmokingPolicy,
                    EnableStripePayment = entity.EnableStripePayment,
                    EnablePayAtHotel = entity.EnablePayAtHotel
                },

                Images = entity.Images?.Select(i => new HotelImageDto
                {
                    ImageUrl = i.ImageUrl,
                    Caption = i.Caption,
                    AltText = i.AltText,
                    DisplayOrder = i.DisplayOrder,
                    IsPrimary = i.IsPrimary,
                    Category = i.Category
                }).ToList() ?? new List<HotelImageDto>(),
                Amenities = entity.Amenities?.Select(ha => new AmenityDto
                {
                    Id = ha.Amenity.Id,
                    CreatedAt = ha.Amenity.CreatedAt,
                    UpdatedAt = ha.Amenity.UpdatedAt,
                    Name = ha.Amenity.Name,
                    Description = ha.Amenity.Description,
                    Icon = ha.Amenity.Icon,
                    Type = ha.Amenity.Type,
                    IsActive = ha.Amenity.IsActive
                }).ToList() ?? new List<AmenityDto>(),
                RecentReviews = entity.Reviews?.Take(10).Select(r => new ReviewDto
                {
                    Id = r.Id,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    HotelId = r.HotelId,
                    HotelName = entity.Name,
                    GuestId = r.GuestId,
                    GuestName = $"{r.Guest.FirstName} {r.Guest.LastName}",
                    GuestAvatarUrl = r.Guest.AvatarUrl,
                    Rating = r.Rating,
                    Title = r.Title,
                    Comment = r.Comment,
                    CleanlinessRating = r.CleanlinessRating,
                    ServiceRating = r.ServiceRating,
                    LocationRating = r.LocationRating,
                    ValueRating = r.ValueRating,
                    IsVerified = r.IsVerified,
                    StayDate = r.StayDate,
                    PublishedAt = r.PublishedAt,
                    ManagementResponse = r.ManagementResponse,
                    Images = r.Images?.Select(ri => new ReviewImageDto
                    {
                        Id = ri.Id,
                        ImageUrl = ri.ImageUrl,
                        Caption = ri.Caption
                    }).ToList() ?? new List<ReviewImageDto>()
                }).ToList() ?? new List<ReviewDto>()
            };
        }

        public static Hotel ToEntity(CreateHotelDto dto)
        {
            return new Hotel
            {
                BrandId = dto.BrandId,
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Website = dto.Website,
                StarRating = dto.StarRating,
                TaxId = dto.TaxId,
                CheckInTime = dto.CheckInTime,
                CheckOutTime = dto.CheckOutTime,
                CancellationPolicy = dto.CancellationPolicy,
                ChildPolicy = dto.ChildPolicy,
                PetPolicy = dto.PetPolicy,
                TotalRooms = dto.TotalRooms,
                CommissionRate = dto.CommissionRate,
                IsActive = true,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(UpdateHotelDto dto, Hotel entity)
        {
            if (dto.Name != null) entity.Name = dto.Name;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.ImageUrl != null) entity.ImageUrl = dto.ImageUrl;
            if (dto.Address != null) entity.Address = dto.Address;
            if (dto.City != null) entity.City = dto.City;
            if (dto.State != null) entity.State = dto.State;
            if (dto.Country != null) entity.Country = dto.Country;
            if (dto.PostalCode != null) entity.PostalCode = dto.PostalCode;
            if (dto.Latitude.HasValue) entity.Latitude = dto.Latitude.Value;
            if (dto.Longitude.HasValue) entity.Longitude = dto.Longitude.Value;
            if (dto.PhoneNumber != null) entity.PhoneNumber = dto.PhoneNumber;
            if (dto.Email != null) entity.Email = dto.Email;
            if (dto.Website != null) entity.Website = dto.Website;
            if (dto.StarRating.HasValue) entity.StarRating = dto.StarRating.Value;
            if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;
            if (dto.IsVerified.HasValue) entity.IsVerified = dto.IsVerified.Value;
            if (dto.CheckInTime != null) entity.CheckInTime = dto.CheckInTime;
            if (dto.CheckOutTime != null) entity.CheckOutTime = dto.CheckOutTime;
            if (dto.CancellationPolicy != null) entity.CancellationPolicy = dto.CancellationPolicy;
            if (dto.ChildPolicy != null) entity.ChildPolicy = dto.ChildPolicy;
            if (dto.PetPolicy != null) entity.PetPolicy = dto.PetPolicy;
            if (dto.TotalRooms.HasValue) entity.TotalRooms = dto.TotalRooms.Value;
            if (dto.CommissionRate.HasValue) entity.CommissionRate = dto.CommissionRate.Value;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        #endregion

        #region Room Mappings

        public static RoomDto ToDto(Room entity)
        {
            return new RoomDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                HotelId = entity.HotelId,
                HotelName = entity.Hotel?.Name ?? "",
                RoomNumber = entity.RoomNumber,
                Floor = entity.Floor,
                Type = entity.Type,
                BedType = entity.BedType,
                NumberOfBeds = entity.NumberOfBeds,
                MaxOccupancy = entity.MaxOccupancy,
                BasePrice = entity.BasePrice,
                WeekendPrice = entity.WeekendPrice,
                HolidayPrice = entity.HolidayPrice,
                SizeInSquareMeters = entity.SizeInSquareMeters,
                Status = entity.Status,
                Description = entity.Description,
                HasView = entity.HasView,
                ViewDescription = entity.ViewDescription,
                IsAccessible = entity.IsAccessible
            };
        }

        public static RoomDetailDto ToDetailDto(Room entity)
        {
            return new RoomDetailDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                HotelId = entity.HotelId,
                HotelName = entity.Hotel?.Name ?? "",
                RoomNumber = entity.RoomNumber,
                Floor = entity.Floor,
                Type = entity.Type,
                BedType = entity.BedType,
                NumberOfBeds = entity.NumberOfBeds,
                MaxOccupancy = entity.MaxOccupancy,
                BasePrice = entity.BasePrice,
                WeekendPrice = entity.WeekendPrice,
                HolidayPrice = entity.HolidayPrice,
                SizeInSquareMeters = entity.SizeInSquareMeters,
                Status = entity.Status,
                Description = entity.Description,
                HasView = entity.HasView,
                ViewDescription = entity.ViewDescription,
                IsAccessible = entity.IsAccessible,
                ImageUrl = entity.Images?.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
                SmokingAllowed = entity.SmokingAllowed,
                IsPetFriendly = entity.IsPetFriendly,
                HasConnectingRoom = entity.HasConnectingRoom,
                ConnectingRoomId = entity.ConnectingRoomId,
                AccessibilityFeatures = entity.AccessibilityFeatures,
                Amenities = entity.Amenities?.Select(ra => new AmenityDto
                {
                    Id = ra.Amenity.Id,
                    CreatedAt = ra.Amenity.CreatedAt,
                    UpdatedAt = ra.Amenity.UpdatedAt,
                    Name = ra.Amenity.Name,
                    Description = ra.Amenity.Description,
                    Icon = ra.Amenity.Icon,
                    Type = ra.Amenity.Type,
                    IsActive = ra.Amenity.IsActive
                }).ToList() ?? new List<AmenityDto>(),
                Images = entity.Images?.Select(i => new RoomImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Caption = i.Caption,
                    DisplayOrder = i.DisplayOrder,
                    IsPrimary = i.IsPrimary
                }).ToList() ?? new List<RoomImageDto>()
            };
        }

        public static Room ToEntity(CreateRoomDto dto)
        {
            return new Room
            {
                HotelId = dto.HotelId,
                RoomNumber = dto.RoomNumber,
                Floor = dto.Floor,
                Type = dto.Type,
                BedType = dto.BedType,
                NumberOfBeds = dto.NumberOfBeds,
                MaxOccupancy = dto.MaxOccupancy,
                BasePrice = dto.BasePrice,
                WeekendPrice = dto.WeekendPrice,
                HolidayPrice = dto.HolidayPrice,
                SizeInSquareMeters = dto.SizeInSquareMeters,
                Description = dto.Description,
                HasView = dto.HasView,
                ViewDescription = dto.ViewDescription,
                SmokingAllowed = dto.SmokingAllowed,
                IsPetFriendly = dto.IsPetFriendly,
                HasConnectingRoom = dto.HasConnectingRoom,
                ConnectingRoomId = dto.ConnectingRoomId,
                IsAccessible = dto.IsAccessible,
                AccessibilityFeatures = dto.AccessibilityFeatures,
                Status = RoomStatus.Available,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(UpdateRoomDto dto, Room entity)
        {
            if (dto.Floor != null) entity.Floor = dto.Floor;
            if (dto.Type.HasValue) entity.Type = dto.Type.Value;
            if (dto.BedType.HasValue) entity.BedType = dto.BedType.Value;
            if (dto.NumberOfBeds.HasValue) entity.NumberOfBeds = dto.NumberOfBeds.Value;
            if (dto.MaxOccupancy.HasValue) entity.MaxOccupancy = dto.MaxOccupancy.Value;
            if (dto.BasePrice.HasValue) entity.BasePrice = dto.BasePrice.Value;
            if (dto.WeekendPrice.HasValue) entity.WeekendPrice = dto.WeekendPrice.Value;
            if (dto.HolidayPrice.HasValue) entity.HolidayPrice = dto.HolidayPrice.Value;
            if (dto.SizeInSquareMeters.HasValue) entity.SizeInSquareMeters = dto.SizeInSquareMeters.Value;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.HasView.HasValue) entity.HasView = dto.HasView.Value;
            if (dto.ViewDescription != null) entity.ViewDescription = dto.ViewDescription;
            if (dto.SmokingAllowed.HasValue) entity.SmokingAllowed = dto.SmokingAllowed.Value;
            if (dto.IsPetFriendly.HasValue) entity.IsPetFriendly = dto.IsPetFriendly.Value;
            if (dto.HasConnectingRoom.HasValue) entity.HasConnectingRoom = dto.HasConnectingRoom.Value;
            if (dto.ConnectingRoomId.HasValue) entity.ConnectingRoomId = dto.ConnectingRoomId.Value;
            if (dto.IsAccessible.HasValue) entity.IsAccessible = dto.IsAccessible.Value;
            if (dto.AccessibilityFeatures != null) entity.AccessibilityFeatures = dto.AccessibilityFeatures;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        #endregion

        #region Amenity Mappings

        public static AmenityDto ToDto(Amenity entity)
        {
            return new AmenityDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Name = entity.Name,
                Description = entity.Description,
                Icon = entity.Icon,
                Type = entity.Type,
                IsActive = entity.IsActive
            };
        }

        public static Amenity ToEntity(CreateAmenityDto dto)
        {
            return new Amenity
            {
                Name = dto.Name,
                Description = dto.Description,
                Icon = dto.Icon,
                Type = dto.Type,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(UpdateAmenityDto dto, Amenity entity)
        {
            if (dto.Name != null) entity.Name = dto.Name;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.Icon != null) entity.Icon = dto.Icon;
            if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        #endregion

        #region Booking Mappings

        public static BookingDto ToDto(Booking entity)
        {
            return new BookingDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                HotelId = entity.HotelId,
                HotelName = entity.Hotel?.Name ?? "",
                GuestId = entity.GuestId,
                GuestName = entity.GuestName ?? $"{entity.Guest.FirstName} {entity.Guest.LastName}",
                GuestEmail = entity.GuestEmail ?? entity.Guest.Email,
                ConfirmationNumber = entity.ConfirmationNumber,
                CheckInDate = entity.CheckInDate,
                CheckOutDate = entity.CheckOutDate,
                NumberOfGuests = entity.NumberOfGuests,
                NumberOfRooms = entity.NumberOfRooms,
                TotalAmount = entity.TotalAmount,
                Currency = entity.Currency,
                Status = entity.Status,
                IsPaid = entity.IsPaid,
                BookedAt = entity.BookedAt
            };
        }

        public static BookingDetailDto ToDetailDto(Booking entity)
        {
            return new BookingDetailDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                HotelId = entity.HotelId,
                HotelName = entity.Hotel?.Name ?? "",
                GuestId = entity.GuestId,
                GuestName = entity.GuestName ?? $"{entity.Guest.FirstName} {entity.Guest.LastName}",
                GuestEmail = entity.GuestEmail ?? entity.Guest.Email,
                ConfirmationNumber = entity.ConfirmationNumber,
                CheckInDate = entity.CheckInDate,
                CheckOutDate = entity.CheckOutDate,
                NumberOfGuests = entity.NumberOfGuests,
                NumberOfRooms = entity.NumberOfRooms,
                TotalAmount = entity.TotalAmount,
                Currency = entity.Currency,
                Status = entity.Status,
                IsPaid = entity.IsPaid,
                BookedAt = entity.BookedAt,
                Subtotal = entity.Subtotal,
                TaxAmount = entity.TaxAmount,
                ServiceFee = entity.ServiceFee,
                DiscountAmount = entity.DiscountAmount,
                SpecialRequests = entity.SpecialRequests,
                CancellationPolicy = entity.Hotel?.CancellationPolicy,
                ConfirmedAt = entity.ConfirmedAt,
                CheckedInAt = entity.CheckedInAt,
                CheckedOutAt = entity.CheckedOutAt,
                AppliedCouponCode = entity.AppliedCouponCode,
                Rooms = entity.BookingRooms?.Select(br => new BookingRoomDetailDto
                {
                    Id = br.Id,
                    RoomId = br.RoomId,
                    RoomNumber = br.Room?.RoomNumber,
                    RoomType = br.Room?.Type ?? RoomType.Standard,
                    Price = br.Price,
                    NumberOfAdults = br.NumberOfAdults,
                    NumberOfChildren = br.NumberOfChildren,
                    NumberOfInfants = br.NumberOfInfants,
                    GuestName = br.GuestName,
                    SpecialRequests = br.SpecialRequests
                }).ToList() ?? new List<BookingRoomDetailDto>(),
                Payments = entity.Payments?.Select(p => new PaymentDto
                {
                    Id = p.Id,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    BookingId = p.BookingId,
                    TransactionId = p.TransactionId,
                    Amount = p.Amount,
                    Currency = p.Currency,
                    Method = p.Method,
                    Status = p.Status,
                    ProcessedAt = p.ProcessedAt,
                    Gateway = p.Gateway,
                    CardLast4Digits = p.CardLast4Digits,
                    ReceiptUrl = p.ReceiptUrl
                }).ToList() ?? new List<PaymentDto>(),
                HotelImageUrl = entity.Hotel?.ImageUrl,
                HotelAddress = entity.Hotel?.Address,
                HotelCity = entity.Hotel?.City,
                HotelPhoneNumber = entity.Hotel?.PhoneNumber
            };
        }

        public static Booking ToEntity(CreateBookingDto dto)
        {
            return new Booking
            {
                HotelId = dto.HotelId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                GuestName = dto.GuestName,
                GuestEmail = dto.GuestEmail,
                GuestPhoneNumber = dto.GuestPhoneNumber,
                GuestAddress = dto.GuestAddress,
                GuestNationality = dto.GuestNationality,
                SpecialRequests = dto.SpecialRequests,
                PaymentMethod = dto.PaymentMethod,
                Currency = dto.Currency ?? "USD",
                Status = BookingStatus.Pending,
                BookedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(UpdateBookingDto dto, Booking entity)
        {
            if (dto.CheckInDate.HasValue) entity.CheckInDate = dto.CheckInDate.Value;
            if (dto.CheckOutDate.HasValue) entity.CheckOutDate = dto.CheckOutDate.Value;
            if (dto.NumberOfGuests.HasValue) entity.NumberOfGuests = dto.NumberOfGuests.Value;
            if (dto.SpecialRequests != null) entity.SpecialRequests = dto.SpecialRequests;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        #endregion

        #region Payment Mappings

        public static PaymentDto ToDto(Payment entity)
        {
            return new PaymentDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                BookingId = entity.BookingId,
                TransactionId = entity.TransactionId,
                Amount = entity.Amount,
                Currency = entity.Currency,
                Method = entity.Method,
                Status = entity.Status,
                ProcessedAt = entity.ProcessedAt,
                Gateway = entity.Gateway,
                CardLast4Digits = entity.CardLast4Digits,
                ReceiptUrl = entity.ReceiptUrl
            };
        }

        public static Payment ToEntity(CreatePaymentDto dto)
        {
            return new Payment
            {
                BookingId = dto.BookingId,
                Amount = dto.Amount,
                Currency = dto.Currency ?? "USD",
                Method = dto.Method,
                Gateway = dto.Gateway,
                CardLast4Digits = dto.CardLast4Digits,
                TransactionId = dto.TransactionId,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        #endregion

        #region Review Mappings

        public static ReviewDto ToDto(Review entity)
        {
            return new ReviewDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                HotelId = entity.HotelId,
                HotelName = entity.Hotel?.Name ?? "",
                GuestId = entity.GuestId,
                GuestName = $"{entity.Guest.FirstName} {entity.Guest.LastName}",
                GuestAvatarUrl = entity.Guest.AvatarUrl,
                Rating = entity.Rating,
                Title = entity.Title,
                Comment = entity.Comment,
                CleanlinessRating = entity.CleanlinessRating,
                ServiceRating = entity.ServiceRating,
                LocationRating = entity.LocationRating,
                ValueRating = entity.ValueRating,
                IsVerified = entity.IsVerified,
                StayDate = entity.StayDate,
                PublishedAt = entity.PublishedAt,
                ManagementResponse = entity.ManagementResponse,
                Images = entity.Images?.Select(i => new ReviewImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Caption = i.Caption
                }).ToList() ?? new List<ReviewImageDto>()
            };
        }

        public static Review ToEntity(CreateReviewDto dto)
        {
            return new Review
            {
                HotelId = dto.HotelId,
                BookingId = dto.BookingId,
                Rating = dto.Rating,
                Title = dto.Title,
                Comment = dto.Comment,
                CleanlinessRating = dto.CleanlinessRating,
                ServiceRating = dto.ServiceRating,
                LocationRating = dto.LocationRating,
                ValueRating = dto.ValueRating,
                Status = ReviewStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(UpdateReviewDto dto, Review entity)
        {
            if (dto.Rating.HasValue) entity.Rating = dto.Rating.Value;
            if (dto.Title != null) entity.Title = dto.Title;
            if (dto.Comment != null) entity.Comment = dto.Comment;
            if (dto.CleanlinessRating.HasValue) entity.CleanlinessRating = dto.CleanlinessRating.Value;
            if (dto.ServiceRating.HasValue) entity.ServiceRating = dto.ServiceRating.Value;
            if (dto.LocationRating.HasValue) entity.LocationRating = dto.LocationRating.Value;
            if (dto.ValueRating.HasValue) entity.ValueRating = dto.ValueRating.Value;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        #endregion

        #region Promotion Mappings

        public static PromotionDto ToDto(Promotion entity)
        {
            var now = DateTime.UtcNow;
            return new PromotionDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Name = entity.Name,
                Description = entity.Description,
                Code = entity.Code,
                Type = entity.Type,
                Status = entity.Status,
                DiscountValue = entity.DiscountValue,
                MaxDiscountAmount = entity.MaxDiscountAmount,
                MinBookingAmount = entity.MinBookingAmount,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                MaxUsageCount = entity.MaxUsageCount,
                CurrentUsageCount = entity.CurrentUsageCount,
                MaxUsagePerUser = entity.MaxUsagePerUser,
                BrandId = entity.BrandId,
                BrandName = entity.Brand?.Name,
                HotelId = entity.HotelId,
                HotelName = entity.Hotel?.Name,
                MinNights = entity.MinNights,
                MinDaysBeforeCheckIn = entity.MinDaysBeforeCheckIn,
                IsPublic = entity.IsPublic,
                IsValid = entity.Status == PromotionStatus.Active &&
                          entity.StartDate <= now &&
                          entity.EndDate >= now &&
                          (entity.MaxUsageCount == null || entity.CurrentUsageCount < entity.MaxUsageCount)
            };
        }

        public static Promotion ToEntity(CreatePromotionDto dto)
        {
            return new Promotion
            {
                Name = dto.Name,
                Description = dto.Description,
                Code = dto.Code.ToUpper(),
                Type = dto.Type,
                Status = PromotionStatus.Draft,
                DiscountValue = dto.DiscountValue,
                MaxDiscountAmount = dto.MaxDiscountAmount,
                MinBookingAmount = dto.MinBookingAmount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                MaxUsageCount = dto.MaxUsageCount,
                MaxUsagePerUser = dto.MaxUsagePerUser,
                BrandId = dto.BrandId,
                HotelId = dto.HotelId,
                MinNights = dto.MinNights,
                MinDaysBeforeCheckIn = dto.MinDaysBeforeCheckIn,
                IsPublic = dto.IsPublic,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(UpdatePromotionDto dto, Promotion entity)
        {
            if (dto.Name != null) entity.Name = dto.Name;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.Status.HasValue) entity.Status = dto.Status.Value;
            if (dto.DiscountValue.HasValue) entity.DiscountValue = dto.DiscountValue.Value;
            if (dto.MaxDiscountAmount.HasValue) entity.MaxDiscountAmount = dto.MaxDiscountAmount.Value;
            if (dto.MinBookingAmount.HasValue) entity.MinBookingAmount = dto.MinBookingAmount.Value;
            if (dto.StartDate.HasValue) entity.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) entity.EndDate = dto.EndDate.Value;
            if (dto.MaxUsageCount.HasValue) entity.MaxUsageCount = dto.MaxUsageCount.Value;
            if (dto.MaxUsagePerUser.HasValue) entity.MaxUsagePerUser = dto.MaxUsagePerUser.Value;
            if (dto.MinNights.HasValue) entity.MinNights = dto.MinNights.Value;
            if (dto.MinDaysBeforeCheckIn.HasValue) entity.MinDaysBeforeCheckIn = dto.MinDaysBeforeCheckIn.Value;
            if (dto.IsPublic.HasValue) entity.IsPublic = dto.IsPublic.Value;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        public static CouponDto ToDto(Coupon entity)
        {
            return new CouponDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                PromotionId = entity.PromotionId,
                PromotionName = entity.Promotion?.Name ?? "",
                Code = entity.Code,
                Status = entity.Status,
                AssignedToUserId = entity.AssignedToUserId,
                ExpiresAt = entity.ExpiresAt,
                DiscountApplied = entity.DiscountApplied,
                UsedAt = entity.UsedAt
            };
        }

        #endregion

        #region Wishlist Mappings

        public static WishlistDto ToDto(Wishlist entity)
        {
            return new WishlistDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                UserId = entity.UserId,
                HotelId = entity.HotelId,
                Note = entity.Note,
                Hotel = ToDto(entity.Hotel)
            };
        }

        public static Wishlist ToEntity(AddToWishlistDto dto, Guid userId)
        {
            return new Wishlist
            {
                UserId = userId,
                HotelId = dto.HotelId,
                Note = dto.Note,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        #endregion

        #region Notification Mappings

        public static NotificationDto ToDto(Notification entity)
        {
            return new NotificationDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Type = entity.Type,
                Channel = entity.Channel,
                Status = entity.Status,
                Title = entity.Title,
                Message = entity.Message,
                ActionUrl = entity.ActionUrl,
                BookingId = entity.BookingId,
                PromotionId = entity.PromotionId,
                ReadAt = entity.ReadAt
            };
        }

        public static Notification ToEntity(SendNotificationDto dto)
        {
            return new Notification
            {
                UserId = dto.UserId,
                Type = dto.Type,
                Channel = dto.Channel,
                Status = NotificationStatus.Pending,
                Title = dto.Title,
                Message = dto.Message,
                ActionUrl = dto.ActionUrl,
                BookingId = dto.BookingId,
                PromotionId = dto.PromotionId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        #endregion

        #region Guest Profile Mappings

        public static GuestProfileDto ToGuestProfileDto(User entity, int totalBookings, int completedStays, int totalReviews)
        {
            return new GuestProfileDto
            {
                Id = entity.Id,
                Email = entity.Email,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                PhoneNumber = entity.PhoneNumber,
                AvatarUrl = entity.AvatarUrl,
                Nationality = entity.Nationality,
                PreferredLanguage = entity.PreferredLanguage,
                PreferredCurrency = entity.PreferredCurrency,
                EmailNotificationsEnabled = entity.EmailNotificationsEnabled,
                SmsNotificationsEnabled = entity.SmsNotificationsEnabled,
                DateOfBirth = entity.DateOfBirth,
                Address = entity.Address,
                City = entity.City,
                Country = entity.Country,
                CreatedAt = entity.CreatedAt,
                LastLoginAt = entity.LastLoginAt,
                TotalBookings = totalBookings,
                CompletedStays = completedStays,
                TotalReviews = totalReviews
            };
        }

        public static RecentlyViewedHotelDto ToDto(RecentlyViewedHotel entity)
        {
            return new RecentlyViewedHotelDto
            {
                HotelId = entity.HotelId,
                HotelName = entity.Hotel?.Name ?? "",
                HotelImageUrl = entity.Hotel?.ImageUrl,
                City = entity.Hotel?.City,
                StarRating = entity.Hotel?.StarRating ?? 0,
                AverageRating = entity.Hotel?.AverageRating,
                MinPrice = entity.Hotel?.Rooms?.Any() == true
                    ? entity.Hotel.Rooms.Min(r => r.BasePrice)
                    : null,
                ViewedAt = entity.ViewedAt,
                ViewCount = entity.ViewCount
            };
        }

        public static void UpdateGuestPreferences(UpdateGuestPreferencesDto dto, User entity)
        {
            if (dto.PreferredLanguage != null) entity.PreferredLanguage = dto.PreferredLanguage;
            if (dto.PreferredCurrency != null) entity.PreferredCurrency = dto.PreferredCurrency;
            if (dto.EmailNotificationsEnabled.HasValue) entity.EmailNotificationsEnabled = dto.EmailNotificationsEnabled.Value;
            if (dto.SmsNotificationsEnabled.HasValue) entity.SmsNotificationsEnabled = dto.SmsNotificationsEnabled.Value;
            if (dto.Nationality != null) entity.Nationality = dto.Nationality;
            if (dto.IdDocumentType != null) entity.IdDocumentType = dto.IdDocumentType;
            if (dto.IdDocumentNumber != null) entity.IdDocumentNumber = dto.IdDocumentNumber;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        #endregion
    }
}
