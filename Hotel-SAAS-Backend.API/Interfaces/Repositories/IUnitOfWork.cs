namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IAmenityRepository Amenities { get; }
        IBrandRepository Brands { get; }
        IHotelRepository Hotels { get; }
        IRoomRepository Rooms { get; }
        IUserRepository Users { get; }
        IBookingRepository Bookings { get; }
        IPaymentRepository Payments { get; }
        IReviewRepository Reviews { get; }

        /// <summary>
        /// Saves all changes made in this unit of work to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> CommitAsync();

        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        Task RollbackTransactionAsync();
    }
}
