using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/onboarding")]
    public class HotelOnboardingController : ControllerBase
    {
        private readonly IHotelOnboardingService _onboardingService;

        public HotelOnboardingController(IHotelOnboardingService onboardingService)
        {
            _onboardingService = onboardingService;
        }

        #region Public/Partner Endpoints

        /// <summary>
        /// Create a new hotel partner application
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<HotelOnboardingDto>>> CreateApplication(
            [FromBody] CreateOnboardingDto createDto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var application = await _onboardingService.CreateAsync(userId, createDto);
                return CreatedAtAction(nameof(GetApplicationById), new { id = application.Id }, new ApiResponseDto<HotelOnboardingDto>
                {
                    Success = true,
                    Message = "Application created successfully. Please complete all required fields and upload documents.",
                    Data = application
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<HotelOnboardingDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get my applications
        /// </summary>
        [Authorize]
        [HttpGet("my-applications")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<HotelOnboardingDto>>>> GetMyApplications()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var applications = await _onboardingService.GetMyApplicationsAsync(userId);
            return Ok(new ApiResponseDto<IEnumerable<HotelOnboardingDto>>
            {
                Success = true,
                Data = applications
            });
        }

        /// <summary>
        /// Get application by ID
        /// </summary>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<HotelOnboardingDto>>> GetApplicationById(Guid id)
        {
            var application = await _onboardingService.GetByIdAsync(id);
            if (application == null)
            {
                return NotFound(new ApiResponseDto<HotelOnboardingDto>
                {
                    Success = false,
                    Message = "Application not found"
                });
            }

            return Ok(new ApiResponseDto<HotelOnboardingDto>
            {
                Success = true,
                Data = application
            });
        }

        /// <summary>
        /// Update application (only draft or documents required status)
        /// </summary>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<HotelOnboardingDto>>> UpdateApplication(
            Guid id,
            [FromBody] UpdateOnboardingDto updateDto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var application = await _onboardingService.UpdateAsync(id, userId, updateDto);
                return Ok(new ApiResponseDto<HotelOnboardingDto>
                {
                    Success = true,
                    Message = "Application updated successfully",
                    Data = application
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<HotelOnboardingDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Delete draft application
        /// </summary>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteApplication(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _onboardingService.DeleteAsync(id, userId);
                return Ok(new ApiResponseDto<bool>
                {
                    Success = result,
                    Message = result ? "Application deleted successfully" : "Failed to delete application",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Submit application for review
        /// </summary>
        [Authorize]
        [HttpPost("{id}/submit")]
        public async Task<ActionResult<ApiResponseDto<HotelOnboardingDto>>> SubmitApplication(
            Guid id,
            [FromBody] SubmitOnboardingDto submitDto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var application = await _onboardingService.SubmitAsync(id, userId, submitDto);
                return Ok(new ApiResponseDto<HotelOnboardingDto>
                {
                    Success = true,
                    Message = "Application submitted successfully. Our team will review it shortly.",
                    Data = application
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<HotelOnboardingDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Upload a document
        /// </summary>
        [Authorize]
        [HttpPost("{id}/documents")]
        public async Task<ActionResult<ApiResponseDto<OnboardingDocumentDto>>> UploadDocument(
            Guid id,
            [FromBody] UploadDocumentDto uploadDto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var document = await _onboardingService.UploadDocumentAsync(id, userId, uploadDto);
                return Ok(new ApiResponseDto<OnboardingDocumentDto>
                {
                    Success = true,
                    Message = "Document uploaded successfully",
                    Data = document
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<OnboardingDocumentDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Delete a document
        /// </summary>
        [Authorize]
        [HttpDelete("documents/{documentId}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteDocument(Guid documentId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _onboardingService.DeleteDocumentAsync(documentId, userId);
                return Ok(new ApiResponseDto<bool>
                {
                    Success = result,
                    Message = result ? "Document deleted successfully" : "Failed to delete document",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        #endregion

        #region Admin Endpoints

        /// <summary>
        /// Get all applications (admin only)
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("admin/all")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<OnboardingSummaryDto>>>> GetAllApplications()
        {
            var applications = await _onboardingService.GetAllAsync();
            return Ok(new ApiResponseDto<IEnumerable<OnboardingSummaryDto>>
            {
                Success = true,
                Data = applications
            });
        }

        /// <summary>
        /// Get pending applications (admin only)
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("admin/pending")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<OnboardingSummaryDto>>>> GetPendingApplications()
        {
            var applications = await _onboardingService.GetPendingReviewAsync();
            return Ok(new ApiResponseDto<IEnumerable<OnboardingSummaryDto>>
            {
                Success = true,
                Data = applications
            });
        }

        /// <summary>
        /// Search applications (admin only)
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("admin/search")]
        public async Task<ActionResult<ApiResponseDto<PagedResultDto<OnboardingSummaryDto>>>> SearchApplications(
            [FromQuery] string? query,
            [FromQuery] OnboardingStatus? status,
            [FromQuery] string? country,
            [FromQuery] string? city,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = true)
        {
            var result = await _onboardingService.SearchAsync(
                query, status, country, city, page, pageSize, sortBy, sortDescending);
            return Ok(new ApiResponseDto<PagedResultDto<OnboardingSummaryDto>>
            {
                Success = true,
                Data = result
            });
        }

        /// <summary>
        /// Get onboarding statistics (admin only)
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("admin/stats")]
        public async Task<ActionResult<ApiResponseDto<OnboardingStatsDto>>> GetStats()
        {
            var stats = await _onboardingService.GetStatsAsync();
            return Ok(new ApiResponseDto<OnboardingStatsDto>
            {
                Success = true,
                Data = stats
            });
        }

        /// <summary>
        /// Review application (admin only)
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("admin/{id}/review")]
        public async Task<ActionResult<ApiResponseDto<HotelOnboardingDto>>> ReviewApplication(
            Guid id,
            [FromBody] ReviewOnboardingDto reviewDto)
        {
            try
            {
                var reviewerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var application = await _onboardingService.ReviewAsync(id, reviewerId, reviewDto);
                return Ok(new ApiResponseDto<HotelOnboardingDto>
                {
                    Success = true,
                    Message = $"Application status updated to {reviewDto.NewStatus}",
                    Data = application
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<HotelOnboardingDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Approve application and create brand/hotel/subscription (admin only)
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("admin/{id}/approve")]
        public async Task<ActionResult<ApiResponseDto<HotelOnboardingDto>>> ApproveApplication(Guid id)
        {
            try
            {
                var approverId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var application = await _onboardingService.ApproveAsync(id, approverId);
                return Ok(new ApiResponseDto<HotelOnboardingDto>
                {
                    Success = true,
                    Message = "Application approved! Brand, Hotel, and Subscription have been created.",
                    Data = application
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<HotelOnboardingDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Review a document (admin only)
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("admin/documents/{documentId}/review")]
        public async Task<ActionResult<ApiResponseDto<OnboardingDocumentDto>>> ReviewDocument(
            Guid documentId,
            [FromBody] ReviewDocumentDto reviewDto)
        {
            try
            {
                var reviewerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var document = await _onboardingService.ReviewDocumentAsync(documentId, reviewerId, reviewDto);
                return Ok(new ApiResponseDto<OnboardingDocumentDto>
                {
                    Success = true,
                    Message = $"Document status updated to {reviewDto.Status}",
                    Data = document
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<OnboardingDocumentDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        #endregion
    }
}
