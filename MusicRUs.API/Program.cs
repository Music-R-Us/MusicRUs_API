using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Data;
using MusicRUs.API.Backend.Logic.GoogleAuth;
using MusicRUs.API.Services;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Repositories;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Data.Interceptors;
using System.Text.Json.Serialization;
using MusicRUs.API.Backend.Logic.Spotify;
using MusicRUs.API.Backend.Logic.Ticketmaster;
using MusicRUs.API.Backend.Logic.Hashing;
using System.Text;
using MusicRUs.API.Backend.Logic.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MusicRUs.API.Backend.Logic.Encryption;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi;
using MusicRUs.API.Backend.Logic.TwoFactor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT", 
            Description = "Paste the accessToken from login (without 'Bearer ' prefix)."
        };

        document.Security = 
        [
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            }
        ];
        return Task.CompletedTask;
    });
});

builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());

// Hashing
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

// Two-factor authentication
builder.Services.AddSingleton<ITotpService, TotpService>();
builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();

// JWT
builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .Validate(o => Encoding.UTF8.GetByteCount(o.SigningKey ?? "") >= 32,
        "Jwt:SigningKey mangler eller er under 32 bytes (user-secrets eller miljøvariabler).")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer) && !string.IsNullOrWhiteSpace(o.Audience) && o.ExpiresMinutes > 0,
        "Jwt:Issuer, Jwt:Audience og Jwt:ExpiresMinutes skal være sat i appsettings.json.")
    .ValidateOnStart();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<ITokenDenylist, TokenDenylist>();
var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false; // Keep "sub" as "sub" instead of a long Microsoft claim name
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = JwtRegisteredClaimNames.UniqueName
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var jti = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Jti);
                var denylist = context.HttpContext.RequestServices.GetRequiredService<ITokenDenylist>();
                if (jti is null || denylist.IsRevoked(jti))
                    context.Fail("Token has been revoked.");
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

// Rate limiting: max 5 attempts per minute per IP on login, signup and password/e-mail changes
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: GetClientIp(context),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

// Encryption
builder.Services.AddOptions<EncryptionOptions>()
    .Bind(builder.Configuration.GetSection(EncryptionOptions.SectionName))
    .Validate(o => EncryptionOptions.KeyLength(o.Key) == 32 && EncryptionOptions.KeyLength(o.HmacKey) >= 32,
        "Encryption:Key og Encryption:HmacKeymangler i user-secrets eller miljøvariabler.")
    .ValidateOnStart();
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();

// Google Auth
builder.Services
    .AddOptions<GoogleAuthOptions>()
    .Bind(builder.Configuration.GetSection(GoogleAuthOptions.SectionName))
    .Validate(options => !string.IsNullOrWhiteSpace(options.ClientId),
        "GoogleAuth:ClientId is required.")
    .ValidateOnStart();

builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();

// Spotify
builder.Services.AddOptions<SpotifyOptions>()
    .Bind(builder.Configuration.GetSection(SpotifyOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.ClientId) && !string.IsNullOrWhiteSpace(o.ClientSecret),
        "Spotify:ClientId og Spotify:ClientSecret mangler i user-secrets eller miljøvariabler.")
    .ValidateOnStart();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<ISpotifyClient, SpotifyClient>(client =>
    client.BaseAddress = new Uri("https://api.spotify.com/"));


// Ticketmaster
builder.Services.Configure<TicketmasterOptions>(
    builder.Configuration.GetSection(TicketmasterOptions.SectionName));
builder.Services.AddHttpClient<ITicketmasterClient, TicketmasterClient>(client =>
{
    client.BaseAddress = new Uri("https://app.ticketmaster.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
});
    
// Repositories (Registering repositories for Dependency Injection)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IReadRepository<>), typeof(Repository<>)); 
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IForumProfileRepository, ForumProfileRepository>();
builder.Services.AddScoped<IMusicCollectionRepository, MusicCollectionRepository>();
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IForumPostRepository, ForumPostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ITrackRepository, TrackRepository>();
builder.Services.AddScoped<IConcertRepository, ConcertRepository>();
builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUserBlockRepository, UserBlockRepository>();
builder.Services.AddScoped<IArtistSubscriptionRepository, ArtistSubscriptionRepository>();

// Services (Registering services for Dependency Injection)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IForumProfileService, ForumProfileService>();
builder.Services.AddScoped<IMusicCollectionService, MusicCollectionService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
builder.Services.AddScoped<ICatalogImportService, CatalogImportService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IForumPostService, ForumPostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IConcertService, ConcertService>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IUserBlockService, UserBlockService>();
builder.Services.AddScoped<IArtistSubscriptionService, ArtistSubscriptionService>();

// Database
var connectionString = builder.Configuration.GetConnectionString("MusicRUs")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:MusicRUs mangler i user-secrets eller miljøvariabler.");

builder.Services.AddSingleton<TimestampInterceptor>();
builder.Services.AddSingleton<DeletionLogInterceptor>();
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention()
           .AddInterceptors(
               sp.GetRequiredService<TimestampInterceptor>(),
               sp.GetRequiredService<DeletionLogInterceptor>()));

// CORS
const string FrontendCorsPolicy = "Frontend";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Development: allow any localhost/127.0.0.1-port
            policy.SetIsOriginAllowed(origin =>
                    Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
                    (uri.Host == "localhost" || uri.Host == "127.0.0.1"))
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();
app.UseCors(FrontendCorsPolicy);
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();;
app.MapControllers();
app.Run();


// Gets the user's IP address. Once hosted on the server, Cloudflare puts it in the CF-Connecting-IP header.
// Locally we use the connection's IP instead.
static string GetClientIp(HttpContext context) =>
    context.Request.Headers["CF-Connecting-IP"].FirstOrDefault()
    ?? context.Connection.RemoteIpAddress?.ToString()
    ?? "unknown";