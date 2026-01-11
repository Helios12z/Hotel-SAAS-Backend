using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IReviewService
    {
        Task<ReviewDto?> GetReviewByIdAsync(Guid id);
        Task<IEnumerable<ReviewDto>> GetReviewsByHotelAsync(Guid hotelId, int page = 1, int pageSize = 10);
        Task<IEnumerable<ReviewDto>> GetReviewsByGuestAsync(Guid guestId);
        Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto);
        Task<ReviewDto?> UpdateReviewAsync(Guid id, UpdateReviewDto updateReviewDto);
        Task<bool> DeleteReviewAsync(Guid id);
        Task<bool> ApproveReviewAsync(Guid id);
        Task<bool> RejectReviewAsync(Guid id);
        Task<bool> AddManagementResponseAsync(Guid id, string response);
    }
}
