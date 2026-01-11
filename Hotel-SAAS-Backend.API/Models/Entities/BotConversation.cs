namespace Hotel_SAAS_Backend.API.Models.Entities
{
    /// <summary>
    /// Represents a chat conversation with the AI assistant
    /// </summary>
    public class BotConversation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? OwnerId { get; set; } // Optional user ID if logged in
        public string? GuestIdentifier { get; set; } // For guest users (session ID)
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ICollection<BotMessage> Messages { get; set; } = new List<BotMessage>();
    }

    /// <summary>
    /// Represents a message in a bot conversation
    /// </summary>
    public class BotMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ConversationId { get; set; }
        public string Role { get; set; } = string.Empty; // "system", "user", "assistant"
        public string Content { get; set; } = string.Empty;
        public int? TokenCount { get; set; }
        public long? LatencyMs { get; set; } // For assistant messages

        // RAG-related fields
        public string? SourceReferences { get; set; } // JSON array of source info

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual BotConversation Conversation { get; set; } = null!;
    }

    /// <summary>
    /// Knowledge document for RAG system
    /// Contains hotel information, policies, FAQs, etc.
    /// </summary>
    public class KnowledgeDocument
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // "hotel", "room", "policy", "faq", etc.
        public string? Source { get; set; } // URL, file path, or manual entry
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ICollection<KnowledgeChunk> Chunks { get; set; } = new List<KnowledgeChunk>();
    }

    /// <summary>
    /// Chunk of knowledge text with embedding for RAG
    /// </summary>
    public class KnowledgeChunk
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DocumentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int ChunkIndex { get; set; } // Order within document
        public float[]? Embedding { get; set; } // Vector embedding (1024-dim for BGE-M3)

        // Metadata
        public string? Metadata { get; set; } // JSON with additional info (hotel_id, room_type, etc.)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual KnowledgeDocument Document { get; set; } = null!;
    }
}
