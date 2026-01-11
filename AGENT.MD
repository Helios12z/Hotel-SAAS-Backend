# AGENT.MD - Hotel SAAS Backend

## Project Overview

Multi-brand hotel management system backend (similar to Traveloka) built with ASP.NET Core 9.0, PostgreSQL with pgvector, and JWT authentication.

## Tech Stack

| Component | Technology |
|-----------|------------|
| Framework | ASP.NET Core 9.0 Web API |
| Language | C# / .NET 9.0 |
| Database | PostgreSQL 16+ with pgvector |
| ORM | Entity Framework Core 9.0 |
| Auth | JWT with BCrypt password hashing |
| Docs | Swagger/OpenAPI |
| Mapping | AutoMapper |

## Project Structure

```
Hotel-SAAS-Backend.API/
??? Controllers/          # API endpoints (REST controllers)
??? Services/             # Business logic layer
?   ??? AI/               # BotService, RAGService
?   ??? Embedding/        # OllamaEmbedderService
?   ??? LLM/              # GeminiLLMProvider
??? Repositories/         # Data access layer (EF Core)
??? Interfaces/           # Contracts for services & repositories
?   ??? Services/
?   ??? Repositories/
??? Models/
?   ??? Entities/         # Database entities (EF Core models)
?   ??? DTOs/             # Request/Response data transfer objects
?   ??? Enums/            # Enumerations
?   ??? Options/          # Configuration POCO classes
??? Data/                 # ApplicationDbContext, SeedData
??? Mapping/              # AutoMapper profiles
??? Migrations/           # EF Core migrations
??? Utils/                # Utility classes
```

## Architecture Pattern

**Clean Architecture**: `Controller ? Service ? Repository ? DbContext`

- **Controllers**: Handle HTTP requests, validation, route to services
- **Services**: Business logic, orchestration, transaction management
- **Repositories**: Data access, LINQ queries, Unit of Work pattern
- **Entities**: Database models with navigation properties

## Key Files

| File | Purpose |
|------|---------|
| `Program.cs` | App configuration, DI registration, middleware pipeline |
| `Data/ApplicationDbContext.cs` | EF Core DbContext with Fluent API config |
| `Data/SeedData.cs` | Initial data seeding |
| `appsettings.json` | Configuration (DB, JWT, CORS, AI services) |

## Domain Entities

### Core Entities
- `Brand` - Hotel brand (parent of hotels)
- `Hotel` - Hotel property with location, ratings, images
- `Room` - Hotel rooms with pricing, amenities
- `User` - System users with roles
- `Booking` - Reservations with status workflow
- `Payment` - Payment transactions
- `Review` - Guest reviews with moderation

### Supporting Entities
- `Amenity`, `HotelAmenity`, `RoomAmenity` - Amenities system
- `HotelImage`, `RoomImage`, `ReviewImage` - Image attachments
- `RoomPricing` - Dynamic room pricing
- `BookingRoom` - Many-to-many booking-room relationship
- `Facet`, `FacetValue` - Search/filter facets

### AI Entities
- `BotConversation`, `BotMessage` - AI chatbot conversations
- `KnowledgeDocument`, `KnowledgeChunk` - RAG knowledge base

## User Roles (Enum: UserRole)

| Role | Access Level |
|------|--------------|
| `SuperAdmin` | Full system access |
| `BrandAdmin` | Manage hotels within brand |
| `HotelManager` | Manage specific hotel |
| `Receptionist` | Handle bookings, check-in/out |
| `Guest` | Book rooms, write reviews |

## Enums

- `BookingStatus`: Pending, Confirmed, CheckedIn, CheckedOut, Cancelled, NoShow
- `PaymentStatus`: Pending, Processing, Completed, Failed, Refunded, PartiallyRefunded
- `PaymentMethod`: CreditCard, DebitCard, BankTransfer, Cash, EWallet
- `ReviewStatus`: Pending, Approved, Rejected
- `RoomStatus`: Available, Occupied, Reserved, Maintenance, OutOfOrder
- `RoomType`: Standard, Deluxe, Suite, Executive, Presidential, Family, Accessible
- `BedType`: Single, Double, Queen, King, Twin, California_King, Sofa_Bed, Bunk
- `AmenityType`: Hotel, Room, Both
- `UserStatus`: Active, Inactive, Suspended, Deleted

## API Controllers

| Controller | Base Route | Purpose |
|------------|------------|---------|
| `AuthController` | `/api/auth` | Login, register, refresh token |
| `UsersController` | `/api/users` | User management |
| `BrandsController` | `/api/brands` | Brand CRUD |
| `HotelsController` | `/api/hotels` | Hotel CRUD, search |
| `RoomsController` | `/api/rooms` | Room management |
| `BookingsController` | `/api/bookings` | Booking lifecycle |
| `PaymentsController` | `/api/payments` | Payment processing |
| `ReviewsController` | `/api/reviews` | Review submission/moderation |
| `AmenitiesController` | `/api/amenities` | Amenity management |
| `BotController` | `/api/bot` | AI chatbot interactions |

## Configuration Sections (appsettings.json)

```json
{
  "ConnectionStrings": { "DefaultConnection": "..." },
  "Jwt": { "Issuer", "Audience", "Base64Key", "AccessTokenMinutes", "RefreshTokenDays" },
  "Cors": { "AllowedOrigins": [] },
  "Embedding": { "BaseUrl", "Model", "Dimension" },
  "LLM": { "Provider", "BaseUrl", "ApiKey", "Model", "MaxTokens", "Temperature" },
  "RAG": { "MaxContextLength", "TopKResults", "MinSimilarityScore" }
}
```

## Common Commands

```bash
# Build
dotnet build

# Run (development)
dotnet run

# Run tests
dotnet test

# EF Core Migrations
dotnet ef migrations add <MigrationName>
dotnet ef database update
dotnet ef migrations remove

# Publish for production
dotnet publish -c Release -o ./publish
```

## Database Setup

1. PostgreSQL 16+ with pgvector extension required
2. Run `CREATE EXTENSION vector;` in database
3. Migrations auto-apply on startup (`db.Database.Migrate()` in Program.cs)

## Authentication Flow

1. `POST /api/auth/login` ? Returns `accessToken` + `refreshToken`
2. Include `Authorization: Bearer {accessToken}` header
3. `POST /api/auth/refresh-token` when access token expires

## Coding Conventions

- **Naming**: PascalCase for public members, camelCase for parameters
- **Async**: All I/O operations use async/await
- **DTOs**: Suffix with `Dto` (e.g., `HotelDto`, `CreateHotelDto`)
- **Interfaces**: Prefix with `I` (e.g., `IHotelService`, `IHotelRepository`)
- **Repositories**: Use `IUnitOfWork` for transaction management
- **Nullable**: Enabled project-wide (`<Nullable>enable</Nullable>`)

## Key Dependencies (NuGet)

- `Npgsql.EntityFrameworkCore.PostgreSQL` - PostgreSQL provider
- `Pgvector.EntityFrameworkCore` - Vector operations for AI
- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT auth
- `Swashbuckle.AspNetCore` - Swagger/OpenAPI
- `AutoMapper` - Object mapping
- `BCrypt.Net-Next` - Password hashing

## AI Services

| Service | Purpose |
|---------|---------|
| `OllamaEmbedderService` | Text embeddings via Ollama (bge-m3 model) |
| `GeminiLLMProvider` | LLM responses via Google Gemini |
| `RAGService` | Retrieval-augmented generation |
| `BotService` | Chatbot conversation management |

## Tips for AI Agents

1. **Adding new entity**: Create Entity ? DTO ? Repository ? Service ? Controller, add DbSet in ApplicationDbContext
2. **Adding new endpoint**: Add method in Controller, inject required service
3. **Database changes**: Create migration after modifying entities
4. **New service**: Create interface in `Interfaces/Services/`, implementation in `Services/`, register in `Program.cs`
5. **Authorization**: Use `[Authorize(Roles = "...")]` attribute on controllers/actions
