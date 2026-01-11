# Hotel SAAS Backend

A multi-brand hotel management system backend similar to Traveloka, built with ASP.NET Core 9.0, PostgreSQL with pgvector support for AI features, and JWT authentication.

## Features

- **Multi-Brand Support**: Manage multiple hotel brands under one platform
- **Hotel Management**: Complete CRUD operations for hotels with detailed information
- **Room Management**: Room types, pricing, availability tracking
- **Booking System**: Full booking lifecycle (pending → confirmed → checked-in → checked-out)
- **Payment Processing**: Payment tracking with refund support
- **Reviews & Ratings**: Guest reviews with moderation workflow
- **Amenities Management**: Configurable hotel and room amenities
- **User Management**: Role-based access control (SuperAdmin, BrandAdmin, HotelManager, Receptionist, Guest)
- **JWT Authentication**: Secure token-based authentication with refresh tokens
- **AI Ready**: PostgreSQL with pgvector extension for future AI features (search, recommendations)

## Tech Stack

- **Framework**: ASP.NET Core 9.0 Web API
- **Language**: C# / .NET 9.0
- **Database**: PostgreSQL 16+ with pgvector extension
- **ORM**: Entity Framework Core 9.0
- **Authentication**: JWT (JSON Web Tokens) with BCrypt password hashing
- **API Documentation**: Swagger/OpenAPI
- **Mapping**: AutoMapper
- **Architecture**: Clean Architecture (Controllers → Services → Repositories → Entities)

## Project Structure

```
Hotel-SAAS-Backend/
├── Hotel-SAAS-Backend.API/
│   ├── Controllers/          # API endpoints
│   ├── Services/             # Business logic
│   ├── Repositories/         # Data access layer
│   ├── Interfaces/           # Service & Repository interfaces
│   ├── Models/
│   │   ├── Entities/         # Database entities
│   │   ├── DTOs/             # Data transfer objects
│   │   ├── Enums/            # Enumerations
│   │   └── Options/          # Configuration options
│   ├── Data/                 # DbContext & Seed data
│   ├── Mapping/              # AutoMapper profiles
│   └── Scripts/              # Setup scripts
```

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL 16+ with pgvector extension
- Git

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Hotel-SAAS-Backend/Hotel-SAAS-Backend.API
   ```

2. **Install pgvector extension for PostgreSQL**

   **Windows:**
   - Download from https://github.com/pgvector/pgvector/releases
   - Copy files to PostgreSQL lib directory
   - Run: `CREATE EXTENSION vector;` in your database

   **Linux/macOS:**
   ```bash
   cd /tmp
   git clone --branch v0.3.2 https://github.com/pgvector/pgvector.git
   cd pgvector
   make
   sudo make install
   ```

3. **Configure the database**

   Edit `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=hotel_saas_db;Username=postgres;Password=your_password"
     },
     "Jwt": {
       "Issuer": "Hotel-SAAS-Backend",
       "Audience": "Hotel-SAAS-Client",
       "Base64Key": "YOUR_BASE64_ENCODED_SECRET_KEY_MINIMUM_256_BITS"
     }
   }
   ```

   Generate a secure JWT key:
   ```bash
   # Generate a random key and encode to base64
   openssl rand -base64 32
   ```

4. **Run setup scripts**

   **Windows (PowerShell):**
   ```powershell
   .\Scripts\setup-windows.ps1
   ```

   **Linux/macOS:**
   ```bash
   chmod +x Scripts/setup-linux.sh
   ./Scripts/setup-linux.sh
   ```

   Or manually:
   ```bash
   # Install EF Core tools
   dotnet tool install --global dotnet-ef

   # Restore packages
   dotnet restore

   # Create and apply migrations
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access Swagger UI**
   - Open browser: `http://localhost:5000`
   - Or the port displayed in the console

## Default Admin Credentials

```
Email: admin@hotelsaas.com
Password: Admin123!
```

**IMPORTANT**: Change the default password immediately after first login!

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/refresh-token` - Refresh access token
- `POST /api/auth/logout` - Logout
- `POST /api/auth/change-password` - Change password

### Hotels
- `GET /api/hotels` - Get all hotels
- `GET /api/hotels/{id}` - Get hotel by ID
- `GET /api/hotels/{id}/details` - Get hotel details with amenities
- `GET /api/hotels/search?query={text}` - Search hotels
- `POST /api/hotels` - Create hotel (Admin only)
- `PUT /api/hotels/{id}` - Update hotel (Admin only)

### Rooms
- `GET /api/hotels/{hotelId}/rooms` - Get rooms by hotel
- `GET /api/hotels/{hotelId}/rooms/available` - Search available rooms
- `POST /api/hotels/{hotelId}/rooms` - Create room (Admin only)

### Bookings
- `GET /api/bookings/my-bookings` - Get user's bookings
- `GET /api/bookings/{id}` - Get booking details
- `POST /api/bookings/calculate` - Calculate booking price
- `POST /api/bookings` - Create booking
- `POST /api/bookings/{id}/confirm` - Confirm booking (Admin)
- `POST /api/bookings/{id}/checkin` - Check-in (Admin)
- `POST /api/bookings/{id}/checkout` - Check-out (Admin)

### Reviews
- `GET /api/hotels/{hotelId}/reviews` - Get hotel reviews
- `POST /api/hotels/{hotelId}/reviews` - Submit review
- `POST /api/hotels/{hotelId}/reviews/{id}/approve` - Approve review (Admin)

### Payments
- `GET /api/bookings/{bookingId}/payments` - Get booking payments
- `POST /api/bookings/{bookingId}/payments` - Create payment
- `POST /api/bookings/{bookingId}/payments/{id}/process` - Process payment

## User Roles

| Role | Description |
|------|-------------|
| **SuperAdmin** | Full system access, manage all brands and hotels |
| **BrandAdmin** | Manage hotels within their brand |
| **HotelManager** | Manage specific hotel operations |
| **Receptionist** | Handle check-ins, check-outs, bookings |
| **Guest** | Regular users who book rooms |

## Database Schema

### Core Tables
- `users` - User accounts with role-based access
- `brands` - Hotel brands
- `hotels` - Hotel properties with vector embeddings
- `rooms` - Room inventory with vector embeddings
- `amenities` - Available amenities
- `hotel_amenities` - Hotel-amenity relationships
- `room_amenities` - Room-amenity relationships
- `bookings` - Booking records
- `booking_rooms` - Booking-room relationships
- `payments` - Payment transactions
- `reviews` - Guest reviews with sentiment embeddings

### Vector Columns (for AI features)
- `hotels.embedding` - For semantic search
- `rooms.embedding` - For room recommendations
- `users.preferences_embedding` - For personalization
- `reviews.embedding` - For sentiment analysis
- `reviews.sentiment_embedding` - For review categorization

## Development

### Running Tests
```bash
dotnet test
```

### Building for Production
```bash
dotnet build -c Release
dotnet publish -c Release -o ./publish
```

### Environment Variables
Key environment variables (can be set in appsettings.json):

```bash
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=...
Jwt__Base64Key=...
Jwt__Issuer=...
Jwt__Audience=...
```

## Security Features

- **Password Hashing**: BCrypt with salt
- **JWT Authentication**: Short-lived access tokens (120 min)
- **Refresh Tokens**: Long-lived refresh tokens (30 days)
- **Role-Based Authorization**: Attribute-based authorization
- **CORS**: Configurable allowed origins
- **HTTPS**: Enabled in production

## Future Enhancements (AI Features)

The database is prepared for AI features using pgvector:

1. **Semantic Search**: Find hotels based on natural language queries
2. **Personalized Recommendations**: AI-powered room/hotel suggestions
3. **Sentiment Analysis**: Automatic review sentiment categorization
4. **Smart Pricing**: Dynamic pricing based on demand patterns
5. **Chatbot**: AI-powered customer support

## Troubleshooting

### Database Connection Issues
- Verify PostgreSQL is running: `pg_isready`
- Check connection string in appsettings.json
- Ensure pgvector extension is installed: `CREATE EXTENSION vector;`

### Migration Errors
- Drop and recreate database: `DROP DATABASE hotel_saas_db; CREATE DATABASE hotel_saas_db;`
- Re-run migrations: `dotnet ef database update`

### JWT Token Issues
- Verify your Base64Key is at least 256 bits (32 bytes when decoded)
- Ensure system time is synchronized

## License

This project is proprietary software.

## Support

For support, contact: support@hotelsaas.com
