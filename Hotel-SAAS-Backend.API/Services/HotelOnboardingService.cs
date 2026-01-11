using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Services
{
    public class HotelOnboardingService(
        IHotelOnboardingRepository onboardingRepository,
        IOnboardingDocumentRepository documentRepository,
        ISubscriptionPlanRepository planRepository,
        ISubscriptionRepository subscriptionRepository,
        IBrandRepository brandRepository,
        IHotelRepository hotelRepository,
        IUserRepository userRepository,
        INotificationService notificationService) : IHotelOnboardingService
    {
        // Applicant operations
        public async Task<HotelOnboardingDto?> GetByIdAsync(Guid id)
        {
            var onboarding = await onboardingRepository.GetByIdWithDetailsAsync(id);
            return onboarding == null ? null : MapToDto(onboarding);
        }

        public async Task<IEnumerable<HotelOnboardingDto>> GetMyApplicationsAsync(Guid applicantId)
        {
            var applications = await onboardingRepository.GetByApplicantAsync(applicantId);
            return applications.Select(MapToDto);
        }

        public async Task<HotelOnboardingDto> CreateAsync(Guid applicantId, CreateOnboardingDto dto)
        {
            var onboarding = new HotelOnboarding
            {
                ApplicantId = applicantId,
                ExistingBrandId = dto.ExistingBrandId,
                BrandName = dto.BrandName,
                BrandDescription = dto.BrandDescription,
                BrandLogoUrl = dto.BrandLogoUrl,
                BrandWebsite = dto.BrandWebsite,
                HotelName = dto.HotelName,
                HotelDescription = dto.HotelDescription,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                StarRating = dto.StarRating,
                TotalRooms = dto.TotalRooms,
                NumberOfFloors = dto.NumberOfFloors,
                ContactName = dto.ContactName,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                ContactPosition = dto.ContactPosition,
                LegalBusinessName = dto.LegalBusinessName,
                TaxId = dto.TaxId,
                BusinessRegistrationNumber = dto.BusinessRegistrationNumber,
                BankName = dto.BankName,
                BankAccountName = dto.BankAccountName,
                BankAccountNumber = dto.BankAccountNumber,
                BankRoutingNumber = dto.BankRoutingNumber,
                BankSwiftCode = dto.BankSwiftCode,
                SelectedPlanId = dto.SelectedPlanId,
                SelectedBillingCycle = dto.SelectedBillingCycle,
                Status = OnboardingStatus.Draft
            };

            var created = await onboardingRepository.CreateAsync(onboarding);
            return MapToDto(await onboardingRepository.GetByIdWithDetailsAsync(created.Id)!);
        }

        public async Task<HotelOnboardingDto> UpdateAsync(Guid id, Guid applicantId, UpdateOnboardingDto dto)
        {
            var onboarding = await onboardingRepository.GetByIdAsync(id) 
                ?? throw new Exception("Onboarding application not found");

            if (onboarding.ApplicantId != applicantId)
                throw new Exception("You don't have permission to update this application");

            if (onboarding.Status != OnboardingStatus.Draft && onboarding.Status != OnboardingStatus.DocumentsRequired)
                throw new Exception("Application cannot be updated in current status");

            // Update fields
            if (dto.BrandName != null) onboarding.BrandName = dto.BrandName;
            if (dto.BrandDescription != null) onboarding.BrandDescription = dto.BrandDescription;
            if (dto.BrandLogoUrl != null) onboarding.BrandLogoUrl = dto.BrandLogoUrl;
            if (dto.BrandWebsite != null) onboarding.BrandWebsite = dto.BrandWebsite;
            if (dto.HotelName != null) onboarding.HotelName = dto.HotelName;
            if (dto.HotelDescription != null) onboarding.HotelDescription = dto.HotelDescription;
            if (dto.Address != null) onboarding.Address = dto.Address;
            if (dto.City != null) onboarding.City = dto.City;
            if (dto.State != null) onboarding.State = dto.State;
            if (dto.Country != null) onboarding.Country = dto.Country;
            if (dto.PostalCode != null) onboarding.PostalCode = dto.PostalCode;
            if (dto.Latitude.HasValue) onboarding.Latitude = dto.Latitude.Value;
            if (dto.Longitude.HasValue) onboarding.Longitude = dto.Longitude.Value;
            if (dto.StarRating.HasValue) onboarding.StarRating = dto.StarRating.Value;
            if (dto.TotalRooms.HasValue) onboarding.TotalRooms = dto.TotalRooms.Value;
            if (dto.NumberOfFloors.HasValue) onboarding.NumberOfFloors = dto.NumberOfFloors.Value;
            if (dto.ContactName != null) onboarding.ContactName = dto.ContactName;
            if (dto.ContactEmail != null) onboarding.ContactEmail = dto.ContactEmail;
            if (dto.ContactPhone != null) onboarding.ContactPhone = dto.ContactPhone;
            if (dto.ContactPosition != null) onboarding.ContactPosition = dto.ContactPosition;
            if (dto.LegalBusinessName != null) onboarding.LegalBusinessName = dto.LegalBusinessName;
            if (dto.TaxId != null) onboarding.TaxId = dto.TaxId;
            if (dto.BusinessRegistrationNumber != null) onboarding.BusinessRegistrationNumber = dto.BusinessRegistrationNumber;
            if (dto.BankName != null) onboarding.BankName = dto.BankName;
            if (dto.BankAccountName != null) onboarding.BankAccountName = dto.BankAccountName;
            if (dto.BankAccountNumber != null) onboarding.BankAccountNumber = dto.BankAccountNumber;
            if (dto.BankRoutingNumber != null) onboarding.BankRoutingNumber = dto.BankRoutingNumber;
            if (dto.BankSwiftCode != null) onboarding.BankSwiftCode = dto.BankSwiftCode;
            if (dto.SelectedPlanId.HasValue) onboarding.SelectedPlanId = dto.SelectedPlanId.Value;
            if (dto.SelectedBillingCycle.HasValue) onboarding.SelectedBillingCycle = dto.SelectedBillingCycle.Value;

            onboarding.UpdatedAt = DateTime.UtcNow;

            await onboardingRepository.UpdateAsync(onboarding);
            return MapToDto(await onboardingRepository.GetByIdWithDetailsAsync(id)!);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid applicantId)
        {
            var onboarding = await onboardingRepository.GetByIdAsync(id) 
                ?? throw new Exception("Onboarding application not found");

            if (onboarding.ApplicantId != applicantId)
                throw new Exception("You don't have permission to delete this application");

            if (onboarding.Status != OnboardingStatus.Draft)
                throw new Exception("Only draft applications can be deleted");

            await onboardingRepository.DeleteAsync(id);
            return true;
        }

        public async Task<HotelOnboardingDto> SubmitAsync(Guid id, Guid applicantId, SubmitOnboardingDto dto)
        {
            var onboarding = await onboardingRepository.GetByIdAsync(id) 
                ?? throw new Exception("Onboarding application not found");

            if (onboarding.ApplicantId != applicantId)
                throw new Exception("You don't have permission to submit this application");

            if (onboarding.Status != OnboardingStatus.Draft && onboarding.Status != OnboardingStatus.DocumentsRequired)
                throw new Exception("Application cannot be submitted in current status");

            if (!dto.AcceptedTerms)
                throw new Exception("You must accept the terms and conditions");

            onboarding.AcceptedTerms = true;
            onboarding.AcceptedTermsAt = DateTime.UtcNow;
            onboarding.Status = OnboardingStatus.Submitted;
            onboarding.SubmittedAt = DateTime.UtcNow;
            onboarding.UpdatedAt = DateTime.UtcNow;

            await onboardingRepository.UpdateAsync(onboarding);

            // Notify admins about new application
            await notificationService.NotifyAdminsAsync(
                "New Hotel Partner Application",
                $"A new hotel partner application has been submitted for {onboarding.HotelName}",
                NotificationType.SystemAlert,
                $"/admin/onboarding/{id}");

            return MapToDto(await onboardingRepository.GetByIdWithDetailsAsync(id)!);
        }

        // Document operations
        public async Task<OnboardingDocumentDto> UploadDocumentAsync(Guid onboardingId, Guid applicantId, UploadDocumentDto dto)
        {
            var onboarding = await onboardingRepository.GetByIdAsync(onboardingId) 
                ?? throw new Exception("Onboarding application not found");

            if (onboarding.ApplicantId != applicantId)
                throw new Exception("You don't have permission to upload documents to this application");

            var document = new OnboardingDocument
            {
                OnboardingId = onboardingId,
                Type = dto.Type,
                FileName = dto.FileName,
                FileUrl = dto.FileUrl,
                FileType = dto.FileType,
                FileSize = dto.FileSize,
                ExpiryDate = dto.ExpiryDate,
                Status = DocumentStatus.Pending
            };

            var created = await documentRepository.CreateAsync(document);
            return MapDocumentToDto(created);
        }

        public async Task<bool> DeleteDocumentAsync(Guid documentId, Guid applicantId)
        {
            var document = await documentRepository.GetByIdAsync(documentId) 
                ?? throw new Exception("Document not found");

            var onboarding = await onboardingRepository.GetByIdAsync(document.OnboardingId);
            if (onboarding == null || onboarding.ApplicantId != applicantId)
                throw new Exception("You don't have permission to delete this document");

            if (document.Status == DocumentStatus.Approved)
                throw new Exception("Approved documents cannot be deleted");

            await documentRepository.DeleteAsync(documentId);
            return true;
        }

        // Admin operations
        public async Task<IEnumerable<OnboardingSummaryDto>> GetAllAsync()
        {
            var applications = await onboardingRepository.GetAllAsync();
            return applications.Select(MapToSummaryDto);
        }

        public async Task<IEnumerable<OnboardingSummaryDto>> GetPendingReviewAsync()
        {
            var applications = await onboardingRepository.GetPendingReviewAsync();
            return applications.Select(MapToSummaryDto);
        }

        public async Task<PagedResultDto<OnboardingSummaryDto>> SearchAsync(
            string? query,
            OnboardingStatus? status,
            string? country,
            string? city,
            int page = 1,
            int pageSize = 20,
            string? sortBy = null,
            bool sortDescending = true)
        {
            var (items, totalCount) = await onboardingRepository.SearchAsync(
                query, status, country, city, page, pageSize, sortBy, sortDescending);

            return new PagedResultDto<OnboardingSummaryDto>
            {
                Items = items.Select(MapToSummaryDto).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<HotelOnboardingDto> ReviewAsync(Guid id, Guid reviewerId, ReviewOnboardingDto dto)
        {
            var onboarding = await onboardingRepository.GetByIdAsync(id) 
                ?? throw new Exception("Onboarding application not found");

            onboarding.Status = dto.NewStatus;
            onboarding.ReviewedById = reviewerId;
            onboarding.ReviewedAt = DateTime.UtcNow;
            onboarding.ReviewNotes = dto.ReviewNotes;
            
            if (dto.NewStatus == OnboardingStatus.Rejected)
                onboarding.RejectionReason = dto.RejectionReason;

            onboarding.UpdatedAt = DateTime.UtcNow;

            await onboardingRepository.UpdateAsync(onboarding);

            // Notify applicant
            var notificationType = dto.NewStatus == OnboardingStatus.Rejected 
                ? NotificationType.SystemAlert 
                : NotificationType.BookingConfirmation;
            var message = dto.NewStatus switch
            {
                OnboardingStatus.UnderReview => "Your application is now under review.",
                OnboardingStatus.DocumentsRequired => "Additional documents are required for your application.",
                OnboardingStatus.Rejected => $"Your application has been rejected. Reason: {dto.RejectionReason}",
                _ => "Your application status has been updated."
            };

            await notificationService.CreateAsync(new CreateNotificationDto
            {
                UserId = onboarding.ApplicantId,
                Title = "Application Status Update",
                Message = message,
                Type = notificationType
            });

            return MapToDto(await onboardingRepository.GetByIdWithDetailsAsync(id)!);
        }

        public async Task<HotelOnboardingDto> ApproveAsync(Guid id, Guid approverId)
        {
            var onboarding = await onboardingRepository.GetByIdWithDetailsAsync(id) 
                ?? throw new Exception("Onboarding application not found");

            if (onboarding.Status != OnboardingStatus.UnderReview)
                throw new Exception("Only applications under review can be approved");

            // Create Brand (or use existing)
            Brand brand;
            if (onboarding.ExistingBrandId.HasValue)
            {
                brand = await brandRepository.GetByIdAsync(onboarding.ExistingBrandId.Value) 
                    ?? throw new Exception("Existing brand not found");
            }
            else
            {
                brand = new Brand
                {
                    Name = onboarding.BrandName,
                    Description = onboarding.BrandDescription,
                    LogoUrl = onboarding.BrandLogoUrl,
                    Website = onboarding.BrandWebsite,
                    Email = onboarding.ContactEmail,
                    PhoneNumber = onboarding.ContactPhone,
                    TaxId = onboarding.TaxId,
                    BusinessLicense = onboarding.BusinessRegistrationNumber,
                    IsActive = true
                };
                brand = await brandRepository.CreateAsync(brand);
            }

            // Create Hotel
            var hotel = new Hotel
            {
                BrandId = brand.Id,
                Name = onboarding.HotelName,
                Description = onboarding.HotelDescription,
                Address = onboarding.Address,
                City = onboarding.City,
                State = onboarding.State,
                Country = onboarding.Country,
                PostalCode = onboarding.PostalCode,
                Latitude = onboarding.Latitude,
                Longitude = onboarding.Longitude,
                StarRating = onboarding.StarRating,
                TotalRooms = onboarding.TotalRooms,
                NumberOfFloors = onboarding.NumberOfFloors,
                Email = onboarding.ContactEmail,
                PhoneNumber = onboarding.ContactPhone,
                TaxId = onboarding.TaxId,
                IsActive = true,
                IsVerified = true
            };
            hotel = await hotelRepository.CreateAsync(hotel);

            // Create Subscription
            Subscription? subscription = null;
            if (onboarding.SelectedPlanId.HasValue)
            {
                var plan = await planRepository.GetByIdAsync(onboarding.SelectedPlanId.Value);
                if (plan != null)
                {
                    var price = onboarding.SelectedBillingCycle switch
                    {
                        BillingCycle.Monthly => plan.MonthlyPrice,
                        BillingCycle.Quarterly => plan.QuarterlyPrice,
                        BillingCycle.Yearly => plan.YearlyPrice,
                        _ => plan.MonthlyPrice
                    };

                    subscription = new Subscription
                    {
                        BrandId = brand.Id,
                        PlanId = plan.Id,
                        Status = SubscriptionStatus.Trial,
                        BillingCycle = onboarding.SelectedBillingCycle,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddDays(plan.TrialDays),
                        TrialEndDate = DateTime.UtcNow.AddDays(plan.TrialDays),
                        Price = price,
                        Currency = plan.Currency,
                        AutoRenew = true,
                        NextBillingDate = DateTime.UtcNow.AddDays(plan.TrialDays)
                    };
                    subscription = await subscriptionRepository.CreateAsync(subscription);
                }
            }

            // Update applicant to BrandAdmin role
            var applicant = await userRepository.GetByIdAsync(onboarding.ApplicantId);
            if (applicant != null)
            {
                applicant.Role = UserRole.BrandAdmin;
                applicant.BrandId = brand.Id;
                applicant.HotelId = hotel.Id;
                await userRepository.UpdateAsync(applicant);
            }

            // Update onboarding record
            onboarding.Status = OnboardingStatus.Approved;
            onboarding.ApprovedById = approverId;
            onboarding.ApprovedAt = DateTime.UtcNow;
            onboarding.CreatedBrandId = brand.Id;
            onboarding.CreatedHotelId = hotel.Id;
            onboarding.CreatedSubscriptionId = subscription?.Id;
            onboarding.UpdatedAt = DateTime.UtcNow;

            await onboardingRepository.UpdateAsync(onboarding);

            // Notify applicant
            await notificationService.CreateAsync(new CreateNotificationDto
            {
                UserId = onboarding.ApplicantId,
                Title = "?? Congratulations! Your Application is Approved",
                Message = $"Your hotel {onboarding.HotelName} has been approved and is now live on our platform!",
                Type = NotificationType.SystemAlert
            });

            return MapToDto(await onboardingRepository.GetByIdWithDetailsAsync(id)!);
        }

        public async Task<OnboardingDocumentDto> ReviewDocumentAsync(Guid documentId, Guid reviewerId, ReviewDocumentDto dto)
        {
            var document = await documentRepository.GetByIdAsync(documentId) 
                ?? throw new Exception("Document not found");

            document.Status = dto.Status;
            document.ReviewedById = reviewerId;
            document.ReviewedAt = DateTime.UtcNow;
            document.ReviewNotes = dto.ReviewNotes;
            if (dto.Status == DocumentStatus.Rejected)
                document.RejectionReason = dto.RejectionReason;

            document.UpdatedAt = DateTime.UtcNow;

            await documentRepository.UpdateAsync(document);
            return MapDocumentToDto(await documentRepository.GetByIdAsync(documentId)!);
        }

        // Stats
        public async Task<OnboardingStatsDto> GetStatsAsync()
        {
            var all = await onboardingRepository.GetAllAsync();
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            return new OnboardingStatsDto
            {
                TotalApplications = all.Count(),
                DraftApplications = all.Count(o => o.Status == OnboardingStatus.Draft),
                PendingReview = all.Count(o => o.Status == OnboardingStatus.Submitted || o.Status == OnboardingStatus.UnderReview),
                DocumentsRequired = all.Count(o => o.Status == OnboardingStatus.DocumentsRequired),
                ApprovedThisMonth = all.Count(o => o.Status == OnboardingStatus.Approved && o.ApprovedAt >= startOfMonth),
                RejectedThisMonth = all.Count(o => o.Status == OnboardingStatus.Rejected && o.ReviewedAt >= startOfMonth),
                ActivePartners = all.Count(o => o.Status == OnboardingStatus.Active)
            };
        }

        // Mapping methods
        private static HotelOnboardingDto MapToDto(HotelOnboarding o) => new()
        {
            Id = o.Id,
            ApplicantId = o.ApplicantId,
            ApplicantName = o.Applicant != null ? $"{o.Applicant.FirstName} {o.Applicant.LastName}" : "",
            ApplicantEmail = o.Applicant?.Email ?? "",
            ExistingBrandId = o.ExistingBrandId,
            BrandName = o.BrandName,
            BrandDescription = o.BrandDescription,
            BrandLogoUrl = o.BrandLogoUrl,
            BrandWebsite = o.BrandWebsite,
            HotelName = o.HotelName,
            HotelDescription = o.HotelDescription,
            Address = o.Address,
            City = o.City,
            State = o.State,
            Country = o.Country,
            PostalCode = o.PostalCode,
            Latitude = o.Latitude,
            Longitude = o.Longitude,
            StarRating = o.StarRating,
            TotalRooms = o.TotalRooms,
            NumberOfFloors = o.NumberOfFloors,
            ContactName = o.ContactName,
            ContactEmail = o.ContactEmail,
            ContactPhone = o.ContactPhone,
            ContactPosition = o.ContactPosition,
            LegalBusinessName = o.LegalBusinessName,
            TaxId = o.TaxId,
            BusinessRegistrationNumber = o.BusinessRegistrationNumber,
            SelectedPlanId = o.SelectedPlanId,
            SelectedPlanName = o.SelectedPlan?.Name,
            SelectedBillingCycle = o.SelectedBillingCycle,
            Status = o.Status,
            SubmittedAt = o.SubmittedAt,
            ReviewedAt = o.ReviewedAt,
            ReviewNotes = o.ReviewNotes,
            RejectionReason = o.RejectionReason,
            ApprovedAt = o.ApprovedAt,
            CreatedBrandId = o.CreatedBrandId,
            CreatedHotelId = o.CreatedHotelId,
            CreatedSubscriptionId = o.CreatedSubscriptionId,
            Documents = o.Documents.Select(MapDocumentToDto).ToList(),
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt
        };

        private static OnboardingSummaryDto MapToSummaryDto(HotelOnboarding o) => new()
        {
            Id = o.Id,
            HotelName = o.HotelName,
            BrandName = o.BrandName,
            City = o.City,
            Country = o.Country,
            ApplicantName = o.Applicant != null ? $"{o.Applicant.FirstName} {o.Applicant.LastName}" : "",
            ApplicantEmail = o.Applicant?.Email ?? "",
            Status = o.Status,
            TotalRooms = o.TotalRooms,
            StarRating = o.StarRating,
            SubmittedAt = o.SubmittedAt,
            CreatedAt = o.CreatedAt
        };

        private static OnboardingDocumentDto MapDocumentToDto(OnboardingDocument d) => new()
        {
            Id = d.Id,
            Type = d.Type,
            FileName = d.FileName,
            FileUrl = d.FileUrl,
            FileType = d.FileType,
            FileSize = d.FileSize,
            Status = d.Status,
            ReviewedAt = d.ReviewedAt,
            ReviewNotes = d.ReviewNotes,
            RejectionReason = d.RejectionReason,
            ExpiryDate = d.ExpiryDate,
            CreatedAt = d.CreatedAt
        };
    }
}
