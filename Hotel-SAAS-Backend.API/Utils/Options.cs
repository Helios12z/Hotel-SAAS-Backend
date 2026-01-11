namespace Hotel_SAAS_Backend.API.Utils
{
    public class Options
    {
        public class ImgBbOptions
        {
            public string ApiKey { get; set; } = default!;
            public string UploadEndpoint { get; set; } = "https://api.imgbb.com/1/upload";
            public int ExpirationSeconds { get; set; } = 0;
        }

        public class R2Options
        {
            public string AccountId { get; set; } = default!;
            public string AccessKeyId { get; set; } = default!;
            public string SecretAccessKey { get; set; } = default!;
            public string Bucket { get; set; } = default!;
            public string ServiceUrl { get; set; } = default!;
            public string PublicBaseUrl { get; set; } = default!;
            public int PresignedUploadMinutes { get; set; } = 10;
            public int PresignedDownloadMinutes { get; set; } = 5;
        }

        public class UploadPolicyOptions
        {
            public UploadGroup Images { get; set; } = new();
            public UploadGroup Documents { get; set; } = new();
            public class UploadGroup
            {
                public long MaxBytes { get; set; }
                public string[] AllowedContentTypes { get; set; } = Array.Empty<string>();
                public string[]? AllowedExtensions { get; set; }
            }
        }

        /// <summary>
        /// Configuration options for embedding service (Ollama)
        /// </summary>
        public class EmbeddingOptions
        {
            public string BaseUrl { get; set; } = "http://127.0.0.1:11434";
            public string Model { get; set; } = "bge-m3";
            public int Dimension { get; set; } = 1024;
        }

        /// <summary>
        /// Configuration options for LLM provider (Gemini, OpenAI, etc.)
        /// </summary>
        public class LLMOptions
        {
            public string Provider { get; set; } = "Gemini"; // Gemini, OpenAI, Local
            public string BaseUrl { get; set; } = string.Empty;
            public string ApiKey { get; set; } = string.Empty;
            public string Model { get; set; } = "gemini-2.0-flash-exp";
            public int MaxTokens { get; set; } = 4096;
            public double Temperature { get; set; } = 0.7;
        }

        /// <summary>
        /// Configuration options for RAG (Retrieval Augmented Generation)
        /// </summary>
        public class RAGOptions
        {
            public int MaxContextLength { get; set; } = 8000; // Max characters in context
            public int MaxChunkLength { get; set; } = 1000; // Max characters per chunk
            public int TopKResults { get; set; } = 5; // Number of chunks to retrieve
            public double MinSimilarityScore { get; set; } = 0.5; // Minimum similarity threshold
            public int MaxSources { get; set; } = 3; // Max unique sources to include
        }
    }
}
