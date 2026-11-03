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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());

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


// Database
var connectionString = builder.Configuration.GetConnectionString("MusicRUs")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:MusicRUs mangler i user-secrets eller miljøvariabler.");

builder.Services.AddSingleton<TimestampInterceptor>();
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention()
           .AddInterceptors(sp.GetRequiredService<TimestampInterceptor>()));

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
app.UseAuthorization();
app.MapControllers();
app.Run();