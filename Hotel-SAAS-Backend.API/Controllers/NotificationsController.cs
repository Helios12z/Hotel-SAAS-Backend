using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Constants;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrEmpty(userIdClaim) ? Guid.Empty : Guid.Parse(userIdClaim);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<NotificationSummaryDto>>> GetNotifications(
            [FromQuery] int limit = 50)
        {
            var result = await _notificationService.GetUserNotificationsAsync(GetUserId(), limit);
            return Ok(new ApiResponseDto<NotificationSummaryDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<ApiResponseDto<UnreadCountDto>>> GetUnreadCount()
        {
            var result = await _notificationService.GetUnreadCountAsync(GetUserId());
            return Ok(new ApiResponseDto<UnreadCountDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpPost("{id}/read")]
        public async Task<ActionResult<ApiResponseDto<bool>>> MarkAsRead(Guid id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = Messages.Misc.MarkedAsRead,
                Data = true
            });
        }

        [HttpPost("read-all")]
        public async Task<ActionResult<ApiResponseDto<bool>>> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsReadAsync(GetUserId());
            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = Messages.Misc.MarkedAsRead,
                Data = true
            });
        }

        [HttpPost("read-multiple")]
        public async Task<ActionResult<ApiResponseDto<bool>>> MarkMultipleAsRead(
            [FromBody] MarkNotificationsReadDto dto)
        {
            await _notificationService.MarkMultipleAsReadAsync(dto.NotificationIds);
            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = Messages.Misc.MarkedAsRead,
                Data = true
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteNotification(Guid id)
        {
            var result = await _notificationService.DeleteNotificationAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Notification deleted" : "Notification not found",
                Data = result
            });
        }
    }
}
