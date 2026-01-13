using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Services
{
    public class PromotionService(IPromotionRepository promotionRepository) : IPromotionService
    {
        public async Task<PromotionDto?> GetByIdAsync(Guid id)
        {
            var promotion = await promotionRepository.GetByIdAsync(id);
            return promotion == null ? null : Mapper.ToDto(promotion);
        }

        public async Task<PromotionDto?> GetByCodeAsync(string code)
        {
            var promotion = await promotionRepository.GetByCodeAsync(code);
            return promotion == null ? null : Mapper.ToDto(promotion);
        }

        public async Task<IEnumerable<PromotionDto>> GetAllAsync()
        {
            var promotions = await promotionRepository.GetAllAsync();
            return promotions.Select(Mapper.ToDto);
        }

        public async Task<IEnumerable<PromotionDto>> GetActivePromotionsAsync(Guid? brandId = null, Guid? hotelId = null)
        {
            var promotions = await promotionRepository.GetActivePromotionsAsync(brandId, hotelId);
            return promotions.Select(Mapper.ToDto);
        }

        public async Task<IEnumerable<PromotionDto>> GetPublicPromotionsAsync()
        {
            var promotions = await promotionRepository.GetPublicPromotionsAsync();
            return promotions.Select(Mapper.ToDto);
        }

        public async Task<PromotionDto> CreateAsync(CreatePromotionDto dto)
        {
            if (await promotionRepository.CodeExistsAsync(dto.Code))
                throw new InvalidOperationException(Hotel_SAAS_Backend.API.Models.Constants.Messages.Misc.PromotionExists);

            var promotion = Mapper.ToEntity(dto);
            var created = await promotionRepository.CreateAsync(promotion);
            return Mapper.ToDto(created);
        }

        public async Task<PromotionDto> UpdateAsync(Guid id, UpdatePromotionDto dto)
        {
            var promotion = await promotionRepository.GetByIdAsync(id);
            if (promotion == null)
                throw new KeyNotFoundException(Hotel_SAAS_Backend.API.Models.Constants.Messages.Misc.PromotionNotFound);

            Mapper.UpdateEntity(dto, promotion);
            var updated = await promotionRepository.UpdateAsync(promotion);
            return Mapper.ToDto(updated);
        }

        public async Task DeleteAsync(Guid id)
        {
            await promotionRepository.DeleteAsync(id);
        }

        public async Task<bool> ActivateAsync(Guid id)
        {
            var promotion = await promotionRepository.GetByIdAsync(id);
            if (promotion == null) return false;

            promotion.Status = PromotionStatus.Active;
            promotion.UpdatedAt = DateTime.UtcNow;
            await promotionRepository.UpdateAsync(promotion);
            return true;
        }

        public async Task<bool> PauseAsync(Guid id)
        {
            var promotion = await promotionRepository.GetByIdAsync(id);
            if (promotion == null) return false;

            promotion.Status = PromotionStatus.Paused;
            promotion.UpdatedAt = DateTime.UtcNow;
            await promotionRepository.UpdateAsync(promotion);
            return true;
        }

        public async Task<CouponValidationResultDto> ValidateCouponAsync(ValidateCouponDto dto, Guid userId)
        {
            var promotion = await promotionRepository.GetByCodeAsync(dto.Code);

            // Check if promotion exists
            if (promotion == null)
            {
                return new CouponValidationResultDto
                {
                    IsValid = false,
                    ErrorMessage = Hotel_SAAS_Backend.API.Models.Constants.Messages.Misc.CouponInvalid
                };
            }

            var now = DateTime.UtcNow;
            var numberOfNights = (int)(dto.CheckOutDate - dto.CheckInDate).TotalDays;
            var daysBeforeCheckIn = (int)(dto.CheckInDate - now).TotalDays;

            // Validate promotion status and dates
            if (promotion.Status != PromotionStatus.Active)
                return InvalidResult("This promotion is not active");

            if (now < promotion.StartDate)
                return InvalidResult("This promotion has not started yet");

            if (now > promotion.EndDate)
                return InvalidResult("This promotion has expired");

            // Check usage limits
            if (promotion.MaxUsageCount.HasValue && promotion.CurrentUsageCount >= promotion.MaxUsageCount)
                return InvalidResult("This promotion has reached its usage limit");

            // Check scope (brand/hotel)
            if (promotion.HotelId.HasValue && promotion.HotelId != dto.HotelId)
                return InvalidResult("This promotion is not valid for this hotel");

            // Check minimum booking amount
            if (promotion.MinBookingAmount.HasValue && dto.BookingAmount < promotion.MinBookingAmount)
                return InvalidResult($"Minimum booking amount is {promotion.MinBookingAmount:C}");

            // Check minimum nights
            if (promotion.MinNights.HasValue && numberOfNights < promotion.MinNights)
                return InvalidResult($"Minimum stay is {promotion.MinNights} nights");

            // Check early bird condition
            if (promotion.MinDaysBeforeCheckIn.HasValue && daysBeforeCheckIn < promotion.MinDaysBeforeCheckIn)
                return InvalidResult($"Book at least {promotion.MinDaysBeforeCheckIn} days in advance");

            // Calculate discount
            var discount = CalculateDiscount(promotion, dto.BookingAmount, numberOfNights);
            var finalAmount = dto.BookingAmount - discount;

            return new CouponValidationResultDto
            {
                IsValid = true,
                Code = promotion.Code,
                DiscountType = promotion.Type,
                DiscountValue = promotion.DiscountValue,
                CalculatedDiscount = discount,
                FinalAmount = finalAmount
            };
        }

        public async Task<decimal> CalculateDiscountAsync(string code, decimal bookingAmount, int numberOfNights)
        {
            var promotion = await promotionRepository.GetByCodeAsync(code);
            if (promotion == null) return 0;

            return CalculateDiscount(promotion, bookingAmount, numberOfNights);
        }

        private static CouponValidationResultDto InvalidResult(string message)
        {
            return new CouponValidationResultDto { IsValid = false, ErrorMessage = message };
        }

        private static decimal CalculateDiscount(Promotion promotion, decimal bookingAmount, int numberOfNights)
        {
            decimal discount = 0;

            switch (promotion.Type)
            {
                case PromotionType.Percentage:
                    discount = bookingAmount * (promotion.DiscountValue / 100);
                    if (promotion.MaxDiscountAmount.HasValue)
                        discount = Math.Min(discount, promotion.MaxDiscountAmount.Value);
                    break;

                case PromotionType.FixedAmount:
                    discount = promotion.DiscountValue;
                    break;

                case PromotionType.FreeNight:
                    // Example: Book 3 nights, get 1 free (DiscountValue = number of free nights per X nights)
                    var freeNights = numberOfNights / 4; // Every 4 nights, 1 free
                    if (numberOfNights > 0)
                        discount = (bookingAmount / numberOfNights) * freeNights;
                    break;

                case PromotionType.EarlyBird:
                case PromotionType.LastMinute:
                    discount = bookingAmount * (promotion.DiscountValue / 100);
                    if (promotion.MaxDiscountAmount.HasValue)
                        discount = Math.Min(discount, promotion.MaxDiscountAmount.Value);
                    break;
            }

            return Math.Min(discount, bookingAmount); // Discount khng v??t qu booking amount
        }
    }
}
