using Hotel_SAAS_Backend.API.Models.Constants;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            // Apply migrations if needed
            await context.Database.EnsureCreatedAsync();

            // Check if data already exists
            if (await context.Users.AnyAsync())
            {
                // Seed subscription plans if not exists
                await SeedSubscriptionPlansAsync(context);
                return; // Database has been seeded
            }

            // ============================================
            // 0. SEED SUBSCRIPTION PLANS
            // ============================================
            await SeedSubscriptionPlansAsync(context);

            // ============================================
            // 1. SEED USERS
            // ============================================
            var adminUser = new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Email = "admin@hotelsaas.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FirstName = "Super",
                LastName = "Admin",
                Role = UserRole.SuperAdmin,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Users.AddAsync(adminUser);

            // Brand Admins
            var marriottAdmin = new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Email = "marriott.admin@hotelsaas.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Marriott123!"),
                FirstName = "John",
                LastName = "Marriott",
                Role = UserRole.BrandAdmin,
                Status = UserStatus.Active,
                BrandId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Users.AddAsync(marriottAdmin);

            var hiltonAdmin = new User
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Email = "hilton.admin@hotelsaas.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Hilton123!"),
                FirstName = "Sarah",
                LastName = "Hilton",
                Role = UserRole.BrandAdmin,
                Status = UserStatus.Active,
                BrandId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Users.AddAsync(hiltonAdmin);

            // Hotel Managers
            var hotelManager1 = new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Email = "manager.ny@marriott.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                FirstName = "Michael",
                LastName = "Johnson",
                Role = UserRole.HotelManager,
                Status = UserStatus.Active,
                BrandId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), // Marriott brand for this manager
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Users.AddAsync(hotelManager1);

            // Test Guests
            var guestUser = new User
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                Email = "guest@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Guest123!"),
                FirstName = "Jane",
                LastName = "Doe",
                Role = UserRole.Guest,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Users.AddAsync(guestUser);

            // ============================================
            // 2. SEED AMENITIES
            // ============================================
            var amenities = new List<Amenity>
            {
                // General
                new() { Name = "Free WiFi", Type = AmenityType.General, Icon = "wifi", Description = "High-speed wireless internet throughout the property", IsActive = true },
                new() { Name = "Parking", Type = AmenityType.General, Icon = "car", Description = "Complimentary on-site parking", IsActive = true },
                new() { Name = "Pet Friendly", Type = AmenityType.General, Icon = "paw", Description = "Pets allowed with additional fee", IsActive = true },

                // Facilities
                new() { Name = "Swimming Pool", Type = AmenityType.Facilities, Icon = "pool", Description = "Indoor heated pool", IsActive = true },
                new() { Name = "Fitness Center", Type = AmenityType.Facilities, Icon = "dumbbell", Description = "24/7 state-of-the-art gym", IsActive = true },
                new() { Name = "Spa", Type = AmenityType.Facilities, Icon = "spa", Description = "Full-service spa and wellness center", IsActive = true },
                new() { Name = "Restaurant", Type = AmenityType.Facilities, Icon = "utensils", Description = "On-site fine dining restaurant", IsActive = true },
                new() { Name = "Bar/Lounge", Type = AmenityType.Facilities, Icon = "glass", Description = "Rooftop bar with city views", IsActive = true },
                new() { Name = "Business Center", Type = AmenityType.Facilities, Icon = "briefcase", Description = "Fully equipped business center", IsActive = true },
                new() { Name = "Meeting Rooms", Type = AmenityType.Facilities, Icon = "users", Description = "Conference and meeting facilities", IsActive = true },
                new() { Name = "Airport Shuttle", Type = AmenityType.Facilities, Icon = "plane", Description = "Complimentary airport shuttle service", IsActive = true },

                // Room Amenities
                new() { Name = "Air Conditioning", Type = AmenityType.Room, Icon = "snowflake", Description = "Climate control in all rooms", IsActive = true },
                new() { Name = "Smart TV", Type = AmenityType.Room, Icon = "tv", Description = "55-inch 4K smart TV with streaming", IsActive = true },
                new() { Name = "Mini Bar", Type = AmenityType.Room, Icon = "glass", Description = "Stocked mini bar", IsActive = true },
                new() { Name = "Coffee Maker", Type = AmenityType.Kitchen, Icon = "coffee", Description = "Nespresso coffee machine", IsActive = true },
                new() { Name = "Safe", Type = AmenityType.Room, Icon = "lock", Description = "In-room electronic safe", IsActive = true },
                new() { Name = "Balcony", Type = AmenityType.Room, Icon = "sun", Description = "Private balcony with views", IsActive = true },
                new() { Name = "Bathtub", Type = AmenityType.Room, Icon = "bath", Description = "Luxury soaking tub", IsActive = true },
                new() { Name = "Hair Dryer", Type = AmenityType.Room, Icon = "wind", Description = "Professional hair dryer", IsActive = true },
                new() { Name = "Iron & Ironing Board", Type = AmenityType.Room, Icon = "shirt", Description = "In-room ironing facilities", IsActive = true },

                // Services
                new() { Name = "Room Service", Type = AmenityType.Service, Icon = "concierge", Description = "24-hour room service", IsActive = true },
                new() { Name = "24/7 Reception", Type = AmenityType.Service, Icon = "clock", Description = "Round-the-clock front desk", IsActive = true },
                new() { Name = "Concierge", Type = AmenityType.Service, Icon = "bell", Description = "Professional concierge service", IsActive = true },
                new() { Name = "Laundry Service", Type = AmenityType.Service, Icon = "tshirt", Description = "Same-day laundry service", IsActive = true },
                new() { Name = "Daily Housekeeping", Type = AmenityType.Service, Icon = "broom", Description = "Daily housekeeping included", IsActive = true },
                new() { Name = "Wake-up Call", Type = AmenityType.Service, Icon = "phone", Description = "Complimentary wake-up calls", IsActive = true }
            };
            await context.Amenities.AddRangeAsync(amenities);

            await context.SaveChangesAsync();

            // ============================================
            // 3. SEED BRANDS
            // ============================================
            var marriottBrand = new Brand
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "Marriott Hotels & Resorts",
                Description = "World-renowned hospitality brand offering exceptional service and luxurious accommodations across the globe.",
                LogoUrl = null,
                Website = "https://www.marriott.com",
                PhoneNumber = "+1-800-228-9290",
                Email = "info@marriott.com",
                Address = "1041 Fernwood Road, Bethesda, MD 20817, USA",
                City = "Bethesda",
                Country = "United States",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Brands.AddAsync(marriottBrand);

            var hiltonBrand = new Brand
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Name = "Hilton Worldwide",
                Description = "Leading hospitality company with over 6,000 properties across 119 countries, delivering exceptional experiences.",
                LogoUrl = null,
                Website = "https://www.hilton.com",
                PhoneNumber = "+1-800-445-8667",
                Email = "info@hilton.com",
                Address = "7930 Jones Branch Drive, McLean, VA 22102, USA",
                City = "McLean",
                Country = "United States",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Brands.AddAsync(hiltonBrand);

            var interconBrand = new Brand
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Name = "InterContinental Hotels Group",
                Description = "IHG Hotels & Resorts is a global hospitality company with a wide range of luxury and boutique brands.",
                LogoUrl = null,
                Website = "https://www.ihg.com",
                PhoneNumber = "+1-877-424-2449",
                Email = "info@ihg.com",
                Address = "Three Ravinia Drive, Atlanta, GA 30346, USA",
                City = "Atlanta",
                Country = "United States",
                IsActive = true,
                CommissionRate = "15",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Brands.AddAsync(interconBrand);

            var accorBrand = new Brand
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Name = "Accor Hotels",
                Description = "A world-leading hospitality group offering unique experiences in more than 5,000 hotels across 110 countries.",
                LogoUrl = null,
                Website = "https://all.accor.com",
                PhoneNumber = "+33-1-45-38-86-00",
                Email = "info@accor.com",
                Address = "82 Rue Henri Farman, 92130 Issy-les-Moulineaux, France",
                City = "Paris",
                Country = "France",
                IsActive = true,
                CommissionRate = "15",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Brands.AddAsync(accorBrand);

            await context.SaveChangesAsync();

            // ============================================
            // 3.5 SEED SUBSCRIPTIONS
            // ============================================
            var subscriptions = new List<Subscription>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    BrandId = marriottBrand.Id,
                    PlanId = Guid.Parse("11111111-0000-0000-0000-000000000003"), // Premium
                    Status = SubscriptionStatus.Active,
                    StartDate = DateTime.UtcNow.AddMonths(-2),
                    EndDate = DateTime.UtcNow.AddMonths(10),
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    BrandId = hiltonBrand.Id,
                    PlanId = Guid.Parse("11111111-0000-0000-0000-000000000002"), // Professional
                    Status = SubscriptionStatus.Active,
                    StartDate = DateTime.UtcNow.AddMonths(-1),
                    EndDate = DateTime.UtcNow.AddMonths(11),
                    CreatedAt = DateTime.UtcNow.AddMonths(-1)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    BrandId = interconBrand.Id,
                    PlanId = Guid.Parse("11111111-0000-0000-0000-000000000001"), // Basic
                    Status = SubscriptionStatus.Active,
                    StartDate = DateTime.UtcNow.AddDays(-15),
                    EndDate = DateTime.UtcNow.AddDays(350),
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    BrandId = accorBrand.Id,
                    PlanId = Guid.Parse("11111111-0000-0000-0000-000000000001"), // Basic (Trial)
                    Status = SubscriptionStatus.Trial,
                    StartDate = DateTime.UtcNow.AddDays(-5),
                    EndDate = DateTime.UtcNow.AddDays(25),
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                }
            };
            await context.Subscriptions.AddRangeAsync(subscriptions);
            await context.SaveChangesAsync();

            // ============================================
            // 4. SEED HOTELS
            // ============================================
            var nyHotel = new Hotel
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                BrandId = marriottBrand.Id,
                Name = "Marriott Marquis Times Square",
                Description = "Luxury hotel in the heart of Times Square with stunning city views, world-class dining, and exceptional service. Perfect for business travelers and tourists alike.",
                ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800",
                CoverImageUrl = "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=1200",
                Address = "1535 Broadway, New York, NY 10036",
                City = "New York",
                State = "New York",
                Country = "United States",
                PostalCode = "10036",
                Latitude = 40.7580,
                Longitude = -73.9855,
                PhoneNumber = "+1-212-398-1900",
                Email = "reservations.ny@marriott.com",
                Website = "https://www.marriott.com/hotels/travel/nycmq-marriott-marquis-times-square",
                StarRating = 5,
                IsActive = true,
                IsVerified = true,
                CheckInTime = "15:00",
                CheckOutTime = "11:00",
                CancellationPolicy = "Free cancellation up to 48 hours before check-in",
                ChildPolicy = "Children under 12 stay free with existing bedding",
                PetPolicy = "Pets allowed with $150 non-refundable fee",
                TotalRooms = 1950,
                NumberOfFloors = 49,
                AverageRating = 4.5f,
                ReviewCount = 2847,
                YearBuilt = DateTime.SpecifyKind(new DateTime(1985, 1, 1), DateTimeKind.Utc),
                YearRenovated = DateTime.SpecifyKind(new DateTime(2023, 1, 1), DateTimeKind.Utc),
                Embedding = GenerateMockEmbedding(1024), // For AI search
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            hotelManager1.HotelId = nyHotel.Id;
            await context.Hotels.AddAsync(nyHotel);

            var laHotel = new Hotel
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                BrandId = marriottBrand.Id,
                Name = "Marriott Burbank Airport Hotel",
                Description = "Modern hotel conveniently located near Burbank Airport with complimentary shuttle, outdoor pool, fitness center, and meeting facilities.",
                ImageUrl = "https://images.unsplash.com/photo-1564501049412-61c2a3083791?w=800",
                CoverImageUrl = "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=1200",
                Address = "2500 Hollywood Way, Burbank, CA 91505",
                City = "Burbank",
                State = "California",
                Country = "United States",
                PostalCode = "91505",
                Latitude = 34.1975,
                Longitude = -118.3585,
                PhoneNumber = "+1-818-843-4344",
                Email = "reservations.burbank@marriott.com",
                Website = "https://www.marriott.com/hotels/travel/burca-marriott-burbank-airport-hotel",
                StarRating = 4,
                IsActive = true,
                IsVerified = true,
                CheckInTime = "15:00",
                CheckOutTime = "11:00",
                CancellationPolicy = "Free cancellation up to 24 hours before check-in",
                ChildPolicy = "Children under 12 stay free with existing bedding",
                PetPolicy = "Pets not allowed",
                TotalRooms = 289,
                NumberOfFloors = 7,
                AverageRating = 4.2f,
                ReviewCount = 1243,
                YearBuilt = DateTime.SpecifyKind(new DateTime(1990, 1, 1), DateTimeKind.Utc),
                YearRenovated = DateTime.SpecifyKind(new DateTime(2021, 1, 1), DateTimeKind.Utc),
                Embedding = GenerateMockEmbedding(1024),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Hotels.AddAsync(laHotel);

            var parisHotel = new Hotel
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                BrandId = hiltonBrand.Id,
                Name = "Hilton Paris Opera",
                Description = "Elegant 4-star hotel located in the heart of Paris, steps away from the Opera Garnier and major department stores. Classic French hospitality meets modern comfort.",
                ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800",
                CoverImageUrl = "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=1200",
                Address = "108 Rue Saint-Lazare, 75008 Paris",
                City = "Paris",
                Country = "France",
                PostalCode = "75008",
                Latitude = 48.8766,
                Longitude = 2.3246,
                PhoneNumber = "+33-1-40-16-66-66",
                Email = "reservations.paris@hilton.com",
                Website = "https://www.hilton.com/en/hotels/fr/paris/paropgi-hilton-paris-opera",
                StarRating = 4,
                IsActive = true,
                IsVerified = true,
                CheckInTime = "14:00",
                CheckOutTime = "12:00",
                CancellationPolicy = "Free cancellation up to 72 hours before check-in",
                ChildPolicy = "Children under 16 stay free with existing bedding",
                PetPolicy = "Small pets allowed with €50 fee",
                TotalRooms = 260,
                NumberOfFloors = 8,
                AverageRating = 4.3f,
                ReviewCount = 1876,
                YearBuilt = DateTime.SpecifyKind(new DateTime(1975, 1, 1), DateTimeKind.Utc),
                YearRenovated = DateTime.SpecifyKind(new DateTime(2022, 1, 1), DateTimeKind.Utc),
                Embedding = GenerateMockEmbedding(1024),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Hotels.AddAsync(parisHotel);

            var londonHotel = new Hotel
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                BrandId = interconBrand.Id,
                Name = "InterContinental London Park Lane",
                Description = "Located at one of the capital's most prestigious addresses, InterContinental London Park Lane presents modern luxury in the heart of Mayfair.",
                ImageUrl = "https://images.unsplash.com/photo-1517841905240-472988babdf9?w=800",
                CoverImageUrl = "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=1200",
                Address = "One Hamilton Place, Park Lane, London W1J 7QY",
                City = "London",
                Country = "United Kingdom",
                PostalCode = "W1J 7QY",
                Latitude = 51.5038,
                Longitude = -0.1506,
                PhoneNumber = "+44-20-7409-3131",
                Email = "reservations.london@ihg.com",
                Website = "https://www.ihg.com/intercontinental/hotels/gb/en/london/lonhb/hoteldetail",
                StarRating = 5,
                IsActive = true,
                IsVerified = true,
                CheckInTime = "15:00",
                CheckOutTime = "12:00",
                CancellationPolicy = "Free cancellation up to 24 hours before check-in",
                ChildPolicy = "Children stay free",
                PetPolicy = "Pets allowed",
                TotalRooms = 447,
                NumberOfFloors = 8,
                AverageRating = 4.7f,
                ReviewCount = 3210,
                YearBuilt = DateTime.SpecifyKind(new DateTime(1960, 1, 1), DateTimeKind.Utc),
                YearRenovated = DateTime.SpecifyKind(new DateTime(2020, 1, 1), DateTimeKind.Utc),
                Embedding = GenerateMockEmbedding(1024),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Hotels.AddAsync(londonHotel);

            var tokyoHotel = new Hotel
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                BrandId = accorBrand.Id,
                Name = "Sofitel Tokyo City",
                Description = "Experience the essence of French 'art de vivre' in the heart of Tokyo. A perfect blend of luxury and convenience.",
                ImageUrl = "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800",
                CoverImageUrl = "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=1200",
                Address = "1-1-1 City Center, Tokyo",
                City = "Tokyo",
                Country = "Japan",
                PostalCode = "100-0001",
                Latitude = 35.6895,
                Longitude = 139.6917,
                PhoneNumber = "+81-3-1234-5678",
                Email = "reservations.tokyo@sofitel.com",
                Website = "https://all.accor.com/sofitel/tokyo",
                StarRating = 5,
                IsActive = true,
                IsVerified = true,
                CheckInTime = "15:00",
                CheckOutTime = "11:00",
                TotalRooms = 350,
                NumberOfFloors = 25,
                AverageRating = 4.6f,
                ReviewCount = 1500,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Embedding = GenerateMockEmbedding(1024)
            };
            await context.Hotels.AddAsync(tokyoHotel);

            await context.SaveChangesAsync();

            // ============================================
            // 5. SEED HOTEL IMAGES
            // ============================================
            var nyHotelImages = new List<HotelImage>
            {
                new() { HotelId = nyHotel.Id, ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800", Caption = "Exterior view", IsPrimary = true, DisplayOrder = 1, Category = "exterior" },
                new() { HotelId = nyHotel.Id, ImageUrl = "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800", Caption = "Lobby area", IsPrimary = false, DisplayOrder = 2, Category = "lobby" },
                new() { HotelId = nyHotel.Id, ImageUrl = "https://images.unsplash.com/photo-1584132967334-10e028bd69f7?w=800", Caption = "Deluxe room", IsPrimary = false, DisplayOrder = 3, Category = "room" },
                new() { HotelId = nyHotel.Id, ImageUrl = "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800", Caption = "Fitness center", IsPrimary = false, DisplayOrder = 4, Category = "facilities" },
                new() { HotelId = nyHotel.Id, ImageUrl = "https://images.unsplash.com/photo-1559599746-8823b38544c6?w=800", Caption = "Rooftop bar", IsPrimary = false, DisplayOrder = 5, Category = "dining" }
            };
            await context.HotelImages.AddRangeAsync(nyHotelImages);

            await context.SaveChangesAsync();

            // ============================================
            // 6. SEED HOTEL AMENITIES
            // ============================================
            var nyHotelAmenities = new List<HotelAmenity>
            {
                new() { HotelId = nyHotel.Id, AmenityId = amenities.First(a => a.Name == "Free WiFi").Id, AdditionalCost = 0 },
                new() { HotelId = nyHotel.Id, AmenityId = amenities.First(a => a.Name == "Swimming Pool").Id, AdditionalCost = 0 },
                new() { HotelId = nyHotel.Id, AmenityId = amenities.First(a => a.Name == "Fitness Center").Id, AdditionalCost = 0 },
                new() { HotelId = nyHotel.Id, AmenityId = amenities.First(a => a.Name == "Spa").Id, AdditionalCost = 0 },
                new() { HotelId = nyHotel.Id, AmenityId = amenities.First(a => a.Name == "Restaurant").Id, AdditionalCost = 0 },
                new() { HotelId = nyHotel.Id, AmenityId = amenities.First(a => a.Name == "Bar/Lounge").Id, AdditionalCost = 0 },
                new() { HotelId = nyHotel.Id, AmenityId = amenities.First(a => a.Name == "Business Center").Id, AdditionalCost = 0 },
                new() { HotelId = nyHotel.Id, AmenityId = amenities.First(a => a.Name == "Meeting Rooms").Id, AdditionalCost = 1875000m }, // 1,875,000 VND
                new() { HotelId = nyHotel.Id, AmenityId = amenities.First(a => a.Name == "Room Service").Id, AdditionalCost = 0 },
                new() { HotelId = nyHotel.Id, AmenityId = amenities.First(a => a.Name == "Concierge").Id, AdditionalCost = 0 },
                new() { HotelId = nyHotel.Id, AmenityId = amenities.First(a => a.Name == "Airport Shuttle").Id, AdditionalCost = 0 },
                new() { HotelId = nyHotel.Id, AmenityId = amenities.First(a => a.Name == "Laundry Service").Id, AdditionalCost = 625000m } // 625,000 VND
            };
            await context.HotelAmenities.AddRangeAsync(nyHotelAmenities);

            await context.SaveChangesAsync();

            // ============================================
            // 7. SEED ROOMS
            // ============================================
            var rooms = new List<Room>();

            // NY Hotel Rooms - Standard
            for (int i = 1; i <= 20; i++)
            {
                rooms.Add(new Room
                {
                    Id = Guid.NewGuid(),
                    HotelId = nyHotel.Id,
                    RoomNumber = $"{300 + i}",
                    Floor = "3",
                    Type = RoomType.Standard,
                    BedType = BedType.Queen,
                    NumberOfBeds = 1,
                    MaxOccupancy = 2,
                    SizeInSquareMeters = 28,
                    BasePrice = 1500000m, // 1,500,000 VND
                    WeekendPrice = 1800000m, // 1,800,000 VND
                    HolidayPrice = 2200000m, // 2,200,000 VND
                    Status = RoomStatus.Available,
                    Description = "Comfortable standard room with queen bed, city views, and modern amenities.",
                    HasView = true,
                    ViewDescription = "City view",
                    SmokingAllowed = false,
                    IsPetFriendly = false,
                    HasConnectingRoom = i % 5 == 0,
                    IsAccessible = i % 10 == 0,
                    AccessibilityFeatures = i % 10 == 0 ? "Wheelchair accessible, grab bars in bathroom" : null,
                    Embedding = GenerateMockEmbedding(1024),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // NY Hotel Rooms - Deluxe
            for (int i = 1; i <= 15; i++)
            {
                rooms.Add(new Room
                {
                    Id = Guid.NewGuid(),
                    HotelId = nyHotel.Id,
                    RoomNumber = $"{400 + i}",
                    Floor = "4",
                    Type = RoomType.Deluxe,
                    BedType = BedType.King,
                    NumberOfBeds = 1,
                    MaxOccupancy = 2,
                    SizeInSquareMeters = 38,
                    BasePrice = 2500000m, // 2,500,000 VND
                    WeekendPrice = 3000000m, // 3,000,000 VND
                    HolidayPrice = 3800000m, // 3,800,000 VND
                    Status = RoomStatus.Available,
                    Description = "Spacious deluxe room with king bed, sitting area, and premium amenities.",
                    HasView = true,
                    ViewDescription = "Times Square view",
                    SmokingAllowed = false,
                    IsPetFriendly = true,
                    HasConnectingRoom = i % 3 == 0,
                    IsAccessible = i % 5 == 0,
                    AccessibilityFeatures = i % 5 == 0 ? "Fully accessible bathroom" : null,
                    Embedding = GenerateMockEmbedding(1024),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // NY Hotel Rooms - Suite
            for (int i = 1; i <= 10; i++)
            {
                rooms.Add(new Room
                {
                    Id = Guid.NewGuid(),
                    HotelId = nyHotel.Id,
                    RoomNumber = $"{500 + i}",
                    Floor = "5",
                    Type = RoomType.Suite,
                    BedType = BedType.King,
                    NumberOfBeds = 1,
                    MaxOccupancy = 3,
                    SizeInSquareMeters = 55,
                    BasePrice = 4500000m, // 4,500,000 VND
                    WeekendPrice = 5500000m, // 5,500,000 VND
                    HolidayPrice = 7000000m, // 7,000,000 VND
                    Status = RoomStatus.Available,
                    Description = "Luxury suite with separate living room, king bedroom, premium amenities, and stunning views.",
                    HasView = true,
                    ViewDescription = "Panoramic city view",
                    SmokingAllowed = false,
                    IsPetFriendly = true,
                    HasConnectingRoom = i % 2 == 0,
                    IsAccessible = i % 5 == 0,
                    AccessibilityFeatures = i % 5 == 0 ? "Full accessibility with roll-in shower" : null,
                    Embedding = GenerateMockEmbedding(1024),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await context.Rooms.AddRangeAsync(rooms);
            await context.SaveChangesAsync();

            // ============================================
            // 8. SEED ROOM IMAGES
            // ============================================
            var roomImages = new List<RoomImage>();
            var roomTypes = new[] { "Standard", "Deluxe", "Suite" };
            var imageUrls = new[]
            {
                "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800",
                "https://images.unsplash.com/photo-1590490360182-c33d57733427?w=800",
                "https://images.unsplash.com/photo-1584132967334-10e028bd69f7?w=800"
            };

            int imageIndex = 0;
            foreach (var room in rooms.Take(30))
            {
                roomImages.Add(new RoomImage
                {
                    RoomId = room.Id,
                    ImageUrl = imageUrls[imageIndex % 3],
                    Caption = $"{room.Type} room view",
                    IsPrimary = true,
                    DisplayOrder = 1
                });
                imageIndex++;
            }

            await context.RoomImages.AddRangeAsync(roomImages);
            await context.SaveChangesAsync();

            // ============================================
            // 9. SEED ROOM AMENITIES
            // ============================================
            var roomAmenities = new List<RoomAmenity>();
            var allRoomAmenities = amenities.Where(a =>
                a.Type == AmenityType.Room ||
                a.Type == AmenityType.Kitchen).ToList();

            foreach (var room in rooms)
            {
                // All rooms get basic amenities
                roomAmenities.Add(new RoomAmenity
                {
                    RoomId = room.Id,
                    AmenityId = amenities.First(a => a.Name == "Air Conditioning").Id,
                    AdditionalCost = 0
                });
                roomAmenities.Add(new RoomAmenity
                {
                    RoomId = room.Id,
                    AmenityId = amenities.First(a => a.Name == "Smart TV").Id,
                    AdditionalCost = 0
                });
                roomAmenities.Add(new RoomAmenity
                {
                    RoomId = room.Id,
                    AmenityId = amenities.First(a => a.Name == "Coffee Maker").Id,
                    AdditionalCost = 0
                });
                roomAmenities.Add(new RoomAmenity
                {
                    RoomId = room.Id,
                    AmenityId = amenities.First(a => a.Name == "Safe").Id,
                    AdditionalCost = 0
                });

                // Deluxe and Suite rooms get additional amenities
                if (room.Type == RoomType.Deluxe || room.Type == RoomType.Suite)
                {
                    roomAmenities.Add(new RoomAmenity
                    {
                        RoomId = room.Id,
                        AmenityId = amenities.First(a => a.Name == "Mini Bar").Id,
                        AdditionalCost = 0
                    });
                    roomAmenities.Add(new RoomAmenity
                    {
                        RoomId = room.Id,
                        AmenityId = amenities.First(a => a.Name == "Balcony").Id,
                        AdditionalCost = 0
                    });
                    roomAmenities.Add(new RoomAmenity
                    {
                        RoomId = room.Id,
                        AmenityId = amenities.First(a => a.Name == "Bathtub").Id,
                        AdditionalCost = 0
                    });
                }
            }

            await context.RoomAmenities.AddRangeAsync(roomAmenities);
            await context.SaveChangesAsync();

            // ============================================
            // 10. SEED SAMPLE BOOKINGS, PAYMENTS & REVIEWS
            // ============================================
            var random = new Random();
            var hotels = new List<Hotel> { nyHotel, laHotel, parisHotel, londonHotel, tokyoHotel };
            var bookingStatuses = new[] { BookingStatus.Confirmed, BookingStatus.CheckedIn, BookingStatus.CheckedOut, BookingStatus.Cancelled };
            var paymentMethods = new[] { "CreditCard", "BankTransfer", "Cash" };
            
            foreach (var hotel in hotels)
            {
                // Generate ~20 bookings for each hotel in the last 30 days
                for (int i = 0; i < 25; i++)
                {
                    var bookedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30));
                    var checkInDate = bookedAt.AddDays(random.Next(1, 14));
                    var checkOutDate = checkInDate.AddDays(random.Next(1, 7));
                    var status = bookingStatuses[random.Next(bookingStatuses.Length)];
                    var subtotal = random.Next(100, 1000) * 10000m; // 1M to 10M VND
                    var totalAmount = subtotal * 1.11m; // Including tax and fees

                    var bookingId = Guid.NewGuid();
                    var bookingRecord = new Booking
                    {
                        Id = bookingId,
                        HotelId = hotel.Id,
                        GuestId = guestUser.Id,
                        ConfirmationNumber = "BK" + random.Next(100000, 999999),
                        CheckInDate = checkInDate,
                        CheckOutDate = checkOutDate,
                        NumberOfGuests = random.Next(1, 4),
                        NumberOfRooms = 1,
                        Subtotal = subtotal,
                        TaxAmount = subtotal * 0.1m,
                        ServiceFee = subtotal * 0.05m,
                        TotalAmount = totalAmount,
                        Currency = "VND",
                        Status = status,
                        GuestName = i % 2 == 0 ? "John Smith" : "David Wilson",
                        GuestEmail = i % 2 == 0 ? "john@example.com" : "david@example.com",
                        GuestPhoneNumber = "+1-555-" + random.Next(1000, 9999),
                        PaymentMethod = paymentMethods[random.Next(paymentMethods.Length)],
                        IsPaid = status != BookingStatus.Cancelled,
                        BookedAt = bookedAt,
                        ConfirmedAt = bookedAt.AddMinutes(random.Next(5, 60)),
                        CreatedAt = bookedAt,
                        UpdatedAt = bookedAt
                    };
                    await context.Bookings.AddAsync(bookingRecord);

                    // Add Payment for paid bookings
                    if (bookingRecord.IsPaid)
                    {
                        var paymentRecord = new Payment
                        {
                            Id = Guid.NewGuid(),
                            BookingId = bookingId,
                            TransactionId = "TXN" + random.Next(1000000, 9999999),
                            Amount = totalAmount,
                            Currency = "VND",
                            Method = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), bookingRecord.PaymentMethod),
                            Status = PaymentStatus.Completed,
                            ProcessedAt = bookedAt.AddMinutes(random.Next(60, 120)),
                            Gateway = "VNPay",
                            CreatedAt = bookedAt,
                            UpdatedAt = bookedAt
                        };
                        await context.Payments.AddAsync(paymentRecord);
                    }

                    // Add Review for checked out bookings
                    if (status == BookingStatus.CheckedOut && random.Next(0, 2) == 1)
                    {
                        var rating = random.Next(3, 6);
                        var reviewRecord = new Review
                        {
                            Id = Guid.NewGuid(),
                            HotelId = hotel.Id,
                            GuestId = guestUser.Id,
                            BookingId = bookingId,
                            Rating = rating,
                            Title = rating >= 4 ? "Great Experience" : "Good stay",
                            Comment = "Generated review for testing dashboard charts and ratings.",
                            CleanlinessRating = random.Next(3, 6),
                            ServiceRating = random.Next(3, 6),
                            LocationRating = random.Next(4, 6),
                            ValueRating = random.Next(3, 6),
                            IsVerified = true,
                            StayDate = checkInDate,
                            Status = ReviewStatus.Approved,
                            PublishedAt = checkOutDate.AddDays(random.Next(1, 3)),
                            CreatedAt = checkOutDate.AddDays(random.Next(1, 3)),
                            UpdatedAt = checkOutDate.AddDays(random.Next(1, 3)),
                            Embedding = GenerateMockEmbedding(1024)
                        };
                        await context.Reviews.AddAsync(reviewRecord);
                    }
                }
            }

            // Save all changes
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Seed default subscription plans
        /// </summary>
        private static async Task SeedSubscriptionPlansAsync(ApplicationDbContext context)
        {
            if (await context.SubscriptionPlans.AnyAsync())
            {
                return; // Plans already seeded
            }

            var plans = new List<SubscriptionPlan>
            {
                new()
                {
                    Id = Guid.Parse("11111111-0000-0000-0000-000000000001"),
                    Name = "Basic",
                    Description = "Hoàn hảo cho các khách sạn nhỏ độc lập mới bắt đầu sử dụng nền tảng.",
                    PlanType = SubscriptionPlanType.Basic,
                    MonthlyPrice = 725000m, // 725,000 VND (~$29)
                    QuarterlyPrice = 1975000m, // 1,975,000 VND (~$79) - 10% discount
                    YearlyPrice = 7250000m, // 7,250,000 VND (~$290) - 20% discount
                    Currency = "VND",
                    MaxHotels = 1,
                    MaxRoomsPerHotel = 20,
                    MaxUsersPerHotel = 3,
                    CommissionRate = 15,
                    IsActive = true,
                    IsPopular = false,
                    SortOrder = 1
                },
                new()
                {
                    Id = Guid.Parse("11111111-0000-0000-0000-000000000002"),
                    Name = "Professional",
                    Description = "Lý tưởng cho các khách sạn đang phát triển cần nhiều tính năng hơn và mức hoa hồng tốt hơn.",
                    PlanType = SubscriptionPlanType.Standard,
                    MonthlyPrice = 1975000m, // 1,975,000 VND (~$79)
                    QuarterlyPrice = 4975000m, // 4,975,000 VND (~$199) - 10% discount
                    YearlyPrice = 19750000m, // 19,750,000 VND (~$790) - 20% discount
                    Currency = "VND",
                    MaxHotels = 3,
                    MaxRoomsPerHotel = 100,
                    MaxUsersPerHotel = 10,
                    CommissionRate = 12,
                    IsActive = true,
                    IsPopular = true, // Most popular plan
                    SortOrder = 2
                },
                new()
                {
                    Id = Guid.Parse("11111111-0000-0000-0000-000000000003"),
                    Name = "Premium",
                    Description = "Dành cho các khách sạn lớn cần tính năng nâng cao và mức hoa hồng thấp nhất.",
                    PlanType = SubscriptionPlanType.Premium,
                    MonthlyPrice = 4975000m, // 4,975,000 VND (~$199)
                    QuarterlyPrice = 12475000m, // 12,475,000 VND (~$499) - 10% discount
                    YearlyPrice = 49750000m, // 49,750,000 VND (~$1,990) - 20% discount
                    Currency = "VND",
                    MaxHotels = 999,
                    MaxRoomsPerHotel = 999,
                    MaxUsersPerHotel = 999,
                    CommissionRate = 8,
                    IsActive = true,
                    IsPopular = false,
                    SortOrder = 3
                }
            };

            await context.SubscriptionPlans.AddRangeAsync(plans);
            await context.SaveChangesAsync();

            // ============================================
            // 13. SEED PERMISSIONS & ROLE PERMISSIONS
            // ============================================
            await SeedPermissionsAsync(context);
        }

        /// <summary>
        /// Seed default permissions and role-permission mappings
        /// </summary>
        private static async Task SeedPermissionsAsync(ApplicationDbContext context)
        {
            if (await context.Permissions.AnyAsync())
            {
                return; // Permissions already seeded
            }

            // Seed Permissions
            var permissions = new List<Permission>
            {
                // Hotels
                new() { Name = Permissions.Hotels.Create, Description = "Create hotels", Resource = "hotels", Action = "create" },
                new() { Name = Permissions.Hotels.Read, Description = "View hotels", Resource = "hotels", Action = "read" },
                new() { Name = Permissions.Hotels.Update, Description = "Update hotels", Resource = "hotels", Action = "update" },
                new() { Name = Permissions.Hotels.Delete, Description = "Delete hotels", Resource = "hotels", Action = "delete" },

                // Rooms
                new() { Name = Permissions.Rooms.Create, Description = "Create rooms", Resource = "rooms", Action = "create" },
                new() { Name = Permissions.Rooms.Read, Description = "View rooms", Resource = "rooms", Action = "read" },
                new() { Name = Permissions.Rooms.Update, Description = "Update rooms", Resource = "rooms", Action = "update" },
                new() { Name = Permissions.Rooms.Delete, Description = "Delete rooms", Resource = "rooms", Action = "delete" },

                // Bookings
                new() { Name = Permissions.Bookings.Create, Description = "Create bookings", Resource = "bookings", Action = "create" },
                new() { Name = Permissions.Bookings.Read, Description = "View bookings", Resource = "bookings", Action = "read" },
                new() { Name = Permissions.Bookings.Update, Description = "Update bookings", Resource = "bookings", Action = "update" },
                new() { Name = Permissions.Bookings.Delete, Description = "Delete bookings", Resource = "bookings", Action = "delete" },
                new() { Name = Permissions.Bookings.CheckIn, Description = "Check-in guests", Resource = "bookings", Action = "checkin" },
                new() { Name = Permissions.Bookings.CheckOut, Description = "Check-out guests", Resource = "bookings", Action = "checkout" },

                // Users
                new() { Name = Permissions.Users.Create, Description = "Create users", Resource = "users", Action = "create" },
                new() { Name = Permissions.Users.Read, Description = "View users", Resource = "users", Action = "read" },
                new() { Name = Permissions.Users.Update, Description = "Update users", Resource = "users", Action = "update" },
                new() { Name = Permissions.Users.Delete, Description = "Delete users", Resource = "users", Action = "delete" },

                // Brands
                new() { Name = Permissions.Brands.Create, Description = "Create brands", Resource = "brands", Action = "create" },
                new() { Name = Permissions.Brands.Read, Description = "View brands", Resource = "brands", Action = "read" },
                new() { Name = Permissions.Brands.Update, Description = "Update brands", Resource = "brands", Action = "update" },
                new() { Name = Permissions.Brands.Delete, Description = "Delete brands", Resource = "brands", Action = "delete" },

                // Subscriptions
                new() { Name = Permissions.Subscriptions.Read, Description = "View subscriptions", Resource = "subscriptions", Action = "read" },
                new() { Name = Permissions.Subscriptions.Update, Description = "Update subscriptions", Resource = "subscriptions", Action = "update" },

                // Dashboard
                new() { Name = Permissions.Dashboard.View, Description = "View dashboard", Resource = "dashboard", Action = "view" },
                new() { Name = Permissions.Dashboard.ViewAll, Description = "View all dashboards", Resource = "dashboard", Action = "viewall" },

                // Reports
                new() { Name = Permissions.Reports.Revenue, Description = "View revenue reports", Resource = "reports", Action = "revenue" },
                new() { Name = Permissions.Reports.Bookings, Description = "View bookings reports", Resource = "reports", Action = "bookings" },
                new() { Name = Permissions.Reports.Occupancy, Description = "View occupancy reports", Resource = "reports", Action = "occupancy" },
                new() { Name = Permissions.Reports.Inventory, Description = "View inventory reports", Resource = "reports", Action = "inventory" },
                new() { Name = Permissions.Reports.Hotel, Description = "View full hotel reports", Resource = "reports", Action = "hotel" },
                new() { Name = Permissions.Reports.Export, Description = "Export reports", Resource = "reports", Action = "export" }
            };

            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();

            // Seed Role-Permission mappings
            var rolePermissions = new List<RolePermission>();

            // SuperAdmin - All system permissions (not hotel-specific)
            var superAdminPermissions = permissions
                .Where(p => p.Resource != "hotel")
                .Select(p => new RolePermission { Role = UserRole.SuperAdmin, PermissionId = p.Id });
            rolePermissions.AddRange(superAdminPermissions);

            // BrandAdmin - Hotels, Users within brand
            rolePermissions.AddRange(new[]
            {
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Brands.Read).Id },
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Create).Id },
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Read).Id },
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Update).Id },
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Delete).Id },
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Users.Create).Id },
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Users.Read).Id },
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Users.Update).Id },
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Users.Delete).Id },
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Dashboard.View).Id },
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Reports.Revenue).Id },
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Reports.Bookings).Id },
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Reports.Occupancy).Id },
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Reports.Inventory).Id },
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Reports.Hotel).Id },
                new RolePermission { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Reports.Export).Id }
            });

            // HotelManager - Rooms, Bookings within assigned hotel
            rolePermissions.AddRange(new[]
            {
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Read).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Rooms.Create).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Rooms.Read).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Rooms.Update).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Rooms.Delete).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Bookings.Create).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Bookings.Read).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Bookings.Update).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Bookings.Delete).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Bookings.CheckIn).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Bookings.CheckOut).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Users.Create).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Users.Read).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Users.Update).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Users.Delete).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Dashboard.View).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Reports.Revenue).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Reports.Bookings).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Reports.Occupancy).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Reports.Inventory).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Reports.Hotel).Id },
                new RolePermission { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Reports.Export).Id }
            });

            // Receptionist - Check-in/out and read access
            rolePermissions.AddRange(new[]
            {
                new RolePermission { Role = UserRole.Receptionist, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Read).Id },
                new RolePermission { Role = UserRole.Receptionist, PermissionId = permissions.First(p => p.Name == Permissions.Rooms.Read).Id },
                new RolePermission { Role = UserRole.Receptionist, PermissionId = permissions.First(p => p.Name == Permissions.Bookings.Read).Id },
                new RolePermission { Role = UserRole.Receptionist, PermissionId = permissions.First(p => p.Name == Permissions.Bookings.CheckIn).Id },
                new RolePermission { Role = UserRole.Receptionist, PermissionId = permissions.First(p => p.Name == Permissions.Bookings.CheckOut).Id }
            });

            // Staff - Read only
            rolePermissions.AddRange(new[]
            {
                new RolePermission { Role = UserRole.Staff, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Read).Id },
                new RolePermission { Role = UserRole.Staff, PermissionId = permissions.First(p => p.Name == Permissions.Rooms.Read).Id },
                new RolePermission { Role = UserRole.Staff, PermissionId = permissions.First(p => p.Name == Permissions.Bookings.Read).Id }
            });

            await context.RolePermissions.AddRangeAsync(rolePermissions);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Generate mock embedding vector for seed data
        /// In production, this would be generated by the embedding service
        /// </summary>
        private static float[] GenerateMockEmbedding(int dimension)
        {
            var random = new Random();
            var embedding = new float[dimension];
            for (int i = 0; i < dimension; i++)
            {
                // Generate normalized random values between -1 and 1
                embedding[i] = (float)(random.NextDouble() * 2 - 1);
            }

            // Normalize the vector
            var magnitude = Math.Sqrt(embedding.Sum(x => x * x));
            for (int i = 0; i < dimension; i++)
            {
                embedding[i] = (float)(embedding[i] / magnitude);
            }

            return embedding;
        }
    }
}
