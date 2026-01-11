namespace Hotel_SAAS_Backend.API.Services.LLM
{
    /// <summary>
    /// Interface for LLM providers (Gemini, OpenAI, Local, etc.)
    /// </summary>
    public interface ILLMProvider
    {
        /// <summary>
        /// Get the provider name
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Complete a chat conversation with the LLM
        /// </summary>
        /// <param name="messages">List of messages in the conversation</param>
        /// <param name="systemPrompt">Optional system prompt to guide the AI</param>
        /// <param name="temperature">Optional temperature for randomness (0-1)</param>
        /// <param name="maxTokens">Optional maximum tokens in response</param>
        /// <returns>The AI response</returns>
        Task<string> CompleteAsync(
            IList<ChatMessage> messages,
            string? systemPrompt = null,
            double? temperature = null,
            int? maxTokens = null);
    }

    /// <summary>
    /// Represents a message in a chat conversation
    /// </summary>
    public class ChatMessage
    {
        public string Role { get; set; } = string.Empty; // "system", "user", "assistant"
        public string Content { get; set; } = string.Empty;
    }
}
