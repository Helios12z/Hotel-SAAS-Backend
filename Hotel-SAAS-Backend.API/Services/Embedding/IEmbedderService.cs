namespace Hotel_SAAS_Backend.API.Services.Embedding
{
    /// <summary>
    /// Interface for generating vector embeddings from text
    /// </summary>
    public interface IEmbedderService
    {
        /// <summary>
        /// Generate embedding for a single text
        /// </summary>
        /// <param name="text">Text to embed</param>
        /// <returns>Vector embedding as float array</returns>
        Task<float[]> EmbedOneAsync(string text);

        /// <summary>
        /// Generate embeddings for multiple texts
        /// </summary>
        /// <param name="texts">Texts to embed</param>
        /// <returns>Vector embeddings as float arrays</returns>
        Task<IList<float[]>> EmbedManyAsync(IEnumerable<string> texts);

        /// <summary>
        /// Get the dimension of embeddings produced by this service
        /// </summary>
        int Dimension { get; }
    }
}
