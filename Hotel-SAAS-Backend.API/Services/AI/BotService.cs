using System.Text.Json;
using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Services.Embedding;
using Hotel_SAAS_Backend.API.Services.LLM;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Services.AI
{
    /// <summary>
    /// Service for managing AI chatbot conversations
    /// </summary>
    public class BotService
    {
        private readonly ApplicationDbContext _context;
        private readonly RAGService _ragService;
        private readonly ILLMProvider _llm;

        public BotService(
            ApplicationDbContext context,
            RAGService ragService,
            ILLMProvider llm)
        {
            _context = context;
            _ragService = ragService;
            _llm = llm;
        }

        /// <summary>
        /// Create a new conversation
        /// </summary>
        public async Task<BotConversation> CreateConversationAsync(
            Guid? ownerId = null,
            string? guestIdentifier = null,
            string? title = null)
        {
            var conversation = new BotConversation
            {
                OwnerId = ownerId,
                GuestIdentifier = guestIdentifier,
                Title = title ?? "Hội thoại mới",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.BotConversations.Add(conversation);
            await _context.SaveChangesAsync();

            return conversation;
        }

        /// <summary>
        /// Get conversation with messages
        /// </summary>
        public async Task<BotConversation?> GetConversationAsync(Guid conversationId)
        {
            return await _context.BotConversations
                .Include(c => c.Messages.OrderBy(m => m.CreatedAt))
                .FirstOrDefaultAsync(c => c.Id == conversationId);
        }

        /// <summary>
        /// Get user's conversations
        /// </summary>
        public async Task<List<BotConversation>> GetUserConversationsAsync(Guid? ownerId, string? guestIdentifier)
        {
            var query = _context.BotConversations.AsQueryable();

            if (ownerId.HasValue)
            {
                query = query.Where(c => c.OwnerId == ownerId.Value);
            }
            else if (!string.IsNullOrEmpty(guestIdentifier))
            {
                query = query.Where(c => c.GuestIdentifier == guestIdentifier);
            }
            else
            {
                return new List<BotConversation>();
            }

            return await query
                .OrderByDescending(c => c.UpdatedAt)
                .Take(50)
                .ToListAsync();
        }

        /// <summary>
        /// Generate AI response with RAG
        /// </summary>
        public async Task<BotMessage> GenerateResponseAsync(
            Guid conversationId,
            string userMessage,
            Guid? userId = null,
            string? guestIdentifier = null)
        {
            var startTime = DateTime.UtcNow;

            // Get conversation
            var conversation = await _context.BotConversations
                .Include(c => c.Messages.OrderBy(m => m.CreatedAt).Take(20)) // Last 20 messages for context
                .FirstOrDefaultAsync(c => c.Id == conversationId)
                ?? throw new Exception("Conversation not found");

            // Add user message
            var userMsg = new BotMessage
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                Role = "user",
                Content = userMessage,
                CreatedAt = DateTime.UtcNow
            };
            _context.BotMessages.Add(userMsg);

            // Update conversation title if first message
            if (!conversation.Messages.Any())
            {
                conversation.Title = userMessage.Length > 50
                    ? userMessage.Substring(0, 50) + "..."
                    : userMessage;
            }
            conversation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Build RAG system prompt
            var systemPrompt = await _ragService.BuildRagSystemPromptAsync(userMessage);

            // Build message history for LLM
            var chatMessages = new List<ChatMessage>();

            // Add conversation history (excluding the current user message we just added)
            foreach (var msg in conversation.Messages.Take(15)) // Limit context window
            {
                chatMessages.Add(new ChatMessage
                {
                    Role = msg.Role,
                    Content = msg.Content
                });
            }

            // Generate response
            var responseContent = await _llm.CompleteAsync(
                chatMessages,
                systemPrompt);

            var endTime = DateTime.UtcNow;
            var latency = (long)(endTime - startTime).TotalMilliseconds;

            // Create assistant message
            var assistantMsg = new BotMessage
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                Role = "assistant",
                Content = responseContent,
                LatencyMs = latency,
                CreatedAt = DateTime.UtcNow
            };
            _context.BotMessages.Add(assistantMsg);

            conversation.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return assistantMsg;
        }

        /// <summary>
        /// Delete a conversation
        /// </summary>
        public async Task<bool> DeleteConversationAsync(Guid conversationId)
        {
            var conversation = await _context.BotConversations
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null) return false;

            _context.BotConversations.Remove(conversation);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
