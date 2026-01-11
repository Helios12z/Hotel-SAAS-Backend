using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Services
{
    public class ReviewService(IReviewRepository reviewRepository, IHotelRepository hotelRepository) : IReviewService
    {
        public async Task<ReviewDto?> GetReviewByIdAsync(Guid id)
        {
            var review = await reviewRepository.GetByIdAsync(id);
            return review == null ? null : Mapper.ToDto(review);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByHotelAsync(Guid hotelId, int page = 1, int pageSize = 10)
        {
            var reviews = await reviewRepository.GetByHotelAsync(hotelId);
            return reviews
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(Mapper.ToDto);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByGuestAsync(Guid guestId)
        {
            var reviews = await reviewRepository.GetByGuestAsync(guestId);
            return reviews.Select(Mapper.ToDto);
        }

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto)
        {
            var review = new Review
            {
                Id = Guid.NewGuid(),
                HotelId = createReviewDto.HotelId,
                GuestId = Guid.Empty, // Will be set from context
                BookingId = createReviewDto.BookingId,
                Rating = createReviewDto.Rating,
                Title = createReviewDto.Title,
                Comment = createReviewDto.Comment,
                CleanlinessRating = createReviewDto.CleanlinessRating,
                ServiceRating = createReviewDto.ServiceRating,
                LocationRating = createReviewDto.LocationRating,
                ValueRating = createReviewDto.ValueRating,
                IsVerified = createReviewDto.BookingId.HasValue,
                Status = ReviewStatus.Pending
            };

            var createdReview = await reviewRepository.CreateAsync(review);
            return Mapper.ToDto(createdReview);
        }

        public async Task<ReviewDto?> UpdateReviewAsync(Guid id, UpdateReviewDto updateReviewDto)
        {
            var review = await reviewRepository.GetByIdAsync(id);
            if (review == null) return null;

            Mapper.UpdateEntity(updateReviewDto, review);
            var updatedReview = await reviewRepository.UpdateAsync(review);
            return Mapper.ToDto(updatedReview);
        }

        public async Task<bool> DeleteReviewAsync(Guid id)
        {
            await reviewRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ApproveReviewAsync(Guid id)
        {
            var review = await reviewRepository.GetByIdAsync(id);
            if (review == null) return false;

            review.Status = ReviewStatus.Approved;
            review.PublishedAt = DateTime.UtcNow;
            await reviewRepository.UpdateAsync(review);

            // Update hotel rating
            await UpdateHotelRatingAsync(review.HotelId);
            return true;
        }

        public async Task<bool> RejectReviewAsync(Guid id)
        {
            var review = await reviewRepository.GetByIdAsync(id);
            if (review == null) return false;

            review.Status = ReviewStatus.Rejected;
            await reviewRepository.UpdateAsync(review);
            return true;
        }

        public async Task<bool> AddManagementResponseAsync(Guid id, string response)
        {
            var review = await reviewRepository.GetByIdAsync(id);
            if (review == null) return false;

            review.ManagementResponse = response;
            review.ManagementResponseAt = DateTime.UtcNow;
            await reviewRepository.UpdateAsync(review);
            return true;
        }

        private async Task UpdateHotelRatingAsync(Guid hotelId)
        {
            var reviews = await reviewRepository.GetByHotelAsync(hotelId);
            var approvedReviews = reviews.Where(r => r.Status == ReviewStatus.Approved).ToList();

            if (approvedReviews.Any())
            {
                var hotel = await hotelRepository.GetByIdAsync(hotelId);
                if (hotel != null)
                {
                    hotel.AverageRating = (float)approvedReviews.Average(r => r.Rating);
                    hotel.ReviewCount = approvedReviews.Count;
                    await hotelRepository.UpdateAsync(hotel);
                }
            }
        }
    }
}
