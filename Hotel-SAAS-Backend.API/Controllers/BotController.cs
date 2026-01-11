using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Services.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_SAAS_Backend.API.Controllers
{
    /// <summary>
    /// Controller for AI chatbot functionality
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BotController : ControllerBase
    {
        private readonly BotService _botService;

        public BotController(BotService botService)
        {
            _botService = botService;
        }

        /// <summary>
        /// Create a new conversation
        /// </summary>
        [HttpPost("conversations")]
        public async Task<ActionResult<BotConversation>> CreateConversation(
            [FromBody] CreateConversationRequest? request = null)
        {
            Guid? userId = null;
            var userIdClaim = User.FindFirst("user_id")?.Value;
            if (userIdClaim != null)
            {
                userId = Guid.Parse(userIdClaim);
            }

            var conversation = await _botService.CreateConversationAsync(
                ownerId: userId,
                guestIdentifier: request?.GuestIdentifier,
                title: request?.Title);

            return Ok(conversation);
        }

        /// <summary>
        /// Get a conversation with its messages
        /// </summary>
        [HttpGet("conversations/{id}")]
        public async Task<ActionResult<BotConversation>> GetConversation(Guid id)
        {
            var conversation = await _botService.GetConversationAsync(id);

            if (conversation == null)
            {
                return NotFound(new { success = false, message = "Conversation not found" });
            }

            return Ok(conversation);
        }

        /// <summary>
        /// Get user's conversations
        /// </summary>
        [HttpGet("conversations")]
        [Authorize]
        public async Task<ActionResult<List<BotConversation>>> GetUserConversations()
        {
            Guid? userId = null;
            var userIdClaim = User.FindFirst("user_id")?.Value;
            if (userIdClaim != null)
            {
                userId = Guid.Parse(userIdClaim);
            }

            var conversations = await _botService.GetUserConversationsAsync(
                ownerId: userId,
                guestIdentifier: null);

            return Ok(conversations);
        }

        /// <summary>
        /// Generate AI response with RAG
        /// </summary>
        [HttpPost("generate")]
        public async Task<ActionResult<BotMessage>> GenerateResponse(
            [FromBody] GenerateRequest request)
        {
            Guid? userId = null;
            var userIdClaim = User.FindFirst("user_id")?.Value;
            if (userIdClaim != null)
            {
                userId = Guid.Parse(userIdClaim);
            }

            try
            {
                var response = await _botService.GenerateResponseAsync(
                    request.ConversationId,
                    request.Message,
                    userId,
                    request.GuestIdentifier);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a conversation
        /// </summary>
        [HttpDelete("conversations/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteConversation(Guid id)
        {
            var result = await _botService.DeleteConversationAsync(id);

            if (!result)
            {
                return NotFound(new { success = false, message = "Conversation not found" });
            }

            return Ok(new { success = true, message = "Conversation deleted" });
        }

        #region Request DTOs

        public class CreateConversationRequest
        {
            public string? Title { get; set; }
            public string? GuestIdentifier { get; set; }
        }

        public class GenerateRequest
        {
            public Guid ConversationId { get; set; }
            public string Message { get; set; } = string.Empty;
            public string? GuestIdentifier { get; set; }
        }

        #endregion
    }
}
