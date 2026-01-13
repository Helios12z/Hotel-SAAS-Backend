using System.Text.Json;
using System.Text.Json.Serialization;
using Hotel_SAAS_Backend.API.Utils;
using Hotel_SAAS_Backend.API.Models.Constants;

namespace Hotel_SAAS_Backend.API.Services.Embedding
{
    /// <summary>
    /// Embedding service using Ollama with OpenAI-compatible API
    /// Supports BGE-M3 and other embedding models
    /// </summary>
    public class OllamaEmbedderService : IEmbedderService
    {
        private readonly HttpClient _httpClient;
        private readonly Options.EmbeddingOptions _options;
        private readonly JsonSerializerOptions _jsonOptions;

        public int Dimension => _options.Dimension;

        public OllamaEmbedderService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _options = configuration.GetSection("Embedding").Get<Options.EmbeddingOptions>()
                ?? new Options.EmbeddingOptions();

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task<float[]> EmbedOneAsync(string text)
        {
            var results = await EmbedManyAsync(new[] { text });
            return results.First();
        }

        public async Task<IList<float[]>> EmbedManyAsync(IEnumerable<string> texts)
        {
            var textList = texts.ToList();

            // Try OpenAI-compatible endpoint first (Ollama v0.5.0+)
            try
            {
                return await EmbedUsingOpenAiCompatAsync(textList);
            }
            catch
            {
                // Fallback to native Ollama API
                return await EmbedUsingOllamaNativeAsync(textList);
            }
        }

        private async Task<IList<float[]>> EmbedUsingOpenAiCompatAsync(IList<string> texts)
        {
            var payload = new
            {
                model = _options.Model,
                input = texts
            };

            var content = JsonSerializer.Serialize(payload, _jsonOptions);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl.TrimEnd('/')}/v1/embeddings")
            {
                Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OpenAiEmbeddingResponse>(responseBody, _jsonOptions)
                ?? throw new Exception(Messages.Misc.InternalServerError);

            return result.Data.OrderBy(d => d.Index).Select(d => d.Embedding).ToList();
        }

        private async Task<IList<float[]>> EmbedUsingOllamaNativeAsync(IList<string> texts)
        {
            var embeddings = new List<float[]>();

            foreach (var text in texts)
            {
                var payload = new
                {
                    model = _options.Model,
                    prompt = text
                };

                var content = JsonSerializer.Serialize(payload, _jsonOptions);
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl.TrimEnd('/')}/api/embeddings")
                {
                    Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                };

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OllamaEmbeddingResponse>(responseBody, _jsonOptions)
                    ?? throw new Exception(Messages.Misc.InternalServerError);

                embeddings.Add(result.Embedding);
            }

            return embeddings;
        }

        #region Response Models

        private class OpenAiEmbeddingResponse
        {
            [JsonPropertyName("data")]
            public List<OpenAiEmbeddingData> Data { get; set; } = new();
        }

        private class OpenAiEmbeddingData
        {
            [JsonPropertyName("embedding")]
            public float[] Embedding { get; set; } = Array.Empty<float>();

            [JsonPropertyName("index")]
            public int Index { get; set; }
        }

        private class OllamaEmbeddingResponse
        {
            [JsonPropertyName("embedding")]
            public float[] Embedding { get; set; } = Array.Empty<float>();
        }

        #endregion
    }
}
