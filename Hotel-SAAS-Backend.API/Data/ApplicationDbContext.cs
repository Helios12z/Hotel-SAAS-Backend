using Hotel_SAAS_Backend.API.Models.Entities;
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
        }
    }
}
