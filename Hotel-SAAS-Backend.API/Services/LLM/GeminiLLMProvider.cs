using System.Text.Json;
using System.Text.Json.Serialization;
using Hotel_SAAS_Backend.API.Utils;

namespace Hotel_SAAS_Backend.API.Services.LLM
{
    /// <summary>
    /// LLM provider for Google Gemini using OpenAI-compatible API
    /// </summary>
    public class GeminiLLMProvider : ILLMProvider
    {
        private readonly HttpClient _httpClient;
        private readonly Options.LLMOptions _options;
        private readonly JsonSerializerOptions _jsonOptions;

        public string ProviderName => "Gemini";

        public GeminiLLMProvider(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _options = configuration.GetSection("LLM").Get<Options.LLMOptions>()
                ?? new Options.LLMOptions();

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task<string> CompleteAsync(
            IList<ChatMessage> messages,
            string? systemPrompt = null,
            double? temperature = null,
            int? maxTokens = null)
        {
            // Prepare messages - add system prompt as first message if provided
            var apiMessages = new List<object>();

            if (!string.IsNullOrEmpty(systemPrompt))
            {
                apiMessages.Add(new { role = "system", content = systemPrompt });
            }

            foreach (var msg in messages)
            {
                apiMessages.Add(new { role = msg.Role, content = msg.Content });
            }

            var payload = new
            {
                model = _options.Model,
                messages = apiMessages,
                temperature = temperature ?? _options.Temperature,
                max_tokens = maxTokens ?? _options.MaxTokens
            };

            var content = JsonSerializer.Serialize(payload, _jsonOptions);
            var url = $"{_options.BaseUrl.TrimEnd('/')}/chat/completions?key={_options.ApiKey}";

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ChatCompletionResponse>(responseBody, _jsonOptions)
                ?? throw new Exception("Failed to deserialize LLM response");

            return result.Choices.FirstOrDefault()?.Message?.Content
                ?? throw new Exception("No content in LLM response");
        }

        #region Response Models

        private class ChatCompletionResponse
        {
            [JsonPropertyName("choices")]
            public List<Choice> Choices { get; set; } = new();
        }

        private class Choice
        {
            [JsonPropertyName("message")]
            public Message? Message { get; set; }

            [JsonPropertyName("finish_reason")]
            public string? FinishReason { get; set; }
        }

        private class Message
        {
            [JsonPropertyName("content")]
            public string? Content { get; set; }
        }

        #endregion
    }
}
