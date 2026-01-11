using AutoMapper;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Mapping
{
    public class HotelMinPriceResolver : IValueResolver<Hotel, HotelDto, decimal?>
    {
        public decimal? Resolve(Hotel source, HotelDto destination, decimal? destMember, ResolutionContext context)
        {
            return source.Rooms != null && source.Rooms.Any() ? source.Rooms.Min(r => r.BasePrice) : null;
        }
    }

    namespace Hotel_SAAS_Backend.API.Mapping
    {
        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                // User mappings
                CreateMap<User, UserDto>()
                    .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BrandId))
                    .ForMember(dest => dest.HotelId, opt => opt.MapFrom(src => src.HotelId));

                CreateMap<RegisterDto, User>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => UserStatus.Active));

                // Brand mappings
                CreateMap<Brand, BrandDto>()
                    .ForMember(dest => dest.HotelCount, opt => opt.MapFrom(src => src.Hotels != null ? src.Hotels.Count : 0));

                CreateMap<CreateBrandDto, Brand>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

                CreateMap<UpdateBrandDto, Brand>();

                // Hotel mappings
                CreateMap<Hotel, HotelDto>()
                    .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : ""))
                    .ForMember(dest => dest.MinPrice, opt => opt.MapFrom<HotelMinPriceResolver>());

                CreateMap<Hotel, HotelDetailDto>()
                    .IncludeBase<Hotel, HotelDto>()
                    .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : ""))
                    .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                    .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => src.Amenities != null ? src.Amenities.Select(ha => ha.Amenity) : Enumerable.Empty<Amenity>()))
                    .ForMember(dest => dest.RecentReviews, opt => opt.MapFrom(src => src.Reviews.Take(10)));

                CreateMap<CreateHotelDto, Hotel>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                    .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => false));

                CreateMap<UpdateHotelDto, Hotel>();

                // HotelImage mappings
                CreateMap<HotelImage, HotelImageDto>();
                CreateMap<HotelImageDto, HotelImage>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

                // Room mappings
                CreateMap<Room, RoomDto>()
                    .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel != null ? src.Hotel.Name : ""));

                CreateMap<Room, RoomDetailDto>()
                    .IncludeBase<Room, RoomDto>()
                    .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel != null ? src.Hotel.Name : ""))
                    .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Images != null && src.Images.Any(i => i.IsPrimary) ? src.Images.First(i => i.IsPrimary).ImageUrl : null))
                    .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                    .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => src.Amenities != null ? src.Amenities.Select(ra => ra.Amenity) : Enumerable.Empty<Amenity>()));

                CreateMap<RoomImage, RoomImageDto>();

                CreateMap<CreateRoomDto, Room>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => RoomStatus.Available));

                CreateMap<UpdateRoomDto, Room>();

                // Amenity mappings
                CreateMap<Amenity, AmenityDto>();

                CreateMap<CreateAmenityDto, Amenity>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

                CreateMap<UpdateAmenityDto, Amenity>();

                // Booking mappings
                CreateMap<Booking, BookingDto>()
                    .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel != null ? src.Hotel.Name : ""))
                    .ForMember(dest => dest.GuestName, opt => opt.MapFrom(src => src.GuestName ?? src.Guest.FirstName + " " + src.Guest.LastName))
                    .ForMember(dest => dest.GuestEmail, opt => opt.MapFrom(src => src.GuestEmail ?? src.Guest.Email));

                CreateMap<Booking, BookingDetailDto>()
                    .IncludeBase<Booking, BookingDto>()
                    .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel != null ? src.Hotel.Name : ""))
                    .ForMember(dest => dest.GuestName, opt => opt.MapFrom(src => src.GuestName ?? src.Guest.FirstName + " " + src.Guest.LastName))
                    .ForMember(dest => dest.GuestEmail, opt => opt.MapFrom(src => src.GuestEmail ?? src.Guest.Email))
                    .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src => src.BookingRooms))
                    .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments))
                    .ForMember(dest => dest.HotelImageUrl, opt => opt.MapFrom(src => src.Hotel != null ? src.Hotel.ImageUrl : null))
                    .ForMember(dest => dest.HotelAddress, opt => opt.MapFrom(src => src.Hotel != null ? src.Hotel.Address : null))
                    .ForMember(dest => dest.HotelCity, opt => opt.MapFrom(src => src.Hotel != null ? src.Hotel.City : null))
                    .ForMember(dest => dest.HotelPhoneNumber, opt => opt.MapFrom(src => src.Hotel != null ? src.Hotel.PhoneNumber : null));

                CreateMap<BookingRoom, BookingRoomDetailDto>()
                    .ForMember(dest => dest.RoomType, opt => opt.MapFrom(src => src.Room != null ? src.Room.Type : RoomType.Standard));

                CreateMap<CreateBookingDto, Booking>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => BookingStatus.Pending));

                CreateMap<UpdateBookingDto, Booking>();

                // Payment mappings
                CreateMap<Payment, PaymentDto>();

                CreateMap<CreatePaymentDto, Payment>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => PaymentStatus.Pending));

                // Review mappings
                CreateMap<Review, ReviewDto>()
                    .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel != null ? src.Hotel.Name : ""))
                    .ForMember(dest => dest.GuestName, opt => opt.MapFrom(src => src.Guest.FirstName + " " + src.Guest.LastName))
                    .ForMember(dest => dest.GuestAvatarUrl, opt => opt.MapFrom(src => src.Guest.AvatarUrl))
                    .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images));

                CreateMap<ReviewImage, ReviewImageDto>();

                CreateMap<CreateReviewDto, Review>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ReviewStatus.Pending));

                CreateMap<UpdateReviewDto, Review>();
            }
        }
    }
}
