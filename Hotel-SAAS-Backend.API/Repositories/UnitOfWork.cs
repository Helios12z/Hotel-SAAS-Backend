using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Hotel_SAAS_Backend.API.Repositories
{
    /// <summary>
    /// Unit of Work implementation that manages repositories and database transactions.
    /// Provides a single point for managing data operations across multiple repositories.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        // Lazy-loaded repository properties
        private IAmenityRepository? _amenities;
        private IBrandRepository? _brands;
        private IHotelRepository? _hotels;
        private IRoomRepository? _rooms;
        private IUserRepository? _users;
        private IBookingRepository? _bookings;
        private IPaymentRepository? _payments;
        private IReviewRepository? _reviews;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IAmenityRepository Amenities => _amenities ??= new AmenityRepository(_context);
        public IBrandRepository Brands => _brands ??= new BrandRepository(_context);
        public IHotelRepository Hotels => _hotels ??= new HotelRepository(_context);
        public IRoomRepository Rooms => _rooms ??= new RoomRepository(_context);
        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IBookingRepository Bookings => _bookings ??= new BookingRepository(_context);
        public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);
        public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);

        /// <summary>
        /// Saves all changes made in this unit of work to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public async Task<int> CommitAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the exception here if you have a logger
                throw new InvalidOperationException(
                    "An error occurred while saving changes to the database. " +
                    "See the inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction is in progress.");
            }

            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction is in progress.");
            }

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Disposes the unit of work and any active transactions.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the resources used by this unit of work.
        /// </summary>
        /// <param name="disposing">True if disposing managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose transaction if still active
                    _transaction?.Dispose();

                    // Dispose context
                    _context.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
