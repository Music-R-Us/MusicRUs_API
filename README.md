# MusicRUs API

Backend for **MusicRUs**, a social platform for music lovers and collectors. Users can keep track of their physical music collection (vinyl, CD, cassette), discuss music in a forum, follow artists, find concerts and chat with friends.

The API is built with **ASP.NET Core (.NET 10)** and **PostgreSQL**, and it fetches music data from **Spotify** and concert data from **Ticketmaster**.

> Developed as a final exam project (svendeprøve) by the Music-R-Us team.

---

## Features

| Area | Description |
|---|---|
| **Users and login** | Sign up, log in with username or e-mail, Google login, logout, and update username, e-mail and password |
| **Two-factor (2FA)** | TOTP via an authenticator app, set up with a QR code |
| **Catalog** | Artists, albums, tracks and genres are fetched from Spotify and stored in the database |
| **Music collection** | Register albums you *own* or *want*, with format (Vinyl, CD, Cassette, Digital) and condition (Goldmine scale) |
| **Forum profile** | Public/private profile with favorite artists, favorite albums, genres and profile colors |
| **Forum** | Posts linked to a track, album or artist, with genres and comments |
| **Concerts** | Upcoming concerts for an artist via Ticketmaster |
| **Artist subscriptions** | Follow artists |
| **Friends and messages** | Friend requests and private messages between users |
| **Blocking** | Soft blocking: content between two users who have blocked each other is hidden from both |

---

## Tech stack

- **.NET 10 / ASP.NET Core Web API**
- **Entity Framework Core 10** with **Npgsql** (PostgreSQL) and snake_case naming
- **JWT Bearer** authentication
- **Argon2** (Konscious) for password hashing
- **AES-GCM** encryption of e-mails, and HMAC for lookups
- **Otp.NET** and **QRCoder** for 2FA
- **Google.Apis.Auth** for Google login
- **Swagger / OpenAPI** for documentation
- **xUnit** and **NSubstitute** for unit tests
- **Docker** for hosting

---

## Architecture

The project uses a layered architecture with a focus on the SOLID principles:

```
Controller  →  Service  →  Repository  →  AppDbContext (EF Core)  →  PostgreSQL
  (HTTP)       (logic)    (data access)
```

- **Controllers** handle HTTP, read the user id from the JWT and translate exceptions into status codes.
- **Services** contain the business logic (validation, ownership, blocking).
- **Repositories** handle data access. The generic interfaces are split into `IReadRepository<T>` (read only) and `IRepository<T>` (read and write). The catalog repositories (Album, Artist, Genre, Track) are read-only, because their data comes from the Spotify import.
- **DTOs** separate the API contract from the database models.
- **Interceptors** set `CreatedAt`/`UpdatedAt` automatically and log deletions to a `DeletionLog` table.
- Everything is registered through **Dependency Injection** in `Program.cs`.

### Folder structure

```
MusicRUs_API/
├── MusicRUs.API/
│   ├── Backend/Logic/     # Encryption, GoogleAuth, Hashing, Jwt, Spotify, Ticketmaster, TwoFactor, Helpers
│   ├── Controllers/       # API endpoints
│   ├── Data/              # AppDbContext, Configurations, Interceptors, Migrations
│   ├── DTOs/              # Request and response models
│   ├── Models/Entities/   # Database entities and enums
│   ├── Repositories/      # Data access (+ Interfaces/)
│   ├── Services/          # Business logic (+ Interfaces/)
│   └── Program.cs
├── MusicRUs.API.Tests/    # Unit tests (Controllers, Models, Security, Services)
├── Dockerfile
└── MusicRUs_API.slnx
```

---

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL
- A Spotify Developer app (Client ID and Client Secret)
- A Google OAuth Client ID
- A Ticketmaster API key (optional, only needed for concerts)

### 1. Clone the repository

```bash
git clone https://github.com/Music-R-Us/MusicRUs_API.git
cd MusicRUs_API
```

### 2. Configure secrets

Secrets are **not** stored in the repository. Set them with user-secrets (locally) or environment variables (server):

```bash
cd MusicRUs.API
dotnet user-secrets set "ConnectionStrings:MusicRUs" "Host=localhost;Database=musicrus;Username=...;Password=..."
dotnet user-secrets set "Jwt:SigningKey" "<at least 32 bytes>"
dotnet user-secrets set "Encryption:Key" "<32 bytes, Base64>"
dotnet user-secrets set "Encryption:HmacKey" "<at least 32 bytes, Base64>"
dotnet user-secrets set "Spotify:ClientId" "<client id>"
dotnet user-secrets set "Spotify:ClientSecret" "<client secret>"
dotnet user-secrets set "Ticketmaster:ApiKey" "<api key>"
```

You can generate a 32-byte Base64 key with:

```bash
openssl rand -base64 32
```

> The API validates its configuration on startup and stops with a clear error message if anything is missing.

`Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpiresMinutes` and `GoogleAuth:ClientId` are set in `appsettings.json`.

### 3. Create the database

```bash
dotnet tool restore
dotnet ef database update --project MusicRUs.API
```

### 4. Run the API

```bash
dotnet run --project MusicRUs.API
```

In Development, Swagger UI is available at `https://localhost:5230/swagger`.

To test protected endpoints, log in, copy the `accessToken` and paste it under **Authorize** in Swagger (without the `Bearer ` prefix).

---

## Tests

```bash
dotnet test
```

The test project uses **xUnit** and **NSubstitute** and covers, among other things:

- **Security:** password hashing, encryption, JWT, token denylist, TOTP/2FA, login and lockout, and DTO validation
- **Services and controllers:** Album and Genre
- **Models:** Album and Genre

---

## Security

- Passwords are hashed with **Argon2**.
- E-mails are stored **encrypted** (AES-GCM) and looked up through an **HMAC**.
- **JWT** with validation of issuer, audience, signature and lifetime.
- **Logout** revokes the token through a denylist (`jti`).
- **Lockout:** 3 failed login attempts lock the account for 15 minutes.
- **Rate limiting:** max 5 attempts per minute per IP on login, sign-up, 2FA and e-mail/password changes.
- **2FA** with TOTP.
- **CORS:** localhost in Development, otherwise only origins from `Cors:AllowedOrigins`.
- **Soft blocking:** profiles, posts, comments and collections of blocked users are hidden and return `404`, so it is never revealed that you have been blocked.

---

## Docker

```bash
docker build -t musicrus-api .
docker run -p 8080:8080 \
  -e ConnectionStrings__MusicRUs="..." \
  -e Jwt__SigningKey="..." \
  -e Encryption__Key="..." \
  -e Encryption__HmacKey="..." \
  -e Spotify__ClientId="..." \
  -e Spotify__ClientSecret="..." \
  musicrus-api
```

The container listens on port **8080** and runs as a non-root user.

---

## API overview

All endpoints are under `/api`. The full documentation is available in Swagger.

| Controller | Route | Contents |
|---|---|---|
| User | `/api/User` | Sign up, log in, log out, profile, update and delete user |
| Google Auth | `/api/google-auth` | Log in with Google |
| Two-Factor | `/api/2fa` | Setup, enable, disable, verify |
| Artist / Album / Track / Genre | `/api/Artist`, `/api/Album`, `/api/Track`, `/api/Genre` | Catalog and search |
| Concert | `/api/Concert` | Concerts for an artist |
| MusicCollection | `/api/MusicCollection` | Owned and wanted albums |
| ForumProfile | `/api/ForumProfile` | Profiles and search |
| ForumPost | `/api/ForumPost` | Forum posts |
| Comment | `/api/Comment` | Comments |
| ArtistSubscription | `/api/ArtistSubscription` | Follow artists |
| Friendship | `/api/Friendship` | Friend requests |
| Message | `/api/Message` | Private messages |
| UserBlock | `/api/UserBlock` | Block and unblock users |
