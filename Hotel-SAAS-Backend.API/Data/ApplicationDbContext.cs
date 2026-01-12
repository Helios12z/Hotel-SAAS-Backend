using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Hotel_SAAS_Backend.API.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        // Brands
        public DbSet<Brand> Brands { get; set; }

        // Hotels
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<HotelImage> HotelImages { get; set; }
        public DbSet<HotelAmenity> HotelAmenities { get; set; }

        // Rooms
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }
        public DbSet<RoomAmenity> RoomAmenities { get; set; }
        public DbSet<RoomPricing> RoomPricing { get; set; }

        // Amenities
        public DbSet<Amenity> Amenities { get; set; }

        // Users
        public DbSet<User> Users { get; set; }

        // Bookings
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingRoom> BookingRooms { get; set; }

        // Payments
        public DbSet<Payment> Payments { get; set; }

        // Reviews
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewImage> ReviewImages { get; set; }

        // Facets for search/filter
        public DbSet<Facet> Facets { get; set; }
        public DbSet<FacetValue> FacetValues { get; set; }

        // AI Chat & Knowledge Base
        public DbSet<BotConversation> BotConversations { get; set; }
        public DbSet<BotMessage> BotMessages { get; set; }
        public DbSet<KnowledgeDocument> KnowledgeDocuments { get; set; }
        public DbSet<KnowledgeChunk> KnowledgeChunks { get; set; }

        // Promotions & Coupons
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<UserPromotion> UserPromotions { get; set; }

        // Wishlist
        public DbSet<Wishlist> Wishlists { get; set; }

        // Notifications
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }

        // Guest Profile
        public DbSet<RecentlyViewedHotel> RecentlyViewedHotels { get; set; }

        // Subscriptions
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionInvoice> SubscriptionInvoices { get; set; }

        // Hotel Onboarding
        public DbSet<HotelOnboarding> HotelOnboardings { get; set; }
        public DbSet<OnboardingDocument> OnboardingDocuments { get; set; }

        // Permissions
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserHotelPermission> UserHotelPermissions { get; set; }

        // Additional Charges (Late checkout, minibar, etc.)
        public DbSet<AdditionalCharge> AdditionalCharges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Brand Configuration
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("brands");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(50);
                entity.Property(e => e.Website).HasMaxLength(500);
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.Email);
            });

            // Hotel Configuration
            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.ToTable("hotels");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(50);
                entity.Property(e => e.Website).HasMaxLength(500);
                entity.Property(e => e.StarRating).HasDefaultValue(3);
                entity.Property(e => e.CheckInTime).HasMaxLength(5).HasDefaultValue("14:00");
                entity.Property(e => e.CheckOutTime).HasMaxLength(5).HasDefaultValue("12:00");
                entity.Property(e => e.StripeAccountId).HasMaxLength(100);
                entity.Property(e => e.TaxRate).HasPrecision(5, 4).HasDefaultValue(0.10m);
                entity.Property(e => e.ServiceFeeRate).HasPrecision(5, 4).HasDefaultValue(0.05m);
                entity.Property(e => e.ExtraBedPrice).HasPrecision(18, 2);
                entity.Property(e => e.CommissionRate).HasPrecision(5, 2);
                entity.HasIndex(e => e.BrandId);
                entity.HasIndex(e => e.City);
                entity.HasIndex(e => e.Country);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.AverageRating);

                entity.HasOne(e => e.Brand)
                    .WithMany(b => b.Hotels)
                    .HasForeignKey(e => e.BrandId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Hotel Image Configuration
            modelBuilder.Entity<HotelImage>(entity =>
            {
                entity.ToTable("hotel_images");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(1000);
                entity.HasIndex(e => e.HotelId);
                entity.HasIndex(e => e.IsPrimary);

                entity.HasOne(e => e.Hotel)
                    .WithMany(h => h.Images)
                    .HasForeignKey(e => e.HotelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Room Configuration
            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("rooms");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RoomNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.BasePrice).HasPrecision(18, 2);
                entity.Property(e => e.WeekendPrice).HasPrecision(18, 2);
                entity.Property(e => e.HolidayPrice).HasPrecision(18, 2);
                entity.HasIndex(e => e.HotelId);
                entity.HasIndex(e => e.RoomNumber);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.HotelId, e.RoomNumber }).IsUnique();

                entity.HasOne(e => e.Hotel)
                    .WithMany(h => h.Rooms)
                    .HasForeignKey(e => e.HotelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ConnectingRoom)
                    .WithMany()
                    .HasForeignKey(e => e.ConnectingRoomId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Room Image Configuration
            modelBuilder.Entity<RoomImage>(entity =>
            {
                entity.ToTable("room_images");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(1000);
                entity.HasIndex(e => e.RoomId);

                entity.HasOne(e => e.Room)
                    .WithMany(r => r.Images)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Amenity Configuration
            modelBuilder.Entity<Amenity>(entity =>
            {
                entity.ToTable("amenities");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.IsActive);
            });

            // Hotel Amenity Configuration
            modelBuilder.Entity<HotelAmenity>(entity =>
            {
                entity.ToTable("hotel_amenities");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AdditionalCost).HasPrecision(18, 2);
                entity.HasIndex(e => e.HotelId);
                entity.HasIndex(e => e.AmenityId);

                entity.HasOne(e => e.Hotel)
                    .WithMany(h => h.Amenities)
                    .HasForeignKey(e => e.HotelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Amenity)
                    .WithMany(a => a.HotelAmenities)
                    .HasForeignKey(e => e.AmenityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Room Amenity Configuration
            modelBuilder.Entity<RoomAmenity>(entity =>
            {
                entity.ToTable("room_amenities");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AdditionalCost).HasPrecision(18, 2);
                entity.HasIndex(e => e.RoomId);
                entity.HasIndex(e => e.AmenityId);

                entity.HasOne(e => e.Room)
                    .WithMany(r => r.Amenities)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Amenity)
                    .WithMany(a => a.RoomAmenities)
                    .HasForeignKey(e => e.AmenityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(50);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Role);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.BrandId);
                entity.HasIndex(e => e.HotelId);

                entity.HasOne(e => e.Brand)
                    .WithMany()
                    .HasForeignKey(e => e.BrandId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Hotel)
                    .WithMany()
                    .HasForeignKey(e => e.HotelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Booking Configuration
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("bookings");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ConfirmationNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Subtotal).HasPrecision(18, 2);
                entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
                entity.Property(e => e.ServiceFee).HasPrecision(18, 2);
                entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.GuestEmail).HasMaxLength(255);
                entity.Property(e => e.GuestPhoneNumber).HasMaxLength(50);
                entity.HasIndex(e => e.ConfirmationNumber).IsUnique();
                entity.HasIndex(e => e.HotelId);
                entity.HasIndex(e => e.GuestId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CheckInDate);
                entity.HasIndex(e => e.CheckOutDate);

                entity.HasOne(e => e.Hotel)
                    .WithMany(h => h.Bookings)
                    .HasForeignKey(e => e.HotelId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Guest)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(e => e.GuestId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Booking Room Configuration
            modelBuilder.Entity<BookingRoom>(entity =>
            {
                entity.ToTable("booking_rooms");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.HasIndex(e => e.BookingId);
                entity.HasIndex(e => e.RoomId);

                entity.HasOne(e => e.Booking)
                    .WithMany(b => b.BookingRooms)
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Room)
                    .WithMany(r => r.BookingRooms)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Payment Configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TransactionId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.RefundAmount).HasPrecision(18, 2);
                entity.Property(e => e.CardLast4Digits).HasMaxLength(4);
                entity.HasIndex(e => e.TransactionId).IsUnique();
                entity.HasIndex(e => e.BookingId);
                entity.HasIndex(e => e.Status);

                entity.HasOne(e => e.Booking)
                    .WithMany(b => b.Payments)
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Review Configuration
            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("reviews");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Comment).IsRequired();
                entity.Property(e => e.Title).HasMaxLength(500);
                entity.HasIndex(e => e.HotelId);
                entity.HasIndex(e => e.GuestId);
                entity.HasIndex(e => e.BookingId);
                entity.HasIndex(e => e.Rating);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.IsVerified);

                entity.HasOne(e => e.Hotel)
                    .WithMany(h => h.Reviews)
                    .HasForeignKey(e => e.HotelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Guest)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(e => e.GuestId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Booking)
                    .WithMany()
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Review Image Configuration
            modelBuilder.Entity<ReviewImage>(entity =>
            {
                entity.ToTable("review_images");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(1000);
                entity.HasIndex(e => e.ReviewId);

                entity.HasOne(e => e.Review)
                    .WithMany(r => r.Images)
                    .HasForeignKey(e => e.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Room Pricing Configuration
            modelBuilder.Entity<RoomPricing>(entity =>
            {
                entity.ToTable("room_pricing");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.HasIndex(e => e.RoomId);
                entity.HasIndex(e => e.StartDate);
                entity.HasIndex(e => e.EndDate);
                entity.HasIndex(e => e.IsActive);

                entity.HasOne(e => e.Room)
                    .WithMany()
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Facet Configuration
            modelBuilder.Entity<Facet>(entity =>
            {
                entity.ToTable("facets");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.IsActive);
            });

            // Facet Value Configuration
            modelBuilder.Entity<FacetValue>(entity =>
            {
                entity.ToTable("facet_values");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Value).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.HasIndex(e => e.FacetId);

                entity.HasOne(e => e.Facet)
                    .WithMany(f => f.Values)
                    .HasForeignKey(e => e.FacetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // BotConversation Configuration
            modelBuilder.Entity<BotConversation>(entity =>
            {
                entity.ToTable("bot_conversations");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.GuestIdentifier).HasMaxLength(200);
                entity.HasIndex(e => e.OwnerId);
                entity.HasIndex(e => e.GuestIdentifier);
                entity.HasIndex(e => e.UpdatedAt);
            });

            // BotMessage Configuration
            modelBuilder.Entity<BotMessage>(entity =>
            {
                entity.ToTable("bot_messages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.SourceReferences).HasColumnType("jsonb");
                entity.HasIndex(e => e.ConversationId);
                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(e => e.Conversation)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(e => e.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // KnowledgeDocument Configuration
            modelBuilder.Entity<KnowledgeDocument>(entity =>
            {
                entity.ToTable("knowledge_documents");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Source).HasMaxLength(1000);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.IsActive);
            });

            // KnowledgeChunk Configuration
            modelBuilder.Entity<KnowledgeChunk>(entity =>
            {
                entity.ToTable("knowledge_chunks");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Metadata).HasColumnType("jsonb");
                entity.HasIndex(e => e.DocumentId);
                entity.HasIndex(e => e.ChunkIndex);

                entity.HasOne(e => e.Document)
                    .WithMany(d => d.Chunks)
                    .HasForeignKey(e => e.DocumentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Promotion Configuration
            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.ToTable("promotions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DiscountValue).HasPrecision(18, 2);
                entity.Property(e => e.MaxDiscountAmount).HasPrecision(18, 2);
                entity.Property(e => e.MinBookingAmount).HasPrecision(18, 2);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.StartDate);
                entity.HasIndex(e => e.EndDate);

                entity.HasOne(e => e.Brand)
                    .WithMany()
                    .HasForeignKey(e => e.BrandId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Hotel)
                    .WithMany()
                    .HasForeignKey(e => e.HotelId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Coupon Configuration
            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.ToTable("coupons");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DiscountApplied).HasPrecision(18, 2);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.PromotionId);
                entity.HasIndex(e => e.Status);

                entity.HasOne(e => e.Promotion)
                    .WithMany(p => p.Coupons)
                    .HasForeignKey(e => e.PromotionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.AssignedToUser)
                    .WithMany()
                    .HasForeignKey(e => e.AssignedToUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.UsedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.UsedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.UsedInBooking)
                    .WithMany()
                    .HasForeignKey(e => e.UsedInBookingId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // UserPromotion Configuration
            modelBuilder.Entity<UserPromotion>(entity =>
            {
                entity.ToTable("user_promotions");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.PromotionId }).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Promotion)
                    .WithMany()
                    .HasForeignKey(e => e.PromotionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Wishlist Configuration
            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.ToTable("wishlists");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Note).HasMaxLength(500);
                entity.HasIndex(e => new { e.UserId, e.HotelId }).IsUnique();
                entity.HasIndex(e => e.UserId);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Hotel)
                    .WithMany()
                    .HasForeignKey(e => e.HotelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Notification Configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notifications");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.ActionUrl).HasMaxLength(1000);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Booking)
                    .WithMany()
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Promotion)
                    .WithMany()
                    .HasForeignKey(e => e.PromotionId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // EmailTemplate Configuration
            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.ToTable("email_templates");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Body).IsRequired();
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // RecentlyViewedHotel Configuration
            modelBuilder.Entity<RecentlyViewedHotel>(entity =>
            {
                entity.ToTable("recently_viewed_hotels");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.HotelId }).IsUnique();
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.ViewedAt);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Hotel)
                    .WithMany()
                    .HasForeignKey(e => e.HotelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // SubscriptionPlan Configuration
            modelBuilder.Entity<SubscriptionPlan>(entity =>
            {
                entity.ToTable("subscription_plans");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.MonthlyPrice).HasPrecision(18, 2);
                entity.Property(e => e.QuarterlyPrice).HasPrecision(18, 2);
                entity.Property(e => e.YearlyPrice).HasPrecision(18, 2);
                entity.Property(e => e.CommissionRate).HasPrecision(5, 2);
                entity.Property(e => e.Currency).HasMaxLength(10);
                entity.HasIndex(e => e.PlanType);
                entity.HasIndex(e => e.IsActive);
            });

            // Subscription Configuration
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.ToTable("subscriptions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.DiscountPercentage).HasPrecision(5, 2);
                entity.Property(e => e.Currency).HasMaxLength(10);
                entity.Property(e => e.CancellationReason).HasMaxLength(1000);
                entity.HasIndex(e => e.BrandId);
                entity.HasIndex(e => e.PlanId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.EndDate);

                entity.HasOne(e => e.Brand)
                    .WithMany()
                    .HasForeignKey(e => e.BrandId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Plan)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(e => e.PlanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SubscriptionInvoice Configuration
            modelBuilder.Entity<SubscriptionInvoice>(entity =>
            {
                entity.ToTable("subscription_invoices");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Subtotal).HasPrecision(18, 2);
                entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
                entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.Currency).HasMaxLength(10);
                entity.Property(e => e.TransactionId).HasMaxLength(100);
                entity.Property(e => e.InvoicePdfUrl).HasMaxLength(1000);
                entity.HasIndex(e => e.InvoiceNumber).IsUnique();
                entity.HasIndex(e => e.SubscriptionId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.DueDate);

                entity.HasOne(e => e.Subscription)
                    .WithMany(s => s.Invoices)
                    .HasForeignKey(e => e.SubscriptionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // HotelOnboarding Configuration
            modelBuilder.Entity<HotelOnboarding>(entity =>
            {
                entity.ToTable("hotel_onboardings");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BrandName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.HotelName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.City).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ContactName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ContactEmail).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ContactPhone).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LegalBusinessName).IsRequired().HasMaxLength(300);
                entity.Property(e => e.TaxId).HasMaxLength(100);
                entity.Property(e => e.BusinessRegistrationNumber).HasMaxLength(100);
                entity.Property(e => e.BankName).HasMaxLength(200);
                entity.Property(e => e.BankAccountName).HasMaxLength(200);
                entity.Property(e => e.BankAccountNumber).HasMaxLength(100);
                entity.Property(e => e.BankRoutingNumber).HasMaxLength(100);
                entity.Property(e => e.BankSwiftCode).HasMaxLength(50);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.HasIndex(e => e.ApplicantId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.SubmittedAt);
                entity.HasIndex(e => e.Country);
                entity.HasIndex(e => e.City);

                entity.HasOne(e => e.Applicant)
                    .WithMany()
                    .HasForeignKey(e => e.ApplicantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ExistingBrand)
                    .WithMany()
                    .HasForeignKey(e => e.ExistingBrandId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.SelectedPlan)
                    .WithMany()
                    .HasForeignKey(e => e.SelectedPlanId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.ReviewedBy)
                    .WithMany()
                    .HasForeignKey(e => e.ReviewedById)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.ApprovedBy)
                    .WithMany()
                    .HasForeignKey(e => e.ApprovedById)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // OnboardingDocument Configuration
            modelBuilder.Entity<OnboardingDocument>(entity =>
            {
                entity.ToTable("onboarding_documents");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FileUrl).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.FileType).HasMaxLength(100);
                entity.Property(e => e.ReviewNotes).HasMaxLength(1000);
                entity.Property(e => e.RejectionReason).HasMaxLength(500);
                entity.HasIndex(e => e.OnboardingId);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.Status);

                entity.HasOne(e => e.Onboarding)
                    .WithMany(o => o.Documents)
                    .HasForeignKey(e => e.OnboardingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ReviewedBy)
                    .WithMany()
                    .HasForeignKey(e => e.ReviewedById)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Permission Configuration
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("permissions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Resource).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.Resource);
            });

            // RolePermission Configuration
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("role_permissions");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.Role, e.PermissionId }).IsUnique();

                entity.HasOne(e => e.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // UserHotelPermission Configuration
            modelBuilder.Entity<UserHotelPermission>(entity =>
            {
                entity.ToTable("user_hotel_permissions");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.HotelId, e.PermissionId }).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Permission)
                    .WithMany(p => p.UserHotelPermissions)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // AdditionalCharge Configuration
            modelBuilder.Entity<AdditionalCharge>(entity =>
            {
                entity.ToTable("additional_charges");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Amount).HasPrecision(18, 2);

                entity.HasOne(e => e.Booking)
                    .WithMany(b => b.AdditionalCharges)
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
