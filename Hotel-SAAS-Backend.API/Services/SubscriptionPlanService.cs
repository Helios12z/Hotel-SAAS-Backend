using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Constants;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Services
{
    public class SubscriptionPlanService(ISubscriptionPlanRepository planRepository) : ISubscriptionPlanService
    {
        public async Task<SubscriptionPlanDto?> GetByIdAsync(Guid id)
        {
            var plan = await planRepository.GetByIdAsync(id);
            return plan == null ? null : MapToDto(plan);
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetAllAsync()
        {
            var plans = await planRepository.GetAllAsync();
            return plans.Select(MapToDto);
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetActiveAsync()
        {
            var plans = await planRepository.GetActiveAsync();
            return plans.Select(MapToDto);
        }

        public async Task<SubscriptionPlanDto> CreateAsync(CreateSubscriptionPlanDto dto)
        {
            var plan = new SubscriptionPlan
            {
                Name = dto.Name,
                Description = dto.Description,
                PlanType = dto.PlanType,
                MonthlyPrice = dto.MonthlyPrice,
                QuarterlyPrice = dto.QuarterlyPrice,
                YearlyPrice = dto.YearlyPrice,
                Currency = dto.Currency,
                MaxHotels = dto.MaxHotels,
                MaxRoomsPerHotel = dto.MaxRoomsPerHotel,
                MaxUsersPerHotel = dto.MaxUsersPerHotel,
                CommissionRate = dto.CommissionRate,
                HasAnalytics = dto.HasAnalytics,
                HasAdvancedReporting = dto.HasAdvancedReporting,
                HasApiAccess = dto.HasApiAccess,
                HasPrioritySupport = dto.HasPrioritySupport,
                HasChannelManager = dto.HasChannelManager,
                HasRevenueManagement = dto.HasRevenueManagement,
                HasMultiLanguage = dto.HasMultiLanguage,
                HasCustomBranding = dto.HasCustomBranding,
                HasDedicatedAccountManager = dto.HasDedicatedAccountManager,
                TrialDays = dto.TrialDays,
                IsPopular = dto.IsPopular,
                SortOrder = dto.SortOrder,
                IsActive = true
            };

            var created = await planRepository.CreateAsync(plan);
            return MapToDto(created);
        }

        public async Task<SubscriptionPlanDto> UpdateAsync(Guid id, UpdateSubscriptionPlanDto dto)
        {
            var plan = await planRepository.GetByIdAsync(id) 
                ?? throw new Exception(Messages.Subscription.PlanNotFound);

            if (dto.Name != null) plan.Name = dto.Name;
            if (dto.Description != null) plan.Description = dto.Description;
            if (dto.MonthlyPrice.HasValue) plan.MonthlyPrice = dto.MonthlyPrice.Value;
            if (dto.QuarterlyPrice.HasValue) plan.QuarterlyPrice = dto.QuarterlyPrice.Value;
            if (dto.YearlyPrice.HasValue) plan.YearlyPrice = dto.YearlyPrice.Value;
            if (dto.MaxHotels.HasValue) plan.MaxHotels = dto.MaxHotels.Value;
            if (dto.MaxRoomsPerHotel.HasValue) plan.MaxRoomsPerHotel = dto.MaxRoomsPerHotel.Value;
            if (dto.MaxUsersPerHotel.HasValue) plan.MaxUsersPerHotel = dto.MaxUsersPerHotel.Value;
            if (dto.CommissionRate.HasValue) plan.CommissionRate = dto.CommissionRate.Value;
            if (dto.HasAnalytics.HasValue) plan.HasAnalytics = dto.HasAnalytics.Value;
            if (dto.HasAdvancedReporting.HasValue) plan.HasAdvancedReporting = dto.HasAdvancedReporting.Value;
            if (dto.HasApiAccess.HasValue) plan.HasApiAccess = dto.HasApiAccess.Value;
            if (dto.HasPrioritySupport.HasValue) plan.HasPrioritySupport = dto.HasPrioritySupport.Value;
            if (dto.HasChannelManager.HasValue) plan.HasChannelManager = dto.HasChannelManager.Value;
            if (dto.HasRevenueManagement.HasValue) plan.HasRevenueManagement = dto.HasRevenueManagement.Value;
            if (dto.HasMultiLanguage.HasValue) plan.HasMultiLanguage = dto.HasMultiLanguage.Value;
            if (dto.HasCustomBranding.HasValue) plan.HasCustomBranding = dto.HasCustomBranding.Value;
            if (dto.HasDedicatedAccountManager.HasValue) plan.HasDedicatedAccountManager = dto.HasDedicatedAccountManager.Value;
            if (dto.TrialDays.HasValue) plan.TrialDays = dto.TrialDays.Value;
            if (dto.IsActive.HasValue) plan.IsActive = dto.IsActive.Value;
            if (dto.IsPopular.HasValue) plan.IsPopular = dto.IsPopular.Value;
            if (dto.SortOrder.HasValue) plan.SortOrder = dto.SortOrder.Value;

            plan.UpdatedAt = DateTime.UtcNow;

            var updated = await planRepository.UpdateAsync(plan);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await planRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            var plan = await planRepository.GetByIdAsync(id) 
                ?? throw new Exception(Messages.Subscription.PlanNotFound);
            
            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.UtcNow;
            await planRepository.UpdateAsync(plan);
            return plan.IsActive;
        }

        private static SubscriptionPlanDto MapToDto(SubscriptionPlan plan) => new()
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            PlanType = plan.PlanType,
            MonthlyPrice = plan.MonthlyPrice,
            QuarterlyPrice = plan.QuarterlyPrice,
            YearlyPrice = plan.YearlyPrice,
            Currency = plan.Currency,
            MaxHotels = plan.MaxHotels,
            MaxRoomsPerHotel = plan.MaxRoomsPerHotel,
            MaxUsersPerHotel = plan.MaxUsersPerHotel,
            CommissionRate = plan.CommissionRate,
            HasAnalytics = plan.HasAnalytics,
            HasAdvancedReporting = plan.HasAdvancedReporting,
            HasApiAccess = plan.HasApiAccess,
            HasPrioritySupport = plan.HasPrioritySupport,
            HasChannelManager = plan.HasChannelManager,
            HasRevenueManagement = plan.HasRevenueManagement,
            HasMultiLanguage = plan.HasMultiLanguage,
            HasCustomBranding = plan.HasCustomBranding,
            HasDedicatedAccountManager = plan.HasDedicatedAccountManager,
            TrialDays = plan.TrialDays,
            IsActive = plan.IsActive,
            IsPopular = plan.IsPopular
        };
    }
}
