using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Services.Embedding;
using Hotel_SAAS_Backend.API.Utils;
using Hotel_SAAS_Backend.API.Models.Constants;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Services.AI
{
    /// <summary>
    /// Service for RAG (Retrieval Augmented Generation) operations
    /// </summary>
    public class RAGService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmbedderService _embedder;
        private readonly Options.RAGOptions _options;

        public RAGService(
            ApplicationDbContext context,
            IEmbedderService embedder,
            IConfiguration configuration)
        {
            _context = context;
            _embedder = embedder;
            _options = configuration.GetSection("RAG").Get<Options.RAGOptions>()
                ?? new Options.RAGOptions();
        }

        /// <summary>
        /// Search knowledge chunks by embedding similarity (cosine distance)
        /// </summary>
        public async Task<List<(KnowledgeChunk Chunk, double Score)>> SearchByEmbeddingAsync(
            float[] embedding,
            int? topK = null,
            string? category = null)
        {
            var k = topK ?? _options.TopKResults;
            var embeddingStr = $"[{string.Join(",", embedding)}]";

            // Build SQL query with pgvector cosine distance
            var sql = $@"
                SELECT kc.*, kd.*, 1 - (kc.embedding <=> '{embeddingStr}'::vector) as similarity_score
                FROM knowledge_chunks kc
                INNER JOIN knowledge_documents kd ON kc.document_id = kd.id
                WHERE kd.is_active = true
                AND kc.embedding IS NOT NULL";

            if (!string.IsNullOrEmpty(category))
            {
                sql += $" AND kd.category = @category";
            }

            sql += $@"
                ORDER BY kc.embedding <=> '{embeddingStr}'::vector
                LIMIT {k}";

            // Execute query
            await using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;

            if (!string.IsNullOrEmpty(category))
            {
                var param = command.CreateParameter();
                param.ParameterName = "@category";
                param.Value = category;
                command.Parameters.Add(param);
            }

            await _context.Database.OpenConnectionAsync();
            await using var reader = await command.ExecuteReaderAsync();

            var results = new List<(KnowledgeChunk, double)>();

            while (await reader.ReadAsync())
            {
                var chunk = new KnowledgeChunk
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    DocumentId = reader.GetGuid(reader.GetOrdinal("document_id")),
                    Content = reader.GetString(reader.GetOrdinal("content")),
                    ChunkIndex = reader.GetInt32(reader.GetOrdinal("chunk_index")),
                    Metadata = reader.IsDBNull(reader.GetOrdinal("metadata"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("metadata")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                };

                var score = reader.GetDouble(reader.GetOrdinal("similarity_score"));

                if (score >= _options.MinSimilarityScore)
                {
                    // Load the document separately
                    chunk.Document = await _context.KnowledgeDocuments
                        .FirstOrDefaultAsync(d => d.Id == chunk.DocumentId);

                    results.Add((chunk, score));
                }
            }

            await _context.Database.CloseConnectionAsync();

            return results.OrderByDescending(r => r.Item2).ToList();
        }

        /// <summary>
        /// Build RAG system prompt from retrieved knowledge chunks
        /// </summary>
        public async Task<string> BuildRagSystemPromptAsync(string userQuery)
        {
            // 1. Embed the user query
            var queryEmbedding = await _embedder.EmbedOneAsync(userQuery);

            // 2. Search for relevant chunks
            var relevantChunks = await SearchByEmbeddingAsync(queryEmbedding);

            if (!relevantChunks.Any())
            {
                // No relevant context found
                return GetDefaultSystemPrompt();
            }

            // 3. Build context from chunks
            var contextBuilder = new System.Text.StringBuilder();
            var totalChars = 0;
            var sources = new Dictionary<string, int>(); // source -> chunk count

            contextBuilder.AppendLine("=== NGỮ CẢNH TỪ CƠ SỞ KIẾN THỤC ===");
            contextBuilder.AppendLine();

            foreach (var (chunk, score) in relevantChunks)
            {
                var chunkContent = chunk.Content.Length > _options.MaxChunkLength
                    ? chunk.Content.Substring(0, _options.MaxChunkLength) + "..."
                    : chunk.Content;

                var source = chunk.Document.Title;

                // Track source usage
                if (!sources.ContainsKey(source))
                {
                    sources[source] = 0;
                }
                sources[source]++;

                // Add chunk to context
                contextBuilder.AppendLine($"【{source}】({Math.Round(score * 100)}% độ tin cậy)");
                contextBuilder.AppendLine(chunkContent);
                contextBuilder.AppendLine();

                totalChars += chunkContent.Length;

                if (totalChars >= _options.MaxContextLength)
                {
                    break;
                }
            }

            // 4. Build source summary
            var topSources = sources.OrderByDescending(s => s.Value)
                .Take(_options.MaxSources)
                .Where(s => s.Value >= sources.Values.Max() * 0.5)
                .Select(s => s.Key)
                .ToList();

            // 5. Build final system prompt
            var systemPrompt = GetSystemPromptWithContext(contextBuilder.ToString(), topSources);
            return systemPrompt;
        }

        /// <summary>
        /// Calculate cosine distance between two vectors
        /// </summary>
        private async Task<double> CalculateCosineDistanceAsync(float[] vector1, float[] vector2)
        {
            if (vector1.Length != vector2.Length)
            {
                throw new ArgumentException(Messages.Misc.VectorDimensionMismatch);
            }

            double dotProduct = 0;
            double magnitude1 = 0;
            double magnitude2 = 0;

            for (int i = 0; i < vector1.Length; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                magnitude1 += vector1[i] * vector1[i];
                magnitude2 += vector2[i] * vector2[i];
            }

            magnitude1 = Math.Sqrt(magnitude1);
            magnitude2 = Math.Sqrt(magnitude2);

            if (magnitude1 == 0 || magnitude2 == 0)
            {
                return 0;
            }

            var cosineSimilarity = dotProduct / (magnitude1 * magnitude2);
            return 1 - cosineSimilarity; // Convert to distance
        }

        private string GetDefaultSystemPrompt()
        {
            return @"Bạn là trợ lý AI cho hệ thống đặt phòng khách sạn Hotel SAAS.

Nguyên tắc:
- Trả lời bằng Tiếng Việt một cách thân thiện, chuyên nghiệp
- Nếu không có đủ thông tin, hãy hỏi khách hàng để làm rõ
- Luôn đề xuất các hành động cụ thể (đặt phòng, xem chi tiết, v.v.)
- Giữ câu trả lời ngắn gọn và súc tích";
        }

        private string GetSystemPromptWithContext(string context, List<string> sources)
        {
            var sourceList = string.Join(", ", sources);
            return $@"Bạn là trợ lý AI cho hệ thống đặt phòng khách sạn Hotel SAAS.

Nguyên tắc bắt buộc:
- CHỈ được sử dụng thông tin có trong phần ""NGỮ CẢNH"" bên dưới
- KHÔNG được tự bịa thông tin không có trong ngữ cảnh
- Khi trích dẫn chính sách hoặc quy định, phải giữ nguyên nội dung
- Nếu thông tin không có trong ngữ cảnh, hãy nói ""Trong thông tin mình có, không thấy có phần này. Bạn có thể cho mình biết thêm chi tiết không?""
- Luôn đề xuất các hành động cụ thể (đặt phòng, xem chi tiết, liên hệ, v.v.)
- Giữ câu trả lời ngắn gọn, súc tích và dễ hiểu

Nguồn thông tin: {sourceList}

{context}";
        }
    }
}
