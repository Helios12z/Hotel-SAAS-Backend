# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Multi-brand hotel management system backend (Traveloka-like) built with **ASP.NET Core 9.0 Web API**, **PostgreSQL 16+ with pgvector**, and **JWT authentication**.

## Build & Run Commands

```bash
# Build the project
dotnet build

# Run development server
dotnet run

# Run tests
dotnet test

# EF Core migrations
dotnet ef migrations add <MigrationName>
dotnet ef database update

# Publish for production
dotnet publish -c Release -o ./publish
```

## Architecture

**Clean Architecture** pattern: `Controller -> Service -> Repository -> DbContext`

```
Hotel-SAAS-Backend.API/
├── Controllers/          # HTTP endpoints
├── Services/             # Business logic (includes AI/ subfolder)
├── Repositories/         # Data access (EF Core)
├── Interfaces/           # Contracts (Services/, Repositories/)
├── Models/               # Entities/, DTOs/, Enums/, Options/
├── Data/                 # ApplicationDbContext, SeedData
├── Mapping/              # AutoMapper profiles
└── Utils/                # Utility classes
```

## Key Patterns

- **DTOs**: Suffix with `Dto` (e.g., `HotelDto`, `CreateHotelDto`)
- **Interfaces**: Prefix with `I` (e.g., `IHotelService`)
- **Repositories**: Use `IUnitOfWork` for transaction management
- **Response format**: All endpoints return `ApiResponseDto<T>` wrapper
- **Async**: All I/O operations use async/await

## Configuration

Key settings in `appsettings.json`:
- `ConnectionStrings:DefaultConnection` - PostgreSQL connection
- `Jwt` - JWT authentication settings
- `Embedding` / `LLM` / `RAG` - AI services configuration

## Database

- PostgreSQL 16+ with **pgvector extension** required
- Migrations auto-apply on startup
- Seed data creates default admin: `admin@hotelsaas.com` / `Admin123!`

## Authentication

`POST /api/auth/login` returns `accessToken` + `refreshToken`. Use `Authorization: Bearer {token}` header.

## AI Services

- `OllamaEmbedderService` - Text embeddings (bge-m3, 1024 dimensions)
- `GeminiLLMProvider` - LLM via Google Gemini
- `RAGService` - Retrieval-augmented generation

## Adding New Features

1. **New entity**: Create Entity class, DTOs, Repository (interface + impl), Service (interface + impl), Controller, add DbSet to ApplicationDbContext
2. **New endpoint**: Add method in Controller, inject required service
3. **Database changes**: Create migration after modifying entities
4. **New service**: Create interface in `Interfaces/Services/`, implementation in `Services/`, register in `Program.cs`
5. **Authorization**: Use `[Authorize(Roles = "...")]` attribute

## Detailed Documentation

See `AGENT.MD` for comprehensive entity definitions, API endpoints, enums, and detailed guidance.
