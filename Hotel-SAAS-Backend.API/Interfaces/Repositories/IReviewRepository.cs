using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        Task<Review?> GetByIdAsync(Guid id);
        Task<IEnumerable<Review>> GetByHotelAsync(Guid hotelId);
        Task<IEnumerable<Review>> GetByGuestAsync(Guid guestId);
        Task<IEnumerable<Review>> GetByStatusAsync(ReviewStatus status);
        Task<IEnumerable<Review>> GetVerifiedReviewsAsync(Guid hotelId);
        Task<Review> CreateAsync(Review review);
        Task<Review> UpdateAsync(Review review);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> HasReviewedAsync(Guid guestId, Guid bookingId);
    }
}
